using Mio.Translation.Japanese.Edrdg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    internal class TextNode : Node
    {
        public EdrdgEntry? EdrdgEntry { get; set; }
    }
}
