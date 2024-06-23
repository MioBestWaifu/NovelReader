using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mio.Translation.Japanese.Edrdg
{
    [MessagePackObject]
    [Union(0, typeof(JmdictEntry))]
    [Union(1, typeof(NameEntry))]
    public abstract class EdrdgEntry
    {
        [Key(0)]
        public int EntryId { get; private set; }
        [Key(1)]
        public List<KanjiElement>? KanjiElements { get; private set; }
        [Key(2)]
        public List<ReadingElement> ReadingElements { get; private set; }
        //I am yet to find an entry with multiple sense tags, but it is suposed to be possible.
        [Key(3)]
        public List<SenseElement> SenseElements { get; private set; }

        [IgnoreMember]
        public object readingElementsLock = new object();

        public EdrdgEntry()
        {

        }
        [JsonConstructor]
        [SerializationConstructor]
        public EdrdgEntry(int entryId, List<KanjiElement>? kanjiElements, List<ReadingElement> readingElements, List<SenseElement> senseElements)
        {
            EntryId = entryId;
            KanjiElements = kanjiElements;
            ReadingElements = readingElements;
            SenseElements = senseElements;
        }

        public EdrdgEntry(XElement element)
        {
            ReadingElements = new List<ReadingElement>();
            SenseElements = new List<SenseElement>();

            EntryId = int.Parse(element.Element("ent_seq")!.Value);

            var kanjiElements = element.Elements("k_ele");
            KanjiElements = new List<KanjiElement>();
            if (kanjiElements.Any())
            {
                foreach (var kanjiElement in kanjiElements)
                {
                    KanjiElements.Add(new KanjiElement(kanjiElement));
                }
            }

            var readingElements = element.Elements("r_ele");
            foreach (var readingElement in readingElements)
            {
                ReadingElements.Add(new ReadingElement(readingElement));
            }

            var senseElements = element.Elements("sense");
            foreach (var senseElement in senseElements)
            {
                SenseElements.Add(new SenseElement(senseElement));
            }
        }

        public static int ParsePriority(string notation)
        {
            return notation switch
            {
                //Values other than of nfXX are arbitrary and should be revised.
                "ichi1" => 15,
                "news1" => 15,
                "spec1" => 15,
                "gai1" => 15,
                "ichi2" => 30,
                "news2" => 30,
                "spec2" => 30,
                "gai2" => 30,
                _ => DetermineNfPriority(notation)
            };
        }

        public static int DetermineNfPriority(string notation)
        {
            try
            {
                string numericalPart = notation.Substring(2);
                return int.Parse(numericalPart);
            } catch (Exception e)
            {
                return 50;
            }
        }
    }
}
