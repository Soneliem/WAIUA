﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WAIUA.Helpers;
using WAIUA.Objects;

namespace WAIUA.ViewModels;

public partial class MatchViewModel : ObservableObject
{
    public delegate void EventAction();

    [ObservableProperty] private int _countdownTime = 80;
    [ObservableProperty] private DispatcherTimer _countTimer;
    [ObservableProperty] private List<Player> _leftPlayerList;
    [ObservableProperty] private MatchDetails _match;
    [ObservableProperty] private LoadingOverlay _overlay;
    [ObservableProperty] private string _refreshTime = "-";
    private int _resettime = 80;
    [ObservableProperty] private List<Player> _rightPlayerList;

    public MatchViewModel()
    {
        _countTimer = new DispatcherTimer();
        _countTimer.Tick += UpdateTimersAsync;
        _countTimer.Interval = new TimeSpan(0, 0, 1);

        Match = new MatchDetails();
        Overlay = new LoadingOverlay
        {
            Header = "Loading",
            Content = "Getting Match Details",
            IsBusy = false
        };

        LeftPlayerList = new List<Player>();
        RightPlayerList = new List<Player>();
    }

    public event EventAction GoHomeEvent;

    [RelayCommand]
    private void PassiveLoadAsync()
    {
        if (!_countTimer.IsEnabled) _countTimer.Start();
    }

    [RelayCommand]
    private async Task PassiveLoadCheckAsync()
    {
        if (!_countTimer.IsEnabled)
        {
            _countTimer.Start();
            await GetMatchInfoAsync().ConfigureAwait(false);
        }
    }

    [RelayCommand]
    private void StopPassiveLoadAsync()
    {
        CountTimer?.Stop();
        RefreshTime = "-";
    }

    private async void UpdateTimersAsync(object sender, EventArgs e)
    {
        RefreshTime = CountdownTime + "s";
        if (CountdownTime <= 0)
        {
            CountdownTime = _resettime;
            await GetMatchInfoAsync().ConfigureAwait(false);
        }

        CountdownTime--;
    }

    [RelayCommand]
    private async Task GetMatchInfoAsync()
    {
        Overlay = new LoadingOverlay
        {
            IsBusy = true,
            Header = "Loading",
            Progress = 0
        };

        try
        {
            LiveMatch newLiveMatch = new();
            if (await LiveMatch.LiveMatchChecksAsync().ConfigureAwait(false))
            {
                var AllPlayers = new List<Player>();
                Overlay.Content = "Getting Player Details";
                AllPlayers = await newLiveMatch.LiveMatchOutputAsync(UpdatePercentage).ConfigureAwait(false);

                if (newLiveMatch.Status != "PREGAME")
                {
                    _resettime = 120;
                    CountdownTime = 120;
                }

                if (newLiveMatch.QueueId == "deathmatch" || AllPlayers.Count > 10)
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

                if (newLiveMatch.MatchInfo != null)
                    Match = newLiveMatch.MatchInfo;

                UpdateStats();

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
            // ignored
        }
        finally
        {
            Overlay.IsBusy = false;
        }

        GC.Collect();
    }

    private async void UpdateStats()
    {
        // List<Task> tasks = new();
        var AllPlayers = LeftPlayerList.Concat(RightPlayerList).ToList();
        foreach (var player in AllPlayers)
            // var t1 = LiveMatch.GetMatchHistoryAsync(player.PlayerUiData.Puuid);
            // player.MatchHistoryData = t1.Result;
            player.MatchHistoryData = await LiveMatch.GetMatchHistoryAsync(player.PlayerUiData.Puuid).ConfigureAwait(false);
    }

    private void UpdatePercentage(int percentage)
    {
        Overlay.Progress = percentage;
    }
}