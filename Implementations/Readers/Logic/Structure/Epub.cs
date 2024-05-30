using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Readers.Logic.Structure
{
    internal class Epub
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        private ZipArchive zipReference;
        public List<(string, Chapter)> TableOfContents { get; } = [];

        public Epub(ZipArchive zip)
        {
            zipReference = zip;
        }
    }
}
