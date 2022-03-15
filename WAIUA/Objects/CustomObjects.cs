using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WAIUA.Objects;

[INotifyPropertyChanged]
public partial class MatchDetails
{
    [ObservableProperty] public string gameMode;
    [ObservableProperty] public string map;
    [ObservableProperty] public Uri mapImage;
    [ObservableProperty] public string server;
}
public class Player
{
    public Uri AgentImage { get; set; }
    public Uri AgentName { get; set; }
    public string PlayerName { get; set; }
    public int AccountLevel { get; set; }
    public int MaxRR { get; set; } = 100;
    public int PreviousGameMmr { get; set; }
    public int PreviousPreviousGameMmr { get; set; }
    public int PreviousPreviousPreviousGameMmr { get; set; }
    public string PreviousGameMmrColour { get; set; }
    public string PreviousPreviousGameMmrColour { get; set; }
    public string PreviousPreviousPreviousGameMmrColour { get; set; }
    public int RankProgress { get; set; }
    public Uri Rank { get; set; }
    public Uri PreviousRank { get; set; }
    public Uri PreviousPreviousRank { get; set; }
    public Uri PreviousPreviousPreviousRank { get; set; }
    public string RankName { get; set; }
    public string PreviousRankName { get; set; }
    public string PreviousPreviousRankName { get; set; }
    public string PreviousPreviousPreviousRankName { get; set; }
    public Uri PhantomImage { get; set; }
    public Uri VandalImage { get; set; }
    public string PhantomName { get; set; }
    public string VandalName { get; set; }
    public Uri TrackerUri { get; set; }
    public Visibility TrackerDisabled { get; set; }
    public Visibility TrackerEnabled { get; set; }
    public Guid PartyUuid { get; set; }
    public string PartyColour { get; set; }
    public string BackgroundColour { get; set; }
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
