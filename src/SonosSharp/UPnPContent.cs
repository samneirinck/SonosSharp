using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SonosSharp
{
    public class UPnPContentStream : Stream
    {
        private long _position;
        private static readonly byte[] PreContentBytes = Encoding.UTF8.GetBytes(@"<Envelope xmlns=""http://schemas.xmlsoap.org/soap/envelope/""><Body>");
        private readonly byte[] _contentBytes;
        private static readonly byte[] PostContentBytes = Encoding.UTF8.GetBytes(@"</Body></Envelope>");


        public UPnPContentStream(string actionName, string serviceNamespace)
        {
            _contentBytes = Encoding.UTF8.GetBytes($@"<a:{actionName} xmlns:a=""{serviceNamespace}"" />");
        }


        public override void Flush()
        {
        }
        

        public override int Read(byte[] buffer, int offset, int count)
        {
            int numBytesRead = 0;
            if (_position < PreContentBytes.Length)
            {
                int leftToRead = PreContentBytes.Length - (int)_position;
                Array.Copy(PreContentBytes, (int)_position, buffer, offset, Math.Min(leftToRead, count));
            }   
            return numBytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            Position = offset;
            return Position;
        }

        public override void SetLength(long value)
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead { get; } = true;
        public override bool CanSeek { get; } = true;
        public override bool CanWrite { get; } = false;
        public override long Length { get; }

        public override long Position
        {
            get { return _position; }
            set
            {
                if (value > Length)
                {
                    throw new ArgumentOutOfRangeException("Position");
                }
                _position = value;
            }
        }
    }

    public class UPnPContent : ByteArrayContent
    {
        //private static string template =
        //    @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body>{0}</s:Body></s:Envelope>";

        //private HttpContent _actualContent;

        public UPnPContent(string actionName)
            : base(null)
        {
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return null;
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 20;
            return true;

        }
    }
}
