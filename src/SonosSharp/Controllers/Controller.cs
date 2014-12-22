using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SonosSharp.Eventing;

namespace SonosSharp.Controllers
{
    public abstract class Controller
    {
        public abstract string ServiceType { get; }

        public string ServiceId { get; set; }
        public string ScpdUrl { get; set; }
        public string ControlUrl { get; set; }
        public string EventUrl { get; set; }

        public BasicHttpServer HttpServer { get; set; }

        protected string IpAddress { get; private set; }
        
        public string ActionNamespace
        {
            get
            {
                return ServiceType;
            }
        }
        protected XNamespace ActionNS { get { return ActionNamespace; } }
        protected string SoapBodyTemplate
        {
            get { return "<?xml version=\"1.0\" encoding=\"utf-8\"?><s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body>{0}</s:Body></s:Envelope>"; }
        }



        protected Controller(string ipAddress)
        {
            IpAddress = ipAddress;
        }


        protected Task InvokeActionAsync(string action)
        {
            return InvokeActionAsync(action, null);
        }
        protected async Task InvokeActionAsync(string action, Dictionary<string, object> properties)
        {
            var requestString = BuildRequestString(action, properties);
            var httpClient = new HttpClient();
            var content = new StringContent(requestString);

            content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml");
            content.Headers.Add("SOAPAction", String.Format("\"{0}#{1}\"", ActionNamespace, action));

            var result = await httpClient.PostAsync(String.Format("http://{0}:{1}/{2}", IpAddress, Constants.SonosPortNumber, ControlUrl.StartsWith("/") ? ControlUrl.Substring(1) : ControlUrl), content);
            if (!result.IsSuccessStatusCode)
            {
                throw new InvalidOperationException();
            }
        }

        protected Task<T> InvokeFuncAsync<T>(string action)
        {
            return InvokeFuncAsync<T>(action, null);
        }
        protected async Task<T> InvokeFuncAsync<T>(string action, Dictionary<string, object> properties)
        {
            string resultString = await InvokeFuncWithResultAsync(action, properties);

            var singleResult = ((XElement)((XElement)((XElement)XElement.Parse(resultString).FirstNode).FirstNode).FirstNode).Value;

            return (T)Convert.ChangeType(singleResult, typeof(T), CultureInfo.CurrentCulture);
        }

        protected Task<string> InvokeFuncWithResultAsync(string action)
        {
            return InvokeFuncWithResultAsync(action, null);
        }
        protected async Task<string> InvokeFuncWithResultAsync(string action,
                                                                       Dictionary<string, object> properties)
        {
            var requestString = BuildRequestString(action, properties);
            var httpClient = new HttpClient();
            var content = new StringContent(requestString);

            content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml");
            content.Headers.Add("SOAPAction", String.Format("\"{0}#{1}\"", ActionNamespace, action));

            var result = await httpClient.PostAsync(String.Format("http://{0}:{1}/{2}", IpAddress, Constants.SonosPortNumber, ControlUrl.StartsWith("/") ? ControlUrl.Substring(1) : ControlUrl), content);

            var arr = await result.Content.ReadAsByteArrayAsync();
            string resultString = Encoding.UTF8.GetString(arr, 0, arr.Length);
            return resultString;
        }

        private string BuildRequestString(string action, Dictionary<string, object> properties)
        {
            var requestBuilder = new StringBuilder();

            requestBuilder.AppendFormat("<u:{0} xmlns:u=\"{1}\">", action, ActionNamespace);

            if (properties == null)
            {
                properties = new Dictionary<string, object>();
            }

            if (!properties.ContainsKey("InstanceID"))
            {
                properties["InstanceID"] = 0;
            }

            foreach (var property in properties)
            {
                requestBuilder.AppendFormat("<{0}>{1}</{0}>", property.Key, property.Value);
            }

            requestBuilder.AppendFormat("</u:{0}>", action);
            return String.Format(SoapBodyTemplate, requestBuilder);
        }

        public async Task SubscribeToEventsAsync()
        {
            if (String.IsNullOrEmpty(this.EventUrl))
                throw new InvalidOperationException("Cannot subscribe to events on a controller without EventUrl set");

            if (this.HttpServer == null)
                throw new InvalidOperationException("Unable to subscribe to events without http server");

            if (!HttpServer.IsRunning)
            {
                await this.HttpServer.StartAsync();
            }

            var httpClient = new HttpClient();

            var requestMessage = new HttpRequestMessage();
            requestMessage.Method = new HttpMethod("SUBSCRIBE");
            requestMessage.RequestUri = new Uri(String.Format("http://{0}:{1}{2}", IpAddress, Constants.SonosPortNumber, EventUrl));
            requestMessage.Headers.Host = String.Format("{0}:{1}", IpAddress, Constants.SonosPortNumber);
            requestMessage.Headers.Add("CALLBACK", "<" + this.HttpServer.CallbackUrl + ">");
            requestMessage.Headers.Add("NT", "upnp:event");
            requestMessage.Headers.Add("TIMEOUT", "Second-300");

            var result = await httpClient.SendAsync(requestMessage);
            if (!result.IsSuccessStatusCode)
            {
                string res = await result.Content.ReadAsStringAsync();
                throw new Exception("Unable to subscribe for events");
            }
        }

        public async Task UnsubscribeFromEventsAsync()
        {
            if (String.IsNullOrEmpty(this.EventUrl))
                throw new InvalidOperationException("Cannot subscribe to events on a controller without EventUrl set");

            if (this.HttpServer == null)
                throw new InvalidOperationException("Unable to subscribe to events without http server");

            if (this.HttpServer.IsRunning)
            {
                await this.HttpServer.StopAsync();
            }
        }

    }
}
