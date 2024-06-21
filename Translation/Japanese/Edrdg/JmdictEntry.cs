using MessagePack;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Mio.Translation.Japanese.Edrdg
{
    [MessagePackObject]
    public class JmdictEntry : EdrdgEntry
    {
        public JmdictEntry()
        {

        }

        [SerializationConstructor]
        public JmdictEntry(int entryId, List<KanjiElement>? kanjiElements, List<ReadingElement> readingElements,
            List<SenseElement> senseElements) : base(entryId, kanjiElements, readingElements, senseElements)
        {
        }

        public JmdictEntry(XElement element) : base(element)
        {
        }
    }
}
