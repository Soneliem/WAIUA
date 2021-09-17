using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace WAIUA.Controls
{
    public partial class NormalPlayerCell : UserControl
    {
        public NormalPlayerCell()
        {
            InitializeComponent();
        }

        public string Card
        {
            get { return (string) GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }

        public static readonly DependencyProperty CardProperty =
            DependencyProperty.Register("Card", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

        public string Level
        {
            get { return (string) GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register("Level", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

        public string AgentPicture
        {
            get { return (string) GetValue(AgentPictureProperty); }
            set { SetValue(AgentPictureProperty, value); }
        }

        public static readonly DependencyProperty AgentPictureProperty =
            DependencyProperty.Register("AgentPicture", typeof(string), typeof(NormalPlayerCell),
                new PropertyMetadata(null));

        public string Agent
        {
            get { return (string) GetValue(AgentProperty); }
            set { SetValue(AgentProperty, value); }
        }

        public static readonly DependencyProperty AgentProperty =
            DependencyProperty.Register("Agent", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

        public string IGN
        {
            get { return (string) GetValue(IGNProperty); }
            set { SetValue(IGNProperty, value); }
        }

        public static readonly DependencyProperty IGNProperty =
            DependencyProperty.Register("IGN", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

        public string PMatch
        {
            get { return (string) GetValue(PMatchProperty); }
            set { SetValue(PMatchProperty, value); }
        }

        public static readonly DependencyProperty PMatchProperty =
            DependencyProperty.Register("PMatch", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

        public string PPMatch
        {
            get { return (string) GetValue(PPMatchProperty); }
            set { SetValue(PPMatchProperty, value); }
        }

        public static readonly DependencyProperty PPMatchProperty =
            DependencyProperty.Register("PPMatch", typeof(string), typeof(NormalPlayerCell),
                new PropertyMetadata(null));

        public string PPPMatch
        {
            get { return (string) GetValue(PPPMatchProperty); }
            set { SetValue(PPPMatchProperty, value); }
        }

        public static readonly DependencyProperty PPPMatchProperty =
            DependencyProperty.Register("PPPMatch", typeof(string), typeof(NormalPlayerCell),
                new PropertyMetadata(null));

        public string PRankName
        {
            get { return (string) GetValue(PRankNameProperty); }
            set { SetValue(PRankNameProperty, value); }
        }

        public static readonly DependencyProperty PRankNameProperty =
            DependencyProperty.Register("PRankName", typeof(string), typeof(NormalPlayerCell),
                new PropertyMetadata(null));

        public string PPRankName
        {
            get { return (string) GetValue(PPRankNameProperty); }
            set { SetValue(PPRankNameProperty, value); }
        }

        public static readonly DependencyProperty PPRankNameProperty =
            DependencyProperty.Register("PPRankName", typeof(string), typeof(NormalPlayerCell),
                new PropertyMetadata(null));

        public string PPPRankName
        {
            get { return (string) GetValue(PPPRankNameProperty); }
            set { SetValue(PPPRankNameProperty, value); }
        }

        public static readonly DependencyProperty PPPRankNameProperty =
            DependencyProperty.Register("PPPRankName", typeof(string), typeof(NormalPlayerCell),
                new PropertyMetadata(null));

        public string RankName
        {
            get { return (string) GetValue(RankNameProperty); }
            set { SetValue(RankNameProperty, value); }
        }

        public static readonly DependencyProperty RankNameProperty =
            DependencyProperty.Register("RankName", typeof(string), typeof(NormalPlayerCell),
                new PropertyMetadata(null));

        public string PRank
        {
            get { return (string) GetValue(PRankProperty); }
            set { SetValue(PRankProperty, value); }
        }

        public static readonly DependencyProperty PRankProperty =
            DependencyProperty.Register("PRank", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

        public string PPRank
        {
            get { return (string) GetValue(PPRankProperty); }
            set { SetValue(PPRankProperty, value); }
        }

        public static readonly DependencyProperty PPRankProperty =
            DependencyProperty.Register("PPRank", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

        public string PPPRank
        {
            get { return (string) GetValue(PPPRankProperty); }
            set { SetValue(PPPRankProperty, value); }
        }

        public static readonly DependencyProperty PPPRankProperty =
            DependencyProperty.Register("PPPRank", typeof(string), typeof(NormalPlayerCell),
                new PropertyMetadata(null));

        public string Rank
        {
            get { return (string) GetValue(RankProperty); }
            set { SetValue(RankProperty, value); }
        }

        public static readonly DependencyProperty RankProperty =
            DependencyProperty.Register("Rank", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

        public string RankProgress
        {
            get { return (string) GetValue(RankProgressProperty); }
            set { SetValue(RankProgressProperty, value); }
        }

        public static readonly DependencyProperty RankProgressProperty =
            DependencyProperty.Register("RankProgress", typeof(string), typeof(NormalPlayerCell),
                new PropertyMetadata(null));

        public string MaxRR
        {
            get { return (string) GetValue(MaxRRProperty); }
            set { SetValue(MaxRRProperty, value); }
        }

        public static readonly DependencyProperty MaxRRProperty =
            DependencyProperty.Register("MaxRR", typeof(string), typeof(NormalPlayerCell), new PropertyMetadata(null));

        public string TrackerUrl
        {
            get { return (string) GetValue(TrackerUrlProperty); }
            set { SetValue(TrackerUrlProperty, value); }
        }

        public static readonly DependencyProperty TrackerUrlProperty =
            DependencyProperty.Register("TrackerUrl", typeof(string), typeof(NormalPlayerCell),
                new PropertyMetadata(null));

        public string TrackerEnabled
        {
            get { return (string) GetValue(TrackerEnabledProperty); }
            set { SetValue(TrackerEnabledProperty, value); }
        }

        public static readonly DependencyProperty TrackerEnabledProperty =
            DependencyProperty.Register("TrackerEnabled", typeof(string), typeof(NormalPlayerCell),
                new PropertyMetadata(null));

        public string TrackerDisabled
        {
            get { return (string) GetValue(TrackerDisabledProperty); }
            set { SetValue(TrackerDisabledProperty, value); }
        }

        public static readonly DependencyProperty TrackerDisabledProperty =
            DependencyProperty.Register("TrackerDisabled", typeof(string), typeof(NormalPlayerCell),
                new PropertyMetadata(null));

        private void HandleLinkClick(object sender, RequestNavigateEventArgs e)
        {

            Hyperlink hl = (Hyperlink)sender;
            string navigateUri = hl.NavigateUri.ToString();
            System.Diagnostics.Debug.WriteLine($"Hyperlink Clicked: {navigateUri}");
            Process.Start(new ProcessStartInfo(navigateUri) { UseShellExecute = true });
            e.Handled = true;
        }

    }
}