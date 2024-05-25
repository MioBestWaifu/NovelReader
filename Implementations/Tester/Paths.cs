using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Tester
{

    internal class Paths
    {
        public string ToEdrdg { get; private set; }
        public string ToUnidic { get; private set; }
        public string ToConvertedDictionary { get; private set; }
        public string ToTracking { get; private set; }

        public Paths(string toEdrdg, string toUnidic, string toConvertedDictionary, string toTracking)
        {
            ToEdrdg = toEdrdg;
            ToUnidic = toUnidic;
            ToConvertedDictionary = toConvertedDictionary;
            ToTracking = toTracking;
        }
    }
}
