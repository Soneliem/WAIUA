using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using WAIUA.Commands;

namespace WAIUA.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public class Player
        {
            public string[] data;
            public static Player[] players;
            public const int MAX_PLAYERS = 10;

            static Player()
            {
                players = new Player[MAX_PLAYERS];
                for (var x = 0; x < MAX_PLAYERS; x++)
                {
                    players[x] = new Player();
                }
            }

            public static string[] Player0 => Player.players[0].data;
            /*public static string[] Player1 => Player.players[1].data;
            public static string[] Player2 => Player.players[2].data;
            public static string[] Player3 => Player.players[3].data;
            public static string[] Player4 => Player.players[4].data;
            public static string[] Player5 => Player.players[5].data;
            public static string[] Player6 => Player.players[6].data;
            public static string[] Player7 => Player.players[7].data;
            public static string[] Player8 => Player.players[8].data;
            public static string[] Player9 => Player.players[9].data;*/

            private static void SetPlayer(int number) => Player.players[number].data = Main.LiveMatchOutput(number);

            public static Player GetPlayerInfo()
            {
                var output = new Player() { };
                try
                {
                    Main.LiveMatchSetup();
                    /*Parallel.For(0, 10, i =>
                    {
                        SetPlayer(i);
                    });*/
                    SetPlayer(0);
                }
                catch (System.Exception)
                {
                }
                return output;
            }
        }

        private string[] _player0Prop;
        public string[] Player0
        {
            get { return _player0Prop; }
            set
            {
                _player0Prop = value;

                OnPropertyChanged(nameof(Player0));
                System.Diagnostics.Debug.WriteLine(_player0Prop.GetValue(1));
            }
        }
        private string _testOutput;
        public string TestOutput { get => _testOutput; set => SetProperty(ref _testOutput, value, nameof(TestOutput)); }

        public HomeViewModel()
        {
            Player.GetPlayerInfo();
            //_player0Prop = Player.Player0;
            //System.Diagnostics.Debug.WriteLine(Player0.GetValue(1));
            _testOutput = RandomNumberGenerator.GetInt32(0, 100).ToString();
            System.Diagnostics.Debug.WriteLine(_testOutput);
        }

        private void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            field = newValue;
            OnPropertyChanged(propertyName);
        }


    }
}