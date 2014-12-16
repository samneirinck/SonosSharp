using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SonosSharp
{
    public class ZoneGroup
    {
        private readonly string _id;
        private readonly string _coordinator;
        private readonly ReadOnlyCollection<ZoneGroupMember> _members;

        public ZoneGroup(XElement zoneGroupElement)
        {
            _id = zoneGroupElement.GetAttributeValueSafe("ID");
            _coordinator = zoneGroupElement.GetAttributeValueSafe("Coordinator");
            _members = new ReadOnlyCollection<ZoneGroupMember>(
                zoneGroupElement.Elements("ZoneGroupMember").Select(x => new ZoneGroupMember(x)).ToList()
                );
        }

        public string Id { get { return _id; } }
        public string Coordinator { get { return _coordinator; } }
        public ReadOnlyCollection<ZoneGroupMember> Members { get { return _members; } }
    }
}
