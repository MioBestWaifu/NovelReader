using Mio.Reader.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class PdfMetadata : BookMetadata
    {
        public double MarginRight { get; set; }
        public double MarginBottom { get; set; }
        //According to Pidpdf documentation, this is internal to the PDF and mean nothing absolutely. Still useful to compare texts (i hope).
        public double BodyFontSize { get; set; }

        public PaperSize PaperSize { get; set; }

        public PdfMetadata(string title, string author, string path, string coverBase64) : base(title, author, path, coverBase64)
        {
        }
    }
}
