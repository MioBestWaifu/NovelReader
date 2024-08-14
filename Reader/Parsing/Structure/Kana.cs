using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class Kana : JapaneseCharacter
    {
        public bool IsYoon { get; private set; }
        //Used to render and translate yooned kana
        public string? Composition { get; set; }
        public Kana(char literal, bool isYoon) : base(literal)
        {
            IsYoon = isYoon;
        }

        public string Reading { get; set; } = "";
        //Could use some properties about the kana, like if it's a dakuten, or if is katakana or hiragana
    }
}
