using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WAIUA.Objects;

[INotifyPropertyChanged]
public partial class MatchDetails
{
    [ObservableProperty] private string gameMode;
    [ObservableProperty] private string map;
    [ObservableProperty] private Uri mapImage;
    [ObservableProperty] private string server;
}
[INotifyPropertyChanged]
public partial class Player
{
    [ObservableProperty] private Uri agentImage;
    [ObservableProperty] private Uri agentName;
    [ObservableProperty] private string playerName;
    [ObservableProperty] private int accountLevel;
    [ObservableProperty] private int maxRR = 100;
    [ObservableProperty] private int previousGameMmr;
    [ObservableProperty] private int previousPreviousGameMmr;
    [ObservableProperty] private int previousPreviousPreviousGameMmr;
    [ObservableProperty] private string previousGameMmrColour;
    [ObservableProperty] private string previousPreviousGameMmrColour;
    [ObservableProperty] private string previousPreviousPreviousGameMmrColour;
    [ObservableProperty] private int rankProgress;
    [ObservableProperty] private Uri rank;
    [ObservableProperty] private Uri previousRank;
    [ObservableProperty] private Uri previousPreviousRank;
    [ObservableProperty] private Uri previousPreviousPreviousRank;
    [ObservableProperty] private string rankName;
    [ObservableProperty] private string previousRankName;
    [ObservableProperty] private string previousPreviousRankName;
    [ObservableProperty] private string previousPreviousPreviousRankName;
    [ObservableProperty] private Uri phantomImage;
    [ObservableProperty] private Uri vandalImage;
    [ObservableProperty] private string phantomName;
    [ObservableProperty] private string vandalName;
    [ObservableProperty] private Uri trackerUri;
    [ObservableProperty] private Visibility trackerDisabled;
    [ObservableProperty] private Visibility trackerEnabled;
    [ObservableProperty] private Guid partyUuid;
    [ObservableProperty] private string partyColour;
    [ObservableProperty] private string backgroundColour;
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
