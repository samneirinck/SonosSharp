using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonosSharp.Controllers
{
    public class SystemPropertiesController : Controller
    {
        public SystemPropertiesController(string ipAddress)
            : base(ipAddress)
        {
        }

        public const string ServiceTypeValue = "urn:schemas-upnp-org:service:SystemProperties:1";

        public override string ServiceType
        {
            get { return ServiceTypeValue; }
        }

    }
}
