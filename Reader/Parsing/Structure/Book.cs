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
        public List<(string, Chapter)> TableOfContents = [];

        public Book(BookMetadata metadata)
        {
            Metadata = metadata;
        }
    }
}
