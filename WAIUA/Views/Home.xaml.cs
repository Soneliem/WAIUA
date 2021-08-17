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
            public class Player
            {
                public string[] data;
                public static Player[] players;
                public const int MAX_PLAYERS = 10;
                static Player()
                {
                    players = new Player[MAX_PLAYERS];
                    for (var x = 0; x< MAX_PLAYERS; x++)
                    {
                        players[x] = new Player();
                    }
                }
            }

            public static string[] Player0 => Player.players[0].data;
            public static string[] Player1 => Player.players[1].data;
            public static string[] Player2 => Player.players[2].data;
            public static string[] Player3 => Player.players[3].data;
            public static string[] Player4 => Player.players[4].data;
            public static string[] Player5 => Player.players[5].data;
            public static string[] Player6 => Player.players[6].data;
            public static string[] Player7 => Player.players[7].data;
            public static string[] Player8 => Player.players[8].data;
            public static string[] Player9 => Player.players[9].data;

            private static void SetPlayer(int number) => Player.players[number].data = APIConnection.LiveMatchOutput(number);

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