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
        public bool Running { get; protected set; }

        public async Task<int> Process(Command command)
        {
            switch (command.Action)
            {
                case null:
                    return 400;
                case "add":
                    if (!Running)
                    {
                        return 403;
                    }
                    return await ValidateAndRegister(command);
                case "start":
                    Start();
                    return 200;
                case "stop":
                    Stop();
                    return 200;
                default:
                    return 400;
            }
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

        public virtual void Start()
        {
            Running = true;
        }

        public virtual void Stop()
        {
            Running = false;
        }
    }
}
