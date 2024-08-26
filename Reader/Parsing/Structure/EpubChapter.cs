using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class EpubChapter : Chapter
    {
        public bool IsImagesOnly { get; private set; } = true;
        public ZipArchiveEntry FileReference { get; private set; }

        public EpubChapter(ZipArchiveEntry fileReference)
        {
            FileReference = fileReference;
        }

        public override void PushLineToIndex(int index, List<Node> line)
        {
            if (IsImagesOnly && line.Any(x => x is TextNode))
                IsImagesOnly = false;
            base.PushLineToIndex(index, line);
        }
    }
}
