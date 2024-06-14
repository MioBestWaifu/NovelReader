using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using Mio.Reader.Parsing.Structure;

namespace Mio.Reader.Parsing
{
    internal static class EpubLoader
    {
        public async static Task<EpubMetadata> LoadMetadata (string path)
        {
            ZipArchive archive = ZipFile.OpenRead(path);
            var res =  await EpubMetadataResolver.ResolveMetadata(path,archive);
            archive.Dispose();
            return res;
        }

        /// <summary>
        /// Does not actually load the content of the Epub, but rather references and indexes the files in a organized fashion.
        /// The only contents that are loaded are the metadata and the table of contents.
        /// </summary>
        /// <param name="path"></param>
        public async static Task<Epub> LoadEpub(EpubMetadata metadata)
        {
            ZipArchive archive = ZipFile.OpenRead(metadata.Path);

            Epub epub = new Epub(archive,metadata);
            //Not parallel because it is (probably) a small list.
            Dictionary<string, ZipArchiveEntry> namedEntries = new Dictionary<string, ZipArchiveEntry>();
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                Debug.WriteLine(entry.FullName);
                namedEntries[entry.FullName] = entry;
            }
            string standardOpf = await new StreamReader(namedEntries[metadata.Standards].Open()).ReadToEndAsync();

            List<(string, ZipArchiveEntry)> contents = EpubMetadataResolver.ResolveChapters(namedEntries[metadata.Standards], standardOpf);

            foreach (var pair in contents)
            {
                epub.TableOfContents.Add((pair.Item1, new Chapter(pair.Item2)));
            }

            return epub;
        }
    }
}
