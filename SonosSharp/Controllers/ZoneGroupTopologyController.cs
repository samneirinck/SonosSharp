using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonosSharp.Controllers
{
    public class ZoneGroupTopologyController : Controller
    {
        public ZoneGroupTopologyController(string ipAddress)
            : base(ipAddress)
        {
        }

        public const string ServiceTypeValue = "urn:schemas-upnp-org:service:ZoneGroupTopology:1";

        public override string ServiceType
        {
            get { return ServiceTypeValue; }
        }

    }
}
