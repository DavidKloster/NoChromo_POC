using Newtonsoft.Json;
using SharpCaster.Models;
using SharpCaster.Models.CustomTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpCaster.Channels
{
    public class YouTubeChannel : ChromecastChannel
    {
        public static string Urn = "urn:x-cast:com.google.youtube.mdx";
        private int _rid = 0;
        private int _req_count = 0;
        private string _gsessionid;
        private string _sid;
        private string _screenid;
        private YoutubeLoungeBinding _currentlounge;
        public event EventHandler<string> ScreenIdChanged;
        public YouTubeChannel(ChromeCastClient client) : base(client, Urn)
        {
            MessageReceived += YouTubeChannel_MessageReceived;
            ScreenIdChanged += YouTubeChannel_ScreenIdChanged;
        }

        private void YouTubeChannel_ScreenIdChanged(object sender, string e)
        {
            _screenid = e;
        }

        private void YouTubeChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            var json = e.Message.PayloadUtf8;
            var response = JsonConvert.DeserializeObject<YouTubeSessionStatusResponse>(json);
            ScreenIdChanged?.Invoke(this, response.Data.ScreenId);
        }
        
        public bool InSession()
        {
            if (_gsessionid.Any() && _currentlounge != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task StartSession()
        {
            await GetLoungeID();
            await BindToLounge();
        }
        public async void PlayVideo()
        {
            if (!InSession())
            {
                await StartSession();
            }
            using (var client = new HttpClient())
            {
                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("screen_ids", _screenid));
                var req = new HttpRequestMessage(HttpMethod.Post, YoutubeChannelConfiguration.LOUNGE_TOKEN_URL) { Content = new FormUrlEncodedContent(nvc) };
                var nice = await client.SendAsync(req);
                _currentlounge = JsonConvert.DeserializeObject<YoutubeLoungeBinding>(await nice.Content.ReadAsStringAsync());
            
            }

        }
        public async Task<string> BindToLounge()
        {
            if (_screenid.Any() && _currentlounge != null)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-YouTube-LoungeId-Token", _currentlounge.screens.First().loungeToken);
                    var req = new HttpRequestMessage(HttpMethod.Post, YoutubeChannelConfiguration.BIND_URL + $"?RID={this._rid}&VER=8&CVER=1&id={new Random().Next(1, Int32.MaxValue)}&device=REMOTE_CONTROL&name=CliteriusRex&mdx-version=3&pairing_type=cast&app=android-phone-13.14.55");
                    try
                    {
                        var nice = await client.SendAsync(req);
                        var regdata = await nice.Content.ReadAsStringAsync();
                        _gsessionid = Regex.Match(regdata, "\"S\",(.*?)]").Groups[1].Value.Replace("\"","");
                        _sid = Regex.Match(regdata, "\"c\",\"(.*?)\",\\\"").Groups[1].Value;
                        return regdata;
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }                         
               
                }

            }
            else
            {
                return null;
            }
        }
        public async Task<YoutubeLoungeBinding> GetLoungeID()
        {

            if (_screenid.Any())
            {
                using (var client = new HttpClient())
                {
                    var nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("screen_ids",_screenid));
                    var req = new HttpRequestMessage(HttpMethod.Post, YoutubeChannelConfiguration.LOUNGE_TOKEN_URL) { Content = new FormUrlEncodedContent(nvc) };
                    var nice = await client.SendAsync(req);
                    _currentlounge = JsonConvert.DeserializeObject<YoutubeLoungeBinding>(await nice.Content.ReadAsStringAsync());
                    return _currentlounge;
                }

            }
            else
            {
                return null;
            }
        }

    }

    public static class YoutubeChannelConfiguration
    {
        public static string YOUTUBE_BASE_URL = @"https://www.youtube.com/";
        public static string BIND_URL = YOUTUBE_BASE_URL + @"api/lounge/bc/bind";
        public static string LOUNGE_TOKEN_URL = YOUTUBE_BASE_URL + @"api/lounge/pairing/get_lounge_token_batch";
        public static string QUEUE_AJAX_URL = YOUTUBE_BASE_URL + "watch_queue_ajax";


      
    }
    public static class YouTubeChannelExtesion
    {
        public static YouTubeChannel GetYouTubeChannel(this IEnumerable<IChromecastChannel> channels)
        {
            return (YouTubeChannel)channels.First(x => x.Namespace == YouTubeChannel.Urn);
        }
    }
}
