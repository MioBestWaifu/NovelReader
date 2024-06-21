using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Translation.Japanese.Edrdg
{
    [MessagePackObject]
    public class TranslationElement
    {
        [Key(0)]
        public string Translation { get; private set; }

        [SerializationConstructor]
        public TranslationElement(string translation)
        {
            Translation = translation;
        }
    }
}
