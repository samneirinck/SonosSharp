using System;
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
        private int portNumber = 8085;
        private string ipAddress;

        protected override async Task StartInternalAsync()
        {
            var hostnames = NetworkInformation.GetHostNames();

            var ips = hostnames.Select(x => x.DisplayName).ToList();
            ipAddress = ips.First();

            _listener = new StreamSocketListener();
            _listener.Control.QualityOfService = SocketQualityOfService.Normal;
            _listener.ConnectionReceived += OnConnectionReceived;
            //await _listener.BindServiceNameAsync("36648");
            await _listener.BindServiceNameAsync(portNumber.ToString());
            _isRunning = true;
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


        private async Task HandleRequest(StreamSocket socket)
        {
            //Initialize IO classes
            DataReader reader = new DataReader(socket.InputStream);
            DataWriter writer = new DataWriter(socket.OutputStream);
            writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

            //handle actual HTTP request
            String request = await StreamReadLine(reader);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            string httpMethod = tokens[0].ToUpper();
            string httpUrl = tokens[1];

            //read HTTP headers - contents ignored in this sample
            while (!String.IsNullOrEmpty(await StreamReadLine(reader))) ;

            // ... writing of the HTTP response happens here
            StringBuilder ret = new StringBuilder();
            try
            {
                bool notFound = false;

                //HTTP header
                ret.AppendLine("HTTP/1.0 200 OK");
                ret.AppendLine("Content-Type: text/html");
                ret.AppendLine("Connection: close");
                ret.AppendLine("");

                //beginning of HTML element
                ret.AppendLine("<html><body><h1>Okidoki</h1></body></html>");
                writer.WriteString(ret.ToString());
            }

            catch (Exception ex)//any exception leads to an Internal server error
            {
                writer.WriteString("HTTP/1.0 500 Internal server error\r\n");
                writer.WriteString("Connection: close\r\n");
                writer.WriteString("\r\n");
                writer.WriteString(ex.Message);
            }


            await writer.StoreAsync();//actually write data to the network interface

            socket.Dispose();
        }

        public override string CallbackUrl
        {
            get { return String.Format("http://{0}:{1}/", ipAddress, portNumber); }
        }

    }
}
