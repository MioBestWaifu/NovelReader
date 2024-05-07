using Maria.Common.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Maria.Common.Communication
{
    //This is also inadequate. The delegate should return a message. The server should be stoppable and resumable. 
    //It is not made for the finished application.
    public class CommandServer
    {
        public delegate void CommandReceived(Command command);
        public event CommandReceived OnCommandReceived;
        private HttpListener httpListener;

        public CommandServer()
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:47100/");
            httpListener.Start();
            Task.Run(() => Listen());
        }

        public async Task Listen()
        {
            while (true)
            {
                HttpListenerContext context = await httpListener.GetContextAsync();
                RaiseCommandReceived(context);
            }
        }

        //This should have exception handling.
        private void RaiseCommandReceived(HttpListenerContext context)
        {
            string requestBody;

            using (Stream body = context.Request.InputStream)
            {
                using (StreamReader reader = new StreamReader(body, context.Request.ContentEncoding))
                {
                    requestBody = reader.ReadToEnd();
                    Console.WriteLine($"Received request payload: {requestBody}");
                }
            }

            Command command = JsonSerializer.Deserialize<Command>(requestBody);

            OnCommandReceived?.Invoke(command);

            context.Response.StatusCode = 200;
            context.Response.Close();
        }

    }
}
