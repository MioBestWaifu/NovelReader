using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Communication.Commanding
{
    public class Command
    {
        public List<string> Prefixes { get; set; }
        public string Action { get; set; }
        public List<string> Options { get; set; }
        public Dictionary<string, string> Modifiers { get; set; }

        public Command(string action)
        {
            Action = action;
        }
    }
}
