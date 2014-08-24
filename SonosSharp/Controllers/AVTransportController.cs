using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SonosSharp.Controllers
{
    public class AVTransportController : Controller
    {
        public AVTransportController(string ipAddress) : base(ipAddress)
        {
        }

        protected override string ControlUrl
        {
            get { return "/MediaRenderer/AVTransport/Control"; }
        }

        protected override string ActionNamespace
        {
            get { return "urn:schemas-upnp-org:service:AVTransport:1"; }
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

            return new PositionInfo
                {
                    TrackNumber = Int32.Parse(responseNode.Element("Track").Value),
                    TrackDuration = TimeSpan.Parse(responseNode.Element("TrackDuration").Value),
                    TrackMetaDataRaw = responseNode.Element("TrackMetaData").Value,
                    TrackUri = responseNode.Element("TrackURI").Value,
                    RelativeTime = TimeSpan.Parse(responseNode.Element("RelTime").Value),
                    AbsoluteTime = responseNode.Element("AbsTime").Value,
                    RelativeCount = Int32.Parse(responseNode.Element("RelCount").Value),
                    AbsoluteCount = Int32.Parse(responseNode.Element("AbsCount").Value)
                };
        }

        public Task PlayAsync(int playSpeed)
        {
            return InvokeActionAsync("Play", new Dictionary<string, object> {{"Speed", playSpeed}});
        }
    }
}
