using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WAIUA.Commands;
using WAIUA.Helpers;
using WAIUA.Objects;

namespace WAIUA.ViewModels;

public partial class MatchViewModel : ObservableObject
{
    public MatchViewModel()
    {
        Match = new MatchDetails();
        Overlay = new LoadingOverlay()
        {
            Header = "Loading", Content = "Getting Match Details"
        };
        PlayerList = new List<Player>(new List<Player>(10));
    }

    [ObservableProperty] private MatchDetails _match;
    [ObservableProperty] private LoadingOverlay _overlay;
    [ObservableProperty] private List<Player> _playerList;

    [ICommand]
    private async Task GetPlayerInfoAsync()
    {
        Overlay.IsBusy = true;
        Overlay.Header = "Loading";
        Overlay.Progress = 0;

        try
        {
            var newMatch = new Match();
            if (await Helpers.Match.LiveMatchChecksAsync(false).ConfigureAwait(false))
            {
                Overlay.Content = "Getting Player Details";
                PlayerList = await newMatch.LiveMatchOutputAsync(UpdatePercentage).ConfigureAwait(false);

                if (newMatch.MatchInfo != null)
                    Match = newMatch.MatchInfo;

                Overlay.IsBusy = false;
            }
        }
        catch (Exception)
        {
            Overlay.IsBusy = false;
            Debugger.Break();
        }
        finally
        {
            Overlay.IsBusy = false;
        }
        Overlay.IsBusy = false;

    }

    private void UpdatePercentage(int percentage)
    {
        Overlay.Progress = percentage;
    }
    
}