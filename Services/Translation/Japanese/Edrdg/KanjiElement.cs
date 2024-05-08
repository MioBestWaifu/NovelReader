using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Translation.Japanese.Edrdg
{
    /// <summary>
    /// From the k_ele tag.
    /// </summary>
    internal class KanjiElement
    {
        /// <summary>
        /// From the keb tag.
        /// </summary>
        public string Kanji { get; private set; }
        //From the ke_inf tag. I dont know what to do with this and it doenst seem necessary now, so i will declare it
        //but not use it.
        private string Info { get; set; } 
        public JapanesePriority Priority { get; private set; }
    }
}
