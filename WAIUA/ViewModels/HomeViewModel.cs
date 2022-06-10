using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using FontAwesome6;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using WAIUA.Helpers;
using WAIUA.Objects;
using WAIUA.Views;
using static WAIUA.Helpers.Login;

namespace WAIUA.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    public delegate void EventAction();

    [ObservableProperty] private int _countdownTime = 15;
    [ObservableProperty] private DispatcherTimer _countTimer;
    [ObservableProperty] private LoadingOverlay _overlay;

    [ObservableProperty] private List<Player> _playerList;

    // [ObservableProperty] private string _queueTime = "-";
    [ObservableProperty] private string _refreshTime = "-";

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
    private void StopTimer() => _countTimer.Stop();


    [ICommand]
    private async Task LoadNowAsync()
    {
        if (!await LiveMatch.LiveMatchChecksAsync().ConfigureAwait(false)) return;
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
        // Overlay.IsBusy = true;
        Application.Current.Dispatcher.Invoke(() =>
        {
            Home.ValorantStatus.Icon = EFontAwesomeIcon.Solid_Question;
            Home.ValorantStatus.Foreground = new SolidColorBrush(Color.FromRgb(0, 126, 249));
            Home.AccountStatus.Icon = EFontAwesomeIcon.Solid_Question;
            Home.AccountStatus.Foreground = new SolidColorBrush(Color.FromRgb(0, 126, 249));
            Home.MatchStatus.Icon = EFontAwesomeIcon.Solid_Question;
            Home.MatchStatus.Foreground = new SolidColorBrush(Color.FromRgb(0, 126, 249));
        });

        if (await Checks.CheckLocalAsync().ConfigureAwait(false))
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Home.ValorantStatus.Icon = EFontAwesomeIcon.Solid_Check;
                Home.ValorantStatus.Foreground = new SolidColorBrush(Color.FromRgb(50, 226, 178));
            });
            if (await Checks.CheckLoginAsync().ConfigureAwait(false))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Home.AccountStatus.Icon = EFontAwesomeIcon.Solid_Check;
                    Home.AccountStatus.Foreground = new SolidColorBrush(Color.FromRgb(50, 226, 178));
                });
                if (await Checks.CheckMatchAsync().ConfigureAwait(false))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Home.MatchStatus.Icon = EFontAwesomeIcon.Solid_Check;
                        Home.MatchStatus.Foreground = new SolidColorBrush(Color.FromRgb(50, 226, 178));
                    });
                    CountTimer?.Stop();
                    // Overlay.IsBusy = false;
                    GoMatchEvent?.Invoke();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Home.MatchStatus.Icon = EFontAwesomeIcon.Solid_Xmark;
                        Home.MatchStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 70, 84));
                    });
                }
            }
            else
            {
                await LocalLoginAsync().ConfigureAwait(false);
                await LocalRegionAsync().ConfigureAwait(false);
                if (await Checks.CheckLoginAsync().ConfigureAwait(false))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Home.AccountStatus.Icon = EFontAwesomeIcon.Solid_Check;
                        Home.AccountStatus.Foreground = new SolidColorBrush(Color.FromRgb(50, 226, 178));
                    });
                    if (await Checks.CheckMatchAsync().ConfigureAwait(false))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Home.MatchStatus.Icon = EFontAwesomeIcon.Solid_Check;
                            Home.MatchStatus.Foreground = new SolidColorBrush(Color.FromRgb(50, 226, 178));
                        });
                        CountTimer?.Stop();
                        // Overlay.IsBusy = false;
                        GoMatchEvent?.Invoke();
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Home.MatchStatus.Icon = EFontAwesomeIcon.Solid_Xmark;
                            Home.MatchStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 70, 84));
                        });
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Home.AccountStatus.Icon = EFontAwesomeIcon.Solid_Xmark;
                        Home.AccountStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 70, 84));
                        Home.MatchStatus.Icon = EFontAwesomeIcon.Solid_Xmark;
                        Home.MatchStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 70, 84));
                    });
                }
            }
        }
        else
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Home.ValorantStatus.Icon = EFontAwesomeIcon.Solid_Xmark;
                Home.ValorantStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 70, 84));
                Home.AccountStatus.Icon = EFontAwesomeIcon.Solid_Xmark;
                Home.AccountStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 70, 84));
                Home.MatchStatus.Icon = EFontAwesomeIcon.Solid_Xmark;
                Home.MatchStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 70, 84));
            });
        }

        // Overlay.IsBusy = false;
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