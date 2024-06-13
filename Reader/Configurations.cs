using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader
{
    internal class Configurations
    {
        public string PathToUnidic { get; set; }
        public static Configurations Current { get; } = new Configurations();
        private Configurations()
        {

        }

    }
}
