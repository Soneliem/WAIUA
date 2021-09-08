using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

namespace WAIUA.Views
{
    /// <summary>
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class Info : UserControl
    {
        public Info()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            CurrentVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            LatestVersion.Text = GetLatestVerion();
        }

        private string GetLatestVerion()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load("https://raw.githubusercontent.com/Soneliem/WAIUA/master/WAIUA/VersionInfo.xml");
            XmlNodeList result = xml.GetElementsByTagName("version");
            return result[0].InnerText;

        }
    }
}