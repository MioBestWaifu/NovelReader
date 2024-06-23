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
        //This is actually multiple tags. Should look in what to do about it later.
        [Key(1)]
        public int Priority { get; set; }

        [Key(2)]
        public List<KanjiProperty> Properties { get; set; }

        public KanjiElement()
        {

        }

        [JsonConstructor]
        [SerializationConstructor]
        public KanjiElement(string kanji, int priority, List<KanjiProperty> properties)
        {
            Kanji = kanji;
            Priority = priority;
            Properties = properties;
        }

        //All those constructors could use some refactoring.
        public KanjiElement(XElement element)
        {
            Kanji = element.Element("keb")!.Value;

            List<string> priorityStrings = element.Elements("ke_pri").Select(x => x.Value).ToList();
            List<int> priorityInts = priorityStrings.Select(EdrdgEntry.ParsePriority).ToList();
            Priority = priorityInts.Count == 0 ? 50 : priorityInts.Min();

            Properties = new List<KanjiProperty>();
            var keInfs = element.Elements("ke_inf");
            foreach (var keInf in keInfs)
            {
                try
                {
                    var kanjiProperty = PropertyConverter.StringToKanjiProperty(keInf.Value);
                    Properties.Add(kanjiProperty);
                }
                catch
                {
                    //This catch is just to ignore errors. TODO log this to some file.
                }
            }
        }
    }
}
