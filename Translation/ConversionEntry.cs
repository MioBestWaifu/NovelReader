using MessagePack;
using Mio.Translation.Japanese.Edrdg;

namespace Mio.Translation
{
    [MessagePackObject]
    public class ConversionEntry
    {
        [Key(0)]
        public string Key { get; set; }
        [Key(1)]
        public EdrdgEntry Value { get; set; }
        [Key(2)]
        public int Priority { get; set; }

        [SerializationConstructor]
        public ConversionEntry(string key, EdrdgEntry value, int priority)
        {
            Key = key;
            Value = value;
            Priority = priority;
        }
    }
}
