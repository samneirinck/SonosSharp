using System;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SonosSharp.Discovery
{
    public class DeviceDiscovery : IDeviceDiscovery
    {
        public async Task SearchForSonosDevicesAsync(Action<string> deviceFound)
        {
            var socket = new DatagramSocket();
            socket.MessageReceived += (sender, args) =>
            {
                DataReader reader = args.GetDataReader();

                uint count = reader.UnconsumedBufferLength;
                string data = reader.ReadString(count);
                foreach (
                    string x in
                        data.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
                {
                    int indexOfColon = x.IndexOf(':');

                    if (indexOfColon > 0 && x.Length > indexOfColon +1)
                    {
                        string key = x.Substring(0, indexOfColon);
                        string value = x.Substring(indexOfColon + 2);

                        if (String.Equals("location", key, StringComparison.OrdinalIgnoreCase))
                        {
                            deviceFound(value);
                        }
                    }

                }
            };
            IOutputStream stream = await socket.GetOutputStreamAsync(new HostName("239.255.255.250"), "1900");


            const string message = "M-SEARCH * HTTP/1.1\r\n" +
                                   "HOST: 239.255.255.250:1900\r\n" +
                                   "ST:urn:schemas-upnp-org:device:ZonePlayer:1\r\n" +
                                   "MAN:\"ssdp:discover\"\r\n" +
                                   "MX:3\r\n\r\n";

            var writer = new DataWriter(stream) { UnicodeEncoding = UnicodeEncoding.Utf8 };
            writer.WriteString(message);
            await writer.StoreAsync();
        }
    }
}
