using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Maria.Common.Communication.Commanding
{
    public class SuffixDefinition
    {
        //Temporarily with public sets
        [JsonInclude]
        public bool Required { get; set; }
        [JsonInclude]
        public List<string> Values { get; set; }
    }
}
