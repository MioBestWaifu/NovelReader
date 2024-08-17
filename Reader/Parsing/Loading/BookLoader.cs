using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using Mio.Reader.Parsing.Structure;
using Mio.Reader.Services;

namespace Mio.Reader.Parsing.Loading
{
    internal abstract class BookLoader
    {
        protected Parser parser;
        public abstract Task<BookMetadata> LoadMetadata(string path);

        /// <summary>
        /// Does not actually load the content of the Epub, but rather references and indexes the files in a organized fashion.
        /// The only contents that are loaded are the metadata and the table of contents.
        /// </summary>
        /// <param name="path"></param>
        public abstract Task<Book> LoadBook(BookMetadata metadata);

        public abstract Task LoadChapterContent(Chapter chapter);

        public async Task<int> BreakChapterToLines(Chapter chapter)
        {
            return await parser.BreakChapterToLines(chapter);
        }

        public static BookLoader GetLoader(string path, ConfigurationsService configs,ImageParsingService imageParsingService)
        {
            if (path.EndsWith(".epub"))
            {
                return new EpubLoader(configs,imageParsingService);
            }
            else
            {
                throw new NotSupportedException("The file type is not supported.");
            }
        }
    }
}
