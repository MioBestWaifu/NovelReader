using iText.Kernel.Pdf.Xobject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig.Core;

namespace Mio.Reader.Parsing
{
    //All of this is temporary, will be refactored.
    public class PdfParsingElement : ParsingElement
    {
        public bool IsImage { get; set; }
        public string Text { get; set; }
        public int Page { get; set; }
        public double FontSize { get; set; }
        public byte[] Image { get; set; }
        public string Extension { get; set; }
        public PdfRectangle BoundingBox { get; set; }

        public double RightMostPoint { get {
                return BoundingBox.BottomRight.X;
            } 
        }

        public double BottomMostPoint
        {
            get
            {
                return BoundingBox.BottomRight.Y;
            }
        }
    }
}
