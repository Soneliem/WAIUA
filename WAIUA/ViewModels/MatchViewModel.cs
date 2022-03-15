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

public partial class MatchViewModel : ViewModelBase
{
    public MatchViewModel()
    {

        Match = new MatchDetails();
        Overlay = new LoadingOverlay();
        PlayerList = new List<Player>();
    }

    [ObservableProperty]
    private MatchDetails _match;
    [ObservableProperty]
    private LoadingOverlay _overlay;
    [ObservableProperty]
    private List<Player> _playerList;
    [ICommand]
    private async Task GetPlayerInfoAsync()
    {
        Overlay.IsBusy = true;
        Overlay.Header = "Loading"; 

        try
        {
            var newMatch = new LiveMatch();

            Overlay.Content = "Getting Match Details";
            if (await newMatch.LiveMatchChecksAsync(false).ConfigureAwait(false))
            {
                Overlay.Content = "Getting Player Details";
                PlayerList = await newMatch.LiveMatchOutputAsync().ConfigureAwait(false);

                if (newMatch.MatchInfo != null) Match = newMatch.MatchInfo;

                Overlay.IsBusy = false;
            }
        }
        catch (Exception)
        {
            Debugger.Break();
            Overlay.IsBusy = false;
        }
    }

}