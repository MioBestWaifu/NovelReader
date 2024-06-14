using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class EpubInteraction (EpubMetadata metadata)
    {
        public EpubMetadata Metadata { get; set; } = metadata;
        public int LastPage { get; set; } = 0;
        //Because the number of pages in a chapter might change if the user is vieweing in different window sizes.
        public int LastTimeNumberOfPages { get; set; } = 0;
        public int LastChapter { get; set; } = 0;
        public DateTime LastRead { get; set; }
    }
}
