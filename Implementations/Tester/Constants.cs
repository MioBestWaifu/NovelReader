using Maria.Commons.Communication;
using Maria.Commons.Recordkeeping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Maria.Tester
{
    internal class Constants
    {
        public static Paths Paths { get; private set; }

        public static void Initialize(bool isDevelopment)
        {
            if (isDevelopment)
            {
                Paths = Serializer.DeserializeJson<Paths>(File.ReadAllText("paths.dev.json"))!;
            }
            else
            {
                Paths = Serializer.DeserializeJson<Paths>(File.ReadAllText("paths.json"))!;
            }
        }
    }
}
