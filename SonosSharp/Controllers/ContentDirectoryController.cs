using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonosSharp.Controllers
{
    public class ContentDirectoryController : Controller
    {
        public ContentDirectoryController(string ipAddress)
            : base(ipAddress)
        {
        }
        public const string ServiceTypeValue = "urn:schemas-upnp-org:service:ContentDirectory:1";

        public override string ServiceType
        {
            get { return ServiceTypeValue; }
        }

    }
}
