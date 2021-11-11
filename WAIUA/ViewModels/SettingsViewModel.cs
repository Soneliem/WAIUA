using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;
using System.Windows.Input;

namespace WAIUA.ViewModels
{
	internal class SettingsViewModel : ViewModelBase
	{
		public ICommand NavigateHomeCommand { get; }
		public ICommand NavigateInfoCommand { get; }
		public ICommand NavigateSettingsCommand { get; }

		public SettingsViewModel(INavigationService homeNavigationService, INavigationService infoNavigationService,
			INavigationService settingsNavigationService)
		{
			NavigateHomeCommand = new NavigateCommand(homeNavigationService);
			NavigateInfoCommand = new NavigateCommand(infoNavigationService);
			NavigateSettingsCommand = new NavigateCommand(settingsNavigationService);
		}
	}
}