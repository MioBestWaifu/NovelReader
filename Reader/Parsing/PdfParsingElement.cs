using iText.Kernel.Pdf.Xobject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing
{
    //All of this is temporary, will be refactored.
    public class PdfParsingElement : ParsingElement
    {
        public bool IsImage { get; set; }
        public string Text { get; set; }
        public byte[] Image { get; set; }
        public string Extension { get; set; }
    }
}
