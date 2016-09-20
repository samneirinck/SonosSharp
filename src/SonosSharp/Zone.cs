using System.Collections.Generic;
using System.Xml.Linq;

namespace SonosSharp
{
    public class Zone
    {
        public string Id { get; private set; }
        public IReadOnlyCollection<Device> Devices { get; private set; }


        public static Zone FromXElement(XElement element)
        {
            var zone = new Zone();

            zone.Id = element.Attribute("ID").Value;

            return zone;
        }
    }
}
