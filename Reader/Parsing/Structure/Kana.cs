using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class Kana : JapaneseCharacter
    {
        public Kana(char literal) : base(literal)
        {
        }

        public string Reading { get; set; } = "";
        //Could use some properties about the kana, like if it's a dakuten, or if is katakana or hiragana
    }
}
