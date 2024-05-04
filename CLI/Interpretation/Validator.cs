using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.CLI.Interpretation
{
    internal class Validator
    {
        private static readonly List<string> validRoots = ["tracking","tell"];

        public static bool IsValidRoot(string word)
        {
           return validRoots.Contains(word);
        }
    }
}
