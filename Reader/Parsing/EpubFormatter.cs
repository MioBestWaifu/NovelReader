using Mio.Reader.Parsing.Structure;
using Mio.Translation.Japanese;
using Mio.Translation.Japanese.Edrdg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
#if WINDOWS
using Windows.Graphics.Printing.Workflow;
#endif

namespace Mio.Reader.Parsing
{

    //Maybe shoulde be EpubParser or something like that
    //Also, thats a lot of stuff. Maybe it should be split into multiple classes.
    internal static class EpubFormatter
    {

        private static XNamespace xhtmlNs = "http://www.w3.org/1999/xhtml";
        private static XNamespace epubNs = "http://www.idpf.org/2007/ops";
        private static XNamespace opfNs = "http://www.idpf.org/2007/opf";
        private static XNamespace ncxNs = "http://www.daisy.org/z3986/2005/ncx/";
        private static XNamespace svgNs = "http://www.w3.org/2000/svg";
        private static XNamespace xlinkNs = "http://www.w3.org/1999/xlink";
        //([,!?、。．。「」『』…．！？：；（）()'\"“”])";
        //include any other separators that might be missing
        //Why two fields? Class regex as above is fast for regexing, list is faster for comparing.
        private static readonly List<string> separatorsAsList = new List<string> { ",", "!", "?", "、", "。", "．", "「", "」", "『", "』", "…", "．", "！", "？", "：", "；", "（", "）", "(", ")", "'", "\"", "“", "”" };

        private static readonly string separatorsRegex = "([" + string.Join("", separatorsAsList.Select(Regex.Escape)) + "])";

        public static JapaneseAnalyzer analyzer;
        private static JapaneseTranslator translator = new JapaneseTranslator();

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


        public static List<(string, ZipArchiveEntry)> ListChapters(ZipArchiveEntry standardsEntry, string standardsXml)
        {
            XDocument doc = XDocument.Parse(standardsXml);

            // Get the package element
            XElement packageElement = doc.Root;

            XAttribute versionAttribute = packageElement.Attribute("version");

            string version = versionAttribute?.Value;
            List<(string, ZipArchiveEntry)> chaptersFromSpine = ListChaptersFromSpine(standardsEntry, doc);
            List<(string, ZipArchiveEntry)> namedChapters = new List<(string, ZipArchiveEntry)>();

            try
            {
                if (version == "3.0")
                {
                    List<(string, ZipArchiveEntry)> chaptersFromNav = ListChaptersFromNav(standardsEntry, doc);
                    foreach (var spineChapter in chaptersFromSpine)
                    {
                        var navChapter = chaptersFromNav.FirstOrDefault(c => c.Item2 == spineChapter.Item2);
                        namedChapters.Add(navChapter.Item2 != null ? navChapter : spineChapter);
                    }
                }
                else if (version == "2.0")
                {
                    List<(string, ZipArchiveEntry)> chaptersFromToc = ListChaptersFromToc(standardsEntry);
                    foreach (var spineChapter in chaptersFromSpine)
                    {
                        var tocChapter = chaptersFromToc.FirstOrDefault(c => c.Item2 == spineChapter.Item2);
                        namedChapters.Add(tocChapter.Item2 != null ? tocChapter : spineChapter);
                    }
                }
                else
                {
                    throw new Exception("Unsupported EPUB version");
                }
            }
            //The Zip access does not throw a exception if the file is not found, the ListChapters methods do.
            //This should happen if the for whatever reason there is no nav or toc file, or it is not found.
            catch (FileNotFoundException e)
            {
                namedChapters = chaptersFromSpine;
            }

            return namedChapters;
        }


