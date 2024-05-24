using CsvHelper;
using Maria.Services.Recordkeeping.Records;
using MessagePack;
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
        public static Writer Instance { get; private set; }

        //Could use a concurrent collection. A Bag will do, because the order is only important in Analysis and it can sor that out.
        private List<TrackingRecord> browserBuffer = new List<TrackingRecord>();
        private List<TrackingRecord> processBuffer = new List<TrackingRecord>();

        private SemaphoreSlim browserSemaphore = new SemaphoreSlim(1, 1);
        private SemaphoreSlim processSemaphore = new SemaphoreSlim(1, 1);

        private Writer()
        {
            //This is a singleton
        }
        

        public async Task AddBrowserRecord(TrackingRecord record)
        {
            await browserSemaphore.WaitAsync();
            browserBuffer.Add(record);
            browserSemaphore.Release();
        }

        public async Task AddProcessRecord(TrackingRecord record)
        {
            await processSemaphore.WaitAsync();
            processBuffer.Add(record);
            processSemaphore.Release();
        }

        //Maybe this should be conditioned on wether there even are records to flush. Design choice to be made.
        /// <summary>
        /// Reruns every intervalInSeconds. Does not stop itself. To stop, kill instance
        /// </summary>
        /// <param name="intervalInSeconds"></param>
        /// <returns></returns>
        public async Task FlushAll(int intervalInSeconds)
        {
            while (true)
            {
                await Task.Delay(intervalInSeconds * 1000);
                await browserSemaphore.WaitAsync();
                await processSemaphore.WaitAsync();
                Console.WriteLine("FLUSHING");

                /*
                 * The fact that no checks of record's times against this value are made mean that the manager of the instance
                 * is responsible for calling this method at the right time. One obvious circumstance just as the day is about to turn.
                 * It could be checked, but with too many records would be resource intensive, unless done in a smart way and at the time of implementation i dont feel like putting time into it.
                 * Another possible solution to the day-synchronization problem would be to have a function somewhere that 
                 * revises the first and last file of days to sync them properly, this function being called in opportune times.
                 * Of course, by the same logic there may be syncronization problems between any two flushes, but as of 
                 * implementation they seem unimportant since the multiple flushes in a day are only for keeping files relatively small.
                 */
                DateTime now = DateTime.Now;
                string browserPath = @$"{Constants.Paths.ToTracking}\browser\{now.Year}\{now.Month}\{now.Day}\{now.Hour}-{now.Minute}-{now.Second}.bin";
                string processPath = @$"{Constants.Paths.ToTracking}\process\{now.Year}\{now.Month}\{now.Day}\{now.Hour}-{now.Minute}-{now.Second}.bin";

                Directory.CreateDirectory(Path.GetDirectoryName(browserPath));
                Directory.CreateDirectory(Path.GetDirectoryName(processPath));

                byte[] browserBytes = MessagePackSerializer.Serialize(browserBuffer);
                byte[] processBytes = MessagePackSerializer.Serialize(processBuffer);
                File.WriteAllBytes(browserPath, browserBytes);
                File.WriteAllBytes(processPath, processBytes);

                browserBuffer.Clear();
                processBuffer.Clear();

                processSemaphore.Release();
                browserSemaphore.Release();
            }
        }

        public static void CreateInstance()
        {
            Instance = new Writer();
        }

        public static void KillInstance()
        {
            Instance = null;
        }

    }
}
