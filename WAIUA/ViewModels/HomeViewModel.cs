using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;
using WAIUA.Commands;

namespace WAIUA.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private string[] _player0Prop;

        private string[] _player1Prop;

        private string[] _player2Prop;

        private string[] _player3Prop;

        private string[] _player4Prop;

        private string refreshTime;

        public HomeViewModel(INavigationService homeNavigationService, INavigationService infoNavigationService,
            INavigationService settingsNavigationService, INavigationService matchNavigationService)
        {
            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateInfoCommand = new NavigateCommand(infoNavigationService);
            NavigateSettingsCommand = new NavigateCommand(settingsNavigationService);
            NavigateMatchCommand = new NavigateCommand(matchNavigationService);
            LoadNowCommand = new RelayCommand(o => { LoadNow(); }, o => true);
            UpdateStatus();
        }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateInfoCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand NavigateMatchCommand { get; }

        public ICommand LoadNowCommand { get; }


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

        public string RefreshTime
        {
            get => refreshTime;
            set => SetProperty(ref refreshTime, nameof(RefreshTime));
        }

        private void LoadNow()
        {
            var newMatch = new Main();
            if (newMatch.LiveMatchChecks(false))
                if (NavigateMatchCommand.CanExecute(null))
                    NavigateMatchCommand.Execute(null);
        }

        public void UpdateParty()
        {
            if (GetPlayerInfo())
            {
                _player0Prop = Player.Player0;
                _player1Prop = Player.Player1;
                _player2Prop = Player.Player2;
                _player3Prop = Player.Player3;
                _player4Prop = Player.Player4;
            }
        }

        private void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            field = newValue;
            OnPropertyChanged(propertyName);
        }

        private bool GetPlayerInfo()
        {
            var output = false;
            try
            {
                var newMatch = new Main();
                Parallel.For(0, 5, i => { Player.players[i].Data = null; });

                try
                {
                    Parallel.For(0, 5, i => { Player.players[i].Data = newMatch.LiveMatchOutput((sbyte) i); });
                }
                catch (Exception)
                {
                }

                output = true;
            }
            catch (Exception)
            {
            }

            return output;
        }

        public void UpdateStatus()
        {
        }

        public class Player
        {
            public static Player[] players;
            public string[] Data;

            static Player()
            {
                players = new Player[5];
                for (sbyte x = 0; x < 5; x++) players[x] = new Player();
            }

            public static string[] Player0 => players[0].Data;
            public static string[] Player1 => players[1].Data;
            public static string[] Player2 => players[2].Data;
            public static string[] Player3 => players[3].Data;
            public static string[] Player4 => players[4].Data;
        }
    }
}