﻿using Maria.Commons.Communication;
using Maria.Commons.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Commons.Testing
{
    public class TrackBrowserTester
    {
        public static async void Start(int intervalInSeconds = 0)
        {
            Task.Delay(intervalInSeconds * 1000).Wait();
            CommandClient commandClient = new CommandClient();
            Command command = new Command();
            command.Action = "start";
            command.Module = "tracking";
            command.Submodule = "browser";
            commandClient.SendCommand(command);
        }

        public static async void Stop(int intervalInSeconds = 0)
        {
            Task.Delay(intervalInSeconds * 1000).Wait();
            CommandClient commandClient = new CommandClient();
            Command command = new Command();
            command.Action = "stop";
            command.Module = "tracking";
            command.Submodule = "browser";
            commandClient.SendCommand(command);
        }

        public static async void Add(string url, string title, int amount, int startDelay = 0, int intervalInSeconds = 0)
        {
            Task.Delay(startDelay * 1000).Wait();
            CommandClient commandClient = new CommandClient();
            Command command = new Command();
            command.Action = "add";
            command.Module = "tracking";
            command.Submodule = "browser";
            command.Options = new Dictionary<string, string>
            {
                { "url", url },
                { "title", title }
            };
            for (int i = 0; i < amount; i++)
            {
                commandClient.SendCommand(command);
                Task.Delay(intervalInSeconds * 1000).Wait();
            }
        }

        public static async void Mock(int intervalInSeconds, int hour)
        {
            Task.Delay(intervalInSeconds * 1000).Wait();
            CommandClient commandClient = new CommandClient();
            Command command = new Command();
            command.Action = "mock";
            command.Module = "tracking";
            command.Submodule = "browser";
            command.Options = new Dictionary<string, string>
            {
                { "hour", hour.ToString()}
            };
            commandClient.SendCommand(command);
        }
    }
}