using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maria.Readers.Handlers
{
    //Maybe shoulde be EpubParser or something like that
    //Also, thats a lot of stuff. Maybe it should be split into multiple classes.
    internal static class EpubFormatter
    {

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
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

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
