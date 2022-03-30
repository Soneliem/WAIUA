using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WAIUA.Helpers;
using WAIUA.Objects;
using WAIUA.Views;
using static WAIUA.Helpers.Login;
using Match = WAIUA.Helpers.Match;

namespace WAIUA.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private static readonly Uri question = new("pack://application:,,,/Assets/question.png");
    private static readonly Uri refresh = new("pack://application:,,,/Assets/refresh.png");
    private static readonly Uri check = new("pack://application:,,,/Assets/check.png");
    private static readonly Uri cross = new("pack://application:,,,/Assets/cross.png");

    [ObservableProperty]
    private int countdownTime = 15;
    [ObservableProperty]
    private DispatcherTimer _countTimer;
    [ObservableProperty]
    private Match _newMatch = new();
    [ObservableProperty]
    private List<Player> _playerList = new(5);
    [ObservableProperty]
    private string _queueTime = "-";
    [ObservableProperty]
    private string _refreshTime = "-";
    [ObservableProperty]
    private string _toggleBtnTxt = "Wait for Next Match";
    [ObservableProperty]
    private Uri _accountStatus = question;
    [ObservableProperty]
    private Uri _gameStatus = question;
    [ObservableProperty]
    private Uri _matchStatus = question;

    public delegate void EventAction();
    public event EventAction GoMatchEvent;
    public HomeViewModel()
    {

    }

    [ICommand]
    private async Task LoadNowAsync()
    {
        if (!await Match.LiveMatchChecksAsync(false).ConfigureAwait(false)) return;
        var matchDets = await Match.GetLiveMatchDetailsAsync().ConfigureAwait(false);
        GoMatchEvent?.Invoke();
        await UpdateChecksAsync().ConfigureAwait(false);
    }

    [ICommand]
    private async Task PassiveLoadAsync()
    {
        _countTimer = new DispatcherTimer();
        _countTimer.Tick += UpdateTimersAsync;
        _countTimer.Interval = new TimeSpan(0, 0, 1);

        _countTimer.Start();
        _newMatch = new Match();
        if (await Match.LiveMatchChecksAsync(true).ConfigureAwait(false))
            GoMatchEvent?.Invoke();
        ToggleBtnTxt = "Waiting for match";

        
        await UpdateChecksAsync().ConfigureAwait(false);
    }

    [ICommand]
    private Task StopPassiveLoadAsync()
    {
        CountTimer.Stop();
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
        GameStatus = refresh;
        AccountStatus = question;
        MatchStatus = question;
        if (await CheckLocalAsync().ConfigureAwait(false))
        {
            GameStatus = check;
            AccountStatus = refresh;
            if (await GetSetPpuuidAsync().ConfigureAwait(false))
            {
                AccountStatus = check;
                MatchStatus = refresh;
                if (await CheckMatchIdAsync().ConfigureAwait(false))
                {
                    MatchStatus = check;
                    CountTimer?.Stop();

                    GoMatchEvent?.Invoke();

                }
                else
                {
                    MatchStatus = cross;
                }
            }
            else
            {
                await LocalLoginAsync().ConfigureAwait(false);
                await LocalRegionAsync().ConfigureAwait(false);
                if (await GetSetPpuuidAsync().ConfigureAwait(false))
                {
                    AccountStatus = check;
                    MatchStatus = refresh;
                    if (await CheckMatchIdAsync().ConfigureAwait(false))
                    {
                        MatchStatus = check;
                        CountTimer?.Stop();
                        GoMatchEvent?.Invoke();
                    }
                    else
                    {
                        MatchStatus = cross;
                    }
                }
                else
                {
                    AccountStatus = cross;
                    MatchStatus = cross;
                }
                    
            }
        }
        else
        {
            GameStatus = cross;
            AccountStatus = cross;
            MatchStatus = cross;
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
            // var newMatch = new Match();
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