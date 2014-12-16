using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SonosSharp.Eventing
{
    public class HttpServer : BasicHttpServer
    {
        private bool _isRunning;
        private StreamSocketListener _listener;
        private int portNumber = 8088;
        private string ipAddress;

        protected override async Task StartInternalAsync()
        {
            var hostnames = NetworkInformation.GetHostNames();

            var ips = hostnames.Select(x => x.DisplayName).ToList();
            ipAddress = ips.Last();

            _listener = new StreamSocketListener();
            _listener.Control.QualityOfService = SocketQualityOfService.Normal;
            _listener.ConnectionReceived += _listener_ConnectionReceived;
            _listener.ConnectionReceived += OnConnectionReceived;
            //await _listener.BindServiceNameAsync("36648");
            await _listener.BindServiceNameAsync(portNumber.ToString());
            _isRunning = true;
        }

        void _listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            
        }

        protected override Task StopInternalAsync()
        {
            throw new NotImplementedException();
        }


        public void Stop()
        {
            if (_listener != null)
            {
                _listener.Dispose();
            }
        }

        private void OnConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            HandleRequest(args.Socket);
        }

        private static async Task<string> StreamReadLine(DataReader reader)
        {
            int next_char;
            string data = "";
            while (true)
            {
                await reader.LoadAsync(1);
                next_char = reader.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                data += Convert.ToChar(next_char);
            }
            return data;
        }


        private async Task<string> ReadToEndAsync(DataReader reader)
        {
            StringBuilder stringBuilder = new StringBuilder();
            uint length = 128;
            uint consumed = length;
            reader.InputStreamOptions = InputStreamOptions.Partial;

            while (consumed >= length)
            {
                consumed = await reader.LoadAsync(length);

                stringBuilder.Append(reader.ReadString(consumed));
            }
            return stringBuilder.ToString();
        }

        private async Task HandleRequest(StreamSocket socket)
        {

            //Initialize IO classes
            DataReader inputReader = new DataReader(socket.InputStream);
            DataWriter outputWriter = new DataWriter(socket.OutputStream);
            outputWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

            try
            {
                //handle actual HTTP request
                string requestString = await ReadToEndAsync(inputReader);

                bool processed = base.ProcessHttpRequest(requestString);

                if (processed)
                {

                    //HTTP header
                    outputWriter.WriteString("HTTP/1.0 200 OK\r\n");
                    outputWriter.WriteString("Connection: close\r\n");
                }
                else
                {
                    outputWriter.WriteString("HTTP/1.0 500 Internal server error\r\n");
                    outputWriter.WriteString("Connection: close\r\n");
                    outputWriter.WriteString("\r\n");
                }

                await outputWriter.StoreAsync(); //actually write data to the network interface
            }
            catch (Exception ex)
            {
                outputWriter.WriteString("HTTP/1.0 500 Internal server error\r\n");
                outputWriter.WriteString("Connection: close\r\n");
                outputWriter.WriteString("\r\n");
                outputWriter.WriteString(ex.ToString());
            }
            finally
            {
                socket.Dispose();
            }
        }

        public override string CallbackUrl
        {
            get { return String.Format("http://{0}:{1}/", ipAddress, portNumber); }
        }

    }
}
