using MessagePack;
using Mio.Translation.Elements;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Mio.Translation.Entries
{
    //This is terrible inheritance, but is required for the MessagePack union serialization.
    [MessagePackObject]
    public class JmdictEntry : DatabaseEntry
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
