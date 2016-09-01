using System;
using System.Threading.Tasks;
using Xunit;
using SonosSharp.Services;

namespace SonosSharp.UnitTests
{
    public class DeviceDiscovery_DiscoverDevicesAsyncShould
    {
        private readonly DeviceDiscovery _service;

        public DeviceDiscovery_DiscoverDevicesAsyncShould()
        {
            _service = new DeviceDiscovery();
        }

        [Fact]
        public async Task DiscoverDevices() 
        {
            var result = await _service.DiscoverDevicesAsync();

            Assert.NotNull(result);
        }
    }
}
