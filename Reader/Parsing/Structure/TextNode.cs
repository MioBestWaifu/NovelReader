using Mio.Translation.Japanese.Edrdg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class TextNode : Node
    {
        public List<JmdictEntry>? JmdictEntries { get; set; }

    }
}
