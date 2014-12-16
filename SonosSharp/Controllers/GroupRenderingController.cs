namespace SonosSharp.Controllers
{
    public class GroupRenderingController : Controller
    {
        public GroupRenderingController(string ipAddress) : base(ipAddress)
        {
        }

        public const string ServiceTypeValue = "urn:upnp-org:serviceId:GroupRenderingControl:1";

        public override string ServiceType
        {
            get { return ServiceTypeValue; }
        }

    }
}
