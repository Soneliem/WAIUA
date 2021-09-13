using System;
using System.Net;
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

        public void LogIn()
        {
            CookieContainer cookie = new CookieContainer();
            string user = usernameBox.Text;
            string pass = passwordBox.Password;
            while (true)
            {
                if (String.IsNullOrEmpty(user))
                {
                    authStatusBox.Text = "Please enter your credentials";
                    break;
                }
                else if (String.IsNullOrEmpty(pass))
                {
                    authStatusBox.Text = "Please enter your credentials";
                    break;
                }
                else if (this.RegionList.SelectedIndex == -1)
                {
                    authStatusBox.Text = "Please select a region";
                    break;
                }
                else
                {
                    authStatusBox.Text = "Authenticating...";
                    Login(cookie, user, pass);
                    CheckAuth(cookie);
                    break;
                }
            }
        }

        public void CheckAuth(CookieContainer cookie)
        {
            while (true)
            {
                if (String.IsNullOrEmpty(GetIGUsername(cookie, PPUUID)))
                {
                    authStatusBox.Text = "Not Authenticated";
                    break;
                }
                else
                {
                    authStatusBox.Text = $"Authenticated as: {GetIGUsername(cookie, PPUUID)}";
                    break;
                }
            }
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
                authStatusBox.Text = "Valorant need to be running for auto signin";
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