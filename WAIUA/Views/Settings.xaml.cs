using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Controls;
using WAIUA.Properties;
using static WAIUA.Commands.Main;

namespace WAIUA.Views
{
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }

        private List<CultureInfo> LanguageList = new List<CultureInfo>();

        private void CheckAuth()
        {
            AuthStatusBox.Text = Properties.Resources.Refreshing;
            if (!GetSetPPUUID())
                AuthStatusBox.Text = Properties.Resources.AuthStatusFail;
            else AuthStatusBox.Text = $"{Properties.Resources.AuthStatusAuthAs}{GetIGUsername(PPUUID)}";
        }

        private void Button_Click2(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckAuth();
        }

        private void Button_Click3(object sender, System.Windows.RoutedEventArgs e)
        {
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
        }

        private void ListBox_Selected(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            int index = combo.SelectedIndex;
            Thread.CurrentThread.CurrentCulture = LanguageList[index];
            Thread.CurrentThread.CurrentUICulture = LanguageList[index];
            Properties.Settings.Default.Language = LanguageList[index].IetfLanguageTag;
        }

        private static IEnumerable<CultureInfo> GetAvailableCultures()
        {
            List<CultureInfo> result = new List<CultureInfo>();
            ResourceManager rm = new ResourceManager(typeof(Resources));

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (CultureInfo culture in cultures)
            {
                try
                {
                    if (culture.Equals(CultureInfo.InvariantCulture)) continue;

                    ResourceSet rs = rm.GetResourceSet(culture, true, false);
                    if (rs != null)
                        result.Add(culture);
                }
                catch (CultureNotFoundException)
                {
                    //TODO
                }
            }
            return result;
        }

        private void LanguageList_OnDropDownOpened(object sender, EventArgs e)
        {
            if (LanguageCombo.Items.Count == 0)
            {
                foreach (CultureInfo language in GetAvailableCultures())
                {
                    LanguageCombo.Items.Add(language.NativeName);
                    LanguageList.Add(language);
                }
            }
        }
    }
}