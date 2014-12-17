using System.Collections.Generic;
using System.Globalization;
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
using System.Text.RegularExpressions;
using SonosSharp.MusicService;

namespace SonosSharp
{
    public class SonosController
    {
        private string _deviceDescriptionUrl;
        private string _ipAddress;

        private readonly BasicHttpServer _httpServer;
        private readonly List<Controller> _allControllers;
        private XNamespace DeviceNamespace = "urn:schemas-upnp-org:device-1-0";


        public event EventHandler<VolumeChangedEventArgs> VolumeChanged;

        protected void OnVolumeChanged(int masterVolume, int leftVolume, int rightVolume)
        {
            if (VolumeChanged != null)
            {
                VolumeChanged(this, new VolumeChangedEventArgs(masterVolume, leftVolume, rightVolume));
            }
        }

        public SonosController(string deviceDescriptionUrl)
            : this(deviceDescriptionUrl, null)
        {
        }

        public SonosController(string deviceDescriptionUrl, BasicHttpServer basicHttpServer)
        {
            if (deviceDescriptionUrl == null)
                throw new ArgumentNullException("deviceDescriptionUrl");

            if (deviceDescriptionUrl.Length <= 0)
                throw new ArgumentException("deviceDescriptionUrl cannot be empty", deviceDescriptionUrl);

            _deviceDescriptionUrl = deviceDescriptionUrl;
            _httpServer = basicHttpServer;
            _allControllers = new List<Controller>();

            if (basicHttpServer != null)
            {
                basicHttpServer.VariableChanged += HttpServerOnVariableChanged;
            }

            var ipAddressRegex = Regex.Match(deviceDescriptionUrl, @"http://([^/:]+).*");
            if (!ipAddressRegex.Success)
            {
                throw new Exception("Failed to look for ip address in the following string: " + deviceDescriptionUrl);
            }
            this._ipAddress = ipAddressRegex.Groups[1].Value;

            //_avTransportController = new AVTransportController(ipAddress);
            //_renderingController = new RenderingController(ipAddress);
            //_groupRenderingController = new GroupRenderingController(ipAddress);

            //_allControllers.Add(_avTransportController);
            //_allControllers.Add(_renderingController);
            //_allControllers.Add(_groupRenderingController);

            //if (basicHttpServer != null)
            //{
            //    foreach (var controller in _allControllers)
            //    {
            //        controller.HttpServer = basicHttpServer;
            //    }
            //    basicHttpServer.VariableChanged += HttpServerOnVariableChanged;
            //}
        }


        private Controller CreateControllerByServiceType(string serviceType, string ipAddress)
        {
            Controller controller = null;
            
            switch (serviceType)
            {
                case AlarmClockController.ServiceTypeValue:
                    controller = new AlarmClockController(ipAddress);
                    break;

                case AVTransportController.ServiceTypeValue:
                    controller = new AVTransportController(ipAddress);
                    break;

                case ConnectionManagerController.ServiceTypeValue:
                    controller = new ConnectionManagerController(ipAddress);
                    break;

                case ContentDirectoryController.ServiceTypeValue:
                    controller = new ContentDirectoryController(ipAddress);
                    break;

                case DevicePropertiesController.ServiceTypeValue:
                    controller = new DevicePropertiesController(ipAddress);
                    break;

                case GroupManagementController.ServiceTypeValue:
                    controller = new GroupManagementController(ipAddress);
                    break;

                case GroupRenderingController.ServiceTypeValue:
                    controller = new GroupRenderingController(ipAddress);
                    break;

                case MusicServicesController.ServiceTypeValue:
                    controller = new MusicServicesController(ipAddress);
                    break;

                case QueueController.ServiceTypeValue:
                    controller = new QueueController(ipAddress);
                    break;

                case RenderingController.ServiceTypeValue:
                    controller = new RenderingController(ipAddress);
                    break;

                case SystemPropertiesController.ServiceTypeValue:
                    controller = new SystemPropertiesController(ipAddress);
                    break;

                case ZoneGroupTopologyController.ServiceTypeValue:
                    controller = new ZoneGroupTopologyController(ipAddress);
                    break;
            }

            if (controller != null)
            {
                controller.HttpServer = _httpServer;
            }

            return controller;
        }

        public async Task InitializeAsync()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(_deviceDescriptionUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(response.ToString());
            }

            string responseContents = await response.Content.ReadAsStringAsync();

            var element = XElement.Parse(responseContents);


            var allServices = element.Descendants(DeviceNamespace + "service").ToList();
            foreach (var service in allServices)
            {
                string serviceType = service.GetElementValueSafe(DeviceNamespace + "serviceType");
                if (!String.IsNullOrEmpty(serviceType))
                {
                    var controller = CreateControllerByServiceType(serviceType, _ipAddress);
                    if (controller != null)
                    {
                        FillControllerProperties(controller, service);
                        _allControllers.Add(controller);
                    }
                }
            }

            //return SonosDevice.Create(responseContents);

        }

        private void FillControllerProperties(Controller controller, XElement service)
        {
            controller.ServiceId = service.GetElementValueSafe(DeviceNamespace + "serviceId");
            controller.ScpdUrl = service.GetElementValueSafe(DeviceNamespace + "SCPDURL");
            controller.ControlUrl = service.GetElementValueSafe(DeviceNamespace + "controlURL");
            controller.EventUrl = service.GetElementValueSafe(DeviceNamespace + "eventSubURL");
        }


