using MessagePack;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Mio.Translation.Japanese.Edrdg
{
    /// <summary>
    /// From the k_ele tag.
    /// </summary>
    /// 
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
        [Key(1)]
        public int Priority { get; set; }

        public KanjiElement()
        {

        }

        [JsonConstructor]
        [SerializationConstructor]
        public KanjiElement(string kanji, int priority)
        {
            Kanji = kanji;
            Priority = priority;
        }

        //All those constructors could use some refactoring.
        public KanjiElement(XElement element)
        {
            Kanji = element.Element("keb")!.Value;

            List<string> priorityStrings = element.Elements("ke_pri").Select(x => x.Value).ToList();
            List<int> priorityInts = priorityStrings.Select(EdrdgEntry.ParsePriority).ToList();
            Priority = priorityInts.Count == 0 ? 50 : priorityInts.Min();
        }
    }
}
