using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SonosSharp.Eventing
{
    public class HttpServer : BasicHttpServer
    {
        private HttpListener _listener;
        public override string CallbackUrl
        {
            get { return "http://169.254.80.80:8889/"; }
        }

        protected override async System.Threading.Tasks.Task StartInternalAsync()
        {
            await Task.Run(() =>
                {
                    _listener = new HttpListener();
                    _listener.Prefixes.Add(CallbackUrl);
                    _listener.Start();
                    ThreadPool.QueueUserWorkItem(state =>
                        {
                            while (true)
                            {
                                var context = _listener.GetContext();
                                StringBuilder fullRequest = new StringBuilder();

                                foreach (string header in context.Request.Headers)
                                {
                                    fullRequest.AppendLine(String.Format("{0}: {1}", header, context.Request.Headers[header]));
                                }

                                fullRequest.AppendLine();

                                using (var reader = new StreamReader(context.Request.InputStream))
                                {
                                    fullRequest.Append(reader.ReadToEnd());
                                }

                                bool result = base.ProcessHttpRequest(fullRequest.ToString());

                                context.Response.StatusCode = result ? 200 : 500;

                                context.Response.Close();

                            }
                        });
                });
        }

        protected override async System.Threading.Tasks.Task StopInternalAsync()
        {
        }
    }
}
