using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WAIUA.Objects;


public class MatchDetails
{
    public string GameMode { get; set; }
    public string Map { get; set; }
    public Uri MapImage { get; set; }
    public string Server { get; set; }
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

public class LoadingOverlay : INotifyPropertyChanged
{
    private bool _isBusy;
    private double _progress;
    private string _header;
    private string _content;

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged("IsBusy");
        }
    }

    public double Progress
    {
        get => _progress;
        set
        {
            _progress = value;
            OnPropertyChanged("Progress");
        }
    }

    public string Header {
        get => _header;
        set
        {
            _header = value;
            OnPropertyChanged("Header");
        }
    }
    public string Content {
        get => _content;
        set
        {
            _content = value;
            OnPropertyChanged("Content");
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
