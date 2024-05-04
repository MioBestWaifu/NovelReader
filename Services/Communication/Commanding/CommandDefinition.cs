using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Communication.Commanding
{
    public class CommandDefinition
    {
        public Dictionary<string, PrefixDefinition> Prefixes { get; set; }
        public SuffixDefinition Suffix { get; set; }
        public Dictionary<string, OptionDefinition> Options { get; set; }
    }
}
