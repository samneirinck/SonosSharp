namespace SonosSharp.Controllers
{
    public class GroupRenderingController : Controller
    {
        public GroupRenderingController(string ipAddress) : base(ipAddress)
        {
        }

        public override string ActionNamespace
        {
            get { return "urn:upnp-org:serviceId:GroupRenderingControl"; }
        }

        public override string ServiceID
        {
            get { return "urn:upnp-org:serviceId:GroupRenderingControl"; }
        }
    }
}
