using CsvHelper;
using Maria.Services.Recordkeeping.Records;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Recordkeeping
{
    //Designed to be a singleton
    internal class Writer
    {
        //Should be customizable
        public string BasePath { get; set; } = @"D:\Programs\maria-chan\Tests\";

        private List<TrackingRecord> browsersBuffer = new List<TrackingRecord>();
        private List<TrackingRecord> processesBuffer = new List<TrackingRecord>();

        private SemaphoreSlim browsersSemaphore = new SemaphoreSlim(1, 1);
        private SemaphoreSlim processesSemaphore = new SemaphoreSlim(1, 1);
        

        public async Task AddBrowserRecord(TrackingRecord record)
        {
            await browsersSemaphore.WaitAsync();
            browsersBuffer.Add(record);
            browsersSemaphore.Release();
        }

        public async Task AddProcessRecord(TrackingRecord record)
        {
            await processesSemaphore.WaitAsync();
            processesBuffer.Add(record);
            processesSemaphore.Release();
        }

        public async Task FlushAll()
        {
            await browsersSemaphore.WaitAsync();
            await processesSemaphore.WaitAsync();

            /*
             * The fact that no checks of record's times against this value are made mean that the manager of the instance
             * is responsible for calling this method at the right time. One obvious circunstance just as the day is about to turn.
             * It could be checked, but with too many records would be resource intensive, unless done in a smart way and at the time of implementation i dont feel like putting time into it.
             * Another possible solution to the day-synchronization problem would be to have a function somewhere that 
             * revises the first and last file of days to sync them properly, this function being called in opportune times.
             * Of course, by the same logic there may be syncronization problems between any two flushes, but as of 
             * implementation they seem unimportant since the multiple flushes in a day are only for keeping files relatively small.
             */
            DateTime now = DateTime.Now;
            string browsersPath = @$"{BasePath}\browsers\{now.Year}\{now.Month}\{now.Day}\{now.Hour}-{now.Minute}-{now.Second}.csv";
            string processesPath = @$"{BasePath}\processes\{now.Year}\{now.Month}\{now.Day}\{now.Hour}-{now.Minute}-{now.Second}.csv";

            Directory.CreateDirectory(Path.GetDirectoryName(browsersPath));
            Directory.CreateDirectory(Path.GetDirectoryName(processesPath));

            using (var writer = new StreamWriter(browsersPath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(browsersBuffer);
            }

            using (var writer = new StreamWriter(processesPath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(processesBuffer);
            }

            browsersBuffer.Clear();
            processesBuffer.Clear();

            processesSemaphore.Release();
            browsersSemaphore.Release();
        }

    }
}
