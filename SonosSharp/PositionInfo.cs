using System;

namespace SonosSharp
{
    public class PositionInfo
    {
        public int TrackNumber { get; set; }
        public TimeSpan TrackDuration { get; set; }
        public string TrackMetaData { get; set; }
        public string TrackUri { get; set; }
        public TimeSpan RelativeTime { get; set; }
        public string AbsoluteTime { get; set; }
        public int RelativeCount { get; set; }
        public int AbsoluteCount { get; set; }
    }
}