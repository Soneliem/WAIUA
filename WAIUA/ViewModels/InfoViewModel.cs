using System.Windows.Input;
using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;

namespace WAIUA.ViewModels
{
    internal class InfoViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateInfoCommand { get; }
        public ICommand NavigateAccountCommand { get; }

        public InfoViewModel(INavigationService homeNavigationService, INavigationService infoNavigationService, INavigationService accountNavigationService)
        {
            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateInfoCommand = new NavigateCommand(infoNavigationService);
            NavigateAccountCommand = new NavigateCommand(accountNavigationService);
        }
    }
}