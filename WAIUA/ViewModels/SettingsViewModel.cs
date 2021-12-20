using System.Windows.Input;
using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;

namespace WAIUA.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(INavigationService homeNavigationService, INavigationService infoNavigationService,
            INavigationService settingsNavigationService)
        {
            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateInfoCommand = new NavigateCommand(infoNavigationService);
            NavigateSettingsCommand = new NavigateCommand(settingsNavigationService);
        }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateInfoCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
    }
}