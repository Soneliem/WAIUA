using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;
using WAIUA.Commands;
using WAIUA.Helpers;

namespace WAIUA.ViewModels
{
    public class MatchViewModel : ViewModelBase
    {
        private string[] _player0Prop;

        private string[] _player1Prop;

        private string[] _player2Prop;

        private string[] _player3Prop;

        private string[] _player4Prop;

        private string[] _player5Prop;

        private string[] _player6Prop;

        private string[] _player7Prop;

        private string[] _player8Prop;

        private string[] _player9Prop;
        private string _server;
        private string _gameMode;
        private string _map;
        private string _mapImg;

        public MatchViewModel(INavigationService homeNavigationService, INavigationService matchNavigationService)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            GetPlayerInfoCommand = new RelayCommand(o => { UpdatePlayersAsync().ConfigureAwait(false); }, o => true);
            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateMatchCommand = new NavigateCommand(matchNavigationService);
            Mouse.OverrideCursor = Cursors.Arrow;
        }
        public ICommand GetPlayerInfoCommand { get; }
        public string[] Player0
        {
            get => _player0Prop;
            set => SetProperty(ref _player0Prop, value, nameof(Player0));
        }

        public string[] Player1
        {
            get => _player1Prop;
            set => SetProperty(ref _player1Prop, value, nameof(Player1));
        }

        public string[] Player2
        {
            get => _player2Prop;
            set => SetProperty(ref _player2Prop, value, nameof(Player2));
        }

        public string[] Player3
        {
            get => _player3Prop;
            set => SetProperty(ref _player3Prop, value, nameof(Player3));
        }

        public string[] Player4
        {
            get => _player4Prop;
            set => SetProperty(ref _player4Prop, value, nameof(Player4));
        }

        public string[] Player5
        {
            get => _player5Prop;
            set => SetProperty(ref _player5Prop, value, nameof(Player5));
        }

        public string[] Player6
        {
            get => _player6Prop;
            set => SetProperty(ref _player6Prop, value, nameof(Player6));
        }

        public string[] Player7
        {
            get => _player7Prop;
            set => SetProperty(ref _player7Prop, value, nameof(Player7));
        }

        public string[] Player8
        {
            get => _player8Prop;
            set => SetProperty(ref _player8Prop, value, nameof(Player8));
        }

        public string[] Player9
        {
            get => _player9Prop;
            set => SetProperty(ref _player9Prop, value, nameof(Player9));
        }

        public string Server
        {
            get => _server;
            set => SetProperty(ref _server, value, nameof(Server));
        }

        public string GameMode
        {
            get => _gameMode;
            set => SetProperty(ref _gameMode, value, nameof(GameMode));
        }

        public string Map
        {
            get => _map;
            set => SetProperty(ref _map, value, nameof(Map));
        }

        public string MapImg
        {
            get => _mapImg;
            set => SetProperty(ref _mapImg, value, nameof(MapImg));
        }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateMatchCommand { get; }

        private Task UpdatePlayerAsync(int i, string[] value)
        {
            Player.players[i].Data = value;
            return Task.CompletedTask;
        }

        private async Task<bool> GetPlayerInfoAsync()
        {
            var output = false;
            try
            {
                var newMatch = new LiveMatch();

                var tasks = Enumerable.Range(0, 10)
                    .Select(i => UpdatePlayerAsync(i, null))
                    .ToArray();
                await Task.WhenAll(tasks).ConfigureAwait(false);

                if (await newMatch.LiveMatchChecksAsync(false).ConfigureAwait(false))
                {
                    output = true;
                    Parallel.For(0, 10, async i => { Player.players[i].Data = await newMatch.LiveMatchOutputAsync((sbyte)i).ConfigureAwait(false); });

                    var tasks2 = Enumerable.Range(0, 10)
                        .Select(async i => UpdatePlayerAsync(i, await newMatch.LiveMatchOutputAsync((sbyte)i).ConfigureAwait(false)))
                        .ToArray();
                    await Task.WhenAll(tasks2).ConfigureAwait(false);

                    if (newMatch.Server != null) Server = newMatch.Server;
                    if (newMatch.GameMode != null) GameMode = newMatch.GameMode;
                    if (newMatch.Map != null) Map = newMatch.Map;
                    if (newMatch.MapImage != null) MapImg = newMatch.MapImage;

                    var colours = new List<string>
                        {"Red", "#32e2b2", "DarkOrange", "White", "DeepSkyBlue", "MediumPurple", "SaddleBrown"};
                     
                    string[] newArray = new string[] {"Transparent", "Transparent" , "Transparent" , "Transparent" , "Transparent" , "Transparent" , "Transparent" , "Transparent" , "Transparent" , "Transparent" };
                    for (var i = 0; i < 10; i++)
                    {
                        var colourused = false;
                        var id = Player.players[i].Data[28];
                        for (var j = i + 1; j < 10; j++)
                            if (Player.players[j].Data[28] == id && id != null && id.Length > 11)
                            {
                                newArray[i] = newArray[j] = colours[0];
                                colourused = true;
                            }

                        if (colourused) colours.RemoveAt(0);
                    }
                    for (var i = 0; i < Player.players.Length; i++)
                    {
                        Player.players[i].Data[28] = newArray[i];
                    }

                }


            }
            catch (Exception)
            {
            } 

            return output;
        }

        private async Task UpdatePlayersAsync()
        {
            if (await GetPlayerInfoAsync().ConfigureAwait(false))
            {
                _player0Prop = Player.Player0;
                _player1Prop = Player.Player1;
                _player2Prop = Player.Player2;
                _player3Prop = Player.Player3;
                _player4Prop = Player.Player4;
                _player5Prop = Player.Player5;
                _player6Prop = Player.Player6;
                _player7Prop = Player.Player7;
                _player8Prop = Player.Player8;
                _player9Prop = Player.Player9;
            }
        }

        private void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            field = newValue;
            OnPropertyChanged(propertyName);
        }

        public class Player
        {
            public static Player[] players;
            public string[] Data;

            static Player()
            {
                players = new Player[10];
                for (sbyte x = 0; x < 10; x++) players[x] = new Player();
            }

            public static string[] Player0 => players[0].Data;
            public static string[] Player1 => players[1].Data;
            public static string[] Player2 => players[2].Data;
            public static string[] Player3 => players[3].Data;
            public static string[] Player4 => players[4].Data;
            public static string[] Player5 => players[5].Data;
            public static string[] Player6 => players[6].Data;
            public static string[] Player7 => players[7].Data;
            public static string[] Player8 => players[8].Data;
            public static string[] Player9 => players[9].Data;
        }
    }
}