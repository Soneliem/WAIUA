using AutoUpdaterDotNET;
using MVVMEssentials.Services;
using MVVMEssentials.Stores;
using MVVMEssentials.ViewModels;
using System.Globalization;
using System.Threading;
using System.Windows;
using WAIUA.ViewModels;

namespace WAIUA
{
	public partial class App : Application
	{
		private readonly NavigationStore _navigationStore;
		private readonly ModalNavigationStore _modalNavigationStore;

		public App()
		{
			_navigationStore = new NavigationStore();
			_modalNavigationStore = new ModalNavigationStore();

			if (WAIUA.Properties.Settings.Default.Language.Length >= 3)
			{
				WAIUA.Properties.Settings.Default.Reset();
			}

			;

			if (string.IsNullOrEmpty(WAIUA.Properties.Settings.Default.Language))
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.InstalledUICulture;
				Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
				WAIUA.Properties.Settings.Default.Language = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
			}
			else
			{
				Thread.CurrentThread.CurrentCulture = new CultureInfo(WAIUA.Properties.Settings.Default.Language);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(WAIUA.Properties.Settings.Default.Language);
			}
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			INavigationService navigationService = CreateInfoNavigationService();
			navigationService.Navigate();

			MainWindow = new MainWindow()
			{
				DataContext = new MainViewModel(_navigationStore, _modalNavigationStore)
			};
			MainWindow.Show();

			base.OnStartup(e);
			AutoUpdater.Start("https://raw.githubusercontent.com/Soneliem/WAIUA/master/WAIUA/VersionInfo.xml");
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			WAIUA.Properties.Settings.Default.Save();
		}

		private INavigationService CreateHomeNavigationService()
		{
			return new NavigationService<HomeViewModel>(_navigationStore, CreateHomeViewModel);
		}

		private HomeViewModel CreateHomeViewModel()
		{
			return new HomeViewModel(CreateHomeNavigationService(), CreateInfoNavigationService(),
				CreateSettingsNavigationService());
		}

		private INavigationService CreateInfoNavigationService()
		{
			return new NavigationService<InfoViewModel>(_navigationStore, CreateInfoViewModel);
		}

		private InfoViewModel CreateInfoViewModel()
		{
			return new InfoViewModel(CreateHomeNavigationService(), CreateInfoNavigationService(),
				CreateSettingsNavigationService());
		}

		private INavigationService CreateSettingsNavigationService()
		{
			return new NavigationService<SettingsViewModel>(_navigationStore, CreateSettingsViewModel);
		}

		private SettingsViewModel CreateSettingsViewModel()
		{
			return new SettingsViewModel(CreateHomeNavigationService(), CreateInfoNavigationService(),
				CreateSettingsNavigationService());
		}
	}
}