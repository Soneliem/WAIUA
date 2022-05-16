using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WAIUA.Helpers;
using WAIUA.Objects;

namespace WAIUA.ViewModels;

public partial class NormalmatchViewModel : ObservableObject
{
    [ObservableProperty] private MatchDetails _match;
    [ObservableProperty] private LoadingOverlay _overlay;
    [ObservableProperty] private List<Player> _playerList;

    public NormalmatchViewModel()
    {
        Match = new MatchDetails();
        Overlay = new LoadingOverlay
        {
            Header = "Loading",
            Content = "Getting Match Details",
            IsBusy = false
        };

        PlayerList = new List<Player>();
    }

    [ICommand]
    private async Task GetMatchInfoAsync()
    {
        Overlay.IsBusy = true;
        Overlay.Header = "Loading";
        Overlay.Progress = 0;

        try
        {
            Match newMatch = new();
            if (await Helpers.Match.LiveMatchChecksAsync(false).ConfigureAwait(false))
            {
                Overlay.Content = "Getting Player Details";
                PlayerList = await newMatch.LiveMatchOutputAsync(UpdatePercentage).ConfigureAwait(false);
                var deltaSize = 10 - PlayerList.Count;

                if (deltaSize > 0)
                    PlayerList.AddRange(Enumerable.Repeat(new Player(), deltaSize));

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
        GC.Collect();
    }

    private void UpdatePercentage(int percentage)
    {
        Overlay.Progress = percentage;
    }
}