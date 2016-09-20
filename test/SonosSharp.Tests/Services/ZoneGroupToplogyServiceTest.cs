using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SonosSharp.Services
{
    public class ZoneGroupToplogyServiceTest
    {
        [Fact]
        public async Task Do()
        {
            // Arrange
            var svc = new ZoneGroupTopologyService(null);

            // Act
            await svc.GetZoneGroupStateAsync();

            // Assert
        }
    }
}
