using System.Windows;
using MVVMEssentials.Services;
using MVVMEssentials.Stores;
using MVVMEssentials.ViewModels;
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
        }

        private INavigationService CreateHomeNavigationService()
        {
            return new NavigationService<HomeViewModel>(_navigationStore, CreateHomeViewModel);
        }

        private HomeViewModel CreateHomeViewModel()
        {
            return new HomeViewModel(CreateHomeNavigationService(), CreateInfoNavigationService(), CreateAccountNavigationService());
        }
        private INavigationService CreateInfoNavigationService()
        {
            return new NavigationService<InfoViewModel>(_navigationStore, CreateInfoViewModel);
        }

        private InfoViewModel CreateInfoViewModel()
        {
            return new InfoViewModel(CreateHomeNavigationService(), CreateInfoNavigationService(), CreateAccountNavigationService());
        }
        private INavigationService CreateAccountNavigationService()
        {
            return new NavigationService<AccountViewModel>(_navigationStore, CreateAccountViewModel);
        }

        private AccountViewModel CreateAccountViewModel()
        {
            return new AccountViewModel(CreateHomeNavigationService(), CreateInfoNavigationService(), CreateAccountNavigationService());
        }
    }
}