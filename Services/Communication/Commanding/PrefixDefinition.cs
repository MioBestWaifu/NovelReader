using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Maria.Services.Communication.Commanding
{
    public class PrefixDefinition
    {
        //Temporarily with public sets
        [JsonInclude]
        public List<string> Mutex { get; set; }
    }
}
