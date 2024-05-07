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
    internal class BrowserTracker : Tracker
    {

        public BrowserTracker()
        {
            Running = true;
        }


        public override async Task<int> Register(Command command)
        {
            //Duplicate code with ProcessTracker
            try
            {
                TrackingRecord record = new TrackingRecord();
                //I should register all the options expected somewhere. Also, validate should check if they are there
                record.Name = command.Options["url"];
                if (command.Options.TryGetValue("time", out string time))
                {
                    record.Time = time;
                }
                else
                {
                    TimeSpan timestamp = DateTime.Now.TimeOfDay;
                    record.Time = timestamp.ToString(@"hh\:mm\:ss");
                }
                await Writer.Instance.AddBrowserRecord(record);
                return 200;
            } catch (Exception e)
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
