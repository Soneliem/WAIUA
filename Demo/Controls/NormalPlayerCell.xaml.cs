using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using Demo.Controls;
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
}