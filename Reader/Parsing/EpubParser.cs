using Mio.Reader.Parsing.Structure;
using Mio.Reader.Parsing.Structure.Chars;
using Mio.Reader.Services;
using Mio.Reader.Utilitarians;
using Mio.Translation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mio.Reader.Parsing
{
    internal class EpubParser : Parser
    {
        public EpubParser(ConfigurationsService configs, ImageParsingService imageParsingService) : base(configs, imageParsingService)
        {
        }

        public override async Task<List<Node>> ParseLine(Chapter chapter, int lineIndex)
        {
            try
            {
                EpubParsingElement line = (chapter.OriginalLines[lineIndex] as EpubParsingElement);
                if (line.xElement.Name == Namespaces.xhtmlNs + "p")
                {
                    return await ParseTextElement(line);
                }

                else if (line.xElement.Name == Namespaces.xhtmlNs + "img" || line.xElement.Name == Namespaces.svgNs + "svg")
                {
                    if (line.xElement.Name == Namespaces.svgNs + "svg")
                    {
                        XElement imageElement = line.xElement.Element(Namespaces.svgNs + "image");
                        if (imageElement != null)
                        {
                            return await ParseImageElement((EpubChapter)chapter, imageElement, Namespaces.xlinkNs + "href");
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
                Debug.WriteLine($"Line: {lineIndex}");
                Debug.WriteLine(e.StackTrace);
                return [new Node()];
            }

        }


        protected override async Task<List<Node>> ParseTextElement(ParsingElement originalElement)
        {
            if (originalElement is EpubParsingElement epubElement)
            {
                return await ParseTextElement(epubElement);
            }
            else
            {
                throw new ArgumentException("EpubParser can only parse EpubParsingElements");
            }
        }

        protected async Task<List<Node>> ParseTextElement(EpubParsingElement originalElement)
        {

            string line = GetParagraphText(originalElement.xElement);
            return await ParseTextElement(line);
        }

        

        protected override Task<List<Node>> ParseImageElement(Chapter chapter, ParsingElement originalElement, string srcAttribute)
        {
            if (originalElement is EpubParsingElement epubElement && chapter is EpubChapter epubChapter)
            {
                return ParseImageElement(epubChapter, epubElement.xElement, srcAttribute);
            }
            else
            {
                throw new ArgumentException("EpubParser can only parse EpubParsingElements");
            }
        }

        protected async Task<List<Node>> ParseImageElement(EpubChapter chapter, XElement originalElement, string srcAttribute)
        {
            string path = originalElement.Attribute(srcAttribute)!.Value;
            ZipArchiveEntry imageEntry = Utils.GetRelativeEntry(chapter.FileReference, path);
            return await ParseImageElement(imageEntry);
        }

        private async Task<List<Node>> ParseImageElement(EpubChapter chapter, XElement originalElement, XName srcName)
        {
            string path = originalElement.Attribute(srcName)!.Value;
            ZipArchiveEntry imageEntry = Utils.GetRelativeEntry(chapter.FileReference, path);
            return await ParseImageElement(imageEntry);
        }

        private async Task<List<Node>> ParseImageElement(ZipArchiveEntry imageEntry)
        {
            string base64 = await imageParser.ParseImageEntryToBase64(imageEntry);
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

        public override async Task<int> BreakChapterToLines(Chapter chapter)
        {
            if (chapter is EpubChapter epubChapter)
            {
                return await BreakChapterToLines(epubChapter);
            }
            else
            {
                throw new ArgumentException("EpubParser can only parse EpubChapters");
            }
        }

        public async Task<int> BreakChapterToLines(EpubChapter chapter)
        {
            string orginalXhtml = await new StreamReader(chapter.FileReference.Open()).ReadToEndAsync();
            var x = await BreakXhtmlToLines(orginalXhtml);
            chapter.OriginalLines = [];
            foreach (var line in x)
            {
                chapter.OriginalLines.Add(new EpubParsingElement(line));
            }
            return x.Count;
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
