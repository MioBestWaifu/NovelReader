using Maria.Common.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Tracking
{
    internal class ProcessTracker : Tracker
    {
        public override Task<int> Register(Command command)
        {
            throw new NotImplementedException();
        }

        public override bool Validate(Command command)
        {
            throw new NotImplementedException();
        }
    }
}
