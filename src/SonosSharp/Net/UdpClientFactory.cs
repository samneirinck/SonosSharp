using System.Net;

namespace SonosSharp
{
    public class UdpClientFactory : IUdpClientFactory
    {
        public IUdpClient Create(IPEndPoint endpoint)
        {
            var client = new UdpClientWrapper();

            client.Client.Bind(endpoint);

            return client;
        }
    }
}
