using System.Xml.Linq;
namespace SonosSharp
{
    public static class Constants
    {
        public const int SonosPortNumber = 1400;

        public static XNamespace UpnpEventNamespace = @"urn:schemas-upnp-org:event-1-0";
        public static XNamespace UpnpNamespace = @"urn:schemas-upnp-org:device-1-0";
        public static XNamespace UpnpMetadataNamespace = @"urn:schemas-upnp-org:metadata-1-0/upnp/";
        public static XNamespace SoapNamespace = @"http://schemas.xmlsoap.org/soap/envelope/";
        public static XNamespace RenderingControl = @"urn:schemas-upnp-org:service:RenderingControl:1";
        public static XNamespace PurlDcNamespace = @"http://purl.org/dc/elements/1.1/";
        public static XNamespace DidlNamespace = @"urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/";
    }
}
