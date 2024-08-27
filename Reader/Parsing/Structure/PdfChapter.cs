using System;
using System.Collections.Concurrent;
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
        private ConcurrentDictionary<int, List<int>> pageMap = [];
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

        public void AddLineMapping(int pdfPage, int lineIndex)
        {
            if (!pageMap.ContainsKey(pdfPage))
            {
                pageMap[pdfPage] = new List<int>();
            }
            pageMap[pdfPage].Add(lineIndex);
        }

        public List<PdfNode> GetLinesForPage(int pdfPage)
        {
            pdfPage += startPage;
            List<int> ints = pageMap[pdfPage];
            List<PdfNode> nodes = new List<PdfNode>();
            foreach (int i in ints)
            {
                nodes.Add(PdfLines[i]);
            }
            return nodes;
        }

        public bool IsLineInPage(int line, int page)
        {
            return pageMap.ContainsKey(page) && pageMap[page].Contains(line);
        }
    }
}
