using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                var AllPlayers = new List<Player>();
                Overlay.Content = "Getting Player Details";
                AllPlayers = await newMatch.LiveMatchOutputAsync(UpdatePercentage).ConfigureAwait(false);

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