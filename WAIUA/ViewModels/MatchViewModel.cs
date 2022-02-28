using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
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
        private object[] _player0Prop;

        private object[] _player1Prop;

        private object[] _player2Prop;

        private object[] _player3Prop;

        private object[] _player4Prop;

        private object[] _player5Prop;

        private object[] _player6Prop;

        private object[] _player7Prop;

        private object[] _player8Prop;

        private object[] _player9Prop;
        private string _server;
        private string _gameMode;
        private string _map;
        private Uri _mapImg;

        public MatchViewModel(INavigationService homeNavigationService, INavigationService matchNavigationService)
        {
            GetPlayerInfoCommand = new RelayCommand(o => { UpdatePlayersAsync().ConfigureAwait(false); }, o => true);
            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateMatchCommand = new NavigateCommand(matchNavigationService);
        }
        public ICommand GetPlayerInfoCommand { get; }
        public object[] Player0
        {
            get => _player0Prop;
            set => SetProperty(ref _player0Prop, value, nameof(Player0));
        }

        public object[] Player1
        {
            get => _player1Prop;
            set => SetProperty(ref _player1Prop, value, nameof(Player1));
        }

        public object[] Player2
        {
            get => _player2Prop;
            set => SetProperty(ref _player2Prop, value, nameof(Player2));
        }

        public object[] Player3
        {
            get => _player3Prop;
            set => SetProperty(ref _player3Prop, value, nameof(Player3));
        }

        public object[] Player4
        {
            get => _player4Prop;
            set => SetProperty(ref _player4Prop, value, nameof(Player4));
        }

        public object[] Player5
        {
            get => _player5Prop;
            set => SetProperty(ref _player5Prop, value, nameof(Player5));
        }

        public object[] Player6
        {
            get => _player6Prop;
            set => SetProperty(ref _player6Prop, value, nameof(Player6));
        }

        public object[] Player7
        {
            get => _player7Prop;
            set => SetProperty(ref _player7Prop, value, nameof(Player7));
        }

        public object[] Player8
        {
            get => _player8Prop;
            set => SetProperty(ref _player8Prop, value, nameof(Player8));
        }

        public object[] Player9
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

        public Uri MapImg
        {
            get => _mapImg;
            set => SetProperty(ref _mapImg, value, nameof(MapImg));
        }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateMatchCommand { get; }

        private Task UpdatePlayerAsync(int i, object[] value)
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
                    // Parallel.For(0, 10, async i => { Player.players[i].Data = await newMatch.LiveMatchOutputAsync((sbyte)i).ConfigureAwait(false); });

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
                            if (Player.players[j].Data[28] == id && id != null && id.ToString().Length > 11)
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
                Player0 = Player.Player0;
                Player1 = Player.Player1;
                Player2 = Player.Player2;
                Player3 = Player.Player3;
                Player4 = Player.Player4;
                Player5 = Player.Player5;
                Player6 = Player.Player6;
                Player7 = Player.Player7;
                Player8 = Player.Player8;
                Player9 = Player.Player9;
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
            public object[] Data;

            static Player()
            {
                players = new Player[10];
                for (sbyte x = 0; x < 10; x++) players[x] = new Player();
            }

            public static object[] Player0 => players[0].Data;
            public static object[] Player1 => players[1].Data;
            public static object[] Player2 => players[2].Data;
            public static object[] Player3 => players[3].Data;
            public static object[] Player4 => players[4].Data;
            public static object[] Player5 => players[5].Data;
            public static object[] Player6 => players[6].Data;
            public static object[] Player7 => players[7].Data;
            public static object[] Player8 => players[8].Data;
            public static object[] Player9 => players[9].Data;
        }

        
    }
}