using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Readers.Handlers
{
    internal class Chapter
    {
        public string Title { get; set; } = "";
        public ZipArchiveEntry FileReference { get; private set; }

        public Chapter (ZipArchiveEntry fileReference)
        {
            FileReference = fileReference;
        }
    }
}
