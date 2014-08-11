using System;
using System.Threading.Tasks;
namespace SonosSharp.Discovery
{
    public interface IDeviceDiscovery
    {
        Task SearchForSonosDevicesAsync(Action<string> deviceFound);
    }
}
