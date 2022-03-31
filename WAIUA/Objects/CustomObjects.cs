using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WAIUA.Objects;

[INotifyPropertyChanged]
public partial class IgnData
{
    [ObservableProperty] private Visibility _trackerDisabled;
    [ObservableProperty] private Visibility _trackerEnabled;
    [ObservableProperty] private Uri _trackerUri;
    [ObservableProperty] private string _username;
}

[INotifyPropertyChanged]
public partial class AgentData
{
    [ObservableProperty] private Uri _image;
    [ObservableProperty] private string _name;
}

[INotifyPropertyChanged]
public partial class PlayerUIData
{
    [ObservableProperty] private string _backgroundColour;
    [ObservableProperty] private Guid _partyUuid;
}

[INotifyPropertyChanged]
public partial class SeasonData
{
    [ObservableProperty] private Guid _currentSeason;
    [ObservableProperty] private Guid _previouspreviouspreviousSeason;
    [ObservableProperty] private Guid _previouspreviousSeason;
    [ObservableProperty] private Guid _previousSeason;
}

[INotifyPropertyChanged]
public partial class SkinData
{
    [ObservableProperty] private Uri _phantomImage;
    [ObservableProperty] private string _phantomName;
    [ObservableProperty] private Uri _vandalImage;
    [ObservableProperty] private string _vandalName;
}

[INotifyPropertyChanged]
public partial class RankData
{
    [ObservableProperty] private int _maxRr = 100;
    [ObservableProperty] private Uri _previouspreviouspreviousrankImage;
    [ObservableProperty] private string _previouspreviouspreviousrankName;
    [ObservableProperty] private Uri _previouspreviousrankImage;
    [ObservableProperty] private string _previouspreviousrankName;
    [ObservableProperty] private Uri _previousrankImage;
    [ObservableProperty] private string _previousrankName;
    [ObservableProperty] private Uri _rankImage;
    [ObservableProperty] private string _rankName;
}

[INotifyPropertyChanged]
public partial class MatchHistoryData
{
    [ObservableProperty] private int _previousGame;
    [ObservableProperty] private string _previousGameColour;
    [ObservableProperty] private int _previouspreviousGame;
    [ObservableProperty] private string _previouspreviousGameColour;
    [ObservableProperty] private int _previouspreviouspreviousGame;
    [ObservableProperty] private string _previouspreviouspreviousGameColour;
    [ObservableProperty] private int _rankProgress;
}

public class ValMap
{
    public string Name { get; set; }
    public Guid UUID { get; set; }
}

public class ValSkin
{
    public string Name { get; set; }
    public Uri Image { get; set; }
}

[INotifyPropertyChanged]
public partial class MatchDetails
{
    [ObservableProperty] private string _gameMode;
    [ObservableProperty] private string _map;
    [ObservableProperty] private Uri _mapImage;
    [ObservableProperty] private string _server;
}

[INotifyPropertyChanged]
public partial class Player
{
    [ObservableProperty] private int _accountLevel;
    [ObservableProperty] private Visibility _active = Visibility.Collapsed;
    [ObservableProperty] private AgentData _agentData;
    [ObservableProperty] private string _backgroundColour;
    [ObservableProperty] private IgnData _ignData;
    [ObservableProperty] private MatchHistoryData _matchHistoryData;
    [ObservableProperty] private string _partyColour;
    [ObservableProperty] private PlayerUIData _playerUiData;
    [ObservableProperty] private RankData _rankData;
    [ObservableProperty] private SkinData _skinData;
}

[INotifyPropertyChanged]
public partial class LoadingOverlay
{
    [ObservableProperty] private string _content;

    [ObservableProperty] private string _header;

    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private double _progress;
}