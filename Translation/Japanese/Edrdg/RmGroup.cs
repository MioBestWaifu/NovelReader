using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Translation.Japanese.Edrdg
{
    [MessagePackObject]
    public class RmGroup
    {
        [Key(0)]
        public List<string> Readings { get; private set; }
        [Key(1)]
        public List<string> Meanings { get; private set; }

        public RmGroup(List<string> readings, List<string> meanings)
        {
            Readings = readings;
            Meanings = meanings;
        }
    }
}
