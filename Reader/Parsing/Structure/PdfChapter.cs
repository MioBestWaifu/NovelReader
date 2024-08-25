using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class PdfChapter : Chapter
    {
        //Temporary, should refactor access to book
        public string pathToPdf;
        public int startPage;
        public int endPage;
        //Refactor this thing.
        public List<PdfNode> PdfLines { get; set; } = [];
        public PdfChapter(int startPage, int endPage)
        {
            this.startPage = startPage;
            this.endPage = endPage;
        }

        public PdfChapter(string title,int startPage, int endPage)
        {
            Title = title.Trim();
            this.startPage = startPage;
            this.endPage = -1;
        }
    }
}
