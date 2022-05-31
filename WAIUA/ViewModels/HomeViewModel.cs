using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WAIUA.Helpers;
using WAIUA.Objects;
using static WAIUA.Helpers.Login;

namespace WAIUA.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    public delegate void EventAction();

    private static readonly Uri Question = new("pack://application:,,,/Assets/question.png");
    private static readonly Uri Check = new("pack://application:,,,/Assets/check.png");
    private static readonly Uri Cross = new("pack://application:,,,/Assets/cross.png");
    
    [ObservableProperty] private Uri _accountStatus = Question;
    [ObservableProperty] private DispatcherTimer _countTimer;
    [ObservableProperty] private Uri _gameStatus = Question;
    [ObservableProperty] private Uri _matchStatus = Question;
    [ObservableProperty] private LoadingOverlay _overlay;

    [ObservableProperty] private List<Player> _playerList;
    // [ObservableProperty] private string _queueTime = "-";
    [ObservableProperty] private string _refreshTime = "-";
    [ObservableProperty] private int _countdownTime = 15;

    public HomeViewModel()
    {
        Overlay = new LoadingOverlay
        {
            Header = "Refreshing",
            Content = "",
            IsBusy = false
        };
    }

    public event EventAction GoMatchEvent;


    [ICommand]
    private async Task LoadNowAsync()
    {
        if (!await Match.LiveMatchChecksAsync(false).ConfigureAwait(false)) return;
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
        await UpdateChecksAsync().ConfigureAwait(false);
    }

    [ICommand]
    private Task StopPassiveLoadAsync()
    {
        CountTimer.Stop();
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
            await UpdateChecksAsync().ConfigureAwait(false);
        }

        CountdownTime--;
    }


    [ICommand]
    private async Task UpdateChecksAsync()
    {
        Overlay.IsBusy = true;
        AccountStatus = Question;
        MatchStatus = Question;
        if (await CheckLocalAsync().ConfigureAwait(false))
        {
            GameStatus = Check;
            if (await CheckLoginAsync().ConfigureAwait(false))
            {
                AccountStatus = Check;
                if (await CheckMatchAsync().ConfigureAwait(false))
                {
                    MatchStatus = Check;
                    CountTimer?.Stop();
                    Overlay.IsBusy = false;
                    GoMatchEvent?.Invoke();
                }
                else
                {
                    MatchStatus = Cross;
                }
            }
            else
            {
                await LocalLoginAsync().ConfigureAwait(false);
                await LocalRegionAsync().ConfigureAwait(false);
                if (await CheckLoginAsync().ConfigureAwait(false))
                {
                    AccountStatus = Check;
                    if (await CheckMatchAsync().ConfigureAwait(false))
                    {
                        MatchStatus = Check;
                        CountTimer?.Stop();
                        Overlay.IsBusy = false;
                        GoMatchEvent?.Invoke();
                    }
                    else
                    {
                        MatchStatus = Cross;
                    }
                }
                else
                {
                    AccountStatus = Cross;
                    MatchStatus = Cross;
                }
            }
        }
        else
        {
            GameStatus = Cross;
            AccountStatus = Cross;
            MatchStatus = Cross;
        }

        Overlay.IsBusy = false;
    }


    public async Task UpdatePartyAsync()
    {
        if (await GetPartyPlayerInfoAsync().ConfigureAwait(false))
        {
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