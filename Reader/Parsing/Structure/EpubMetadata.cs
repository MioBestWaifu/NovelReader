using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class EpubMetadata : BookMetadata
    {

        public string CoverRelativePath { get; set; }
        public string Standards { get; set; }
        public int Version { get; set; }

        public EpubMetadata(string title, string author, string path, string coverBase64, string coverRelativePath, string standards, int version) : base(title, author, path, coverBase64)
        {
            CoverRelativePath = coverRelativePath;
            Standards = standards;
            Version = version;
        }
    }
}
