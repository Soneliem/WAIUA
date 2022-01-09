using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using AutoUpdaterDotNET;
using WAIUA.Helpers;
using WAIUA.Properties;
using static WAIUA.Helpers.Login;
using static WAIUA.Helpers.ValAPI;

namespace WAIUA.Views
{
    public partial class Settings : UserControl
    {
        private readonly List<CultureInfo> LanguageList = new();

        public Settings()
        {
            InitializeComponent();
        }

        private void CheckAuth()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            AuthStatusBox.Text = Properties.Resources.Refreshing;
            if (!GetSetPPUUID())
                AuthStatusBox.Text = Properties.Resources.AuthStatusFail;
            else AuthStatusBox.Text = $"{Properties.Resources.AuthStatusAuthAs} {GetIGUsername(Constants.PPUUID)}";
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private async void Button_Click1(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            CurrentVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            LatestVersion.Text = await Task.Run(GetLatestVersion);
            AutoUpdater.Start("https://raw.githubusercontent.com/Soneliem/WAIUA/master/WAIUA/VersionInfo.xml");
            await CheckAndUpdateJson();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private static string GetLatestVersion()
        {
            var xml = new XmlDocument();
            xml.Load("https://raw.githubusercontent.com/Soneliem/WAIUA/master/WAIUA/VersionInfo.xml");
            var result = xml.GetElementsByTagName("version");
            return result[0].InnerText;
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            CheckAuth();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (CheckLocal())
            {
                LocalLogin();
                LocalRegion();
                CheckAuth();
            }
            else
            {
                AuthStatusBox.Text = Properties.Resources.NoValGame;
            }

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private async void Button_Click4(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            await CheckAndUpdateJson();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private async void Button_Click5(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            await UpdateFiles();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private async void ListBox_Selected(object sender, SelectionChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var combo = (ComboBox) sender;
            var index = combo.SelectedIndex;
            Thread.CurrentThread.CurrentCulture = LanguageList[index];
            Thread.CurrentThread.CurrentUICulture = LanguageList[index];
            Properties.Settings.Default.Language = LanguageList[index].TwoLetterISOLanguageName;
            await UpdateFiles();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private static IEnumerable<CultureInfo> GetAvailableCultures()
        {
            var result = new List<CultureInfo>();
            var rm = new ResourceManager(typeof(Resources));

            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var culture in cultures)
                try
                {
                    if (culture.Equals(CultureInfo.InvariantCulture)) continue;

                    var rs = rm.GetResourceSet(culture, true, false);
                    if (rs != null)
                        result.Add(culture);
                }
                catch (CultureNotFoundException)
                {
                }

            return result;
        }

        private void LanguageList_OnDropDownOpened(object sender, EventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (LanguageCombo.Items.Count == 0)
                foreach (var language in GetAvailableCultures())
                {
                    LanguageCombo.Items.Add(language.NativeName);
                    LanguageList.Add(language);
                }

            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}