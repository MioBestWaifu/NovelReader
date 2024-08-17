using Mio.Reader.Parsing.Structure;
using Mio.Reader.Services;
using Mio.Reader.Utilitarians;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Loading
{
    internal class EpubLoader : BookLoader
    {
        
        private EpubMetadataResolver metadataResolver;

        public EpubLoader(ConfigurationsService configs,ImageParsingService imageParsingService) : base(configs, imageParsingService)
        {
            metadataResolver = new EpubMetadataResolver(imageParsingService);
            parser = new EpubParser(configs,imageParsingService);
        }
        public override async Task<BookMetadata> LoadMetadata(string path)
        {
            ZipArchive archive = ZipFile.OpenRead(path);
            var res = await metadataResolver.ResolveMetadata(path, archive);
            archive.Dispose();
            return res;
        }

        /// <summary>
        /// Does not actually load the content of the Epub, but rather references and indexes the files in a organized fashion.
        /// The only contents that are loaded are the metadata and the table of contents.
        /// </summary>
        /// <param name="path"></param>
        public override async Task<Book> IndexBook(BookMetadata metadata)
        {
            ZipArchive archive = ZipFile.OpenRead(metadata.Path);

            Epub epub = new Epub(archive, metadata);
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
                epub.TableOfContents.Add((pair.Item1, new EpubChapter(pair.Item2)));
            }

            return epub;
        }

        public override Task ParseChapterContent(Chapter chapter, IProgress<int> progressReporter)
        {
            List<Task> parsingTasks = new List<Task>();
            chapter.PrepareLines(chapter.OriginalLines.Count);
            for (int i = 0; i < chapter.OriginalLines.Count; i++)
            {
                int thisIteration = i;
                parsingTasks.Add(parser.ParseLine(chapter, i).ContinueWith(xTask=> { 
                    chapter.PushLineToIndex(thisIteration, xTask.Result);
                    progressReporter.Report(thisIteration);
                }));
            }

            return Task.WhenAll(parsingTasks);
        }

        public override async Task<bool> LoadCover(BookMetadata metadata)
        {
            ZipArchiveEntry coverEntry = await Utils.GetCoverEntry(metadata.Path, metadata.CoverRelativePath);
            metadata.CoverBase64 = await imageParser.ParseImageEntryToBase64(coverEntry);
            //Should really dispose it here? Maybe should keep around to not have to load one again. Maybe a service should manage ZipArchives
            coverEntry.Archive.Dispose();
            return metadata.CoverBase64 != "";
        }

        public override async Task<bool> LoadAndResizeCover(BookMetadata metadata, int newWidth, int newHeight)
        {
            ZipArchiveEntry coverEntry = await Utils.GetCoverEntry(metadata.Path, metadata.CoverRelativePath);
            metadata.CoverBase64 = await imageParser.ParseImageEntryToBase64(coverEntry, newWidth,newHeight);
            //Should really dispose it here? Maybe should keep around to not have to load one again. Maybe a service should manage ZipArchives
            coverEntry.Archive.Dispose();
            return metadata.CoverBase64 != "";
        }
    }
}
