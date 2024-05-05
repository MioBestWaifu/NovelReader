using Maria.Common.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Communication
{
    internal class Interpreter
    {
        public void ProcessComand (Command command)
        {
            Console.WriteLine($"{command}");
        }
    }
}
