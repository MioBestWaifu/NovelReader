using Maria.Commons.Communication;
using Maria.Commons.Communication.Commanding;

namespace Maria.Tracking
{
    internal class TrackingCommandHandler : ICommandHandler
    {
        // stop action returns "Sucess" even when nothing was running in the first place.
        // Of course, thats what it was coded to do, but that's shit design.
        public async Task<string> HandleCommand(Command command)
        {
            int result;
            switch (command.Submodule)
            {
                case "browser":
                    result = await new BrowserTracker().Process(command);
                    break;
                case "process":
                    result = await ProcessTracker.Instance.Process(command);
                    break;
                default:
                    return "Invalid action";
            }

            switch (result)
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
