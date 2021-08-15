using System.Net;
using System.Threading.Tasks;
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

            private static void SetPlayer(int number)
            {
                switch (number)
                {
                    case 0:
                        Player0 = APIConnection.LiveMatchOutput(0);
                        break;

                    case 1:
                        Player1 = APIConnection.LiveMatchOutput(1);
                        break;

                    case 2:
                        Player2 = APIConnection.LiveMatchOutput(2);
                        break;

                    case 3:
                        Player3 = APIConnection.LiveMatchOutput(3);
                        break;

                    case 4:
                        Player4 = APIConnection.LiveMatchOutput(4);
                        break;

                    case 5:
                        Player5 = APIConnection.LiveMatchOutput(5);
                        break;

                    case 6:
                        Player6 = APIConnection.LiveMatchOutput(6);
                        break;

                    case 7:
                        Player7 = APIConnection.LiveMatchOutput(7);
                        break;

                    case 8:
                        Player8 = APIConnection.LiveMatchOutput(8);
                        break;

                    case 9:
                        Player9 = APIConnection.LiveMatchOutput(9);
                        break;

                    default:
                        break;
                }
            }

            public static HomeInfo GetHomeInfo()
            {
                var output = new HomeInfo() { };
                try
                {
                    APIConnection.LiveMatchSetup();
                    Parallel.For(0, 10, i =>
                    {
                        SetPlayer(i);
                    });
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