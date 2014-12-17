using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SonosSharp
{
    public class ZoneGroupMember
    {
        private readonly string _id;
        private readonly string _location;
        private readonly string _name;
        private readonly string _icon;

        public ZoneGroupMember(XElement zoneGroupMemberElement)
        {
            _id = zoneGroupMemberElement.GetAttributeValueSafe("UUID");
            _location = zoneGroupMemberElement.GetAttributeValueSafe("Location");
            _name = zoneGroupMemberElement.GetAttributeValueSafe("ZoneName");
            _icon = zoneGroupMemberElement.GetAttributeValueSafe("Icon");
        }

        public string Id { get { return _id; } }
        public string Location { get { return _location; } }
        public string Name { get { return _name; } }
        public string Icon { get { return _icon; } }
    }
}
