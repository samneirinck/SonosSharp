namespace SonosSharp.Controllers
{
    public class AlarmClockController : Controller
    {
        public const string ServiceTypeValue = "urn:schemas-upnp-org:service:AlarmClock:1";

        public AlarmClockController(string ipAddress)
            : base(ipAddress)
        {

        }
        public override string ServiceType
        {
            get { return ServiceTypeValue; }
        }

    }
}
