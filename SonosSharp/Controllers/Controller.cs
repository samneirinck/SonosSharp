using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SonosSharp.Controllers
{
    public abstract class Controller
    {
        protected abstract string ControlUrl { get; }
        protected abstract string ActionNamespace { get; }

        protected Controller(string ipAddress)
        {
            IpAddress = ipAddress;
        }
        
        protected string IpAddress { get; private set; }
        protected string SoapBodyTemplate
        {
            get { return "<?xml version=\"1.0\" encoding=\"utf-8\"?><s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body>{0}</s:Body></s:Envelope>"; }
        }
        protected XNamespace ActionNS { get { return ActionNamespace; } }

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

            var result = await httpClient.PostAsync(String.Format("http://{0}:{1}{2}", IpAddress, Constants.SonosPortNumber, ControlUrl), content);
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

            var result = await httpClient.PostAsync(String.Format("http://{0}:{1}{2}", IpAddress, Constants.SonosPortNumber, ControlUrl), content);

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
    }
}
