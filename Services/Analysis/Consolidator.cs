using Maria.Services.Recordkeeping.Records;
using MessagePack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Analysis
{
    internal class Consolidator
    {
        /// <summary>
        /// Consolidates the records in the path (expected to be created by Writer) into a minute count of the day
        /// </summary>
        /// <param name="inputPath">The folder with the bins to consolidate</param>
        /// <param name="outputPath">The full path to the consolidated file, including its name</param>
        public static void ConsilidateRecords(string inputPath, string outputPath)
        {
            List<string> files = new List<string>(Directory.GetFiles(inputPath));
            
            List<TrackingRecord> records = new List<TrackingRecord>();
            foreach (string file in files)
            {
                records.AddRange(MessagePackSerializer.Deserialize<List<TrackingRecord>>(File.ReadAllBytes(file)));
            }

            //It is necessary because there is zero guarantee that the records are in order
            records.Sort((x, y) => x.Time.CompareTo(y.Time));

            Dictionary<string, double> minuteCounts = new Dictionary<string, double>();

            for (int i = 0; i < records.Count-1; i++)
            {
                TrackingRecord record = records[i];
                TimeSpan time = TimeSpan.Parse(record.Time);
                double currentRecordMinutes = time.Hours * 60 + time.Minutes + time.Seconds / 60.0;
                time = TimeSpan.Parse(records[i + 1].Time);
                double nextRecordMinutes = time.Hours * 60 + time.Minutes + time.Seconds / 60.0;
                double difference = nextRecordMinutes - currentRecordMinutes;
                if (minuteCounts.ContainsKey(record.Name))
                {
                    minuteCounts[record.Name] += difference;
                }
                else
                {
                    minuteCounts.Add(record.Name, difference);
                }
            }

            //Directory.Delete(inputPath, true);
            File.WriteAllBytes(outputPath, MessagePackSerializer.Serialize(minuteCounts));
        }
    }
}
