using Maria.Common.Communication.Commanding;
using Maria.Services.Tracking;
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
        private readonly Dictionary<string, ICommandHandler> handlers;

        public Interpreter()
        {
            handlers = new Dictionary<string, ICommandHandler>() {
                { "tracking",new TrackingCommandHandler()}
            };
        }
        public async Task ProcessCommand (Command command)
        {
            Console.WriteLine($"Command: {command}");
            bool hasHandlerRegistered = handlers.TryGetValue(command.Module, out ICommandHandler handler);
            if (hasHandlerRegistered)
            {
                Console.WriteLine($"Handler found for action: {command.Module}");
                await handler.HandleCommand(command);
            }
            else
            {
                Console.WriteLine($"No handler registered for module: {command.Module}");
            }
        }

        public void RegisterHandler(ICommandHandler handler, string module)
        { 
            handlers.Add(module, handler);
        }
    }
}
