using System;
using System.Xml.Linq;

namespace SonosSharp
{
    public class PositionInfo
    {
        private string _trackMetaDataRaw;

        public int TrackNumber { get; set; }
        public TimeSpan TrackDuration { get; set; }
        public string TrackMetaDataRaw
        {
            get { return _trackMetaDataRaw; }
            set { _trackMetaDataRaw = value;
                UpdateTrackMetaData();
            }
        }

        private void UpdateTrackMetaData()
        {
            XNamespace upnpNS = "urn:schemas-upnp-org:metadata-1-0/upnp/";
            XNamespace metadataNS = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/";

            try
            {
                XElement element = XElement.Parse(_trackMetaDataRaw).Element(metadataNS + "item");

                TrackMetaData = new TrackMetaData
                    {
                        AlbumArtUri = element.Element(upnpNS + "albumArtURI").Value
                    };
            }
            catch (Exception) { }
        }

        public TrackMetaData TrackMetaData { get; set; }
        public string TrackUri { get; set; }
        public TimeSpan RelativeTime { get; set; }
        public string AbsoluteTime { get; set; }
        public int RelativeCount { get; set; }
        public int AbsoluteCount { get; set; }
    }

    public class TrackMetaData
    {
        public string ProtocolInfo { get; set; }
        public TimeSpan Duration { get; set; }
        public string AlbumArtUri { get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
        public string Creator { get; set; }
    }
}