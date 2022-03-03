using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;
using WAIUA.Commands;
using WAIUA.Helpers;
using WAIUA.Models;

namespace WAIUA.ViewModels;

public class MatchViewModel : ViewModelBase
{
    private string _gameMode;
    private string _map;
    private Uri _mapImg;
    private string _server;
    private bool _isBusy;
    private List<PlayerNew> _playerList;

    public MatchViewModel(INavigationService homeNavigationService, INavigationService matchNavigationService)
    {
        GetPlayerInfoCommand = new RelayCommand(o => { GetPlayerInfoAsync().ConfigureAwait(false); }, o => true);
        NavigateHomeCommand = new NavigateCommand(homeNavigationService);
        NavigateMatchCommand = new NavigateCommand(matchNavigationService);
    }

    public ICommand GetPlayerInfoCommand { get; }
    public string Server
    {
        get => _server;
        set => SetProperty(ref _server, value, nameof(Server));
    }

    public string GameMode
    {
        get => _gameMode;
        set => SetProperty(ref _gameMode, value, nameof(GameMode));
    }

    public string Map
    {
        get => _map;
        set => SetProperty(ref _map, value, nameof(Map));
    }

    public Uri MapImg
    {
        get => _mapImg;
        set => SetProperty(ref _mapImg, value, nameof(MapImg));
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value, nameof(IsBusy));
    }

    public ICommand NavigateHomeCommand { get; }
    public ICommand NavigateMatchCommand { get; }

    public List<PlayerNew> PlayerList
    {
        get => _playerList;
        set => SetProperty(ref _playerList, value, nameof(PlayerList));
    }
    private async Task GetPlayerInfoAsync()
    {

        IsBusy = true;

        try
        {
            var newMatch = new LiveMatch();

            if (await newMatch.LiveMatchChecksAsync(false).ConfigureAwait(false))
            {

                PlayerList = await newMatch.LiveMatchOutputAsync().ConfigureAwait(false);

                if (newMatch.Server != null) Server = newMatch.Server;
                if (newMatch.GameMode != null) GameMode = newMatch.GameMode;
                if (newMatch.Map != null) Map = newMatch.Map;
                if (newMatch.MapImage != null) MapImg = newMatch.MapImage;

                
                IsBusy = false;
            }
        }
        catch (Exception)
        {
            Debugger.Break();
            IsBusy = false;
        }
    }


    private void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
    {
        field = newValue;
        OnPropertyChanged(propertyName);
    }
}