using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonosSharp.Controllers
{
    public class RenderingController : Controller
    {
        public RenderingController(string ipAddress) : base(ipAddress)
        {
        }

        protected override string ControlUrl
        {
            get { return "/MediaRenderer/RenderingControl/Control"; }
        }

        protected override string ActionNamespace
        {
            get { return "urn:schemas-upnp-org:service:RenderingControl:1"; }
        }

        public Task<int> GetVolumeAsync(string channel)
        {
            return InvokeFuncAsync<int>("GetVolume",
                                    new Dictionary<string, object> {{"Channel", channel}});
        }

        public Task<int> GetBassAsync()
        {
            return InvokeFuncAsync<int>("GetBass");
        }

        public async Task<bool> GetHeadphoneConnectedAsync()
        {
            int result = await InvokeFuncAsync<int>("GetHeadphoneConnected");

            return result != 0;
        }

        public async Task<bool> GetLoudnessEnabledAsync()
        {
            int result = await InvokeFuncAsync<int>("GetLoudness");

            return result != 0;
        }

        public async Task<bool> GetIsMutedAsync(string channel)
        {
            int result = await InvokeFuncAsync<int>("GetMute", new Dictionary<string, object> {{"Channel", channel}});

            return result != 0;
        }

        public Task<int> GetTrebleAsync()
        {
            return InvokeFuncAsync<int>("GetTreble");
        }

        public Task<int> GetVolumeInDbAsync(string channel)
        {
            return InvokeFuncAsync<int>("GetVolumeInDBAsync", new Dictionary<string, object> {{"Channel", channel}});
        }


        public Task SetVolumeAsync(int volume, string channel)
        {
            return InvokeActionAsync("SetVolume", new Dictionary<string, object> {{"Channel", channel}, {"DesiredVolume", volume}});
        }

    }
}
