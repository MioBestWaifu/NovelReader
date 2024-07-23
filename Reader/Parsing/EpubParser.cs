using Mio.Reader.Parsing.Structure;
using Mio.Reader.Services;

/* Unmerged change from project 'Reader (net8.0-windows10.0.19041.0)'
Before:
using Mio.Translation;
After:
using Mio.Reader.Utilitarians;
using Mio.Translation;
*/
using Mio.Reader.Utilitarians;
using Mio.Translation;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mio.Reader.Parsing
{

    internal static class EpubParser
    {

        //([,!?、。．。「」『』…．！？：；（）()'\"“”])";
        //include any other separators that might be missing
        //Why two fields? Class regex as above is fast for regexing, list is faster for comparing.
        private static readonly List<string> separatorsAsList = new List<string> { ",", "!", "?", "、", "。", "．", "「", "」", "『", "』", "…", "．", "！", "？", "：", "；", "（", "）", "(", ")", "'", "\"", "“", "”" };

        private static readonly string separatorsRegex = "([" + string.Join("", separatorsAsList.Select(Regex.Escape)) + "])";

        //Obviouslt breaks if configs is not assigned before analyzer, but that should never happen because this field is assinged in the very ConfigurationsService constructor.
        public static ConfigurationsService Configs { get; set; }
        public static Analyzer? analyzer;

        private static Translator translator = new Translator();

        private static ImageParsingService imageParser;

        public static void Initialize(ImageParsingService imageParsingService)
        {
            imageParser = imageParsingService;
        }
        public static async Task<List<Node>> ParseLine(Chapter chapter, XElement line)
        {
            try
            {
                if (line.Name == Namespaces.xhtmlNs + "p")
                {
                    return await ParseTextElement(line);
                }

                else if (line.Name == Namespaces.xhtmlNs + "img" || line.Name == Namespaces.svgNs + "svg")
                {
                    if (line.Name == Namespaces.svgNs + "svg")
                    {
                        XElement imageElement = line.Element(Namespaces.svgNs + "image");
                        if (imageElement != null)
                        {
                            return await ParseImageElement(chapter, imageElement, Namespaces.xlinkNs + "href");
                        }
                    }
                    else
                    {
                        return await ParseImageElement(chapter, line, "src");
                    }
                }

                //What this means is that this invalid line will not throw an error when the UI renders, but the UI 
                //will not render anything with it because it needs a child of Node, not Node itself. So it will count as a line but not take up space or throw errors.
                return [new Node()];
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error parsing line: {e.Message}");
                Debug.WriteLine($"Line: {line}");
                Debug.WriteLine(e.StackTrace);
                return [new Node()];
            }

        }


        private static async Task<List<Node>> ParseTextElement(XElement originalElement)
        {

            string line = GetParagraphText(originalElement);
            if (line == string.Empty)
            {
                return [new TextNode()];
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
                    if (nodes.Count == 0)
                    {
                        nodes.Add(new TextNode() { Characters = {new JapaneseCharacter(sentences[i][0])} });
                    }
                    else
                    {
                        //This will break if the previoues node was not a textnode, but that should never happen.
                        TextNode nodeToAppend = (TextNode)nodes[^1];
                        nodeToAppend.Characters.Add(new JapaneseCharacter(sentences[i][0]));
                    }
                    continue;
                }

                List<Lexeme> lexemes = analyzer.Analyze(sentences[i]);
                foreach (var lexeme in lexemes)
                {
                    TextNode node = new TextNode(lexeme);
                    List<JapaneseCharacter> chars = [];
                    foreach (var character in lexeme.Surface)
                    {
                        if (character == ' ' || character == '\n')
                        {
                            continue;
                        } else if (Analyzer.IsRomaji(character))
                        {
                            chars.Add(new Romaji(character));
                        } else if (Analyzer.IsKana(character))
                        {
                            Kana kana = new Kana(character);
                            chars.Add(kana);
                        } else
                        {
                            //Presumes everything else is a kanji. This may or may not be a sound idea.
                            Kanji kanji = new Kanji(character);
                            chars.Add(kanji);
                        }
                    }
                    node.Characters = chars;
                    nodes.Add(node);
                }
            }

            return nodes;
        }

        private static async Task<List<Node>> ParseImageElement(Chapter chapter, XElement originalElement, string srcAttribute)
        {
            string path = originalElement.Attribute(srcAttribute)!.Value;
            ZipArchiveEntry imageEntry = Utils.GetRelativeEntry(chapter.FileReference, path);
            return ParseImageElement(imageEntry);
        }

        private static async Task<List<Node>> ParseImageElement(Chapter chapter, XElement originalElement, XName srcName)
        {
            string path = originalElement.Attribute(srcName)!.Value;
            ZipArchiveEntry imageEntry = Utils.GetRelativeEntry(chapter.FileReference, path);
            return ParseImageElement(imageEntry);
        }

        private static List<Node> ParseImageElement(ZipArchiveEntry imageEntry)
        {
            Task<string> base64Task = imageParser.ParseImageEntryToBase64(imageEntry);
            base64Task.Wait();
            string base64 = base64Task.Result;
            string type = imageEntry.FullName.Split('.')[^1];

            return [new ImageNode() { Text = base64, Type = type }];
        }

        /// <summary>
        /// Breaks a Xhtml chapter into lines. Paragraphs are considered lines, irrespective of content. Images are considered lines as well.
        /// </summary>
        /// <param name="originalXhtml"></param>
        /// <returns>The </returns>
        private static Task<List<XElement>> BreakXhtmlToLines(string originalXhtml)
        {
            XDocument doc = XDocument.Parse(originalXhtml);

            List<XElement> lines = doc.Descendants().Where(n => n.Name == Namespaces.xhtmlNs + "p" || n.Name == Namespaces.xhtmlNs + "img" || n.Name == Namespaces.svgNs + "svg").ToList();

            return Task.FromResult(lines);
        }

        public static async Task<List<XElement>> BreakChapterToLines(Chapter chapter)
        {
            string orginalXhtml = await new StreamReader(chapter.FileReference.Open()).ReadToEndAsync();

            return await BreakXhtmlToLines(orginalXhtml);
        }

        private static string GetParagraphText(XElement paragraph)
        {
            if (paragraph.IsEmpty || paragraph.HasElements && paragraph.Elements(Namespaces.xhtmlNs + "br").Count() >= 1 && paragraph.Value.Trim() == "")
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
                    // Add the ruby content (kanji) and ignore rt tags, ensuring inner tags text is included without the tags
                    var rubyText = elementNode.Nodes().Where(n => !(n is XElement e && e.Name == Namespaces.xhtmlNs + "rt"))
                        .Select(n => n is XText ? n.ToString() : (n is XElement el ? el.Value : ""));
                    sb.Append(string.Join("", rubyText));
                }
            }

            return sb.ToString();
        }
    }
}
