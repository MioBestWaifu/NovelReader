using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Maria.Services.Translation
{
    //Public due to MessagePack
    [MessagePackObject]
    public class ConversionEntry
    {
        [Key(0)]
        public string Key { get; private set; }
        [Key(1)]
        public int File { get; private set; }
        [Key(2)]
        public int Offset { get; private set; }
        //For optimzation in the future.
        [JsonIgnore]
        [IgnoreMember]
        public int Used { get; private set; }

        [SerializationConstructor]
        public ConversionEntry(string key, int file, int offset)
        {
            Key = key;
            File = file;
            Offset = offset;
            Used = 0;
        }

    }
}
