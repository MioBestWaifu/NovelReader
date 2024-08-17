using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class Epub : Book
    {
        ZipArchive zipReference;
        public Epub(ZipArchive archive, BookMetadata metadata) : base(metadata)
        {
            zipReference = archive;
        }
    }
}
