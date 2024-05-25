using Maria.Translation.Japanese.Edrdg;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Translation
{
    [MessagePackObject]
    public class ConversionEntry
    {
        [Key(0)]
        public string Key { get; set; }
        [Key(1)]
        public EdrdgEntry Value { get; set; }

        [SerializationConstructor]
        public ConversionEntry(string key, EdrdgEntry value)
        {
            Key = key;
            Value = value;
        }
    }
}
