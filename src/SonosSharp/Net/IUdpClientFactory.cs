using System.Net;

namespace SonosSharp
{
    public interface IUdpClientFactory
    {
        IUdpClient Create(IPEndPoint endpoint);
    }
}
