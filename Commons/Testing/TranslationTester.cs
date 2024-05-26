using Maria.Commons.Communication;
using Maria.Commons.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Commons.Testing
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

        public static void TranslateSamplesJp(int delayInSeconds = 10, int intervalInSeconds = 0)
        {
            Task.Delay(delayInSeconds * 1000).Wait();
            //The morphological analyzer is for whatever reason turning this into nemureru. Maybe its just it or its dictionary being shit.
            //Anyway, should look into it. 
            string[] terms = { "眠る", "消えた", "見る", "行く", "来る", "話す", "聞く", "読む", "書く", "歩く" };
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
