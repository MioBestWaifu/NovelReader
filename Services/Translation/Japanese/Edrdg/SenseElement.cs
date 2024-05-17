using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maria.Services.Translation.Japanese.Edrdg
{
    //A bunch of information contained in the sense tag has been ignored for simplicity's sake. Some of them ARE important 
    //for a full implementation, but they are not essential for a basic, working one and so will be left for later.
    //Public due the needs of MessagePack. Altough maybe this should be in Common anyway.
    [MessagePackObject]
    public class SenseElement
    {
        [Key(0)]
        public List<string> Glosses { get; private set; }


        public SenseElement()
        {
        }

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