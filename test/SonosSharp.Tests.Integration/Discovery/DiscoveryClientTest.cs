using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SonosSharp
{
    public class DiscoveryClientTest
    {
        public class TheDiscoverDevicesMethod
        {
            [Fact]
            public async Task FindsDevices()
            {
                // Arrange
                var client = new DiscoveryClient();

                // Act
                var devices = await client.DiscoverDevicesAsync();

                // Assert
                Assert.NotEmpty(devices);
            }
        }
    }
}
