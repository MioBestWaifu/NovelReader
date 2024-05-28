using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Readers.Handlers
{
    internal static class EpubLoader
    {
        /// <summary>
        /// Does not actually load the content of the Epub, but rather references and indexes the files in a organized fashion.
        /// The only contents that are loaded are the metadata and the table of contents.
        /// </summary>
        /// <param name="path"></param>
        public async static Task<Epub> LoadEpub(string path)
        {
            ZipArchive archive = ZipFile.OpenRead(path);

            Epub epub = new Epub(archive);
            //Not parallel because it is (probably) a small list.
            Dictionary<string, ZipArchiveEntry> namedEntries = new Dictionary<string, ZipArchiveEntry>();
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                Debug.WriteLine(entry.FullName);
                namedEntries[entry.FullName] = entry;
            }

            string containerXml = await new StreamReader(namedEntries["META-INF/container.xml"].Open()).ReadToEndAsync();
            
            string standardOpfPath = await EpubFormatter.FindStandardsFile(containerXml);
            string standardOpf = await new StreamReader(namedEntries[standardOpfPath].Open()).ReadToEndAsync();
            string standardOpfFolderPath = string.Join("/", standardOpfPath.Split('/').Reverse().Skip(1).Reverse());

            List<string> contentPaths = EpubFormatter.ListChapters(standardOpfFolderPath,standardOpf);

            foreach (string itemPath in contentPaths)
            {
                ZipArchiveEntry entry = namedEntries[itemPath];
                epub.TableOfContents.Add((itemPath, new Chapter(entry)));
            }

            return epub;
        }
    }
}
