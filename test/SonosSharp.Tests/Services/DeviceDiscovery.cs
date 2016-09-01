using System;
using System.Threading.Tasks;
using Xunit;

namespace SonosSharp.UnitTests
{
    public class DeviceDiscovery_DiscoverDevicesAsyncShould
    {
        private readonly object _service;

        public DeviceDiscovery_DiscoverDevicesAsyncShould()
        {
            _service = new object();
        }

        [Fact]
        public async Task DiscoverDevices() 
        {
            //var result = await _service.DiscoverDevicesAsync();

            //Assert.NotNull(result);
        }
    }
}
