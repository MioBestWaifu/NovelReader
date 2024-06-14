using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public record EpubMetadata (string Title, string Author, string Path,int Version, string CoverBase64, string Standards);
}
