using System;
using System.Windows.Forms;
using vkPlayer.Properties;

namespace vkPlayer
{
    public partial class AuthenticationForm : Form
    {
        public AuthenticationForm()
        {
            InitializeComponent();
        }
        private void AuthenticationFormLoad(object sender, EventArgs e)
        {
            AuthenticationBrowser.Navigate(Resources.url);
        }
        private void LoadingCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                SetAuthenticationData();
                this.Close();
            }
            catch (FormatException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
        private void SetAuthenticationData() 
        {
            string url = AuthenticationBrowser.Url.ToString();
            if (url.Contains("#"))
            {
                authData.Default.token = url.Split('#')[1].Split('&')[0].Split('=')[1];
                authData.Default.id = url.Split('#')[1].Split('&')[2].Split('=')[1];
                authData.Default.auth = true;
                authData.Default.Save();
            }
            else throw new FormatException("Ошибка при загрузке");
        }
    }
}
