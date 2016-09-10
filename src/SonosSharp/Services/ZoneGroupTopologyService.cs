using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SonosSharp.Services
{
    public class ZoneGroupTopologyService : UPnPService
    {
        protected override string SoapNamespace => "urn:schemas-upnp-org:service:ZoneGroupTopology:1";

        public ZoneGroupTopologyService(IPAddress ipAddress)
            : base(ipAddress, "ZoneGroupTopology")
        {
        }

        public async Task<IReadOnlyCollection<Zone>> GetZoneGroupStateAsync()
        {
            //var content = new StreamContent();
            var zones = new List<Zone>();
            Console.WriteLine($"Getting zone group topology from {IpAddress}");

            string result = await InvokeActionAsync("GetZoneGroupState");
            Console.WriteLine(result);

            return new ReadOnlyCollection<Zone>(zones);
        }

    }
}
