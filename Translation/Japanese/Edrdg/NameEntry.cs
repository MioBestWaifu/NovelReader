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
        //I think there is always only one, making it a list because if it isnt its easier to change.
        [Key(4)]
        public List<TranslationElement> TranslationElements { get; private set; }

        public NameEntry()
        {

        }

        [SerializationConstructor]
        public NameEntry(int entryId, List<KanjiElement>? kanjiElements, List<ReadingElement> readingElements, 
            List<SenseElement> senseElements, List<TranslationElement> translationElements): base(entryId, kanjiElements, readingElements, senseElements)
        {
            TranslationElements = translationElements;
        }

        public NameEntry(XElement element) : base(element)
        {
            TranslationElements = new List<TranslationElement>();
            var translations = element.Elements("trans");
            if(translations.Any())
            {
                foreach (var translation in translations)
                {
                    TranslationElements.Add(new TranslationElement(translation.Element("trans_det")!.Value));
                }
            }
        }
    }
}
