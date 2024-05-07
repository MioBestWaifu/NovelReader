using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Recordkeeping.Records.Maps
{
    internal class TrackingMap : ClassMap<TrackingRecord>
    {
        public TrackingMap()
        {
            Map(m => m.Name).Index(1).Name("name");
            Map(m => m.Time).Index(2).Name("time");
        }
    }
}
