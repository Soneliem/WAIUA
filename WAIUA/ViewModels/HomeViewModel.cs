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

    private string accountStatus;

    private string gameStatus;

    private string matchStatus;

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
        LoadNowCommand = new RelayCommand(o => { LoadNow(); }, o => true);
        PassiveEnabledCommand = new RelayCommand(o => { PassiveLoad(); }, o => true);
        PassiveDisabledCommand = new RelayCommand(o => { StopPassiveLoad(); }, o => true);
        
        UpdateChecks();

    }

    public ICommand NavigateHomeCommand { get; }
    public ICommand NavigateInfoCommand { get; }
    public ICommand NavigateSettingsCommand { get; }
    public ICommand NavigateMatchCommand { get; }

    public ICommand LoadNowCommand { get; }

    public ICommand PassiveEnabledCommand { get; }
    public ICommand PassiveDisabledCommand { get; }

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
        get => gameStatus;
        set => SetProperty(ref gameStatus, value);
    }

    public string MatchStatus
    {
        get => matchStatus;
        set => SetProperty(ref matchStatus, value);
    }

    public string AccountStatus
    {
        get => accountStatus;
        set => SetProperty(ref accountStatus, value);
    }

    private void LoadNow()
    {
        _newMatch ??= new LiveMatch();

        if (!_newMatch.LiveMatchChecks(false)) return;
        if (NavigateMatchCommand.CanExecute(null))
            NavigateMatchCommand.Execute(null);
        UpdateChecks();
    }

    private void PassiveLoad()
    {
        _newMatch = new LiveMatch();
        if (_newMatch.LiveMatchChecks(true))
            if (NavigateMatchCommand.CanExecute(null))
                NavigateMatchCommand.Execute(null);

        ToggleBtnTxt = "Waiting for match";

        new DispatcherTimer();

        _countTimer = new DispatcherTimer();
        _countTimer.Tick += UpdateTimers;
        _countTimer.Interval = new TimeSpan(0, 0, 1);

        _countTimer.Start();
        UpdateChecks();
    }

    private void StopPassiveLoad()
    {
        _countTimer.Stop();
        ToggleBtnTxt = "Wait for Next Match";
        RefreshTime = "-";
        _countdownTime = 15;
    }


    private void UpdateTimers(object sender, EventArgs e)
    {
        RefreshTime = _countdownTime + "s";
        if (_countdownTime == 0)
        {
            _countdownTime = 15;
            ToggleBtnTxt = "Refreshing";

            UpdateChecks();
            // if (_newMatch.LiveMatchChecks(true))
            // {
            //     _refreshTimer.Stop();
            //     if (NavigateMatchCommand.CanExecute(null))
            //         NavigateMatchCommand.Execute(null);
            // }
            ToggleBtnTxt = "Waiting for match";
        }

        _countdownTime--;
    }

    private void UpdateChecks()
    {
        GameStatus = "/Assets/refresh.png";
        AccountStatus = "/Assets/question.png";
        MatchStatus = "/Assets/question.png";
        if (CheckLocal())
        {
            GameStatus = "/Assets/check.png";
            AccountStatus = "/Assets/refresh.png";
            if (GetSetPPUUID())
            {
                AccountStatus = "/Assets/check.png";
                MatchStatus = "/Assets/refresh.png";
                if (CheckMatchID())
                {
                    MatchStatus = "/Assets/check.png";
                    if (NavigateMatchCommand.CanExecute(null))
                        NavigateMatchCommand.Execute(null);
                }
                else
                {
                    MatchStatus = "/Assets/cross.png";
                }
            }
            else
            {
                LocalLogin();
                LocalRegion();
                if (GetSetPPUUID())
                {
                    AccountStatus = "/Assets/check.png";
                    if (CheckMatchID())
                    {
                        MatchStatus = "/Assets/check.png";
                        if (NavigateMatchCommand.CanExecute(null))
                            NavigateMatchCommand.Execute(null);
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

    public void UpdateParty()
    {
        if (GetPartyPlayerInfo())
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

    private bool GetPartyPlayerInfo()
    {
        var output = false;
        try
        {
            var newMatch = new LiveMatch();
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