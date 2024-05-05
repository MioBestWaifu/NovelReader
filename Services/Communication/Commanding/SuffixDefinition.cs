using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Maria.Services.Communication.Commanding
{
    public class SuffixDefinition
    {
        [JsonInclude]
        public bool Required { get; private set; }
        [JsonInclude]
        public List<string> Values { get; private set;}
    }
}
