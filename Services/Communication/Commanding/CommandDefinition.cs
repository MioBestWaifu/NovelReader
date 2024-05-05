using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Maria.Services.Communication.Commanding
{
    public class CommandDefinition
    {
        [JsonInclude]
        public Dictionary<string,PrefixDefinition> Prefixes { get; private set; }
        [JsonInclude]
        public SuffixDefinition Suffix { get; private set; }
        [JsonInclude]
        public Dictionary<string, OptionDefinition> Options { get; private set; }
    }
}
