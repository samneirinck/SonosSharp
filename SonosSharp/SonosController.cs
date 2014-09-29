using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SonosSharp.Controllers;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using System.Net;
using SonosSharp.Eventing;

namespace SonosSharp
{
    public class SonosController
    {
        private string _ipAddress;

        private readonly AVTransportController _avTransportController;
        private readonly RenderingController _renderingController;
        private readonly GroupRenderingController _groupRenderingController;

        private readonly List<Controller> _allControllers;

        public SonosController(string ipAddress)
            : this(ipAddress, null)
        {
        }

        public SonosController(string ipAddress, BasicHttpServer basicHttpServer)
        {
            if (ipAddress == null)
                throw new ArgumentNullException("ipAddress");

            if (ipAddress.Length <= 0)
                throw new ArgumentException("Address cannot be empty", ipAddress);

            _allControllers = new List<Controller>();
            _ipAddress = ipAddress;

            _avTransportController = new AVTransportController(ipAddress);
            _renderingController = new RenderingController(ipAddress);
            _groupRenderingController = new GroupRenderingController(ipAddress);

            _allControllers.Add(_avTransportController);
            _allControllers.Add(_renderingController);
            _allControllers.Add(_groupRenderingController);

            if (basicHttpServer != null)
            {
                basicHttpServer.StartAsync();
                foreach (var controller in _allControllers)
                {
                    controller.HttpServer = basicHttpServer;
                }
                basicHttpServer.VariableChanged += HttpServerOnVariableChanged;
            }
        }

        private void HttpServerOnVariableChanged(object sender, HttpVariableChangedEventArgs httpVariableChangedEventArgs)
        {
            
        }

        public async Task<SonosDevice> GetSonosDeviceAsync()
        {
            var httpClient = new HttpClient();

            //var response = await httpClient.GetAsync(String.Format("http://{0}:{1}/xml/device_description.xml", _ipAddress, Constants.SonosPortNumber));
            var response = await httpClient.GetAsync(String.Format("http://{0}:{1}/", _ipAddress, Constants.SonosPortNumber));
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(response.ToString());
            }

            string responseContents = await response.Content.ReadAsStringAsync();

            var element = XElement.Parse(responseContents);

            XNamespace ns = "urn:schemas-upnp-org:device-1-0";

            var allServices = element.Descendants(ns + "service").ToList();

            foreach (var service in allServices)
            {
                string serviceId = service.Element(ns + "serviceId").Value;
                string controlUrl = service.Element(ns + "controlURL").Value;
                string eventUrl = service.Element(ns + "eventSubURL").Value;

                foreach (var controller in _allControllers)
                {
                    if (controller.ServiceID == serviceId)
                    {
                        controller.ControlUrl = controlUrl;
                        controller.EventUrl = eventUrl;
                    }
                }         
            }

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

        public async Task EnableEventsAsync()
        {
            Task[] tasks = new Task[_allControllers.Count];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = _allControllers[i].SubscribeToEventsAsync();
            }
            await Task.Factory.ContinueWhenAll(tasks, tasks1 => {});
        }

    }
}
