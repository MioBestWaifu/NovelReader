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
        //Process process is some strange naming
        public override Task<int> Process(Command command)
        {
            throw new NotImplementedException();
        }

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
