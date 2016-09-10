using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;

namespace SonosSharp.Services
{
    public interface IDeviceDiscovery
    {
        Task DiscoverDevicesAsync();
    }

    public class DeviceDiscovery : IDeviceDiscovery
    {
        private static IPAddress MulticastIpAddress = IPAddress.Parse("239.255.255.250");
        private static IPEndPoint MulticastIpEndpoint = new IPEndPoint(MulticastIpAddress, 1900);

        const string Message = "M-SEARCH * HTTP/1.1\r\n" +
                               "HOST: 239.255.255.250:1900\r\n" +
                               "MAN: \"ssdp:discover\"\r\n" +
                               "MX: 1\r\n" +
                               "ST: urn:schemas-upnp-org:device:ZonePlayer:1\r\n\r\n";

        public Task DiscoverDevicesAsync()
        {
            return DiscoverDevicesAsync(TimeSpan.FromSeconds(3));
        }

        public async Task<IEnumerable<SonosDevice>> DiscoverDevicesAsync(TimeSpan timeout)
        {
            var cts = new CancellationTokenSource(timeout);
            Console.WriteLine("hello");
            List<SonosDevice> devices = new List<SonosDevice>();

            DiscoverDevices().Timeout(timeout).Subscribe(x =>
            {
                devices.Add(x);
            }, () => { cts.Cancel(); }, cts.Token);

            try
            {
                await Task.Delay(timeout, cts.Token).ConfigureAwait(false);
            }
            catch (TaskCanceledException) { }

            return devices;
        }

        public IObservable<SonosDevice> DiscoverDevices()
        {
            return Observable.Create(async (IObserver<SonosDevice> observer, CancellationToken token) =>
            {
                UdpClient udpClient = new UdpClient();
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, 0));

                var bytes = Encoding.UTF8.GetBytes(Message);
                await udpClient.SendAsync(bytes, bytes.Length, MulticastIpEndpoint);

                UdpReceiveResult receiveResult = await udpClient.ReceiveAsync().ConfigureAwait(false);
                string rawSsdpResponse = Encoding.UTF8.GetString(receiveResult.Buffer);

                var firstDevice = new SonosDevice(receiveResult.RemoteEndPoint.Address);

                // We just use 
                ZoneGroupTopologyService svc = new ZoneGroupTopologyService(firstDevice.IpAddress);

                var xyz = await svc.GetZoneGroupStateAsync().ConfigureAwait(false);
                observer.OnNext(new SonosDevice(receiveResult.RemoteEndPoint.Address));
                observer.OnCompleted();

                return udpClient;

            });
        }
    }
}
