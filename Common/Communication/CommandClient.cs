using Maria.Common.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Maria.Common.Communication
{
    //This scheme does not suffice for long-running, multiple comms and will be expanded when needed.
    //It also is inadequate to handle timing-related prefixes
    public class CommandClient
    {
        //Adaptive port and inter-device communication may be achieved by simply editing this value.
        private static string serverUrl = "http://localhost:47100"; 
        private HttpClient httpClient;

        public CommandClient()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(serverUrl);
        }

        public void SendCommand(Command command)
        {
            httpClient.PostAsync($"/{command.Action}", new StringContent(JsonSerializer.Serialize(command))).Wait();
        }
    }
}
