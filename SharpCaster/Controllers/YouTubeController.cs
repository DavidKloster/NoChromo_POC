using System;
using System.Linq;
using System.Threading.Tasks;
using SharpCaster.Channels;
using SharpCaster.Extensions;
using SharpCaster.Models;

namespace SharpCaster.Controllers
{
    public class YouTubeController : BaseMediaController
    {
        public event EventHandler<string> ScreenIdChanged;
        private ChromeCastClient _client;
        public YouTubeController(ChromeCastClient client) : base(client, "233637DE")
        {
            _client = client;
            _client.Channels.GetYouTubeChannel().ScreenIdChanged += OnScreenIdChanged;
        }

        private void OnScreenIdChanged(object sender, string s)
        {
            ScreenIdChanged?.Invoke(this, s);
        }
        public async Task<string> BindToLounge()
        {
            return await _client.Channels.GetYouTubeChannel().BindToLounge();
        }
        public async Task<YoutubeLoungeBinding> GetLoungeID()
        {
            return await _client.Channels.GetYouTubeChannel().GetLoungeID();
        }
        public async Task<string> GetSessionInformation()
        {
            return await _client.Channels.GetYouTubeChannel().GetSessionInformation();
        }
    }

    public static class YouTubeControllerExtensions
    {
        public static async Task<YouTubeController> LaunchYouTube(this ChromeCastClient client)
        {
            if (!client.Channels.Any(x => x.Namespace == "233637DE"))
            {
                client.MakeSureChannelExist(new YouTubeChannel(client));
                var controller = new YouTubeController(client);
                await controller.LaunchApplication();
                return controller;
            }
            else
            {
                client.MakeSureChannelExist(new YouTubeChannel(client));
                var controller = new YouTubeController(client);
                await controller.ConnectApplication();
                return controller;
            }

        
        }
    }
}
