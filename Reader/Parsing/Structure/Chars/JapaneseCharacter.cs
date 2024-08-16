using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure.Chars
{
    public class JapaneseCharacter(char literal)
    {
        public char Literal { get; set; } = literal;
    }
}
