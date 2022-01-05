using SharpCaster.Controllers;
using SharpCaster.Models;
using SharpCaster.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoChromo
{
    public partial class Form1 : Form
    {
        YouTubeController _controller;
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
   
            ChromecastService.Current.ChromeCastClient.ConnectedChanged += ChromeCastClient_ConnectedChanged;
            ChromecastService.Current.ChromeCastClient.ApplicationStarted += ChromeCastClient_ApplicationStarted;
             
           
        }

        private void ChromeCastClient_ApplicationStarted(object sender, SharpCaster.Models.ChromecastStatus.ChromecastApplication e)
        {
           
        }

        private async void ChromeCastClient_ConnectedChanged(object sender, EventArgs e)
        {
            if (_controller == null) _controller =  await ChromecastService.Current.ChromeCastClient.LaunchYouTube();
            _controller.ScreenIdChanged += _controller_ScreenIdChanged;
        }

        private void _controller_ScreenIdChanged(object sender, string e)
        {
            throw new NotImplementedException();
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            ObservableCollection<Chromecast> chromecasts = await ChromecastService.Current.StartLocatingDevices();
            var chromecast = chromecasts.First();
          
             await ChromecastService.Current.ConnectToChromecast(chromecast);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            do
            {
                _controller.SetMute(true);
                Thread.Sleep(300);
                _controller.SetMute(false);
            } while (true);
           
        }
    }
}
