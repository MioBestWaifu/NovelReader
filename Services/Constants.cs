using Maria.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Maria.Services
{
    internal class Constants
    {
        public static Paths Paths { get; private set; }

        public static void Initialize (bool isDevelopment)
        {
            if (isDevelopment)
            {
                Paths = JsonSerializer.Deserialize<Paths>(File.ReadAllText("paths.dev.json"),CommandServer.jsonOptions)!;
            } else
            {
                Paths = JsonSerializer.Deserialize<Paths>(File.ReadAllText("paths.json"),CommandServer.jsonOptions)!;
            }
        }
    }
}
