using Maria.Commons.Communication.Commanding;

namespace Maria.Commons.Communication
{
    public interface ICommandHandler
    {
        //To return a message, or an object as JSON.
        public Task<string> HandleCommand(Command command);
    }
}
