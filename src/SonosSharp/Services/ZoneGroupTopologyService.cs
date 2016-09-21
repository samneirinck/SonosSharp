using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SonosSharp.Services
{
    public class ZoneGroupTopologyService : UPnPService
    {
        protected override string ActionNamespace => "urn:schemas-upnp-org:service:ZoneGroupTopology:1";

        public ZoneGroupTopologyService(IPAddress ipAddress)
            : base(ipAddress, "ZoneGroupTopology")
        {
        }

        public async Task<IReadOnlyCollection<Zone>> GetZoneGroupStateAsync()
        {
            //var content = new StreamContent();
            var zones = new List<Zone>();
            Console.WriteLine($"Getting zone group topology from {IpAddress}");

            XElement zoneGroupState = await InvokeActionAsync("GetZoneGroupState");
            XElement woefke = XElement.Parse(zoneGroupState.Descendants().First().Value);
            foreach (var zoneGroup in woefke.Descendants())
            {
                var zone = new Zone();
                Console.WriteLine(zoneGroup);

                zones.Add(zone);
            }

            return new ReadOnlyCollection<Zone>(zones);
        }

    }
}
