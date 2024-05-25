using Maria.Common.Communication.Commanding;

namespace Maria.Tracking
{
    internal class BrowserTracker : Tracker
    {

        public BrowserTracker()
        {
            Running = true;
        }

        //Commented due to pending design decision on who should be responsible for saving the records
        public override async Task<int> Register(Command command)
        {
            //Duplicate code with ProcessTracker
            /*try
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
                if (command.Options.TryGetValue("title", out string extra))
                {
                    record.Extra = extra;
                }
                await Writer.Instance.AddBrowserRecord(record);
                return 200;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 500;
            }*/

            return 501;

        }

        public override bool Validate(Command command)
        {
            return true;
        }

        //Same as above
        public override void CreateMocks(Command command)
        {
            /*int hour = int.Parse(command.Options["hour"]);
            string[] possibleUrls = { "youtube.com", "facebook.com", "twitter.com", "twitch.tv" };
            List<TrackingRecord> records = new List<TrackingRecord>();
            Random random = new Random();

            for (int i = 0; i < 25; i++)
            {
                TrackingRecord record = new TrackingRecord();
                record.Name = possibleUrls[random.Next(possibleUrls.Length)];
                int minute = random.Next(0, 60);
                int second = random.Next(0, 60);
                record.Time = new TimeSpan(hour, minute, second).ToString(@"hh\:mm\:ss");
                records.Add(record);
            }
            Directory.CreateDirectory($@"{Constants.Paths.ToTracking}\browser\{DateTime.Now.Year}\{DateTime.Now.Month}\{DateTime.Now.Day}");

            byte[] data = MessagePackSerializer.Serialize(records);
            File.WriteAllBytes($@"{Constants.Paths.ToTracking}\browser\{DateTime.Now.Year}\{DateTime.Now.Month}\{DateTime.Now.Day}\{hour}-{random.Next(10)}.bin", data);*/
        }
    }
}
