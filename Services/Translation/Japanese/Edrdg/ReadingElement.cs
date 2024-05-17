using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maria.Services.Translation.Japanese.Edrdg
{
    //Public due the needs of MessagePack. Altough maybe this should be in Common anyway.
    [MessagePackObject]
    public class ReadingElement
    {
        /// <summary>
        /// From the reb tag.
        /// </summary>
        /// 
        [Key(0)]
        public string Reading { get; private set; }
        /// <summary>
        /// From the re_restr tag. Holds a kanji, meaning that this ReadingElement is only valid for that kanji rendering.
        /// Maybe this should be a KanjiElement instead of a string.
        /// </summary>
        /// 
        [Key(1)]
        public string? ReadingRestriction { get; private set; }
        /// <summary>
        /// From the re_inf tag. I dont know what it means or what to do with it, so i will declare it but not use it.
        /// </summary>
        /// 
        [IgnoreMember]
        private string Info { get; set; }
        /// <summary>
        /// From the re_pri tag.
        /// This is actually multiple tags. Should look in what to do about it later.
        /// </summary>
        /// 
        [IgnoreMember]
        private JapanesePriority Priority { get; set; }

        //There is also the re_nokanji tag, but i dont know what to do with it and cant make out how to declare it, so it will be missing here.

        public ReadingElement()
        {

        }

        [JsonConstructor]
        [SerializationConstructor]
        public ReadingElement(string reading, string readingRestriction)
        {
            Reading = reading;
            ReadingRestriction = readingRestriction;
        }

        public ReadingElement(XElement element)
        {
            Reading = element.Element("reb")!.Value;
            ReadingRestriction = element.Element("re_restr")?.Value;
        }
    }
}
