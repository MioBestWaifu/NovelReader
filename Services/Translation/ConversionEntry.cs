using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Maria.Services.Translation
{
    internal class ConversionEntry
    {
        public string Key { get; private set; }
        public int File { get; private set; }
        public int Offset { get; private set; }
        //For optimzation in the future.
        [JsonIgnore]
        public int Used { get; private set; }

        public ConversionEntry(string key, int file, int offset)
        {
            Key = key;
            File = file;
            Offset = offset;
            Used = 0;
        }

    }
}
