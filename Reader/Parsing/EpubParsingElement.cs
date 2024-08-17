using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mio.Reader.Parsing
{
    public class EpubParsingElement : ParsingElement
    {
        public readonly XElement xElement;

        public EpubParsingElement(XElement element)
        {
            xElement = element;
        }
    }
}
