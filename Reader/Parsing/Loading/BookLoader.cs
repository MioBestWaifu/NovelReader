using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using Mio.Reader.Parsing.Structure;
using Mio.Reader.Services;

namespace Mio.Reader.Parsing.Loading
{
    //Could use partial loading and parsing
    internal abstract class BookLoader
    {
        protected Parser parser;
        //This class could contain a reference to a book, and methods such as these would require or allow setting any book or chapter to be worked.
        public abstract Task<BookMetadata> LoadMetadata(string path);

        public abstract Task<Book> IndexBook(BookMetadata metadata);

        /// <summary>
        /// Will parse each original line of the chapter into line of Node, reporting the index of the line as it finished them. 
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="progressReporter"></param>
        /// <returns></returns>
        public abstract Task ParseChapterContent(Chapter chapter, IProgress<int> progressReporter);

        /// <summary>
        /// Will load the content of the entiry chapter and break it into lines of ParsingElement.
        /// </summary>
        /// <param name="chapter"></param>
        /// <returns>The amount of lines</returns>
        public async Task<int> BreakChapterToLines(Chapter chapter)
        {
            return await parser.BreakChapterToLines(chapter);
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="configs"></param>
        /// <param name="imageParsingService"></param>
        /// <returns>An instance of the appropriate derivation of this class.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public static BookLoader GetLoader(string path, ConfigurationsService configs,ImageParsingService imageParsingService)
        {
            if (path.EndsWith(".epub"))
            {
                return new EpubLoader(configs,imageParsingService);
            }
            else
            {
                throw new NotSupportedException($"The {path.Split('.')[^1]} file type is not supported.");
            }
        }
    }
}
