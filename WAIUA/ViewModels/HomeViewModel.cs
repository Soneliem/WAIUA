using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;
using WAIUA.Commands;
using WAIUA.Helpers;
using static WAIUA.Helpers.Login;

namespace WAIUA.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private int _countdownTime = 15;

    private DispatcherTimer _countTimer;
    private LiveMatch _newMatch;
    private string[] _player0Prop;

    private string[] _player1Prop;

    private string[] _player2Prop;

    private string[] _player3Prop;

    private string[] _player4Prop;
    private string _queueTime = "-";

    private string _refreshTime = "-";
    private string _toggleBtnTxt;

    private string _accountStatus;

    private string _gameStatus;

    private string _matchStatus;

    public HomeViewModel(INavigationService homeNavigationService, INavigationService infoNavigationService,
        INavigationService settingsNavigationService, INavigationService matchNavigationService)
    {
        GameStatus = "/Assets/question.png";
        AccountStatus = "/Assets/question.png";
        MatchStatus = "/Assets/question.png";

        ToggleBtnTxt = "Wait for Next Match";
        NavigateHomeCommand = new NavigateCommand(homeNavigationService);
        NavigateInfoCommand = new NavigateCommand(infoNavigationService);
        NavigateSettingsCommand = new NavigateCommand(settingsNavigationService);
        NavigateMatchCommand = new NavigateCommand(matchNavigationService);
        LoadNowCommand = new RelayCommand( o => { LoadNowAsync().ConfigureAwait(false); }, o => true);
        PassiveEnabledCommand = new RelayCommand(o => { PassiveLoadAsync().ConfigureAwait(false); }, o => true);
        PassiveDisabledCommand = new RelayCommand(o => { StopPassiveLoadAsync().ConfigureAwait(false); }, o => true);
        UpdateChecksCommand = new RelayCommand(o => { UpdateChecksAsync().ConfigureAwait(false); }, o => true);
    }

    public ICommand NavigateHomeCommand { get; }
    public ICommand NavigateInfoCommand { get; }
    public ICommand NavigateSettingsCommand { get; }
    public ICommand NavigateMatchCommand { get; }

    public ICommand LoadNowCommand { get; }

    public ICommand PassiveEnabledCommand { get; }
    public ICommand PassiveDisabledCommand { get; }
    public ICommand UpdateChecksCommand { get; }

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
        get => _refreshTime;
        set => SetProperty(ref _refreshTime, value);
    }

    public string QueueTime
    {
        get => _queueTime;
        set => SetProperty(ref _queueTime, value);
    }

    public string ToggleBtnTxt
    {
        get => _toggleBtnTxt;
        set => SetProperty(ref _toggleBtnTxt, value);
    }

    public string GameStatus
    {
        get => _gameStatus;
        set => SetProperty(ref _gameStatus, value);
    }

    public string MatchStatus
    {
        get => _matchStatus;
        set => SetProperty(ref _matchStatus, value);
    }

    public string AccountStatus
    {
        get => _accountStatus;
        set => SetProperty(ref _accountStatus, value);
    }

    private async Task LoadNowAsync()
    {
        _newMatch ??= new LiveMatch();

        if (!await _newMatch.LiveMatchChecksAsync(false).ConfigureAwait(false)) return;
        if (NavigateMatchCommand.CanExecute(null))
        {
            _countTimer.Stop();
            NavigateMatchCommand.Execute(null);
        }
        await UpdateChecksAsync().ConfigureAwait(false);
    }

    private async Task PassiveLoadAsync()
    {
        _countTimer = new DispatcherTimer();
        _countTimer.Tick += UpdateTimersAsync;
        _countTimer.Interval = new TimeSpan(0, 0, 1);

        _countTimer.Start();
        _newMatch = new LiveMatch();
        if (await _newMatch.LiveMatchChecksAsync(true).ConfigureAwait(false))
            if (NavigateMatchCommand.CanExecute(null))
            {
                _countTimer.Stop();
                NavigateMatchCommand.Execute(null);
            }
        ToggleBtnTxt = "Waiting for match";

        
        await UpdateChecksAsync().ConfigureAwait(false);
    }

    private Task StopPassiveLoadAsync()
    {
        _countTimer.Stop();
        ToggleBtnTxt = "Wait for Next Match";
        RefreshTime = "-";
        _countdownTime = 15;
        return Task.CompletedTask;
    }


    private async void UpdateTimersAsync(object sender, EventArgs e)
    {
        RefreshTime = _countdownTime + "s";
        if (_countdownTime == 0)
        {
            _countdownTime = 15;
            ToggleBtnTxt = "Refreshing";

            await UpdateChecksAsync().ConfigureAwait(false);
            ToggleBtnTxt = "Waiting for match";
        }

        _countdownTime--;
    }

    private async Task UpdateChecksAsync()
    {
        GameStatus = "/Assets/refresh.png";
        AccountStatus = "/Assets/question.png";
        MatchStatus = "/Assets/question.png";
        if (await CheckLocalAsync().ConfigureAwait(false))
        {
            GameStatus = "/Assets/check.png";
            AccountStatus = "/Assets/refresh.png";
            if (await GetSetPpuuidAsync().ConfigureAwait(false))
            {
                AccountStatus = "/Assets/check.png";
                MatchStatus = "/Assets/refresh.png";
                if (await CheckMatchIdAsync().ConfigureAwait(false))
                {
                    MatchStatus = "/Assets/check.png";
                    if (NavigateMatchCommand.CanExecute(null))
                    {
                        _countTimer.Stop();
                        NavigateMatchCommand.Execute(null);
                    }
                        
                }
                else
                {
                    MatchStatus = "/Assets/cross.png";
                }
            }
            else
            {
                await LocalLoginAsync().ConfigureAwait(false);
                await LocalRegionAsync().ConfigureAwait(false);
                if (await GetSetPpuuidAsync().ConfigureAwait(false))
                {
                    AccountStatus = "/Assets/check.png";
                    if (await CheckMatchIdAsync().ConfigureAwait(false))
                    {
                        MatchStatus = "/Assets/check.png";
                        if (NavigateMatchCommand.CanExecute(null))
                        {
                            _countTimer.Stop();
                            NavigateMatchCommand.Execute(null);
                        }
                    }
                    else
                    {
                        AccountStatus = "/Assets/cross.png";
                        MatchStatus = "/Assets/cross.png";
                    }
                }
                else
                {
                    AccountStatus = "/Assets/cross.png";
                    MatchStatus = "/Assets/cross.png";
                }
                    
            }
        }
        else
        {
            GameStatus = "/Assets/cross.png";
            AccountStatus = "/Assets/cross.png";
            MatchStatus = "/Assets/cross.png";
        }
    }

    public async Task UpdatePartyAsync()
    {
        if (await GetPartyPlayerInfoAsync().ConfigureAwait(false))
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

    private Task<bool> GetPartyPlayerInfoAsync()
    {
        var output = false;
        try
        {
            var newMatch = new LiveMatch();
            Parallel.For(0, 5, i => { Player.players[i].Data = null; });

            try
            {
                Parallel.For(0, 5, async i => { Player.players[i].Data = await newMatch.LiveMatchOutputAsync((sbyte) i).ConfigureAwait(false); });
            }
            catch (Exception)
            {
            }

            output = true;
        }
        catch (Exception)
        {
        }

        return Task.FromResult(output);
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