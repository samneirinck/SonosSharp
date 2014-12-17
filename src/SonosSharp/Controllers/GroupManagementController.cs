namespace SonosSharp.Controllers
{
    public class GroupManagementController : Controller
    {
        public GroupManagementController(string ipAddress)
            : base(ipAddress)
        {

        }

        public const string ServiceTypeValue = "urn:schemas-upnp-org:service:GroupManagement:1";

        public override string ServiceType
        {
            get { return ServiceTypeValue; }
        }

    }
}
