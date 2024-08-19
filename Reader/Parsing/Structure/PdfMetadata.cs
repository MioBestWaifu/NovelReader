using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class PdfMetadata : BookMetadata
    {
        public float MarginRight { get; set; }
        public float MarginBottom { get; set; }
        public PaperSize PaperSize { get; set; }

        public PdfMetadata(string title, string author, string path, string coverBase64) : base(title, author, path, coverBase64)
        {
        }
    }
}
