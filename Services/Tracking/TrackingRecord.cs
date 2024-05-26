using MessagePack;
using Maria.Commons.Recordkeeping;

namespace Maria.Tracking
{
    [MessagePackObject]
    public class TrackingRecord : Record
    {
        [Key(0)]
        public string Name { get; set; }
        //Maybe this should be a time object of some kind
        [Key(1)]
        public string Time { get; set; }
        //Represents whatever extra information we want to store, like the exact book open on drive, or project on VS.
        [Key(2)]
        public string? Extra { get; set; }
    }
}
