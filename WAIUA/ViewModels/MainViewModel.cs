using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace WAIUA.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private ObservableObject? _selectedViewModel;

    public MainViewModel()
    {
        SelectedViewModel = Ioc.Default.GetRequiredService<HomeViewModel>();
    }

    [RelayCommand]
    public void NavigateHome()
    {
        SelectedViewModel = Ioc.Default.GetRequiredService<HomeViewModel>();
    }

    [RelayCommand]
    public void NavigateInfo()
    {
        SelectedViewModel = Ioc.Default.GetRequiredService<InfoViewModel>();
    }

    [RelayCommand]
    public void NavigateSettings()
    {
        SelectedViewModel = Ioc.Default.GetRequiredService<SettingsViewModel>();
    }

    [RelayCommand]
    public void NavigateMatch()
    {
        SelectedViewModel = Ioc.Default.GetRequiredService<MatchViewModel>();
    }
}