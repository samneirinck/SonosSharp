using SonosSharp.Controllers;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SonosSharp
{
    public class SonosController
    {
        private string _ipAddress;

        private readonly AVTransportController _avTransportController;
        private readonly RenderingController _renderingController;

        public SonosController(string ipAddress)
        {
            if (ipAddress == null)
                throw new ArgumentNullException("ipAddress");

            if (ipAddress.Length <= 0)
                throw new ArgumentException("Address cannot be empty", ipAddress);

            _ipAddress = ipAddress;

            _avTransportController = new AVTransportController(ipAddress);
            _renderingController = new RenderingController(ipAddress);
        }

        public async Task<SonosDevice> GetSonosDeviceAsync()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(String.Format("http://{0}:{1}/xml/device_description.xml", _ipAddress, Constants.SonosPortNumber));
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(response.ToString());
            }

            string responseContents = await response.Content.ReadAsStringAsync();

            return SonosDevice.Create(responseContents);
        }

        public Task<int> GetVolumeAsync(string channel = "Master")
        {
            return _renderingController.GetVolumeAsync(channel);
        }

        public Task<int> GetBassAsync()
        {
            return _renderingController.GetBassAsync();
        }

        public Task<bool> GetIsMutedAsync(string channel = "Master")
        {
            return _renderingController.GetIsMutedAsync(channel);
        }

        public Task SetVolumeAsync(int volume, string channel = "Master")
        {
            return _renderingController.SetVolumeAsync(volume, channel);
        }

        public Task StopAsync()
        {
            return _avTransportController.StopAsync();
        }

        public Task<MediaInfo> GetMediaInfoAsync()
        {
            return _avTransportController.GetMediaInfoAsync();
        }

        public Task<PositionInfo> GetPositionInfoAsync()
        {
            return _avTransportController.GetPositionInfoAsync();
        }

        public Task PlayAsync()
        {
            return _avTransportController.PlayAsync(1);
        }
    }
}
