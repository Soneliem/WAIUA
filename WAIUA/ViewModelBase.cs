using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using WAIUA.ViewModels;

namespace WAIUA
{
    public partial class ViewModelBase: ObservableObject
    {
        [ObservableProperty]
        private ObservableObject? _selectedViewModel;
        [ICommand]
        void NavigateHome()
        {
            SelectedViewModel = Ioc.Default.GetRequiredService<HomeViewModel>();
        }
        [ICommand]
        void NavigateInfo()
        {
            SelectedViewModel = Ioc.Default.GetRequiredService<InfoViewModel>();
        }
        [ICommand]
        void NavigateSettings()
        {
            SelectedViewModel = Ioc.Default.GetRequiredService<SettingsViewModel>();
        }
        [ICommand]
        void NavigateMatch()
        {
            SelectedViewModel = Ioc.Default.GetRequiredService<MatchViewModel>();
        }
    }
}
