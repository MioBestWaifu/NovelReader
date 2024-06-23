using MessagePack;
using Mio.Translation.Properties;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Mio.Translation.Elements
{
    //A bunch of information contained in the sense tag has been ignored for simplicity's sake. Some of them ARE important 
    //for a full implementation, but they are not essential for a basic, working one and so will be left for later.
    [MessagePackObject]
    public class SenseElement
    {
        [Key(0)]
        public List<string> Glosses { get; private set; }

        [Key(1)]
        public List<FieldProperty> Fields { get; private set; }

        [Key(2)]
        public List<MiscProperty> MiscProperties { get; private set; }

        [JsonConstructor]
        [SerializationConstructor]
        public SenseElement(List<string> glosses, List<FieldProperty> fieldProperties, List<MiscProperty> miscProperties)
        {
            Glosses = glosses;
            Fields = fieldProperties;
            MiscProperties = miscProperties;
        }
        public SenseElement(XElement element)
        {
            Glosses = new List<string>();
            Fields = new List<FieldProperty>();
            MiscProperties = new List<MiscProperty>();

            var glosses = element.Elements("gloss");
            foreach (var gloss in glosses)
            {
                Glosses.Add(gloss.Value);
            }

            var fields = element.Elements("field");
            foreach (var field in fields)
            {
                try
                {
                    var fieldProperty = PropertyConverter.StringToFieldProperty(field.Value);
                    Fields.Add(fieldProperty);
                }
                catch
                {
                    //This catch is just to ignore errors. TODO log this to some file.
                }
            }

            var miscs = element.Elements("misc");
            foreach (var misc in miscs)
            {
                try
                {
                    var miscProperty = PropertyConverter.StringToMiscProperty(misc.Value);
                    MiscProperties.Add(miscProperty);
                }
                catch
                {
                    //This catch is just to ignore errors. TODO log this to some file.
                }
            }
        }
    }
}