using System.Linq;
using System.Xml.Linq;
namespace SonosSharp
{
    public class SonosDevice
    {
        private XDocument _description;

        private SonosDevice(string xml)
        {
            _description = XDocument.Parse(xml);

            ModelNumber =GetDeviceProperty("modelNumber");
            ModelDescription = GetDeviceProperty("modelDescription");
            ModelName = GetDeviceProperty("modelName");
            SoftwareVersion = GetDeviceProperty("softwareVersion");
            HardwareVersion = GetDeviceProperty("hardwareVersion");
            RoomName = GetDeviceProperty("roomName");
        }

        public static SonosDevice Create(string xmlDescription)
        {
            var device = new SonosDevice(xmlDescription);

            return device;
        }

        public DeviceType DeviceType { get; private set; }

        public string ModelNumber { get; private set; }
        public string ModelDescription { get; private set; }
        public string ModelName { get; private set; }

        public string SoftwareVersion { get; private set; }
        public string HardwareVersion { get;private set; }

        public string RoomName { get; private set; }



        private string GetDeviceProperty(string propertyName)
        {
            var node = _description.Descendants(Constants.UpnpNamespace + "root")
                                   .Descendants(Constants.UpnpNamespace + "device")
                                   .Descendants(Constants.UpnpNamespace + propertyName).FirstOrDefault();
            return node != null ? node.Value : string.Empty;
        }
    }
}
