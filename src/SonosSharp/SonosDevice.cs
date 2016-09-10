using System.Net;

namespace SonosSharp
{
    public class SonosDevice
    {
        /// <summary>
        /// The IP Address of the Sonos Device.
        /// </summary>
        public IPAddress IpAddress { get; }
        public string Name { get; private set; }

        public SonosDevice(IPAddress ipAddress)
        {
            IpAddress = ipAddress;
        }
    }
}
