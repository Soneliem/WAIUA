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
    public void NavigateNormalmatch()
    {
        SelectedViewModel = Ioc.Default.GetRequiredService<NormalmatchViewModel>();
    }
}