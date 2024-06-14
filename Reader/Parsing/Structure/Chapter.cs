using Mio.Reader.Parsing;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class Chapter
    {
        public string Title { get; set; } = "";
        public ZipArchiveEntry FileReference { get; private set; }
        public List<List<Node>> Lines { get; private set; } = [];
        public bool IsImagesOnly { get; private set; } = true;

        public LoadingStatus LoadStatus { get; set; } = LoadingStatus.Unloaded;
        public Chapter(ZipArchiveEntry fileReference)
        {
            FileReference = fileReference;
        }

        public void PrepareLines(int amount)
        {
            Lines = new List<List<Node>>(amount);
            for (int i = 0; i < amount; i++)
            {
                Lines.Add(new List<Node>());
            }
        }

        public void PushLineToIndex(int index, List<Node> line)
        {
            if(IsImagesOnly && line.Any(x => x is TextNode))
                IsImagesOnly = false;
            Lines[index] = line;
        }
    }
}
