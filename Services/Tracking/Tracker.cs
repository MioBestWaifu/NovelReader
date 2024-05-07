using Maria.Common.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Tracking
{
    internal abstract class Tracker
    {
        
        public async Task<int> Process(Command command)
        {
            if (string.IsNullOrEmpty(command.Action))
            {
                return 400;
            } else if (command.Action == "add") //Should check if it is active once start-stop are implemented
            {
                return await ValidateAndRegister(command);
            }
            return 400;
        }

        public async Task<int> ValidateAndRegister(Command command)
        {
            bool isValid = Validate(command);
            if (!isValid)
            {
                return 400;
            }

            return await Register(command);
        }

        public abstract bool Validate(Command command);

        public abstract Task<int> Register(Command command);
    }
}
