using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using static WAIUA.Commands.Main;

namespace WAIUA.Views
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : UserControl
    {
        public Account()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LogIn();
        }

        private void PasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            LogIn();
        }

        private void LogIn()
        {
            CookieContainer cookie = new CookieContainer();
            string user = UsernameBox.Text;
            string pass = PasswordBox.Password;
            while (true)
            {
                if (String.IsNullOrEmpty(user))
                {
                    AuthStatusBox.Text = "Please enter your credentials";
                    break;
                }
                else if (String.IsNullOrEmpty(pass))
                {
                    AuthStatusBox.Text = "Please enter your credentials";
                    break;
                }
                else if (this.RegionList.SelectedIndex == -1)
                {
                    AuthStatusBox.Text = "Please select a region";
                    break;
                }
                else
                {
                    AuthStatusBox.Text = "Authenticating...";
                    Login(cookie, user, pass);
                    CheckAuth(cookie);
                    break;
                }
            }
        }

        private void CheckAuth(CookieContainer cookie)
        {
            AuthStatusBox.Text = "Refreshing...";
            if (!GetSetPPUUID())
                 AuthStatusBox.Text = "Not Authenticated";
            else AuthStatusBox.Text = $"Authenticated as: {GetIGUsername(cookie, PPUUID)}";
            
        }

        private void Button_Click2(object sender, System.Windows.RoutedEventArgs e)
        {
            CookieContainer cookie = new CookieContainer();
            CheckAuth(cookie);
        }

        private void Button_Click3(object sender, System.Windows.RoutedEventArgs e)
        {
            CookieContainer cookie = new CookieContainer();
            if (CheckLocal())
            {
                LocalLogin();
                LocalRegion();
                CheckAuth(cookie);
            }
            else
            {
                AuthStatusBox.Text = "Valorant need to be running for auto signin";
            }
        }

        private void ListBox_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            switch (RegionList.SelectedIndex)
            {
                case 0:
                    Region = "na";
                    Shard = "na";
                    break;

                case 1:
                    Region = "ap";
                    Shard = "ap";
                    break;

                case 2:
                    Region = "eu";
                    Shard = "eu";

                    break;

                case 3:
                    Region = "ko";
                    Shard = "ko";
                    break;

                case 4:
                    Region = "na";
                    Shard = "br";
                    break;

                case 5:
                    Region = "na";
                    Shard = "latam";
                    break;
            }
        }
    }
}