using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Maria.Services.Communication.Commanding
{
    public class OptionDefinition
    {
        //Temporarily with public sets
        [JsonInclude]
        public OptionValidationMethod ValidateBy { get; set; }

        [JsonInclude]
        public List<string>? Values { get; set; }

        [JsonInclude]
        public string TypeName { get; set; }
    }
}
