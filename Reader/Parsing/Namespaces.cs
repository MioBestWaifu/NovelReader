using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mio.Reader.Parsing
{
    internal static class Namespaces
    {
        public static readonly XNamespace xhtmlNs = "http://www.w3.org/1999/xhtml";
        public static readonly XNamespace epubNs = "http://www.idpf.org/2007/ops";
        public static readonly XNamespace opfNs = "http://www.idpf.org/2007/opf";
        public static readonly XNamespace ncxNs = "http://www.daisy.org/z3986/2005/ncx/";
        public static readonly XNamespace svgNs = "http://www.w3.org/2000/svg";
        public static readonly XNamespace xlinkNs = "http://www.w3.org/1999/xlink";
    }
}
