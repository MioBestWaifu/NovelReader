using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maria.Readers.Handlers
{
    //Maybe shoulde be EpubParser or something like that
    internal static class EpubFormatter
    {

        public static async Task<string> FindStandardsFile(string originalXml)
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


        public static List<string> ListChapters(string pathToStandards,string originalXml)
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
                    contentPaths.Add(pathToStandards+"/"+href);
                }
            }

            return contentPaths;
        }
    }
}
