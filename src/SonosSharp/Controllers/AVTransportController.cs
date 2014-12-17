using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using SonosSharp.Eventing;

namespace SonosSharp.Controllers
{
    public class AVTransportController : Controller
    {
        public const string ServiceTypeValue = "urn:schemas-upnp-org:service:AVTransport:1";

        public AVTransportController(string ipAddress) : base(ipAddress)
        {
        }

        public override string ServiceType
        {
            get { return ServiceTypeValue; }
        }

        public Task StopAsync()
        {
            return InvokeActionAsync("Stop");
        }

        public async Task<MediaInfo> GetMediaInfoAsync()
        {
            var result = await InvokeFuncWithResultAsync("GetMediaInfo");
            XElement element = XElement.Parse(result);

            var responseNode = element.Descendants(ActionNS + "GetMediaInfoResponse").First();

            return new MediaInfo
                {
                    NumberOfTracks = Convert.ToInt32(responseNode.Element("NrTracks").Value),
                    MediaDuration = responseNode.Element("MediaDuration").Value,
                    CurrentUri = responseNode.Element("CurrentURI").Value,
                    CurrentUriMetaData = responseNode.Element("CurrentURIMetaData").Value,
                    NextUri = responseNode.Element("NextURI").Value,
                    NextUriMetaData = responseNode.Element("NextURIMetaData").Value,
                    PlayMedium = responseNode.Element("PlayMedium").Value,
                    RecordMedium = responseNode.Element("RecordMedium").Value,
                    WriteStatus = responseNode.Element("WriteStatus").Value
                };
        }

        public async Task<PositionInfo> GetPositionInfoAsync()
        {
            var result = await InvokeFuncWithResultAsync("GetPositionInfo");
            XElement element = XElement.Parse(result);
            var responseNode = element.Descendants(ActionNS + "GetPositionInfoResponse").First();

            int trackNumber;
            TimeSpan trackDuration;
            string trackMetaDataRaw;
            string trackUri;
            TimeSpan relativeTime;
            int relativeCount;
            int absoluteCount;

            Int32.TryParse(responseNode.Element("Track").Value, out trackNumber);
            TimeSpan.TryParse(responseNode.Element("TrackDuration").Value, out trackDuration);
            trackMetaDataRaw = responseNode.Element("TrackMetaData").Value;
            trackUri = responseNode.Element("TrackURI").Value;
            TimeSpan.TryParse(responseNode.Element("RelTime").Value, out relativeTime);
            Int32.TryParse(responseNode.Element("RelCount").Value, out relativeCount);
            Int32.TryParse(responseNode.Element("AbsCount").Value, out absoluteCount);

            return new PositionInfo
                {
                    TrackNumber = trackNumber,
                    TrackDuration = trackDuration,
                    TrackMetaData = ParseTrackMetaData(trackMetaDataRaw),
                    TrackMetaDataRaw = trackMetaDataRaw,
                    TrackUri = trackUri,
                    RelativeTime = relativeTime,
                    AbsoluteTime = responseNode.Element("AbsTime").Value,
                    RelativeCount = relativeCount,
                    AbsoluteCount = absoluteCount
                };
        }

        private TrackMetaData ParseTrackMetaData(string trackMetaDataRaw)
        {
            string unescaped = trackMetaDataRaw.Replace("&lt;", "<")
                                                .Replace("&gt;", ">")
                                                .Replace("&amp;", "&")
                                                .Replace("&quot;", "\"");

            if (string.IsNullOrWhiteSpace(trackMetaDataRaw) || trackMetaDataRaw.Equals("NOT_IMPLEMENTED"))
            {
                return new TrackMetaData
                {
                    Title = "TV",
                    AlbumArtUri = "/Assets/Icons/tv.png"
                };
            }
            XElement didlElement = XElement.Parse(unescaped).FirstNode as XElement;

            return new TrackMetaData
            {
                AlbumArtUri = didlElement.GetElementValueSafe(Constants.UpnpMetadataNamespace + "albumArtURI"),
                Class = didlElement.GetElementValueSafe(Constants.UpnpMetadataNamespace + "class"),
                Creator = didlElement.GetElementValueSafe(Constants.PurlDcNamespace + "creator"),
                Title = didlElement.GetElementValueSafe(Constants.PurlDcNamespace + "title"),
            };
            //throw new NotImplementedException();
        }

        public Task PlayAsync(int playSpeed)
        {
            return InvokeActionAsync("Play", new Dictionary<string, object> {{"Speed", playSpeed}});
        }

        public async Task<TransportInfo> GetTransportInfoAsync()
        {
            var result = await InvokeFuncWithResultAsync("GetTransportInfo");
            XElement element = XElement.Parse(result);
            var responseNode = element.Descendants(ActionNS + "GetTransportInfoResponse").First();

            return new TransportInfo
            {
                CurrentTransportState = responseNode.Element("CurrentTransportState").Value,
                CurrentTransportStatus = responseNode.Element("CurrentTransportStatus").Value,
                CurrentSpeed = responseNode.Element("CurrentSpeed").Value
            };
        }
    }
}
