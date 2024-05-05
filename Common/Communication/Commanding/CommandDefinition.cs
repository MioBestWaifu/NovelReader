using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Maria.Common.Communication.Commanding
{
    public class CommandDefinition
    {
        //Temporarily with public sets
        [JsonInclude]
        public Dictionary<string, PrefixDefinition> Prefixes { get; set; }
        [JsonInclude]
        public SuffixDefinition Suffix { get; set; }
        [JsonInclude]
        public Dictionary<string, OptionDefinition> Options { get; set; }
    }
}
