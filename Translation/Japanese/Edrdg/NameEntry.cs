using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mio.Translation.Japanese.Edrdg
{
    [MessagePackObject]
    public class NameEntry : EdrdgEntry
    {
        //The document definition says it may be more than one, but in practice i have only seen one. Maybe some has, but i dont know.
        [Key(4)]
        public List<List<NameType>> NameTypes { get; private set; }

        private object nameTypesLock = new object();

        public NameEntry()
        {

        }

        [SerializationConstructor]
        public NameEntry(int entryId, List<KanjiElement>? kanjiElements, List<ReadingElement> readingElements, 
            List<SenseElement> senseElements, List<List<NameType>> types): base(entryId, kanjiElements, readingElements, senseElements)
        {
            NameTypes = types;
        }

        public NameEntry(XElement element) : base(element)
        {
            //To ensure the linking of reading to name type.
            ReadingElements.Clear();
            NameTypes = new List<List<NameType>>();
            NameTypes.Add(new List<NameType>());
            var readingElement = element.Element("r_ele");
            ReadingElements.Add(new ReadingElement(readingElement));
            var nameTypeElements = element.Element("trans").Elements("name_type");
            if(nameTypeElements != null)
            {
                foreach(var nameTypeElement in nameTypeElements)
                {
                    NameTypes[0].Add(PropertyConverter.StringToNameType(nameTypeElement.Value));
                }
            } else
            {
                NameTypes.Add([NameType.UnclassifiedName]);
            }
        }

        public void Append(NameEntry entry)
        {
            lock (nameTypesLock)
            {
                NameTypes.Add(entry.NameTypes[0]);
            }
            lock (readingElementsLock) {
                ReadingElements.Add(entry.ReadingElements[0]);
            }

        }
    }
}
