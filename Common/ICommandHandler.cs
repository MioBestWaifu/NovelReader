using Maria.Common.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Common
{
    public interface ICommandHandler
    {
        //To return a message, or an object as JSON.
        public Task<string> HandleCommand(Command command);
    }
}
