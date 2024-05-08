using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maria.Services.Translation.Japanese.Edrdg
{
    internal class EdrdgEntry
    {
        public int EntryId { get; private set; }
        public List<KanjiElement> KanjiElements { get; private set; }
        public List<ReadingElement> ReadingElements { get; private set; }
        //I am yet to find an entry with multiple sense tags, but it is suposed to be possible.
        public List<SenseElement> SenseElements { get; private set; }

        public EdrdgEntry(XElement element)
        {
            KanjiElements = new List<KanjiElement>();
            ReadingElements = new List<ReadingElement>();
            SenseElements = new List<SenseElement>();

            EntryId = int.Parse(element.Element("ent_seq")!.Value);

            var kanjiElements = element.Elements("k_ele");
            foreach (var kanjiElement in kanjiElements)
            {
                KanjiElements.Add(new KanjiElement(kanjiElement));
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
