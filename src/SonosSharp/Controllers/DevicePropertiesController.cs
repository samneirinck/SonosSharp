namespace SonosSharp.Controllers
{
    class DevicePropertiesController : Controller
    {
        public DevicePropertiesController(string ipAddress)
            : base(ipAddress)
        {

        }

        public const string ServiceTypeValue = "urn:schemas-upnp-org:service:DeviceProperties:1";

        public override string ServiceType
        {
            get { return ServiceTypeValue; }
        }

    }
}
