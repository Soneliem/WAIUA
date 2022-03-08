using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;
using WAIUA.Commands;
using WAIUA.Helpers;
using WAIUA.Objects;

namespace WAIUA.ViewModels;

public class MatchViewModel : ViewModelBase
{
    private MatchDetails _match;
    private LoadingOverlay _overlay;
    private List<Player> _playerList;

    public MatchViewModel(INavigationService homeNavigationService, INavigationService matchNavigationService)
    {
        GetPlayerInfoCommand = new RelayCommand(o => { GetPlayerInfoAsync().ConfigureAwait(false); }, o => true);
        NavigateHomeCommand = new NavigateCommand(homeNavigationService);
        NavigateMatchCommand = new NavigateCommand(matchNavigationService);
        Match = new MatchDetails();
        Overlay = new LoadingOverlay();
        PlayerList = new List<Player>();
    }

    public ICommand GetPlayerInfoCommand { get; }
    

    public MatchDetails Match
    {
        get => _match;
        set => SetProperty(ref _match, value, nameof(Match));
    }

    public LoadingOverlay Overlay
    {
        get => _overlay;
        set => SetProperty(ref _overlay, value, nameof(Overlay));
    } 

    public ICommand NavigateHomeCommand { get; }
    public ICommand NavigateMatchCommand { get; }

    public List<Player> PlayerList
    {
        get => _playerList;
        set => SetProperty(ref _playerList, value, nameof(PlayerList));
    }
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


    private void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
    {
        field = newValue;
        OnPropertyChanged(propertyName);
    }
}