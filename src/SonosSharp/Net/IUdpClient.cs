using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SonosSharp
{
    public interface IUdpClient : IDisposable
    {
        Task<int> SendAsync(byte[] datagram, int bytes, IPEndPoint endPoint);
        Task<UdpReceiveResult> ReceiveAsync();
    }
}
