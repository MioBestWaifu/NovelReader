using Maria.Commons.Recordkeeping;
using Maria.Readers.Logic.Structure;
using Maria.Translation.Japanese;
using Maria.Translation.Japanese.Edrdg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maria.Readers.Logic
{

    //Maybe shoulde be EpubParser or something like that
    //Also, thats a lot of stuff. Maybe it should be split into multiple classes.
    internal static class EpubFormatter
    {

        private static XNamespace xhtmlNs = "http://www.w3.org/1999/xhtml";
        private static XNamespace epubNs = "http://www.idpf.org/2007/ops";
        //([,!?、。．。「」『』…．！？：；（）()'\"“”])";
        //include any other separators that might be missing
        //Why two fields? Class regex as above is fast for regexing, list is faster for comparing.
        private static readonly List<string> separatorsAsList = new List<string> { ",", "!", "?", "、", "。", "．", "「", "」", "『", "』", "…", "．", "！", "？", "：", "；", "（", "）", "(", ")", "'", "\"", "“", "”" };

        private static readonly string separatorsRegex = "([" + string.Join("", separatorsAsList.Select(Regex.Escape)) + "])";


        public async static Task<string> FindStandardsFile(string originalXml)
        {
            // Parse the original XML content as an XML document
            XDocument doc = XDocument.Parse(originalXml);

            // Define the OPF namespace
            XNamespace ns = "urn:oasis:names:tc:opendocument:xmlns:container";

            // Get the rootfile element using the namespace
            XElement rootfile = doc.Descendants(ns + "rootfile").FirstOrDefault();

            // Get the full-path attribute of the rootfile
            string fullPath = rootfile?.Attribute("full-path")?.Value;

            return fullPath;
        }


        public static List<string> ListChapters(string pathToStandards, string originalXml)
        {
            // Parse the original XHTML content as an XML document
            XDocument doc = XDocument.Parse(originalXml);

            // Define the OPF namespace
            XNamespace ns = "http://www.idpf.org/2007/opf";

            // Get the manifest and spine elements using the namespace
            XElement manifest = doc.Root.Element(ns + "manifest");
            XElement spine = doc.Root.Element(ns + "spine");

            // Get all itemrefs in the spine
            var itemrefs = spine.Elements(ns + "itemref");

            // Create a dictionary to map IDs to hrefs from the manifest
            var manifestItems = manifest.Elements(ns + "item")
                .ToDictionary(
                    item => item.Attribute("id").Value,
                    item => item.Attribute("href").Value
                );

            // Collect hrefs of the items in the spine
            List<string> contentPaths = new List<string>();
            foreach (var itemref in itemrefs)
            {
                string idref = itemref.Attribute("idref").Value;
                if (manifestItems.TryGetValue(idref, out string href))
                {
                    //Presumes the contents are in the same directory as the standards file. Are they always? Dont know. If it is found that often they wont be, then more elaborate work is needed.
                    contentPaths.Add(pathToStandards + "/" + href);
                }
            }

            return contentPaths;
        }

        /// <summary>
        /// Loads, parses and translates the textual content of a chapter. Ignores furigana and removes images, but both will be added later.
        /// </summary>
        /// <param name="chapter"></param>
        /// <returns>Wether the chapter is parsable. Some "chapters" are actually just images, and those are to be handled differently. Does not return false if the text format is unexpected, but rather throws an exception.</returns>
        public async static Task<bool> LoadChapter(Chapter chapter)
        {
            //Completly ignores images and unexpected text formats and will throw something if those are passed. Those will be handled later.
            string orginalXhtml = await new StreamReader(chapter.FileReference.Open()).ReadToEndAsync();

            List<XElement> lines = await BreakXhtmlToLines(orginalXhtml);

            //Doing this to make parallel processing easier. The work done on the lines may be heavy, so it is necessary.
            ConcurrentDictionary<int, XElement> indexedLines = new ConcurrentDictionary<int, XElement>();

            for (int i = 0; i < lines.Count; i++)
            {
                indexedLines.TryAdd(i, lines[i]);
            }

            ConcurrentDictionary<int, List<Node>> indexedNodeLines = new ConcurrentDictionary<int, List<Node>>();

            //This is suposedly efficient because it distributes work well, but isn't really because the order of iteration is obviously random but the order the lines are ready is obviously important. One solution would be to use a regular for and create Tasks for each line, achieving parallelism and order. Will do that later.
            Parallel.ForEach(indexedLines, (line) =>
            {
                if (line.Value.Name == xhtmlNs + "p")
                {
                    indexedNodeLines.TryAdd(line.Key, ParseTextNode(line.Value));
                }
                else if (line.Value.Name == xhtmlNs + "img")
                {
                    indexedNodeLines.TryAdd(line.Key, ParseImageNode(chapter,line.Value));
                }
                
            });

            for (int i = 0; i < indexedNodeLines.Count; i++)
            {
                chapter.Lines.Add(indexedNodeLines[i]);
            }
            //await Task.Delay(10000);
            chapter.Loaded = true;
            return true;
        }

        private static List<Node> ParseTextNode(XElement originalElement)
        {
            string line = GetParagraphText(originalElement);
            if(line == string.Empty)
            {
                return [new TextNode(){Text=""}];
            }
            //Breaking lines into sentences for smoother morphological analysis
            string[] sentences = Regex.Split(line, separatorsRegex);
            List<Node> nodes = new List<Node>();
            for (int i = 0, n = sentences.Length; i < n; i++)
            {
                //Should never be zero, i think. If it happens, will cause a bug. Purposefully not checking to see if breaks.

                if (sentences[i].Length == 1 && separatorsAsList.Contains(sentences[i]))
                {
                    //This means that the separators are not interactable as part of a word. This is not a problem, because separators ARE NOT words.
                    nodes.Add(new TextNode() { Text = sentences[i] });
                    continue;
                }

                List<JapaneseLexeme> lexemes = JapaneseTranslator.Instance!.analyzer.Analyze(sentences[i]);
                foreach (var lexeme in lexemes)
                {
                    TextNode node = new TextNode();
                    node.Text = lexeme.Surface;
                    nodes.Add(node);
                    if (JapaneseAnalyzer.signicantCategories.Contains(lexeme.Category))
                    {
                        try
                        {
                            //Presumes will return only one entry because it already went through the Analyzer
                            node.EdrdgEntry = Serializer.DeserializeJson<List<EdrdgEntry>>(JapaneseTranslator.Instance!.Translate(lexeme.BaseForm, true))![0];
                        }
                        catch (Exception e)
                        {
                            /*
                             * One of the nodes that gets errored here is some hiragana MeCab turns into 踏ん反る. The JMDict
                             * does not contain an entry for that, and other EDRDG-based translators cannot find it either, so not my fault.
                             * Sometimes the same happens with other verbs written in hiragana that are found in ohter EDRDG-Based dictionaries, so that is my propably fault,
                             * most likely because the dictionary-building process only uses one key and is overall very faulty and simple.
                             * Also, there is a possibility that the Analyzer is fucking some things up by converting hiragana to kanji. 
                             * I do not understand fucks of Mecab inner workings, so it might actually do a good job of
                             * determining the correct kanjification when many kanji words have the same reading. But if it
                             * doesn't, then it may screw the translation over. Dont know what to do about it if true, 
                             * because even though it is the idea that at some point all possible keys to a word will be in the conversion table,
                             * it may be flexed even in hiragana and Mecab will be needed to deal with that. Maybe this is configurable but I dont know.
                             * 
                             * Another kind of common error is for weird ass fantasy names (testing this with Tensai Oujo to Tensei Reijou no Mahoukakumei) 
                             * and other things (mostly) written in katakana that the Translator cannot make sense of. It is not suposed to either.
                             * Those should: A) be inserted into the dictionary from a custom database or 
                             * B) be parsed to hiragana or C) be ignored.
                             * 
                             * There is also a weird one. Translator fails to find 私. Likely my shitty code to blame,
                             * but i could swear there would be an entry for it. Maybe the problem is that it is a
                             * one-kanji word and therefore better handled with Kanjidic than JMDict? Probably would help as well.
                             * 
                             * Anyways, not fixing any of that now, this version is intended for the display parts only.
                             * 
                             */
                            Debug.WriteLine($"Error translating node: {lexeme.Surface} {lexeme.Category}");
                        }
                    }
                }
            }

            return nodes;
        }

        private static List<Node> ParseImageNode(Chapter chapter, XElement originalElement)
        {
            string path = originalElement.Attribute("src")!.Value;
            ZipArchiveEntry imageEntry = Utils.GetRelativeEntry(chapter.FileReference, path);
            byte[] imageBytes = new byte[imageEntry.Length];
            using (var stream = imageEntry.Open())
            {
                stream.ReadAsync(imageBytes, 0, imageBytes.Length).Wait();
            }
            string base64 = Convert.ToBase64String(imageBytes);

            return [new ImageNode() { Text = base64 }];
        }

        /// <summary>
        /// Breaks a Xhtml chapter into lines. Paragraphs are considered lines, irrespective of content. Images are considered lines as well.
        /// </summary>
        /// <param name="originalXhtml"></param>
        /// <returns>The </returns>
        private static Task<List<XElement>> BreakXhtmlToLines(string originalXhtml)
        {
            // Parse the XHTML content
            XDocument doc = XDocument.Parse(originalXhtml);

            // List to store the lines
            List<XElement> lines = doc.Descendants().Where(n => n.Name == xhtmlNs + "p" || n.Name == xhtmlNs + "img").ToList();

            return Task.FromResult(lines);
        }

        private static string GetParagraphText(XElement paragraph)
        {
            if (paragraph.IsEmpty || (paragraph.HasElements && paragraph.Elements(xhtmlNs + "br").Count() >= 1 && paragraph.Value.Trim() == ""))
            {
                return string.Empty;
            }
            // Use a StringBuilder for efficiency
            StringBuilder sb = new StringBuilder();

            foreach (var node in paragraph.Nodes())
            {
                if (node is XText textNode)
                {
                    sb.Append(textNode.Value);
                }
                else if (node is XElement elementNode)
                {
                    if (elementNode.Name == xhtmlNs + "ruby")
                    {
                        // Add the ruby content (kanji) and ignore rt tags
                        var rubyText = elementNode.Nodes().Where(n => !(n is XElement e && e.Name == xhtmlNs + "rt")).Select(n => n.ToString());
                        sb.Append(string.Join("", rubyText));
                    }
                    else if (elementNode.Name != xhtmlNs + "rt")
                    {
                        sb.Append(elementNode.Value);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
