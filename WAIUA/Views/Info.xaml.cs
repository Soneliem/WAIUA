using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WAIUA.Views
{
    public partial class Info : UserControl
    {
        public Info()
        {
            InitializeComponent();
        }

        private void HandleLinkClick(object sender, RoutedEventArgs e)
        {
            var hl = (Hyperlink) sender;
            var navigateUri = hl.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri) {UseShellExecute = true});
            e.Handled = true;
        }

        private void ImageClick(object sender, RoutedEventArgs e)
        {
            var _button = (Button) sender;
            Process.Start(new ProcessStartInfo(_button.Tag.ToString()) {UseShellExecute = true});
            e.Handled = true;
        }
    }
}