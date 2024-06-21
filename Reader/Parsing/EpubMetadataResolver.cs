using Mio.Reader.Parsing.Structure;
using Mio.Reader.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mio.Reader.Parsing
{
    internal static class EpubMetadataResolver
    {
        private static ImageParsingService imageParser;

        public static void Initialize(ImageParsingService imageParsingService)
        {
            imageParser = imageParsingService;
        }

        public async static Task<string> ResolveStandardsFile(string originalXml)
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

        public static async Task<EpubMetadata> ResolveMetadata(string path, ZipArchive archive)
        {
            //Not parallel because it is (probably) a small list.
            Dictionary<string, ZipArchiveEntry> namedEntries = new Dictionary<string, ZipArchiveEntry>();
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                namedEntries[entry.FullName] = entry;
            }

            string containerXml = await new StreamReader(namedEntries["META-INF/container.xml"].Open()).ReadToEndAsync();

            string standardOpfPath = await ResolveStandardsFile(containerXml);
            string standardsXml = await new StreamReader(namedEntries[standardOpfPath].Open()).ReadToEndAsync();
            XDocument standardsDoc = XDocument.Parse(standardsXml);
            XElement packageElement = standardsDoc.Root;
            XAttribute versionAttribute = packageElement.Attribute("version");
            int version = int.Parse(versionAttribute?.Value[0].ToString() ?? "0");



            string title = ResolveTitle(standardsDoc, version);
            List<string> authors = ResolveAuthors(standardsDoc, version);
            string pathToCover = ResolvePathToCover(standardsDoc, version);
            string coverBase64 = string.Empty;
            try
            {
                ZipArchiveEntry coverEntry = Utils.GetRelativeEntry(namedEntries[standardOpfPath], pathToCover);
                coverBase64 = await imageParser.ParseImageEntryToBase64(coverEntry,440,660);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading cover: {e.Message}");
            }

            return new EpubMetadata(title, string.Join(", ", authors), path, version, coverBase64,pathToCover, standardOpfPath);
        }

        private static string ResolveTitle(XDocument standardsDoc, int version)
        {
            // Define namespaces for EPUB 2 and EPUB 3
            XNamespace dcNs = "http://purl.org/dc/elements/1.1/";

            // Initialize title variable
            string title = string.Empty;

            if (version == 3)
            {
                // For EPUB 3, the metadata is under the package/metadata element
                XElement metadata = standardsDoc.Root.Element(Namespaces.opfNs + "metadata");
                if (metadata != null)
                {
                    // Find the first title element within the metadata section
                    XElement titleElement = metadata.Elements(dcNs + "title").FirstOrDefault();
                    if (titleElement != null)
                    {
                        title = titleElement.Value;
                    }
                }
            }
            else if (version == 2)
            {
                // For EPUB 2, the structure is similar, but handling is provided for consistency
                XElement metadata = standardsDoc.Root.Element(Namespaces.opfNs + "metadata");
                if (metadata != null)
                {
                    // Find the first title element within the metadata section
                    XElement titleElement = metadata.Elements(dcNs + "title").FirstOrDefault();
                    if (titleElement != null)
                    {
                        title = titleElement.Value;
                    }
                }
            }

            return title;
        }


        private static List<string> ResolveAuthors(XDocument standardsDoc, int version)
        {
            // Define the Dublin Core namespace
            XNamespace dcNs = "http://purl.org/dc/elements/1.1/";
            List<string> authors = new List<string>();

            // The metadata element is under the package/metadata for both EPUB 2 and EPUB 3
            XElement metadata = standardsDoc.Root.Element(Namespaces.opfNs + "metadata");
            if (metadata != null)
            {
                // Find all creator elements within the metadata section
                IEnumerable<XElement> authorElements = metadata.Elements(dcNs + "creator");
                foreach (var authorElement in authorElements)
                {
                    // For EPUB 3, we might also want to consider the role attribute to filter authors
                    if (version == 3)
                    {
                        XAttribute roleAttribute = authorElement.Attribute(Namespaces.opfNs + "role");
                        // If role attribute is present and equals 'aut' (author), or if role attribute is not used
                        if (roleAttribute == null || roleAttribute.Value == "aut")
                        {
                            authors.Add(authorElement.Value);
                        }
                    }
                    else
                    {
                        // For EPUB 2, simply add the author's name
                        authors.Add(authorElement.Value);
                    }
                }
            }

            return authors;
        }

        private static string ResolvePathToCover(XDocument standardDoc, int version)
        {
            string coverPath = string.Empty;

            if (version == 3)
            {
                XElement metadata = standardDoc.Root.Element(Namespaces.opfNs + "manifest");
                if (metadata != null)
                {
                    XElement coverItem = metadata.Elements(Namespaces.opfNs + "item")
                                                 .FirstOrDefault(m => (string)m.Attribute("properties") == "cover-image");
                    if (coverItem != null)
                    {
                        coverPath = coverItem.Attribute("href")?.Value;
                    }
                }
            }

            else if (version == 2)
            {
                XElement metadata = standardDoc.Root.Element(Namespaces.opfNs + "metadata");
                XElement coverMeta = metadata?.Elements(Namespaces.opfNs + "meta")
                                         .FirstOrDefault(r => (string)r.Attribute("name") == "cover");
                if (coverMeta != null)
                {
                    string coverId = coverMeta.Attribute("content")!.Value;
                    XElement coverRef = standardDoc.Root.Element(Namespaces.opfNs + "manifest")
                                              .Elements(Namespaces.opfNs + "item")
                                              .FirstOrDefault(i => (string)i.Attribute("id") == coverId);
                    coverPath = coverRef.Attribute("href")?.Value;
                }
            }

            return coverPath;
        }




        public static List<(string, ZipArchiveEntry)> ResolveChapters(ZipArchiveEntry standardsEntry, string standardsXml)
        {
            XDocument doc = XDocument.Parse(standardsXml);

            XElement packageElement = doc.Root;

            XAttribute versionAttribute = packageElement.Attribute("version");

            string version = versionAttribute?.Value;
            List<(string, ZipArchiveEntry)> chaptersFromSpine = ResolveChaptersFromSpine(standardsEntry, doc);
            List<(string, ZipArchiveEntry)> namedChapters = new List<(string, ZipArchiveEntry)>();

            try
            {
                if (version == "3.0")
                {
                    List<(string, ZipArchiveEntry)> chaptersFromNav = ResolveChaptersFromNav(standardsEntry, doc);
                    foreach (var spineChapter in chaptersFromSpine)
                    {
                        var navChapter = chaptersFromNav.FirstOrDefault(c => c.Item2 == spineChapter.Item2);
                        namedChapters.Add(navChapter.Item2 != null ? navChapter : spineChapter);
                    }
                }
                else if (version == "2.0")
                {
                    List<(string, ZipArchiveEntry)> chaptersFromToc = ResolveChaptersFromToc(standardsEntry);
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
        private static List<(string, ZipArchiveEntry)> ResolveChaptersFromNav(ZipArchiveEntry standardsEntry, XDocument standardsDoc)
        {
            XElement manifest = standardsDoc.Root.Element(Namespaces.opfNs + "manifest");

            XElement navItem = manifest.Elements(Namespaces.opfNs + "item")
                .FirstOrDefault(item => (string)item.Attribute("properties") == "nav");

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

            XDocument navDoc = XDocument.Parse(navXml);
            List<(string, ZipArchiveEntry)> contents = new List<(string, ZipArchiveEntry)>();

            XElement tocNav = navDoc.Descendants(Namespaces.xhtmlNs + "nav")
                .FirstOrDefault(nav => (string)nav.Attribute(Namespaces.epubNs + "type") == "toc");

            XElement ol = tocNav?.Element(Namespaces.xhtmlNs + "ol");

            IEnumerable<XElement> lis = ol?.Elements(Namespaces.xhtmlNs + "li");

            foreach (XElement li in lis)
            {
                XElement a = li.Element(Namespaces.xhtmlNs + "a");
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
        private static List<(string, ZipArchiveEntry)> ResolveChaptersFromToc(ZipArchiveEntry standardsEntry)
        {

            ZipArchiveEntry tocNcxEntry = Utils.GetRelativeEntry(standardsEntry, "toc.ncx");

            if (tocNcxEntry is null)
            {
                throw new FileNotFoundException("toc.ncx file not found");
            }

            var tocNcxTask = new StreamReader(tocNcxEntry.Open()).ReadToEndAsync();
            tocNcxTask.Wait();
            string tocNcxXml = tocNcxTask.Result;

            XDocument tocNcxDoc = XDocument.Parse(tocNcxXml);

            XElement navMap = tocNcxDoc.Root.Element(Namespaces.ncxNs + "navMap");

            IEnumerable<XElement> navPoints = navMap.Elements(Namespaces.ncxNs + "navPoint");

            List<(string, ZipArchiveEntry)> chapters = new List<(string, ZipArchiveEntry)>();

            foreach (XElement navPoint in navPoints)
            {
                string text = navPoint.Element(Namespaces.ncxNs + "navLabel").Element(Namespaces.ncxNs + "text").Value;
                string src = navPoint.Element(Namespaces.ncxNs + "content").Attribute("src").Value;

                ZipArchiveEntry entry = Utils.GetRelativeEntry(standardsEntry, src);

                chapters.Add((text, entry));
            }

            return chapters;
        }

        //Fallback
        private static List<(string, ZipArchiveEntry)> ResolveChaptersFromSpine(ZipArchiveEntry standardsEntry, XDocument standardsDoc)
        {

            XElement manifest = standardsDoc.Root.Element(Namespaces.opfNs + "manifest");
            XElement spine = standardsDoc.Root.Element(Namespaces.opfNs + "spine");

            var itemrefs = spine.Elements(Namespaces.opfNs + "itemref");

            var manifestItems = manifest.Elements(Namespaces.opfNs + "item")
                .ToDictionary(
                    item => item.Attribute("id").Value,
                    item => item.Attribute("href").Value
                );

            List<(string, ZipArchiveEntry)> contents = new List<(string, ZipArchiveEntry)>();
            foreach (var itemref in itemrefs)
            {
                string idref = itemref.Attribute("idref").Value;
                if (manifestItems.TryGetValue(idref, out string href))
                {
                    ZipArchiveEntry entry = Utils.GetRelativeEntry(standardsEntry, href);
                    contents.Add((entry.FullName, entry));
                }
            }

            return contents;
        }
    }
}
