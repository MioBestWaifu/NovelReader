using MessagePack;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Maria.Translation.Japanese.Edrdg
{
    //A bunch of information contained in the sense tag has been ignored for simplicity's sake. Some of them ARE important 
    //for a full implementation, but they are not essential for a basic, working one and so will be left for later.
    [MessagePackObject]
    public class SenseElement
    {
        [Key(0)]
        public List<string> Glosses { get; private set; }

        [JsonConstructor]
        [SerializationConstructor]
        public SenseElement(List<string> glosses)
        {
            Glosses = glosses;
        }
        public SenseElement(XElement element)
        {
            Glosses = new List<string>();
            var glosses = element.Elements("gloss");
            foreach (var gloss in glosses)
            {
                Glosses.Add(gloss.Value);
            }
        }
    }
}