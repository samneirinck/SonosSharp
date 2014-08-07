using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonosSharp
{
    public class MediaInfo
    {
        public int NumberOfTracks { get; set; }
        public string MediaDuration { get; set; }
        public string CurrentUri { get; set; }
        public string CurrentUriMetaData { get; set; }
        public string NextUri { get; set; }
        public string NextUriMetaData { get; set; }
        public string PlayMedium { get; set; }
        public string RecordMedium { get; set; }
        public string WriteStatus { get; set; }
    }
}
