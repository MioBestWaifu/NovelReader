using Maria.Commons.Communication;
using Maria.Commons.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Commons.Testing
{
    public class TrackProcessTester
    {
        /// <summary>
        /// Meant to be used in a thread. It puts the whole thread to sleep.
        /// </summary>
        /// <param name="intervalInSeconds"></param>
        public static void StartAndStop(int intervalInSeconds)
        {
            CommandClient commandClient = new CommandClient();
            Command command = new Command();
            command.Action = "start";
            command.Module = "tracking";
            command.Submodule = "process";
            commandClient.SendCommand(command);
            Thread.Sleep(intervalInSeconds * 1000);
            command.Action = "stop";
            commandClient.SendCommand(command);
        }
    }
}
