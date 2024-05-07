using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Recordkeeping.Records
{
    internal class TrackingRecord : Record
    {
        public string Name { get; set; }
        //Maybe this should be a time object of some kind
        public string Time { get; set; }
    }
}
