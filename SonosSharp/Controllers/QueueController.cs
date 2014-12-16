namespace SonosSharp.Controllers
{
    internal class QueueController : Controller
    {
        public QueueController(string ipAddress)
            : base(ipAddress)
        {

        }

        public const string ServiceTypeValue = "urn:schemas-sonos-com:service:Queue:1";

        public override string ServiceType
        {
            get { return ServiceTypeValue; }
        }

    }
}