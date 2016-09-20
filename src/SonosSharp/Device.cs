using System.Net;

namespace SonosSharp
{
    public class Device
    {
        /// <summary>
        /// The IP Address of the Sonos Device.
        /// </summary>
        public IPAddress IpAddress { get; }
        public string Name { get; private set; }

        public Device(IPAddress ipAddress)
        {
            IpAddress = ipAddress;
        }
    }
}
