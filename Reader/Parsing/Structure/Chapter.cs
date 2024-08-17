using Mio.Reader.Parsing.Loading;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public abstract class Chapter
    {
        public string Title { get; set; } = "";
        public List<ParsingElement> OriginalLines { get; set; }
        public List<List<Node>> Lines { get; private set; } = [];
        private int previousTotalTextNodes = 0;
        //Yes, this is horrible, but this property is only used to gauge progress in debug, so it doesn't matter.
        public int TotalTextNodes { get
            {
                try
                {
                    var x = Lines.SelectMany(x => x).Count(x => x is TextNode);
                    previousTotalTextNodes = x;
                    return x;
                }
                catch (Exception)
                {
                    return previousTotalTextNodes;
                }
            } 
        }

        public int FinishedTextNodes { get; set; }

        public LoadingStatus LoadStatus { get; set; } = LoadingStatus.Unloaded;
        

        public virtual void PrepareLines(int amount)
        {
            Lines = new List<List<Node>>(amount);
            for (int i = 0; i < amount; i++)
            {
                Lines.Add(new List<Node>());
            }
        }

        public virtual void PushLineToIndex(int index, List<Node> line)
        {
            Lines[index] = line;
        }
    }
}
