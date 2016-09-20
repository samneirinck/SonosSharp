using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SonosSharp.Services
{
    public abstract class UPnPService
    {
        //<?xml version=\"1.0\" encoding=\"utf-8\"?><s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body>{0}</s:Body></s:Envelope>
        private const int ServicePortNumber = 1400;

        public IPAddress IpAddress { get; }
        protected HttpClient HttpClient { get; }
        protected Uri ServiceUri { get; }
        protected abstract string SoapNamespace { get; }

        protected UPnPService(IPAddress ipAddress, string serviceName)
        {
            if (ipAddress == null)
                throw new ArgumentNullException(nameof(ipAddress));
            if (serviceName == null)
                throw new ArgumentNullException(nameof(serviceName));

            IpAddress = ipAddress;
            HttpClient = new HttpClient();

            ServiceUri = new Uri($"http://{ipAddress}:{ServicePortNumber}/{serviceName}/Control");
        }


        protected async Task<XElement> InvokeActionAsync(string actionName)
        {
            Console.WriteLine("Invoking " + actionName);
            var content = new StringContent(@"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body><u:GetZoneGroupState xmlns:u=""urn:schemas-upnp-org:service:ZoneGroupTopology:1""/></s:Body></s:Envelope>");

            content.Headers.Add("SOAPACTION", "urn:schemas-upnp-org:service:ZoneGroupTopology:1#GetZoneGroupState");

            var htpResult = await HttpClient.PostAsync(ServiceUri, content).ConfigureAwait(false);
            Console.WriteLine($"Got result {htpResult.StatusCode}");

            var xElement = XElement.Load(await htpResult.Content.ReadAsStreamAsync().ConfigureAwait(false));

            return xElement.Descendants().First().Descendants().First().Descendants().First();
        }
    }
}
