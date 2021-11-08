using AutoUpdaterDotNET;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;
using static WAIUA.ValAPI.ValAPI;

namespace WAIUA.Views
{
    public partial class Info : UserControl
    {
        public Info()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            LatestVersion.Text = await Task.Run(GetLatestVerion);
            AutoUpdater.Start("https://raw.githubusercontent.com/Soneliem/WAIUA/master/WAIUA/VersionInfo.xml");
            await CheckAndUpdateJson();
        }

        private string GetLatestVerion()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load("https://raw.githubusercontent.com/Soneliem/WAIUA/master/WAIUA/VersionInfo.xml");
            XmlNodeList result = xml.GetElementsByTagName("version");
            return result[0].InnerText;

        }

        private void HandleLinkClick(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;
            string navigateUri = hl.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}