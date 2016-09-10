using System;
using System.Threading.Tasks;
using Xunit;

namespace SonosSharp.Services
{
    public class DeviceDiscoveryTest
    {
        private readonly DeviceDiscovery _service;

        public DeviceDiscoveryTest()
        {
            _service = new DeviceDiscovery();

        }

        [Fact]
        public async Task DiscoverDevices() 
        {
            await _service.DiscoverDevicesAsync();
        }
    }
}