        //For EPUB 3 
        private static List<(string, ZipArchiveEntry)> ListChaptersFromNav(ZipArchiveEntry standardsEntry, XDocument standardsDoc)
        {
            // Get the manifest element using the namespace
            XElement manifest = standardsDoc.Root.Element(opfNs + "manifest");

            // Find the item with properties="nav"
            XElement navItem = manifest.Elements(opfNs + "item")
                .FirstOrDefault(item => (string)item.Attribute("properties") == "nav");

            // Get the href attribute of the nav item
            string navPath = navItem?.Attribute("href")?.Value;

            ZipArchiveEntry navEntry = Utils.GetRelativeEntry(standardsEntry, navPath);

            if (navEntry is null)
            {
                throw new FileNotFoundException("Nav file not found");
            }

            //Because the read has to be async and i dont want to make this method async
            var navTask = new StreamReader(navEntry.Open()).ReadToEndAsync();
            navTask.Wait();
            string navXml = navTask.Result;

            //ns = "http://www.w3.org/1999/xhtml";
            XDocument navDoc = XDocument.Parse(navXml);
            List<(string, ZipArchiveEntry)> contents = new List<(string, ZipArchiveEntry)>();

            // Get the nav element with epub:type="toc"
            XElement tocNav = navDoc.Descendants(xhtmlNs + "nav")
                .FirstOrDefault(nav => (string)nav.Attribute(epubNs + "type") == "toc");

            // Get the ol element within the nav
            XElement ol = tocNav?.Element(xhtmlNs + "ol");

            // Get all li elements within the ol
            IEnumerable<XElement> lis = ol?.Elements(xhtmlNs + "li");

            // For each li, get the href and text of the a element
            foreach (XElement li in lis)
            {
                XElement a = li.Element(xhtmlNs + "a");
                string href = a?.Attribute("href")?.Value;
                //The split is because the href in the nav file may contain a fragment identifier
                ZipArchiveEntry entry = Utils.GetRelativeEntry(navEntry, href.Split('#')[0]);
                string text = a?.Value;
                contents.Add((text, entry));
            }

            return contents;

        }
        //For EPUB 2
        //untested
        private static List<(string, ZipArchiveEntry)> ListChaptersFromToc(ZipArchiveEntry standardsEntry)
        {

            // Get the toc.ncx entry
            ZipArchiveEntry tocNcxEntry = Utils.GetRelativeEntry(standardsEntry, "toc.ncx");

            if (tocNcxEntry is null)
            {
                throw new FileNotFoundException("toc.ncx file not found");
            }

            // Read the toc.ncx content
            var tocNcxTask = new StreamReader(tocNcxEntry.Open()).ReadToEndAsync();
            tocNcxTask.Wait();
            string tocNcxXml = tocNcxTask.Result;

            // Parse the toc.ncx content as an XML document
            XDocument tocNcxDoc = XDocument.Parse(tocNcxXml);

            // Get the navMap element
            XElement navMap = tocNcxDoc.Root.Element(ncxNs + "navMap");

            // Get all navPoint elements
            IEnumerable<XElement> navPoints = navMap.Elements(ncxNs + "navPoint");

            // Initialize the list of chapters
            List<(string, ZipArchiveEntry)> chapters = new List<(string, ZipArchiveEntry)>();

            // For each navPoint, get the text and content src
            foreach (XElement navPoint in navPoints)
            {
                string text = navPoint.Element(ncxNs + "navLabel").Element(ncxNs + "text").Value;
                string src = navPoint.Element(ncxNs + "content").Attribute("src").Value;

                // Get the ZipArchiveEntry for the chapter file
                ZipArchiveEntry entry = Utils.GetRelativeEntry(standardsEntry, src);

                // Add the chapter to the list
                chapters.Add((text, entry));
            }

            return chapters;
        }

