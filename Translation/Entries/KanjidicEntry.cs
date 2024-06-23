using MessagePack;
using Mio.Translation.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mio.Translation.Entries
{
    [MessagePackObject]
    public class KanjidicEntry
    {
        [Key(0)]
        public char Literal { get; private set; }
        [Key(1)]
        public List<RmElement> RmGroups { get; private set; }

        [SerializationConstructor]
        public KanjidicEntry(char literal, List<RmElement> rmGroups)
        {
            Literal = literal;
            RmGroups = rmGroups;
        }

        public KanjidicEntry(XElement element)
        {
            Literal = element.Element("literal")!.Value[0];
            RmGroups = [];
            var rmElements = element.Element("reading_meaning").Elements("rmgroup");
            //nanori is being ignored here.
            foreach (var rmElement in rmElements)
            {
                var jaKunReadings = rmElement.Elements("reading")
                                     .Where(r => (string)r.Attribute("r_type") == "ja_kun")
                                     .Select(r => r.Value)
                                     .ToList();

                var jaOnReadings = rmElement.Elements("reading")
                                     .Where(r => (string)r.Attribute("r_type") == "ja_on")
                                     .Select(r => r.Value)
                                     .ToList();
                List<string> readings = [.. jaKunReadings, .. jaOnReadings];

                var meanings = rmElement.Elements("meaning")
                        .Where(m => m.Attribute("m_lang") == null) // Filter elements without the m_lang attribute
                        .Select(m => m.Value)
                        .ToList();

                RmGroups.Add(new RmElement(readings, meanings));
            }
        }
    }
}
