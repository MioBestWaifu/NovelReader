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

        public static void StopJp(int delayInSeconds = 0)
        {
            Task.Delay(delayInSeconds * 1000).Wait();
            CommandClient commandClient = new CommandClient();
            Command command = new Command();
            command.Action = "stop";
            command.Module = "translation";
            command.Submodule = "jp";
            commandClient.SendCommand(command);
        }

        public static void TranslateSamplesJp(int delayInSeconds = 10,int intervalInSeconds = 0)
        {
            Task.Delay(delayInSeconds * 1000).Wait();
            string[] terms = ["消えた", "女性同性愛者", "手のひら"];
            CommandClient commandClient = new CommandClient();
            Command command = new Command();
            command.Action = "translate";
            command.Module = "translation";
            command.Submodule = "jp";
            foreach (string term in terms)
            {
                if (!command.Options.ContainsKey("term"))
                {
                    command.Options.Add("term", term);
                }
                else
                {
                    command.Options["term"] = term;
                }
                commandClient.SendCommand(command);
                Task.Delay(intervalInSeconds * 1000).Wait();
            }
        }

        public static void CreateJpDictionary(int delayInSeconds = 0)
        {
            Task.Delay(delayInSeconds * 1000).Wait();
            CommandClient commandClient = new CommandClient();
            Command command = new Command();
            command.Action = "create";
            command.Module = "translation";
            command.Submodule = "jp";
            commandClient.SendCommand(command);
        }   
    }
}
