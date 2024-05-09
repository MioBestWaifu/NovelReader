using Maria.Common.Communication;
using Maria.Common.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Common.Testing
{
    public class TranslationTester
    {
        public static void StartJp(int delayInSeconds = 0)
        {
            Task.Delay(delayInSeconds * 1000).Wait();
            CommandClient commandClient = new CommandClient();
            Command command = new Command();
            command.Action = "start";
            command.Module = "translation";
            command.Submodule = "jp";
            commandClient.SendCommand(command);
        }
    }
}
