using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace WAIUA.Controls
{
	public partial class NormalPlayerCell : UserControl
	{
		public static readonly DependencyProperty CardProperty =
			DependencyProperty.Register("Card", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public static readonly DependencyProperty LevelProperty =
			DependencyProperty.Register("Level", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public static readonly DependencyProperty AgentPictureProperty =
			DependencyProperty.Register("AgentPicture", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty AgentProperty =
			DependencyProperty.Register("Agent", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public static readonly DependencyProperty IGNProperty =
			DependencyProperty.Register("IGN", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public static readonly DependencyProperty VandalSkinProperty =
			DependencyProperty.Register("VandalSkin", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty PhantomSkinProperty =
			DependencyProperty.Register("PhantomSkin", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty VandalSkinNameProperty =
			DependencyProperty.Register("VandalSkinName", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty PhantomSkinNameProperty =
			DependencyProperty.Register("PhantomSkinName", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty PMatchProperty =
			DependencyProperty.Register("PMatch", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public static readonly DependencyProperty PPMatchProperty =
			DependencyProperty.Register("PPMatch", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty PPPMatchProperty =
			DependencyProperty.Register("PPPMatch", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty PMatchColourProperty =
			DependencyProperty.Register("PMatchColour", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty PPMatchColourProperty =
			DependencyProperty.Register("PPMatchColour", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty PPPMatchColourProperty =
			DependencyProperty.Register("PPPMatchColour", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty PRankNameProperty =
			DependencyProperty.Register("PRankName", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty PPRankNameProperty =
			DependencyProperty.Register("PPRankName", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty PPPRankNameProperty =
			DependencyProperty.Register("PPPRankName", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty RankNameProperty =
			DependencyProperty.Register("RankName", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty PRankProperty =
			DependencyProperty.Register("PRank", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public static readonly DependencyProperty PPRankProperty =
			DependencyProperty.Register("PPRank", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public static readonly DependencyProperty PPPRankProperty =
			DependencyProperty.Register("PPPRank", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty RankProperty =
			DependencyProperty.Register("Rank", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public static readonly DependencyProperty RankProgressProperty =
			DependencyProperty.Register("RankProgress", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty MaxRRProperty =
			DependencyProperty.Register("MaxRR", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public static readonly DependencyProperty TrackerUrlProperty =
			DependencyProperty.Register("TrackerUrl", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty TrackerEnabledProperty =
			DependencyProperty.Register("TrackerEnabled", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty TrackerDisabledProperty =
			DependencyProperty.Register("TrackerDisabled", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public static readonly DependencyProperty PartyColourProperty =
			DependencyProperty.Register("PartyColour", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public NormalPlayerCell()
		{
			InitializeComponent();
		}

		public string Card
		{
			get => (string)GetValue(CardProperty);
			set => SetValue(CardProperty, value);
		}

		public string Level
		{
			get => (string)GetValue(LevelProperty);
			set => SetValue(LevelProperty, value);
		}

		public string AgentPicture
		{
			get => (string)GetValue(AgentPictureProperty);
			set => SetValue(AgentPictureProperty, value);
		}

		public string Agent
		{
			get => (string)GetValue(AgentProperty);
			set => SetValue(AgentProperty, value);
		}

		public string IGN
		{
			get => (string)GetValue(IGNProperty);
			set => SetValue(IGNProperty, value);
		}

		public string VandalSkin
		{
			get => (string)GetValue(VandalSkinProperty);
			set => SetValue(VandalSkinProperty, value);
		}

		public string PhantomSkin
		{
			get => (string)GetValue(PhantomSkinProperty);
			set => SetValue(PhantomSkinProperty, value);
		}

		public string VandalSkinName
		{
			get => (string)GetValue(VandalSkinNameProperty);
			set => SetValue(VandalSkinNameProperty, value);
		}

		public string PhantomSkinName
		{
			get => (string)GetValue(PhantomSkinNameProperty);
			set => SetValue(PhantomSkinNameProperty, value);
		}

		public string PMatch
		{
			get => (string)GetValue(PMatchProperty);
			set => SetValue(PMatchProperty, value);
		}

		public string PPMatch
		{
			get => (string)GetValue(PPMatchProperty);
			set => SetValue(PPMatchProperty, value);
		}

		public string PPPMatch
		{
			get => (string)GetValue(PPPMatchProperty);
			set => SetValue(PPPMatchProperty, value);
		}

		public string PMatchColour
		{
			get => (string)GetValue(PMatchColourProperty);
			set => SetValue(PMatchColourProperty, value);
		}

		public string PPMatchColour
		{
			get => (string)GetValue(PPMatchColourProperty);
			set => SetValue(PPMatchColourProperty, value);
		}

		public string PPPMatchColour
		{
			get => (string)GetValue(PPPMatchColourProperty);
			set => SetValue(PPPMatchColourProperty, value);
		}

		public string PRankName
		{
			get => (string)GetValue(PRankNameProperty);
			set => SetValue(PRankNameProperty, value);
		}

		public string PPRankName
		{
			get => (string)GetValue(PPRankNameProperty);
			set => SetValue(PPRankNameProperty, value);
		}

		public string PPPRankName
		{
			get => (string)GetValue(PPPRankNameProperty);
			set => SetValue(PPPRankNameProperty, value);
		}

		public string RankName
		{
			get => (string)GetValue(RankNameProperty);
			set => SetValue(RankNameProperty, value);
		}

		public string PRank
		{
			get => (string)GetValue(PRankProperty);
			set => SetValue(PRankProperty, value);
		}

		public string PPRank
		{
			get => (string)GetValue(PPRankProperty);
			set => SetValue(PPRankProperty, value);
		}

		public string PPPRank
		{
			get => (string)GetValue(PPPRankProperty);
			set => SetValue(PPPRankProperty, value);
		}

		public string Rank
		{
			get => (string)GetValue(RankProperty);
			set => SetValue(RankProperty, value);
		}

		public string RankProgress
		{
			get => (string)GetValue(RankProgressProperty);
			set => SetValue(RankProgressProperty, value);
		}

		public string MaxRR
		{
			get => (string)GetValue(MaxRRProperty);
			set => SetValue(MaxRRProperty, value);
		}

		public string TrackerUrl
		{
			get => (string)GetValue(TrackerUrlProperty);
			set => SetValue(TrackerUrlProperty, value);
		}

		public string TrackerEnabled
		{
			get => (string)GetValue(TrackerEnabledProperty);
			set => SetValue(TrackerEnabledProperty, value);
		}

		public string TrackerDisabled
		{
			get => (string)GetValue(TrackerDisabledProperty);
			set => SetValue(TrackerDisabledProperty, value);
		}

		public string PartyColour
		{
			get => (string)GetValue(PartyColourProperty);
			set => SetValue(PartyColourProperty, value);
		}

		private void HandleLinkClick(object sender, RequestNavigateEventArgs e)
		{
			var hl = (Hyperlink)sender;
			var navigateUri = hl.NavigateUri.ToString();
			Process.Start(new ProcessStartInfo(navigateUri) { UseShellExecute = true });
			e.Handled = true;
		}
	}
}