using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WAIUA.Objects;

[INotifyPropertyChanged]
public partial class IgnData
{
    [ObservableProperty] private string _username;
    [ObservableProperty] private Visibility _trackerEnabled;
    [ObservableProperty] private Visibility _trackerDisabled;
    [ObservableProperty] private Uri _trackerUri;
}

[INotifyPropertyChanged]
public partial class AgentData
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private Uri _image;
}

[INotifyPropertyChanged]
public partial class PlayerUIData
{
    [ObservableProperty] private Guid _partyUuid;
    [ObservableProperty] private String _backgroundColour;
}

[INotifyPropertyChanged]
public partial class SeasonData
{
    [ObservableProperty] private Guid _currentSeason;
    [ObservableProperty] private Guid _previousSeason;
    [ObservableProperty] private Guid _previouspreviousSeason;
    [ObservableProperty] private Guid _previouspreviouspreviousSeason;

}

[INotifyPropertyChanged]
public partial class SkinData
{
    [ObservableProperty] private string _vandalName;
    [ObservableProperty] private Uri _vandalImage;
    [ObservableProperty] private string _phantomName;
    [ObservableProperty] private Uri _phantomImage;

}
[INotifyPropertyChanged]
public partial class RankData
{
    [ObservableProperty] private string _rankName;
    [ObservableProperty] private Uri _rankImage;
    [ObservableProperty] private string _previousrankName;
    [ObservableProperty] private Uri _previousrankImage;
    [ObservableProperty] private string _previouspreviousrankName;
    [ObservableProperty] private Uri _previouspreviousrankImage;
    [ObservableProperty] private string _previouspreviouspreviousrankName;
    [ObservableProperty] private Uri _previouspreviouspreviousrankImage;
    [ObservableProperty] private int _maxRr = 100;

}

[INotifyPropertyChanged]
public partial class MatchHistoryData
{
    [ObservableProperty] private int _rankProgress;
    [ObservableProperty] private int _previousGame;
    [ObservableProperty] private int _previouspreviousGame;
    [ObservableProperty] private int _previouspreviouspreviousGame;
    [ObservableProperty] private string _previousGameColour;
    [ObservableProperty] private string _previouspreviousGameColour;
    [ObservableProperty] private string _previouspreviouspreviousGameColour;

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
    [ObservableProperty] private AgentData _agentData;
    [ObservableProperty] private IgnData _ignData;
    [ObservableProperty] private int _accountLevel;
    [ObservableProperty] private MatchHistoryData _matchHistoryData;
    [ObservableProperty] private RankData _rankData;
    [ObservableProperty] private SkinData _skinData;
    [ObservableProperty] private PlayerUIData _playerUiData;
    [ObservableProperty] private string _partyColour;
    [ObservableProperty] private string _backgroundColour;
}

[INotifyPropertyChanged]
public partial class LoadingOverlay
{
    [ObservableProperty]
    private bool _isBusy;
    [ObservableProperty]
    private double _progress;
    [ObservableProperty]
    private string _header;
    [ObservableProperty]
    private string _content;

}
