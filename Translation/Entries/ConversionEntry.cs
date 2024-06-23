using MessagePack;

namespace Mio.Translation.Entries
{
    [MessagePackObject]
    public class ConversionEntry
    {
        [Key(0)]
        public string Key { get; set; }
        [Key(1)]
        public DatabaseEntry Value { get; set; }
        [Key(2)]
        public int Priority { get; set; }

        [SerializationConstructor]
        public ConversionEntry(string key, DatabaseEntry value, int priority)
        {
            Key = key;
            Value = value;
            Priority = priority;
        }
    }
}
