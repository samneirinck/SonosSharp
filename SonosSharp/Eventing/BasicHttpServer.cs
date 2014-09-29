using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SonosSharp.Eventing
{
    public abstract class BasicHttpServer
    {
        private bool _isRunning;

        public bool IsRunning { get { return _isRunning; } }

        public abstract string CallbackUrl { get; }
        protected abstract Task StartInternalAsync();
        protected abstract Task StopInternalAsync();
        public event EventHandler<HttpVariableChangedEventArgs> VariableChanged;

        public async Task StartAsync()
        {
            _isRunning = true;
            await StartInternalAsync();
        }

        public async Task StopAsync()
        {
            await StopInternalAsync();
            _isRunning = false;
        }

        protected void OnVariableChanged(string variableName, string variableValue)
        {
            if (VariableChanged != null)
            {
                VariableChanged(this, new HttpVariableChangedEventArgs(variableName, variableValue));
            }
        }

        protected bool ProcessHttpRequest(string fullRequest)
        {
            bool success = true;


            using (StringReader reader = new StringReader(fullRequest))
            {
                var headers = GetHeaders(reader);
                if (ShouldHandleRequest(headers))
                {
                    string contents = reader.ReadToEnd();

                    var propertySetElement = XElement.Parse(contents);
                    var variables =
                        propertySetElement.Elements(Constants.UpnpEventNamespace + "property").Elements().ToList();
                    foreach (var variable in variables)
                    {
                        //Debug.WriteLine("{0}: {1}",variable.Name.LocalName, variable.Value);
                        OnVariableChanged(variable.Name.LocalName, variable.Value);
                    }

                }
            }



            return success;

        }

        private bool ShouldHandleRequest(Dictionary<string, string> headers)
        {
            bool shouldHandle = true;

            shouldHandle &= headers.ContainsKey("NT") &&
                              string.Equals(headers["NT"], "upnp:event", StringComparison.OrdinalIgnoreCase);
            shouldHandle &= headers.ContainsKey("NTS") &&
                              string.Equals(headers["NTS"], "upnp:propchange", StringComparison.OrdinalIgnoreCase);
            shouldHandle &= headers.ContainsKey("SID");
            shouldHandle &= headers.ContainsKey("SEQ");

            return shouldHandle;
        }

        private Dictionary<string, string> GetHeaders(StringReader reader)
        {
            var results = new Dictionary<string, string>();
            string currentLine = reader.ReadLine();
            while (!String.IsNullOrWhiteSpace(currentLine))
            {
                int indexOfColon = currentLine.IndexOf(':');
                if (indexOfColon < 0)
                {
                    break;
                }

                string key = currentLine.Substring(0, indexOfColon);
                string value = currentLine.Substring(indexOfColon + 2);


                results.Add(key, value);
                currentLine = reader.ReadLine();
            }
            return results;
        }
    }

    public class HttpVariableChangedEventArgs : EventArgs
    {
        public string VariableName { get; set; }
        public string VariableValue { get; set; }

        public HttpVariableChangedEventArgs(string variableName, string variableValue)
        {
            this.VariableName = variableName;
            this.VariableValue = variableValue;
        }
    }
   
}