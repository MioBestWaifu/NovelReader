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
        [JsonInclude]
        public OptionValidationMethod ValidateBy { get; private set; }

        [JsonInclude]
        public List<string>? Values { get; private set; }

        [JsonInclude]
        public string TypeName { get; private set; };
    }
}
