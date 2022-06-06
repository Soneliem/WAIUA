using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using WAIUA.Helpers;
using WAIUA.Objects;

namespace WAIUA.ViewModels;

public partial class NormalmatchViewModel : ObservableObject
{
    [ObservableProperty] private List<Player> _leftPlayerList;
    [ObservableProperty] private MatchDetails _match;
    [ObservableProperty] private LoadingOverlay _overlay;
    [ObservableProperty] private List<Player> _rightPlayerList;
    [ObservableProperty] private DispatcherTimer _countTimer;
    [ObservableProperty] private int _countdownTime = 20;
    private int _resettime = 20;

    public delegate void EventAction();
    public event EventAction GoHomeEvent;
    
    [ObservableProperty] private string _refreshTime = "-";

    public NormalmatchViewModel()
    {
        Match = new MatchDetails();
        Overlay = new LoadingOverlay
        {
            Header = "Loading",
            Content = "Getting Match Details",
            IsBusy = false
        };

        LeftPlayerList = new List<Player>();
        RightPlayerList = new List<Player>();

        _countTimer = new DispatcherTimer(DispatcherPriority.Normal);
        _countTimer.Tick += UpdateTimersAsync;
        _countTimer.Interval = new TimeSpan(0, 0, 1);
        _countTimer.Start();
    }

    [ICommand]
    private void StopTimer() => _countTimer.Stop();

    private async void UpdateTimersAsync(object sender, EventArgs e)
    {
        RefreshTime = CountdownTime + "s";
        if (CountdownTime == 0)
        {
            CountdownTime = _resettime;
            await GetMatchInfoAsync().ConfigureAwait(false);
        }

        CountdownTime--;
    }

    [ICommand]
    private async Task GetMatchInfoAsync()
    {

        Overlay = new LoadingOverlay()
        {
            IsBusy = true,
            Header = "Loading",
            Progress = 0
        };

        try
        {
            Match newMatch = new();
            if (await Helpers.Match.LiveMatchChecksAsync(false).ConfigureAwait(false))
            {
                var AllPlayers = new List<Player>();
                Overlay.Content = "Getting Player Details";
                AllPlayers = await newMatch.LiveMatchOutputAsync(UpdatePercentage).ConfigureAwait(false);

                if (newMatch.Status != "PREGAME")
                {
                    _resettime = 69;
                    CountdownTime = 69;
                }

                if (newMatch.QueueId == "deathmatch")
                {
                    var mid = AllPlayers.Count / 2;
                    LeftPlayerList = AllPlayers.Take(mid).ToList();
                    RightPlayerList = AllPlayers.Skip(mid).ToList();
                }
                else
                {
                    LeftPlayerList.Clear();
                    RightPlayerList.Clear();
                    foreach (var player in AllPlayers)
                        switch (player.TeamId)
                        {
                            case "Blue":
                                LeftPlayerList.Add(player);
                                break;
                            case "Red":
                                RightPlayerList.Add(player);
                                break;
                        }

                    LeftPlayerList = LeftPlayerList.ToList();
                    RightPlayerList = RightPlayerList.ToList();
                }

                AllPlayers.Clear();

                if (newMatch.MatchInfo != null)
                    Match = newMatch.MatchInfo;

                Overlay.IsBusy = false;
            }
            else
            {
                CountTimer?.Stop();
                GoHomeEvent?.Invoke();
            }
        }
        catch (Exception)
        {
            Overlay.IsBusy = false;
        }
        finally
        {
            Overlay.IsBusy = false;
        }

        GC.Collect();
    }

    private void UpdatePercentage(int percentage)
    {
        Overlay.Progress = percentage;
    }
}