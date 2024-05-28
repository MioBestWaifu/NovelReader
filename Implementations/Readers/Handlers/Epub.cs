using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Readers.Handlers
{
    internal class Epub
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        private ZipArchive zipReference;
        public List<(string, Chapter)> TableOfContents { get; } = [];
        //Images that are not their own "chapters" need to be referenced somehow as well

        public Epub(ZipArchive zip)
        {
            zipReference = zip;
        }
    }
}
