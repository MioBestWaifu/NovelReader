using Maria.Common.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Tracking
{
    internal class TrackingCommandHandler : ICommandHandler
    {
        public async Task<string> HandleCommand(Command command)
        {
            int result;
            switch(command.Submodule)
            {
                case "browser":
                    result = await new BrowserTracker().Process(command);
                    break;
                case "process":
                    result = await new ProcessTracker().Process(command);
                    break;
                default:
                    return "Invalid action";
            }

            switch(result)
            {
                case 200:
                    return "Success";
                case 400:
                    return "Bad request";
                default:
                    return "Internal server error";
            }
        }
    }
}
