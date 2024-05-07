using Maria.Services.Recordkeeping.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Recordkeeping
{
    //Designed to be a singleton
    internal class Writer
    {
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

            //Write to file
            //Clear buffers
            browsersBuffer.Clear();
            processesBuffer.Clear();

            processesSemaphore.Release();
            browsersSemaphore.Release();
        }

    }
}
