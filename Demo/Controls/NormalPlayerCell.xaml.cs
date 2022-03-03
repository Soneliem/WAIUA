using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using Demo.Controls;

namespace Demo.Controls
{
	public partial class NormalPlayerCell : UserControl
	{
		public NormalPlayerCell()
		{
			InitializeComponent();
		}
        public PlayerNew Player
        {
            get => (PlayerNew)GetValue(PlayerProperty);
            set => SetValue(PlayerProperty, value);
        }
        
        public static readonly DependencyProperty PlayerProperty =
            DependencyProperty.Register("Player", typeof(PlayerNew), typeof(NormalPlayerCell),
                new PropertyMetadata(null));
		private void HandleLinkClick(object sender, RequestNavigateEventArgs e)
		{
			Hyperlink hl = (Hyperlink)sender;
			string navigateUri = hl.NavigateUri.ToString();
			Process.Start(new ProcessStartInfo(navigateUri) { UseShellExecute = true });
			e.Handled = true;
		}
    }

    public class PlayerNew
    {
        public Uri AgentImage { get; set; }
        public Uri AgentName { get; set; }
        public string PlayerName { get; set; }
        public int AccountLevel { get; set; }
        public int MaxRR { get; set; }
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
}