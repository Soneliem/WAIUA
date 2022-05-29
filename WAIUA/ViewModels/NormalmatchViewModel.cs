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
    [ObservableProperty] private List<Player> _rightPlayerList;
    [ObservableProperty] private List<Player> _leftPlayerList;

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
    }
    
    List<List<Player>> partition(List<Player> values, int chunkSize)
    {
        return values.Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
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
                List<Player> AllPlayers = new List<Player>();
                Overlay.Content = "Getting Player Details";
                AllPlayers = await newMatch.LiveMatchOutputAsync(UpdatePercentage).ConfigureAwait(false);

                List<List<Player>> partitions = partition(AllPlayers, 2);
                LeftPlayerList = partitions[0];
                if (partitions.Count > 1)
                {
                    RightPlayerList = partitions[1];
                }

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