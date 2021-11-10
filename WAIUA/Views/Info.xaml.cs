using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
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
			Hyperlink hl = (Hyperlink) sender;
			string navigateUri = hl.NavigateUri.ToString();
			Process.Start(new ProcessStartInfo(navigateUri) {UseShellExecute = true});
			e.Handled = true;
		}
	}
}