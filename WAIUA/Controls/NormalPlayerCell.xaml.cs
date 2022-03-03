using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using WAIUA.Models;

namespace WAIUA.Controls
{
    public partial class NormalPlayerCell : UserControl
    {
        public static readonly DependencyProperty PlayerProperty =
            DependencyProperty.Register("Player", typeof(PlayerNew), typeof(NormalPlayerCell), new PropertyMetadata(new PlayerNew()));

        public NormalPlayerCell()
        {
            InitializeComponent();
        }
        public PlayerNew Player
        {
            get => (PlayerNew) GetValue(PlayerProperty);
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
}