using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using WAIUA.Properties;
using static WAIUA.Commands.Main;
using static WAIUA.ValAPI.ValAPI;
using AutoUpdaterDotNET;
using System.Xml;
using System.Windows;
using System.Reflection;
using System.Threading.Tasks;

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
			else AuthStatusBox.Text = $"{Properties.Resources.AuthStatusAuthAs} {GetIGUsername(PPUUID)}";
		}

		private async void Button_Click1(object sender, RoutedEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Wait;
			CurrentVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			LatestVersion.Text = await Task.Run(GetLatestVerion);
			AutoUpdater.Start("https://raw.githubusercontent.com/Soneliem/WAIUA/master/WAIUA/VersionInfo.xml");
			await CheckAndUpdateJson();
			Mouse.OverrideCursor = Cursors.Arrow;
		}

		private static string GetLatestVerion()
		{
			XmlDocument xml = new XmlDocument();
			xml.Load("https://raw.githubusercontent.com/Soneliem/WAIUA/master/WAIUA/VersionInfo.xml");
			XmlNodeList result = xml.GetElementsByTagName("version");
			return result[0].InnerText;
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

		private async void ListBox_Selected(object sender, SelectionChangedEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Wait;
			ComboBox combo = (ComboBox) sender;
			int index = combo.SelectedIndex;
			Thread.CurrentThread.CurrentCulture = LanguageList[index];
			Thread.CurrentThread.CurrentUICulture = LanguageList[index];
			Properties.Settings.Default.Language = LanguageList[index].TwoLetterISOLanguageName;
            await UpdateJson();
			Mouse.OverrideCursor = Cursors.Arrow;
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