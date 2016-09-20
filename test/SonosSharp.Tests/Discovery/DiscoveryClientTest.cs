using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace SonosSharp
{
    public class DiscoveryClientTest
    {
        public class TheCtor
        {
            [Fact]
            public void EnsuresNonNullArguments()
            {
                Assert.Throws<ArgumentNullException>(
                    () => new DiscoveryClient(null));
            }
        }

        public class TheDiscoverDevicesMethod
        {
            [Fact]
            public async Task IsCancellable()
            {
                // Arrange
                var udpClientFactory = Substitute.For<IUdpClientFactory>();
                var receiveTask = TestTask.NeverReturns<UdpReceiveResult>();
                udpClientFactory.Create(Arg.Any<IPEndPoint>()).ReceiveAsync().Returns(receiveTask);
                CancellationTokenSource cts = new CancellationTokenSource();

                var task = new DiscoveryClient(udpClientFactory).DiscoverDevicesAsync(cts.Token);

                // Act
                cts.Cancel();

                // Assert
                await Assert.ThrowsAsync<TaskCanceledException>(() => task);
                Assert.NotEqual(TaskStatus.Running, receiveTask.Status);
            }

            [Fact]
            public async Task ReturnsEmptyCollectionWhenNoDevicesAreFound()
            {
                // Arrange
                var udpClientFactory = Substitute.For<IUdpClientFactory>();
                udpClientFactory.Create(Arg.Any<IPEndPoint>()).ReceiveAsync().Returns(new UdpReceiveResult());

                var discoveryClient = new DiscoveryClient(udpClientFactory);

                // Act
                var result = await discoveryClient.DiscoverDevicesAsync();

                // Assert
                Assert.Empty(result);
            }
        }
    }
}
