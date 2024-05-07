using Maria.Common.Communication.Commanding;
using Maria.Services.Recordkeeping;
using Maria.Services.Recordkeeping.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Tracking
{
    internal class ProcessTracker : Tracker
    {

        public override async Task<int> Register(Command command)
        {
            //Duplicate code with BrowserTracker
            try
            {
                TrackingRecord record = new TrackingRecord();
                //not always true, may be window, depends on the process
                record.Name = command.Options["name"];
                TimeSpan timestamp = DateTime.Now.TimeOfDay;
                record.Time = timestamp.ToString(@"hh\:mm\:ss");
                await Writer.Instance.AddProcessRecord(record);
                return 200;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 500;
            }

        }

        public override bool Validate(Command command)
        {
            return true;
        }
    }
}
