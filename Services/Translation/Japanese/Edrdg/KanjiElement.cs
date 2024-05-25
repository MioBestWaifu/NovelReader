using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maria.Translation.Japanese.Edrdg
{
    /// <summary>
    /// From the k_ele tag.
    /// </summary>
    /// 

    //Public due the needs of MessagePack. Altough maybe this should be in Common anyway.
    [MessagePackObject]
    public class KanjiElement
    {
        /// <summary>
        /// From the keb tag.
        /// </summary>
        [Key(0)]
        public string Kanji { get; private set; }
        //From the ke_inf tag. I dont know what to do with this and it doenst seem necessary now, so i will declare it
        //but not use it.
        [IgnoreMember]
        private string Info { get; set; }
        //This is actually multiple tags. Should look in what to do about it later.
        [IgnoreMember]
        private JapanesePriority Priority { get; set; }

        public KanjiElement()
        {

        }

        [JsonConstructor]
        [SerializationConstructor]
        public KanjiElement(string kanji)
        {
            Kanji = kanji;
        }

        public KanjiElement(XElement element)
        {
            Kanji = element.Element("keb")!.Value;
        }
    }
}
