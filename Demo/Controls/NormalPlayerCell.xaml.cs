using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Demo.Controls
{
	public partial class NormalPlayerCell : UserControl
	{
		public NormalPlayerCell()
		{
			InitializeComponent();
		}

		public string Card
		{
			get { return (string)GetValue(CardProperty); }
			set { SetValue(CardProperty, value); }
		}

		public static readonly DependencyProperty CardProperty =
			DependencyProperty.Register("Card", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public string Level
		{
			get { return (string)GetValue(LevelProperty); }
			set { SetValue(LevelProperty, value); }
		}

		public static readonly DependencyProperty LevelProperty =
			DependencyProperty.Register("Level", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public string AgentPicture
		{
			get { return (string)GetValue(AgentPictureProperty); }
			set { SetValue(AgentPictureProperty, value); }
		}

		public static readonly DependencyProperty AgentPictureProperty =
			DependencyProperty.Register("AgentPicture", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string Agent
		{
			get { return (string)GetValue(AgentProperty); }
			set { SetValue(AgentProperty, value); }
		}

		public static readonly DependencyProperty AgentProperty =
			DependencyProperty.Register("Agent", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public string IGN
		{
			get { return (string)GetValue(IGNProperty); }
			set { SetValue(IGNProperty, value); }
		}

		public static readonly DependencyProperty IGNProperty =
			DependencyProperty.Register("IGN", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public string VandalSkin
		{
			get { return (string)GetValue(VandalSkinProperty); }
			set { SetValue(VandalSkinProperty, value); }
		}

		public static readonly DependencyProperty VandalSkinProperty =
			DependencyProperty.Register("VandalSkin", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string PhantomSkin
		{
			get { return (string)GetValue(PhantomSkinProperty); }
			set { SetValue(PhantomSkinProperty, value); }
		}

		public static readonly DependencyProperty PhantomSkinProperty =
			DependencyProperty.Register("PhantomSkin", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string VandalSkinName
		{
			get { return (string)GetValue(VandalSkinNameProperty); }
			set { SetValue(VandalSkinNameProperty, value); }
		}

		public static readonly DependencyProperty VandalSkinNameProperty =
			DependencyProperty.Register("VandalSkinName", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string PhantomSkinName
		{
			get { return (string)GetValue(PhantomSkinNameProperty); }
			set { SetValue(PhantomSkinNameProperty, value); }
		}

		public static readonly DependencyProperty PhantomSkinNameProperty =
			DependencyProperty.Register("PhantomSkinName", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string PMatch
		{
			get { return (string)GetValue(PMatchProperty); }
			set { SetValue(PMatchProperty, value); }
		}

		public static readonly DependencyProperty PMatchProperty =
			DependencyProperty.Register("PMatch", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public string PPMatch
		{
			get { return (string)GetValue(PPMatchProperty); }
			set { SetValue(PPMatchProperty, value); }
		}

		public static readonly DependencyProperty PPMatchProperty =
			DependencyProperty.Register("PPMatch", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string PPPMatch
		{
			get { return (string)GetValue(PPPMatchProperty); }
			set { SetValue(PPPMatchProperty, value); }
		}

		public static readonly DependencyProperty PPPMatchProperty =
			DependencyProperty.Register("PPPMatch", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string PMatchColour
		{
			get { return (string)GetValue(PMatchColourProperty); }
			set { SetValue(PMatchColourProperty, value); }
		}

		public static readonly DependencyProperty PMatchColourProperty =
			DependencyProperty.Register("PMatchColour", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public string PPMatchColour
		{
			get { return (string)GetValue(PPMatchColourProperty); }
			set { SetValue(PPMatchColourProperty, value); }
		}

		public static readonly DependencyProperty PPMatchColourProperty =
			DependencyProperty.Register("PPMatchColour", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string PPPMatchColour
		{
			get { return (string)GetValue(PPPMatchColourProperty); }
			set { SetValue(PPPMatchColourProperty, value); }
		}

		public static readonly DependencyProperty PPPMatchColourProperty =
			DependencyProperty.Register("PPPMatchColour", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string PRankName
		{
			get { return (string)GetValue(PRankNameProperty); }
			set { SetValue(PRankNameProperty, value); }
		}

		public static readonly DependencyProperty PRankNameProperty =
			DependencyProperty.Register("PRankName", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string PPRankName
		{
			get { return (string)GetValue(PPRankNameProperty); }
			set { SetValue(PPRankNameProperty, value); }
		}

		public static readonly DependencyProperty PPRankNameProperty =
			DependencyProperty.Register("PPRankName", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string PPPRankName
		{
			get { return (string)GetValue(PPPRankNameProperty); }
			set { SetValue(PPPRankNameProperty, value); }
		}

		public static readonly DependencyProperty PPPRankNameProperty =
			DependencyProperty.Register("PPPRankName", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string RankName
		{
			get { return (string)GetValue(RankNameProperty); }
			set { SetValue(RankNameProperty, value); }
		}

		public static readonly DependencyProperty RankNameProperty =
			DependencyProperty.Register("RankName", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string PRank
		{
			get { return (string)GetValue(PRankProperty); }
			set { SetValue(PRankProperty, value); }
		}

		public static readonly DependencyProperty PRankProperty =
			DependencyProperty.Register("PRank", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public string PPRank
		{
			get { return (string)GetValue(PPRankProperty); }
			set { SetValue(PPRankProperty, value); }
		}

		public static readonly DependencyProperty PPRankProperty =
			DependencyProperty.Register("PPRank", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public string PPPRank
		{
			get { return (string)GetValue(PPPRankProperty); }
			set { SetValue(PPPRankProperty, value); }
		}

		public static readonly DependencyProperty PPPRankProperty =
			DependencyProperty.Register("PPPRank", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string Rank
		{
			get { return (string)GetValue(RankProperty); }
			set { SetValue(RankProperty, value); }
		}

		public static readonly DependencyProperty RankProperty =
			DependencyProperty.Register("Rank", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public string RankProgress
		{
			get { return (string)GetValue(RankProgressProperty); }
			set { SetValue(RankProgressProperty, value); }
		}

		public static readonly DependencyProperty RankProgressProperty =
			DependencyProperty.Register("RankProgress", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string MaxRR
		{
			get { return (string)GetValue(MaxRRProperty); }
			set { SetValue(MaxRRProperty, value); }
		}

		public static readonly DependencyProperty MaxRRProperty =
			DependencyProperty.Register("MaxRR", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

		public string TrackerUrl
		{
			get { return (string)GetValue(TrackerUrlProperty); }
			set { SetValue(TrackerUrlProperty, value); }
		}

		public static readonly DependencyProperty TrackerUrlProperty =
			DependencyProperty.Register("TrackerUrl", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string TrackerEnabled
		{
			get { return (string)GetValue(TrackerEnabledProperty); }
			set { SetValue(TrackerEnabledProperty, value); }
		}

		public static readonly DependencyProperty TrackerEnabledProperty =
			DependencyProperty.Register("TrackerEnabled", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string TrackerDisabled
		{
			get { return (string)GetValue(TrackerDisabledProperty); }
			set { SetValue(TrackerDisabledProperty, value); }
		}

		public static readonly DependencyProperty TrackerDisabledProperty =
			DependencyProperty.Register("TrackerDisabled", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		public string PartyColour
		{
			get { return (string)GetValue(PartyColourProperty); }
			set { SetValue(PartyColourProperty, value); }
		}

		public static readonly DependencyProperty PartyColourProperty =
			DependencyProperty.Register("PartyColour", typeof(string), typeof(NormalPlayerCell),
				new PropertyMetadata(null));

		private void HandleLinkClick(object sender, RequestNavigateEventArgs e)
		{
			Hyperlink hl = (Hyperlink)sender;
			string navigateUri = hl.NavigateUri.ToString();
			Process.Start(new ProcessStartInfo(navigateUri) { UseShellExecute = true });
			e.Handled = true;
		}
	}
}