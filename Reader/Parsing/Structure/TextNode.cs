using Mio.Translation.Japanese.Edrdg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class TextNode : Node
    {
        public override string Text
        {
            get => Characters != null ? JoinLiterals() : string.Empty;
            set => throw new NotImplementedException();
        }
        public List<JmdictEntry>? JmdictEntries { get; set; }
        public NameEntry? NameEntry { get; set; }
        public List<JapaneseCharacter> Characters { get; set; } = [];

        private string JoinLiterals()
        {
            string x = string.Concat(Characters.Select(c => c.Literal));
            return x;
        }
    }
}
