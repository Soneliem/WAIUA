using System.Windows;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using WAIUA.ViewModels;

namespace WAIUA;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<MainViewModel>();
        ((App) Application.Current).WindowPlace.Register(this);
    }
}