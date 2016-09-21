using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SonosSharp.Services;

namespace SonosSharp
{
    public class DiscoveryClient : IDiscoveryClient
    {
        private static readonly IPAddress MulticastIpAddress = IPAddress.Parse("239.255.255.250");
        private static readonly IPEndPoint MulticastIpEndpoint = new IPEndPoint(MulticastIpAddress, 1900);
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

        
        private static readonly byte[] MessageBytes = Encoding.UTF8.GetBytes("M-SEARCH * HTTP/1.1\r\n" +
                               "HOST: 239.255.255.250:1900\r\n" +
                               "MAN: \"ssdp:discover\"\r\n" +
                               "MX: 1\r\n" +
                               "ST: urn:schemas-upnp-org:device:ZonePlayer:1\r\n\r\n");

        private readonly IUdpClientFactory _udpClientFactory;

        public DiscoveryClient()
            : this(new UdpClientFactory())
        {
        }

        public DiscoveryClient(IUdpClientFactory udpClientFactory)
        {
            if (udpClientFactory == null)
                throw new ArgumentNullException(nameof(udpClientFactory));

            _udpClientFactory = udpClientFactory;
        }

        public Task<IReadOnlyCollection<Zone>> DiscoverZonesAsync()
        {
            var cancellationToken = new CancellationTokenSource(DefaultTimeout);
            return DiscoverZonesAsync(cancellationToken.Token);
        }

        public Task<IReadOnlyCollection<Zone>> DiscoverZonesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<Device>> DiscoverDevicesAsync()
        {
            return DiscoverDevicesAsync(default(CancellationToken));
        }

        public async Task<IReadOnlyCollection<Device>> DiscoverDevicesAsync(CancellationToken cancellationToken)
        {
            List<Device> devices = new List<Device>();

            using (IUdpClient udpClient = _udpClientFactory.Create(new IPEndPoint(IPAddress.Any, 0)))
            {
                await udpClient.SendAsync(MessageBytes, MessageBytes.Length, MulticastIpEndpoint)
                               .WithCancellation(cancellationToken)
                               .ConfigureAwait(false);

                UdpReceiveResult receiveResult = await udpClient.ReceiveAsync()
                                                                .WithCancellation(cancellationToken)
                                                                .ConfigureAwait(false);

                Console.WriteLine("Received a result");
                if (receiveResult.RemoteEndPoint != null)
                {
                    // We only use SSDP to discover *any* Sonos device.
                    // We then use the UPNP endpoint to discover the rest of the Sonos devices.
                    devices.AddRange(await FindDevicesFromSonosIpAddress(receiveResult.RemoteEndPoint.Address).ConfigureAwait(false));
                }
            }
            
            return new ReadOnlyCollection<Device>(devices);
        }

        private async Task<IReadOnlyCollection<Device>> FindDevicesFromSonosIpAddress(IPAddress deviceIpAddress)
        {
            var service = new ZoneGroupTopologyService(deviceIpAddress);

            var zones = await service.GetZoneGroupStateAsync();

            return null;
        }

        //public async Task<IEnumerable<Device>> DiscoverDevicesAsync(TimeSpan timeout)
        //{
        //    var cts = new CancellationTokenSource(timeout);
        //    Console.WriteLine("hello");
        //    List<Device> devices = new List<Device>();

        //    DiscoverDevices().Timeout(timeout).Subscribe(x =>
        //    {
        //        devices.Add(x);
        //    }, () => { cts.Cancel(); }, cts.Token);

        //    try
        //    {
        //        await Task.Delay(timeout, cts.Token).ConfigureAwait(false);
        //    }
        //    catch (TaskCanceledException) { }

        //    return devices;
        //}

        //public IObservable<Device> DiscoverDevices()
        //{
        //    return Observable.Create(async (IObserver<Device> observer, CancellationToken token) =>
        //    {
        //        IUdpClient udpClient = _udpClientFactory.Create(new IPEndPoint(IPAddress.Any, 0));

        //        var bytes = Encoding.UTF8.GetBytes(Message);
        //        await udpClient.SendAsync(bytes, bytes.Length, MulticastIpEndpoint);

        //        UdpReceiveResult receiveResult = await udpClient.ReceiveAsync().ConfigureAwait(false);
        //        string rawSsdpResponse = Encoding.UTF8.GetString(receiveResult.Buffer);

        //        var firstDevice = new Device(receiveResult.RemoteEndPoint.Address);

        //        // We just use 
        //        ZoneGroupTopologyService svc = new ZoneGroupTopologyService(firstDevice.IpAddress);

        //        var xyz = await svc.GetZoneGroupStateAsync().ConfigureAwait(false);
        //        observer.OnNext(new Device(receiveResult.RemoteEndPoint.Address));
        //        observer.OnCompleted();

        //        return udpClient;

        //    });
        //}
    }


}
