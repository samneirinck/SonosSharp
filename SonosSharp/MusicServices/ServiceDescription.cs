using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SonosSharp.MusicServices;
using System.Xml.Linq;

namespace SonosSharp.MusicService
{
    public class ServiceDescription
    {
        private readonly int _id;
        private readonly string _name;
        private readonly string _version;
        private readonly string _uri;
        private readonly string _secureUri;
        private readonly string _containerType;
        private readonly int _capabilities;
        private readonly int _maxMessagingChars;
        private readonly ServicePolicy _policy;
        private readonly string _stringsUri;
        private readonly string _presentationMapUri;

        public int Id { get { return _id; } }
        public string Name { get { return _name; } }
        public string Version { get { return _version; } }
        public string Uri { get { return _uri; } }
        public string SecureUri { get { return _secureUri; } }
        public string ContainerType { get { return _containerType; } }
        public int Capabilities { get { return _capabilities; } }
        public int MaxMessagingChars { get { return _maxMessagingChars; } }
        public ServicePolicy ServicePolicy { get { return _policy; } }
        public string StringsUri { get { return _stringsUri; } }
        public string PresentationMapUri { get { return _presentationMapUri; } }
        
        public ServiceDescription(XElement serviceElement)
        {
            if (serviceElement == null)
            {
                throw new ArgumentNullException("serviceElement");
            }

            this._id = serviceElement.GetAttributeValueSafe<int>("Id");
            this._name = serviceElement.GetAttributeValueSafe("Name");
            this._version = serviceElement.GetAttributeValueSafe("Version");
            this._uri = serviceElement.GetAttributeValueSafe("Uri");
            this._secureUri = serviceElement.GetAttributeValueSafe("SecureUri");
            this._containerType = serviceElement.GetAttributeValueSafe("ContainerType");
            this._capabilities = serviceElement.GetAttributeValueSafe<int>("Capabilities");
            this._maxMessagingChars = serviceElement.GetAttributeValueSafe<int>("MaxMessagingChars");

            this._policy = new ServicePolicy(serviceElement.Element("Policy"));

            var presentation = serviceElement.Element("Presentation");
            if (presentation != null)
            {
                this._stringsUri = presentation.Element("Strings").GetAttributeValueSafe("Uri");
                this._presentationMapUri = presentation.Element("PresentationMap").GetAttributeValueSafe("Uri");
            }
        }

    }
}
