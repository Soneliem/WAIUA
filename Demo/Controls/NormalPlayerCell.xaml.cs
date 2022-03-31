using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Demo.Controls
{
    public partial class NormalPlayerCell : UserControl
    {
        public static readonly DependencyProperty PlayerProperty =
            DependencyProperty.Register("PlayerCell", typeof(Player), typeof(NormalPlayerCell), new PropertyMetadata(new Player()));

        public NormalPlayerCell()
        {
            InitializeComponent();
        }

        public Player PlayerCell
        {
            get => (Player) GetValue(PlayerProperty);
            set => SetValue(PlayerProperty, value);
        }

        private void HandleLinkClick(object sender, RequestNavigateEventArgs e)
        {
            var hl = (Hyperlink) sender;
            var navigateUri = hl.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri) {UseShellExecute = true});
            e.Handled = true;
        }
    }

    [INotifyPropertyChanged]
    public partial class Player
    {
        [ObservableProperty] private int _accountLevel;
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
}