using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;
using System.Windows.Input;

namespace WAIUA.ViewModels
{
    internal class AccountViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateInfoCommand { get; }
        public ICommand NavigateAccountCommand { get; }

        public AccountViewModel(INavigationService homeNavigationService, INavigationService infoNavigationService, INavigationService accountNavigationService)
        {
            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateInfoCommand = new NavigateCommand(infoNavigationService);
            NavigateAccountCommand = new NavigateCommand(accountNavigationService);
        }
    }
}