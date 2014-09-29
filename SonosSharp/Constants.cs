using System.Xml.Linq;
namespace SonosSharp
{
    public static class Constants
    {
        public const int SonosPortNumber = 1400;

        public static XNamespace UpnpEventNamespace = @"urn:schemas-upnp-org:event-1-0";
        public static XNamespace UpnpNamespace = @"urn:schemas-upnp-org:device-1-0";
        public static XNamespace SoapNamespace = @"http://schemas.xmlsoap.org/soap/envelope/";
        public static XNamespace RenderingControl = @"urn:schemas-upnp-org:service:RenderingControl:1";

    }
}
