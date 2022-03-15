using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WAIUA.Helpers;
using static WAIUA.Helpers.Login;

namespace WAIUA.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    [ObservableProperty]
    private int countdownTime = 15;
    [ObservableProperty]
    private DispatcherTimer _countTimer;
    [ObservableProperty]
    private LiveMatch _newMatch;
    [ObservableProperty]
    private List<Objects.Player> _playerList;
    [ObservableProperty]
    private string _queueTime = "-";
    [ObservableProperty]
    private string _refreshTime = "-";
    [ObservableProperty]
    private string _toggleBtnTxt;
    [ObservableProperty]
    private string _accountStatus;
    [ObservableProperty]
    private string _gameStatus;
    [ObservableProperty]
    private string _matchStatus;

    public HomeViewModel()
    {
        GameStatus = "/Assets/question.png";
        AccountStatus = "/Assets/question.png";
        MatchStatus = "/Assets/question.png";
        ToggleBtnTxt = "Wait for Next Match";

    }

    [ICommand]
    private async Task LoadNowAsync()
    {
        _newMatch ??= new LiveMatch();

        if (!await _newMatch.LiveMatchChecksAsync(false).ConfigureAwait(false)) return;
        if (NavigateMatchCommand.CanExecute(null))
        {
            _countTimer?.Stop();
            
            NavigateMatchCommand.Execute(null);
        }
        await UpdateChecksAsync().ConfigureAwait(false);
    }

    [ICommand]
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

    [ICommand]
    private Task StopPassiveLoadAsync()
    {
        _countTimer.Stop();
        ToggleBtnTxt = "Wait for Next Match";
        RefreshTime = "-";
        CountdownTime = 15;
        return Task.CompletedTask;
    }


    private async void UpdateTimersAsync(object sender, EventArgs e)
    {
        RefreshTime = CountdownTime + "s";
        if (CountdownTime == 0)
        {
            CountdownTime = 15;
            ToggleBtnTxt = "Refreshing";

            await UpdateChecksAsync().ConfigureAwait(false);
            ToggleBtnTxt = "Waiting for match";
        }

        CountdownTime--;
    }

    [ICommand]
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
                        _countTimer?.Stop();
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
                    MatchStatus = "/Assets/refresh.png";
                    if (await CheckMatchIdAsync().ConfigureAwait(false))
                    {
                        MatchStatus = "/Assets/check.png";
                        if (NavigateMatchCommand.CanExecute(null))
                        {

                            _countTimer?.Stop();
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
            // _player0Prop = Player.Player0;
            // _player1Prop = Player.Player1;
            // _player2Prop = Player.Player2;
            // _player3Prop = Player.Player3;
            // _player4Prop = Player.Player4;
        }
    }


    private Task<bool> GetPartyPlayerInfoAsync()
    {
        var output = false;
        try
        {
            // var newMatch = new LiveMatch();
            // Parallel.For(0, 5, i => { Player.Players[i].Data = null; });
            //
            // try
            // {
            //     // Parallel.For(0, 5, async i => { RiotPlayer.players[i].Data = await newMatch.LiveMatchOutputAsync((sbyte) i).ConfigureAwait(false); });
            // }
            // catch (Exception)
            // {
            // }
            //
            // output = true;
        }
        catch (Exception)
        {
        }

        return Task.FromResult(output);
    }
}