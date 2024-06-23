using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Translation.Japanese.Edrdg
{
    public enum KanaProperty
    {
        Gikun, // gikun (meaning as reading) or jukujikun (special kanji reading)
        IrregularKanaUsage, // word containing irregular kana usage
        OutdatedKanaUsage, // out-dated or obsolete kana usage
        RarelyUsedKanaForm, // rarely used kana form
        SearchOnlyKanaForm // search-only kana form
    }
}
