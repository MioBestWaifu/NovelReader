using Mio.Reader.Parsing.Structure.Chars;
using Mio.Translation;
using Mio.Translation.Entries;
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
        public NamedictEntry? NameEntry { get; set; }
        public List<JapaneseCharacter> Characters { get; set; } = [];

        public bool HasFinishedGeneral { get; set; }
        public bool HasFinishedNames { get; set; }
        public bool HasFinishedChars { get; set; }

        public readonly Lexeme? lexeme;

        public TextNode()
        {

        }

        public TextNode(Lexeme lexeme)
        {
            this.lexeme = lexeme;
        }

        private string JoinLiterals()
        {
            string x = string.Concat(Characters.Select(c => c.Literal));
            return x;
        }
    }
}
