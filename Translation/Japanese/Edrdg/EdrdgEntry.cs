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
        [Key(3)]
        //I am yet to find an entry with multiple sense tags, but it is suposed to be possible.
        public List<SenseElement> SenseElements { get; private set; }

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
            if (kanjiElements.Any())
            {
                KanjiElements = new List<KanjiElement>();
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
    }
}
