using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        //This is actually multiple tags. Should look in what to do about it later.
        private JapanesePriority Priority { get; set; }

        public KanjiElement(XElement element)
        {
            Kanji = element.Element("keb")!.Value;
        }
    }
}
