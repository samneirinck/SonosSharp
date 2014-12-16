using SonosSharp.MusicService;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace SonosSharp.Controllers
{
    class MusicServicesController : Controller
    {
        public MusicServicesController(string ipAddress)
            : base(ipAddress)
        {

        }

        public const string ServiceTypeValue = "urn:schemas-upnp-org:service:MusicServices:1";

        public override string ServiceType
        {
            get { return ServiceTypeValue; }
        }


        public async Task<IEnumerable<ServiceDescription>> ListAvailableServicesAsync()
        {
            string result = await InvokeFuncAsync<string>("ListAvailableServices");
            return null;
        }

    }
}
