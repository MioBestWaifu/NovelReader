using Maria.Common.Communication.Commanding;

namespace Maria.Common.Communication
{
    public interface ICommandHandler
    {
        //To return a message, or an object as JSON.
        public Task<string> HandleCommand(Command command);
    }
}
