using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web;
using Newtonsoft.Json.Linq;
using vkPlayer.Properties;

namespace vkPlayer
{
    public partial class PlayerForm : Form
    {
        private List<Audio> audioList;
        WMPLib.IWMPPlaylist playlist;
        WMPLib.IWMPMedia media;

        public PlayerForm()
        {
            InitializeComponent();
        }

        private void PlayerFormLoad(object sender, EventArgs e)
        {
            if (!authData.Default.auth)
            {
                new AuthenticationForm().Show();
            }
            this.Invoke((MethodInvoker)(
                () => {
                        while (!authData.Default.auth) { Thread.Sleep(500); }
                        try
                        {
                            audioList = GetTrackList();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show("Ошибка: " + exception.Message);
                            this.Invoke((MethodInvoker)this.Close);
                        }
                        this.Invoke((MethodInvoker)LoadTrackList);
                }));

        }
        
        private void TrackClick(object sender, MouseEventArgs e)
        {
            if (TrackListBox.Items.Count != 0)
            {
                if (e.Button == MouseButtons.Left)
                {
                    MediaPlayer.Ctlcontrols.play();
                    MediaPlayer.Ctlcontrols.currentItem = MediaPlayer.currentPlaylist.get_Item(TrackListBox.SelectedIndex);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    var y = e.Y / TrackListBox.ItemHeight;
                    if (y < TrackListBox.Items.Count)
                    {
                        TrackListBox.SelectedIndex = TrackListBox.TopIndex + y;
                    }
                    var audio = audioList[TrackListBox.SelectedIndex];
                    var fileName = Resources.path + audio.artist + " - " + audio.title + ".mp3";
                    new WebClient().DownloadFile(audio.url, fileName);
                }
            }
            
        }

        private List<Audio> GetTrackList()
        {
            WebRequest request = WebRequest.Create("https://api.vk.com/method/audio.get?owner_id=" +
                                                    authData.Default.id +
                                                    "need_user=0&access_token=" +
                                                    authData.Default.token);
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var result = reader.ReadToEnd();
            reader.Close();
            response.Close();
            result = HttpUtility.HtmlDecode(result);

            JToken token = JToken.Parse(result);
            return token["response"].Children().Skip(1).Select(c => c.ToObject<Audio>()).ToList();
        }

        private void LoadTrackList()
        {
            playlist = MediaPlayer.playlistCollection.newPlaylist("fromVK"); 
            foreach (var audio in audioList)
            {
                media = MediaPlayer.newMedia(audio.url);
                playlist.appendItem(media);
                TrackListBox.Items.Add(audio.artist + " - " + audio.title);
            }
            MediaPlayer.currentPlaylist = playlist;
            MediaPlayer.Ctlcontrols.stop();
        }

    }
}
