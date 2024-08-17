using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class PdfChapter : Chapter
    {
        public int startPage;
        public int endPage;
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
