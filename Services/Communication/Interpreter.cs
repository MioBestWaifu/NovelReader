using Maria.Common.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Communication
{
    internal class Interpreter
    {
        //Maybe this should return a message or something like that. as of now, the plan is for communication to be decoupled, so it doesnt make sense to return anything. 
        private Dictionary<string, ICommandHandler> handlers = new Dictionary<string, ICommandHandler>();
        public async Task ProcessCommand (Command command)
        {
            Console.WriteLine($"Command: {command}");
            bool hasHandlerRegistered = handlers.TryGetValue(command.Action, out ICommandHandler handler);
            if (hasHandlerRegistered)
            {
                Console.WriteLine($"Handler found for action: {command.Action}");
                await handler.HandleCommand(command);
            }
            else
            {
                Console.WriteLine($"No handler registered for action: {command.Action}");
            }
        }

        public void RegisterHandler(ICommandHandler handler, string action)
        { 
            handlers.Add(action, handler);
        }
    }
}
