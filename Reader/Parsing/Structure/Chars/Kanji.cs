using Mio.Translation.Entries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure.Chars
{
    public class Kanji : JapaneseCharacter
    {
        public Kanji(char literal) : base(literal)
        {
        }

        public KanjidicEntry Entry { get; set; }
    }
}
