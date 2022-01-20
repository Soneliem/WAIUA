using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace WAIUA.Views
{
    public partial class Info : UserControl
    {
        public Info()
        {
            InitializeComponent();
        }

        private void HandleLinkClickAsync(object sender, RequestNavigateEventArgs e)
        {
            var hl = (Hyperlink) sender;
            var navigateUri = hl.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri) {UseShellExecute = true});
            e.Handled = true;
        }

        private void ImageClickAsync(object sender, RoutedEventArgs e)
        {
            var _button = (Button) sender;
            Process.Start(new ProcessStartInfo(_button.Tag.ToString()) {UseShellExecute = true});
            e.Handled = true;
        }
    }
}