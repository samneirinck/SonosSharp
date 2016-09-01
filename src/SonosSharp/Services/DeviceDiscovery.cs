using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace SonosSharp.Services
{
    public interface IDeviceDiscovery
    {
        Task DiscoverDevicesAsync();
    }

    public class DeviceDiscovery : IDeviceDiscovery
    {
        private static IPAddress MulticastIpAddress = IPAddress.Parse("239.255.255.250");
        private static IPEndPoint Test = new IPEndPoint(MulticastIpAddress, 1900);

        const string message = "M-SEARCH * HTTP/1.1\r\n" +
                        "HOST: 239.255.255.250:1900\r\n" +
                        "ST:urn:schemas-upnp-org:device:ZonePlayer:1\r\n" +
                        "MAN:\"ssdp:discover\"\r\n" +
                        "MX:3\r\n\r\n";


        public Task DiscoverDevicesAsync()
        {
             return DiscoverDevicesAsync(TimeSpan.FromSeconds(5));
        }

        public async Task DiscoverDevicesAsync(TimeSpan timeout)
        {
            Console.WriteLine("hello");
            UdpClient client = new UdpClient();
            client.Client.Bind(new IPEndPoint(IPAddress.Any, 1900));

            client.JoinMulticastGroup(MulticastIpAddress);

            var bytes = Encoding.ASCII.GetBytes(message);
            await client.SendAsync(bytes, bytes.Length, Test);

            for (int i = 0; i < 10; ++i)
            {
                var result = await client.ReceiveAsync();
                Console.WriteLine(Encoding.UTF8.GetString(result.Buffer));
            }
        }
    }
}
