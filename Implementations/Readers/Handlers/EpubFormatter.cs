using Maria.Commons.Recordkeeping;
using Maria.Translation.Japanese;
using Maria.Translation.Japanese.Edrdg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maria.Readers.Handlers
{
    //Maybe shoulde be EpubParser or something like that
    //Also, thats a lot of stuff. Maybe it should be split into multiple classes.
    internal static class EpubFormatter
    {

        //([,!?、。．。「」『』…．！？：；（）()'\"“”])";
        //include any other separators that might be missing
        //Why two fields? Class regex as above is faste for regexing, list is faster for comparing.
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

            List<string> lines = await BreakXhtmlToLines(orginalXhtml);

            //Doing this to make parallel processing easier. The work done on the lines may be heavy, so it is necessary.
            ConcurrentDictionary<int, string> indexedStringLines = new ConcurrentDictionary<int, string>();

            for (int i = 0; i < lines.Count; i++)
            {
                indexedStringLines.TryAdd(i, lines[i]);
            }

            ConcurrentDictionary<int, List<Node>> indexedNodeLines = new ConcurrentDictionary<int, List<Node>>();

            Parallel.ForEach(indexedStringLines, (line) =>
            {

                //Breaking lines into sentences for smoother morphological analysis
                string[] sentences = Regex.Split(line.Value, separatorsRegex);
                List<Node> nodes = new List<Node>();
                for (int i = 0, n = sentences.Length; i < n; i++)
                {
                    //Should never be zero, i think. If it happens, will cause a bug. Purposefully not cheking so it breaks if it happens.

                    if (sentences[i].Length == 1 && separatorsAsList.Contains(sentences[i]))
                    {
                        //This means that the separators are not interactable as part of a word. This is not a problem, because separators ARE NOT words.
                        nodes.Add(new Node() { Text = sentences[i] });
                        continue;
                    }

                    List<JapaneseLexeme> lexemes = JapaneseTranslator.Instance!.analyzer.Analyze(sentences[i]);
                    foreach (var lexeme in lexemes)
                    {
                        Node node = new Node();
                        node.Text = lexeme.Surface;
                        nodes.Add(node);
                        try
                        {
                            //Presumes will return only one entry because it already went through the Analyzer
                            node.edrdgEntry = Serializer.DeserializeJson<List<EdrdgEntry>>(JapaneseTranslator.Instance!.Translate(lexeme.BaseForm))![0];
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine($"Error translating node: {lexeme.Surface} {lexeme.Category}");
                            Debug.WriteLine(e.Message);
                        }
                    }
                }

                indexedNodeLines.TryAdd(line.Key, nodes);
            });

            for (int i = 0; i < indexedNodeLines.Count; i++)
            {
                chapter.Lines.Add(indexedNodeLines[i]);
            }

            return true;
        }

        private static Task<List<string>> BreakXhtmlToLines(string originalXhtml)
        {
            // Parse the XHTML content
            XDocument doc = XDocument.Parse(originalXhtml);
            XNamespace xhtmlNs = "http://www.w3.org/1999/xhtml";
            XNamespace epubNs = "http://www.idpf.org/2007/ops";

            // List to store the lines
            List<string> lines = new List<string>();

            // Extract paragraphs (p tags)
            var paragraphs = doc.Descendants(xhtmlNs + "p");

            foreach (var paragraph in paragraphs)
            {
                // Handle empty paragraphs and paragraphs with only <br/>
                if (paragraph.IsEmpty || (paragraph.HasElements && paragraph.Elements(xhtmlNs + "br").Count() == 1 && paragraph.Value.Trim() == ""))
                {
                    lines.Add(string.Empty);
                    continue;
                }

                // Extract text content, ignoring <rt> tags
                string paragraphText = GetParagraphText(paragraph, xhtmlNs, epubNs);
                lines.Add(paragraphText);
            }

            return Task.FromResult(lines);
        }

        private static string GetParagraphText(XElement paragraph, XNamespace xhtmlNs, XNamespace epubNs)
        {
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