        //Fallback
        private static List<(string, ZipArchiveEntry)> ListChaptersFromSpine(ZipArchiveEntry standardsEntry, XDocument standardsDoc)
        {
            // Define the OPF namespace
            XNamespace ns = "http://www.idpf.org/2007/opf";

            // Get the manifest and spine elements using the namespace
            XElement manifest = standardsDoc.Root.Element(ns + "manifest");
            XElement spine = standardsDoc.Root.Element(ns + "spine");

            // Get all itemrefs in the spine
            var itemrefs = spine.Elements(ns + "itemref");

            // Create a dictionary to map IDs to hrefs from the manifest
            var manifestItems = manifest.Elements(ns + "item")
                .ToDictionary(
                    item => item.Attribute("id").Value,
                    item => item.Attribute("href").Value
                );

            // Collect hrefs of the items in the spine
            List<(string, ZipArchiveEntry)> contents = new List<(string, ZipArchiveEntry)>();
            foreach (var itemref in itemrefs)
            {
                string idref = itemref.Attribute("idref").Value;
                if (manifestItems.TryGetValue(idref, out string href))
                {
                    //Presumes the contents are in the same directory as the standards file. Are they always? Dont know. If it is found that often they wont be, then more elaborate work is needed.
                    ZipArchiveEntry entry = Utils.GetRelativeEntry(standardsEntry, href);
                    contents.Add((entry.FullName, entry));
                }
            }

            return contents;
        }

        
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
                else if (line.Value.Name == xhtmlNs + "img" || line.Value.Name == svgNs + "svg")
                {
                    if (line.Value.Name == svgNs + "svg")
                    {
                        XElement imageElement = line.Value.Element(xlinkNs + "image");
                        if (imageElement != null)
                        {
                            indexedNodeLines.TryAdd(line.Key, ParseImageNode(chapter, imageElement, "http://www.w3.org/1999/xlink:href"));
                        }
                    }
                    else
                    {
                        indexedNodeLines.TryAdd(line.Key, ParseImageNode(chapter, line.Value, "src"));
                    }
                }

            });

            for (int i = 0; i < indexedNodeLines.Count; i++)
            {
                chapter.Lines.Add(indexedNodeLines[i]);
            }
            //await Task.Delay(10000);
            chapter.LoadStatus = LoadingStatus.Loaded;
            return true;
        }

        public static Task<List<Node>> ParseLine(Chapter chapter, XElement line)
        {
            try
            {
                if (line.Name == xhtmlNs + "p")
                {
                    return Task.FromResult(ParseTextNode(line));
                }

                else if (line.Name == xhtmlNs + "img" || line.Name == svgNs + "svg")
                {
                    if (line.Name ==  svgNs+ "svg")
                    {
                        XElement imageElement = line.Element(svgNs + "image");
                        if (imageElement != null)
                        {
                            return Task.FromResult(ParseImageNode(chapter, imageElement, xlinkNs+"href"));
                        }
                    }
                    else
                    {
                        return Task.FromResult(ParseImageNode(chapter, line, "src"));
                    }
                }

                return Task.FromResult(new List<Node>());
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error parsing line: {e.Message}");
                Debug.WriteLine($"Line: {line}");
                Debug.WriteLine(e.StackTrace);
                return Task.FromResult(new List<Node>());
            }
        
        }


        private static List<Node> ParseTextNode(XElement originalElement)
        {

            string line = GetParagraphText(originalElement);
            if (line == string.Empty)
            {
                return [new TextNode() { Text = "" }];
            }
            //Breaking lines into sentences for smoother morphological analysis
            string[] sentences = Regex.Split(line, separatorsRegex);
            List<Node> nodes = new List<Node>();
            for (int i = 0, n = sentences.Length; i < n; i++)
            {
                //Should never be zero, i think. If it happens, will cause a bug. Purposefully not checking to see if breaks.
                //Also, This means that the separators are not interactable as part of a word. This is not a problem, because separators ARE NOT words.
                if (sentences[i].Length == 1 && separatorsAsList.Contains(sentences[i]))
                {
                    if(nodes.Count == 0)
                    {
                        nodes.Add(new TextNode() { Text = sentences[i] });
                    } else
                    {
                        nodes[^1].Text += sentences[i];
                    }
                    continue;
                }

                List<JapaneseLexeme> lexemes = analyzer.Analyze(sentences[i]);
                foreach (var lexeme in lexemes)
                {
                    TextNode node = new TextNode();
                    node.Text = lexeme.Surface;
                    nodes.Add(node);
                    if (JapaneseAnalyzer.signicantCategories.Contains(lexeme.Category))
                    {
                        try
                        {
                            //Only one entry because i dont want to deal with multiple entries in the UI now.
                            node.EdrdgEntry = translator.Translate(lexeme.BaseForm)![0];
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

        private static List<Node> ParseImageNode(Chapter chapter, XElement originalElement, string srcAttribute)
        {
            string path = originalElement.Attribute(srcAttribute)!.Value;
            ZipArchiveEntry imageEntry = Utils.GetRelativeEntry(chapter.FileReference, path);
            return ParseImageNode(imageEntry);
        }

        private static List<Node> ParseImageNode(Chapter chapter, XElement originalElement, XName srcName)
        {
            string path = originalElement.Attribute(srcName)!.Value;
            ZipArchiveEntry imageEntry = Utils.GetRelativeEntry(chapter.FileReference, path);
            return ParseImageNode(imageEntry);
        }

        private static List<Node> ParseImageNode (ZipArchiveEntry imageEntry)
        {
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
            List<XElement> lines = doc.Descendants().Where(n => n.Name == xhtmlNs + "p" || n.Name == xhtmlNs + "img" || n.Name == svgNs+"svg").ToList();

            return Task.FromResult(lines);
        }

        public static async Task<List<XElement>> BreakChapterToLines(Chapter chapter)
        {
            string orginalXhtml = await new StreamReader(chapter.FileReference.Open()).ReadToEndAsync();

            return await BreakXhtmlToLines(orginalXhtml);
        }

        private static string GetParagraphText(XElement paragraph)
        {
            if (paragraph.IsEmpty || paragraph.HasElements && paragraph.Elements(xhtmlNs + "br").Count() >= 1 && paragraph.Value.Trim() == "")
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
