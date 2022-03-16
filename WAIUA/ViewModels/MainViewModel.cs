using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace WAIUA.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        SelectedViewModel = Ioc.Default.GetRequiredService<HomeViewModel>();
    }
    [ObservableProperty]
    private ObservableObject? _selectedViewModel;
    [ICommand]
    public void NavigateHome()
    {
        SelectedViewModel = Ioc.Default.GetRequiredService<HomeViewModel>();
    }
    [ICommand]
    public void NavigateInfo()
    {
        SelectedViewModel = Ioc.Default.GetRequiredService<InfoViewModel>();
    }
    [ICommand]
    public void NavigateSettings()
    {
        SelectedViewModel = Ioc.Default.GetRequiredService<SettingsViewModel>();
    }
    [ICommand]
    public void NavigateMatch()
    {
        SelectedViewModel = Ioc.Default.GetRequiredService<MatchViewModel>();
    }
}