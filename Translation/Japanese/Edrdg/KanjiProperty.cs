using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Translation.Japanese.Edrdg
{
    public enum KanjiProperty
    {
        Ateji = 0, // ateji (phonetic) reading
        IrregularKanaUsage = 1, // word containing irregular kana usage
        IrregularKanjiUsage = 2, // word containing irregular kanji usage
        IrregularOkuriganaUsage = 3, // irregular okurigana usage
        OutdatedKanjiUsage = 4, // word containing out-dated kanji or kanji usage
        RarelyUsedKanji = 5, // rarely used kanji form
        SearchOnlyKanji = 6 // search-only kanji form
    }
}
