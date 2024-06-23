using MessagePack;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Mio.Translation.Japanese.Edrdg
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
        /// From the re_pri tag.
        /// This is actually multiple tags. Should look in what to do about it later.
        /// </summary>
        /// 
        [Key(2)]
        public int Priority { get; set; }

        [Key(3)]
        public List<KanaProperty> Properties { get; set; }

        //There is also the re_nokanji tag, but i dont know what to do with it and cant make out how to declare it, so it will be missing here.

        public ReadingElement()
        {

        }

        [JsonConstructor]
        [SerializationConstructor]
        public ReadingElement(string reading, string readingRestriction, int priority, List<KanaProperty> properties)
        {
            Reading = reading;
            ReadingRestriction = readingRestriction;
            Priority = priority;
            Properties = properties;
        }

        public ReadingElement(XElement element)
        {
            Reading = element.Element("reb")!.Value;
            ReadingRestriction = element.Element("re_restr")?.Value;

            List<string> priorityStrings = element.Elements("re_pri").Select(x => x.Value).ToList();
            List<int> priorityInts = priorityStrings.Select(EdrdgEntry.ParsePriority).ToList();
            Priority = priorityInts.Count == 0 ? 50 : priorityInts.Min();

            Properties = new List<KanaProperty>();
            var reInfs = element.Elements("re_inf");
            foreach (var keInf in reInfs)
            {
                try
                {
                    var kanaProperty = PropertyConverter.StringToKanaProperty(keInf.Value);
                    Properties.Add(kanaProperty);
                }
                catch
                {
                    //This catch is just to ignore errors. TODO log this to some 
                }
            }
        }
    }
}
