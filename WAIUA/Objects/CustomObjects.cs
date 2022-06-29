using System;
using System.Windows;
using Microsoft.Toolkit.Mvvm.ComponentModel;

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
public partial class IdentityData
{
    [ObservableProperty] private Uri _image;
    [ObservableProperty] private string _name;
}

[INotifyPropertyChanged]
public partial class PlayerUIData
{
    [ObservableProperty] private string _backgroundColour;
    [ObservableProperty] private string _partyColour;
    [ObservableProperty] private Guid _partyUuid;
    [ObservableProperty] private Guid _Puuid;
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
    [ObservableProperty] private Uri _cardImage;
    [ObservableProperty] private string _cardName;
    [ObservableProperty] private Uri _spray1Image;
    [ObservableProperty] private string _spray1Name;
    [ObservableProperty] private Uri _spray2Image;
    [ObservableProperty] private string _spray2Name;
    [ObservableProperty] private Uri _spray3Image;
    [ObservableProperty] private string _spray3Name;    
    [ObservableProperty] private Uri _classicImage;
    [ObservableProperty] private string _classicName;
    [ObservableProperty] private Uri _shortyImage;
    [ObservableProperty] private string _shortyName;
    [ObservableProperty] private Uri _frenzyImage;
    [ObservableProperty] private string _frenzyName;
    [ObservableProperty] private Uri _ghostImage;
    [ObservableProperty] private string _ghostName;
    [ObservableProperty] private Uri _sherrifImage;
    [ObservableProperty] private string _sherrifName;
    [ObservableProperty] private Uri _stingerImage;
    [ObservableProperty] private string _stingerName;
    [ObservableProperty] private Uri _spectreImage;
    [ObservableProperty] private string _spectreName;
    [ObservableProperty] private Uri _buckyImage;
    [ObservableProperty] private string _buckyName;
    [ObservableProperty] private Uri _judgeImage;
    [ObservableProperty] private string _judgeName;
    [ObservableProperty] private Uri _bulldogImage;
    [ObservableProperty] private string _bulldogName;
    [ObservableProperty] private Uri _guardianImage;
    [ObservableProperty] private string _guardianName;
    [ObservableProperty] private Uri _phantomImage;
    [ObservableProperty] private string _phantomName;
    [ObservableProperty] private Uri _vandalImage;
    [ObservableProperty] private string _vandalName;
    [ObservableProperty] private Uri _marshalImage;
    [ObservableProperty] private string _marshalName;
    [ObservableProperty] private Uri _operatorImage;
    [ObservableProperty] private string _operatorName;
    [ObservableProperty] private Uri _aresImage;
    [ObservableProperty] private string _aresName;
    [ObservableProperty] private Uri _odinImage;
    [ObservableProperty] private string _odinName;
    [ObservableProperty] private Uri _meleeImage;
    [ObservableProperty] private string _meleeName;
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
    [ObservableProperty] private Uri _gameModeImage;
    [ObservableProperty] private string _map;
    [ObservableProperty] private Uri _mapImage;
    [ObservableProperty] private string _server;
}

[INotifyPropertyChanged]
public partial class Player
{
    [ObservableProperty] private string _accountLevel;
    [ObservableProperty] private Visibility _active = Visibility.Collapsed;
    [ObservableProperty] private IdentityData _identityData;
    [ObservableProperty] private IgnData _ignData;
    [ObservableProperty] private MatchHistoryData _matchHistoryData;
    [ObservableProperty] private PlayerUIData _playerUiData;
    [ObservableProperty] private RankData _rankData;
    [ObservableProperty] private SkinData _skinData;
    [ObservableProperty] private string _teamId;
}

[INotifyPropertyChanged]
public partial class LoadingOverlay
{
    [ObservableProperty] private string _content;
    [ObservableProperty] private string _header;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private int _progress;
}