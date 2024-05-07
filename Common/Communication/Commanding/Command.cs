using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Common.Communication.Commanding
{
    public class Command
    {
        public string Action { get; set; } = "";
        public string Module { get; set; } = "";
        public string Submodule { get; set; } = "";
        public Dictionary<string, string> Options { get; set; } = [];

        public override string ToString()
        {
            return $"{Action} {Module} {Submodule}, {string.Join(',', Options)}";
        }
    }
}
