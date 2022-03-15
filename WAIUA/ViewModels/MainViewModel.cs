using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace WAIUA.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        SelectedViewModel = Ioc.Default.GetRequiredService<HomeViewModel>();
    }
}