using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class Epub
    {
        public EpubMetadata Metadata { get; private set; }
        ZipArchive zipReference;
        public List<(string, Chapter)> TableOfContents = [];

        public Epub(ZipArchive zip, EpubMetadata metadata)
        {
            zipReference = zip;
            Metadata = metadata;
        }
    }
}
