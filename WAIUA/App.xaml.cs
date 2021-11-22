using System.Globalization;
using System.Threading;
using System.Windows;
using AutoUpdaterDotNET;
using MVVMEssentials.Services;
using MVVMEssentials.Stores;
using MVVMEssentials.ViewModels;
using WAIUA.Properties;
using WAIUA.ViewModels;
using static WAIUA.ValAPI.ValAPI;

namespace WAIUA
{
	public partial class App : Application
	{
		private readonly ModalNavigationStore _modalNavigationStore;
		private readonly NavigationStore _navigationStore;

		public App()
		{
			_navigationStore = new NavigationStore();
			_modalNavigationStore = new ModalNavigationStore();

			if (Settings.Default.Language.Length >= 3) Settings.Default.Reset();

			if (string.IsNullOrEmpty(Settings.Default.Language))
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.InstalledUICulture;
				Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
				Settings.Default.Language = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
			}
			else
			{
				Thread.CurrentThread.CurrentCulture = new CultureInfo(Settings.Default.Language);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Language);
			}
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			var navigationService = CreateInfoNavigationService();
			navigationService.Navigate();

			MainWindow = new MainWindow
			{
				DataContext = new MainViewModel(_navigationStore, _modalNavigationStore)
			};

			MainWindow.Show();
			base.OnStartup(e);
			await CheckAndUpdateJson();
			AutoUpdater.ShowSkipButton = false;
			AutoUpdater.Start("https://raw.githubusercontent.com/Soneliem/WAIUA/master/WAIUA/VersionInfo.xml");
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			Settings.Default.Save();
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