        private void HttpServerOnVariableChanged(object sender, HttpVariableChangedEventArgs httpVariableChangedEventArgs)
        {
            if (string.Equals(httpVariableChangedEventArgs.VariableName, "LastChange"))
            {
                string contents = httpVariableChangedEventArgs.VariableValue;
                try
                {
                    var eventElement = XElement.Parse(contents);
                    XNamespace ns = eventElement.Name.Namespace;

                    // Is it a volume event?
                    var instance = eventElement.Element(ns + "InstanceID");
                    if (instance != null)
                    {
                        var volumeElements = instance.Elements(ns + "Volume").ToArray();

                        if (volumeElements.Any())
                        {
                            int masterVolume = GetValueSafe(volumeElements, x => x.Attribute("channel") != null && string.Equals(x.Attribute("channel").Value, "Master"), 100);
                            int leftVolume = GetValueSafe(volumeElements, x => x.Attribute("channel") != null && string.Equals(x.Attribute("channel").Value, "LF"), 100);
                            int rightVolume = GetValueSafe(volumeElements, x => x.Attribute("channel") != null && string.Equals(x.Attribute("channel").Value, "RF"), 100);

                            OnVolumeChanged(masterVolume, leftVolume, rightVolume);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private T GetValueSafe<T>(IEnumerable<XElement> elements, Func<XElement, bool> predicate, T defaultValue)
        {
            T returnValue = defaultValue;
            var specificElement = elements.FirstOrDefault(predicate);
            if (specificElement != null)
            {
                var valAttribute = specificElement.Attribute("val");
                if (valAttribute != null)
                {
                    string valueAsString = valAttribute.Value;

                    try
                    {
                        returnValue = (T) Convert.ChangeType(valueAsString, typeof (T), CultureInfo.CurrentCulture);
                    }
                    catch (Exception) 
                    {
                        
                    }

                }
            }
            return returnValue;
        }

        public async Task<SonosDevice> GetSonosDeviceAsync()
        {
            var httpClient = new HttpClient();

            //var response = await httpClient.GetAsync(String.Format("http://{0}:{1}/xml/device_description.xml", _deviceDescriptionUrl, Constants.SonosPortNumber));
            var response = await httpClient.GetAsync(String.Format("http://{0}:{1}/", _deviceDescriptionUrl, Constants.SonosPortNumber));
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
                    //if (controller.ServiceID == serviceId)
                    //{
                    //    controller.ControlUrl = controlUrl;
                    //    controller.EventUrl = eventUrl;
                    //}
                }
            }

            return SonosDevice.Create(responseContents);
        }

        public T GetController<T>() where T : Controller
        {
            return _allControllers.OfType<T>().First();
        }

        public Task<int> GetVolumeAsync(string channel = "Master")
        {
            return GetController<RenderingController>().GetVolumeAsync(channel);
        }

        public Task<int> GetBassAsync()
        {
            return GetController<RenderingController>().GetBassAsync();
        }

        public Task<bool> GetIsMutedAsync(string channel = "Master")
        {
            return GetController<RenderingController>().GetIsMutedAsync(channel);
        }

        public Task SetVolumeAsync(int volume, string channel = "Master")
        {
            return GetController<RenderingController>().SetVolumeAsync(volume, channel);
        }

        public Task StopAsync()
        {
            return GetController<AVTransportController>().StopAsync();
        }

        public Task<MediaInfo> GetMediaInfoAsync()
        {
            return GetController<AVTransportController>().GetMediaInfoAsync();
        }

        public Task<PositionInfo> GetPositionInfoAsync()
        {
            return GetController<AVTransportController>().GetPositionInfoAsync();
        }

        public Task PlayAsync()
        {
            return GetController<AVTransportController>().PlayAsync(1);
        }

        public async Task EnableEventsAsync()
        {
            Task[] tasks = new Task[_allControllers.Count];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = _allControllers[i].SubscribeToEventsAsync();
            }
            await Task.Factory.ContinueWhenAll(tasks, tasks1 => { });
        }


        public Task<TransportInfo> GetTransportInfoAsync()
        {
            return GetController<AVTransportController>().GetTransportInfoAsync();
        }

        public Task<IEnumerable<ServiceDescription>> GetAvailableMusicServicesAsync()
        {
            return GetController<MusicServicesController>().ListAvailableServicesAsync();
        }

        public Task<bool> GetMuteAsync()
        {
            return GetController<RenderingController>().GetMuteAsync();
        }

        public Task MuteAsync()
        {
            return GetController<RenderingController>().SetMuteAsync(true);
        }

        public Task UnmuteAsync()
        {
            return GetController<RenderingController>().SetMuteAsync(false);
        }
    }

    public class VolumeChangedEventArgs : EventArgs
    {
        private readonly int _masterChannel;
        private readonly int _leftChannel;
        private readonly int _rightChannel;

        public int MasterVolume { get { return _masterChannel; } }

        public int LeftVolume
        {
            get { return _leftChannel; }
        }

        public int RightVolume
        {
            get { return _rightChannel; }
        }

        public VolumeChangedEventArgs(int masterChannel, int leftChannel, int rightChannel)
        {
            _masterChannel = masterChannel;
            _leftChannel = leftChannel;
            _rightChannel = rightChannel;
        }
    }

}
