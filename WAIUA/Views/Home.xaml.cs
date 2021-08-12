using System.Net;
using System.Windows;
using System.Windows.Controls;
using WAIUA.Commands;

namespace WAIUA.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public class HomeInfo
        {
            public static string[] Player0 { get; set; }
            public static string[] Player1 { get; set; }
            public static string[] Player2 { get; set; }
            public static string[] Player3 { get; set; }
            public static string[] Player4 { get; set; }
            public static string[] Player5 { get; set; }
            public static string[] Player6 { get; set; }
            public static string[] Player7 { get; set; }
            public static string[] Player8 { get; set; }
            public static string[] Player9 { get; set; }

            public static HomeInfo GetHomeInfo()
            {
                var output = new HomeInfo() { };
                try
                {
                    APIConnection.LiveMatchSetup();
                    Player0 = APIConnection.LiveMatchOutput(0);
                    Player1 = APIConnection.LiveMatchOutput(1);
                    Player2 = APIConnection.LiveMatchOutput(2);
                    Player3 = APIConnection.LiveMatchOutput(3);
                    Player4 = APIConnection.LiveMatchOutput(4);
                    Player5 = APIConnection.LiveMatchOutput(5);
                    Player6 = APIConnection.LiveMatchOutput(6);
                    Player7 = APIConnection.LiveMatchOutput(7);
                    Player8 = APIConnection.LiveMatchOutput(8);
                    Player9 = APIConnection.LiveMatchOutput(9);
                }
                catch (System.Exception)
                {
                }
                return output;
            }
        }

        public Home()
        {
            InitializeComponent();
            CookieContainer cookie = new CookieContainer();
            DataContext = HomeInfo.GetHomeInfo();
            
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}