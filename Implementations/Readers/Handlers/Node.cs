using Maria.Translation.Japanese.Edrdg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Readers.Handlers
{
    internal class Node
    {
        /// <summary>
        /// At translation time, the surface form of the node. After that, the surface form joined to nearby insignificant textual elements.
        /// </summary>
        public string Text { get; set; } = "";
        public EdrdgEntry? EdrdgEntry { get; set; }
    }
}
