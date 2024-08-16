using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class Book
    {
        public BookMetadata Metadata { get; private set; }
        ZipArchive zipReference;
        public List<(string, Chapter)> TableOfContents = [];

        public Book(ZipArchive zip, BookMetadata metadata)
        {
            zipReference = zip;
            Metadata = metadata;
        }
    }
}
