using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonosSharp
{
    public interface IDiscoveryClient
    {
        Task<IReadOnlyCollection<Zone>> DiscoverZonesAsync();
        Task<IReadOnlyCollection<Zone>> DiscoverZonesAsync(CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Device>> DiscoverDevicesAsync();
        Task<IReadOnlyCollection<Device>> DiscoverDevicesAsync(CancellationToken cancellationToken);
    }
}