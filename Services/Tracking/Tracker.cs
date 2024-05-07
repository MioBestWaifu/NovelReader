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
