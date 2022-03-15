using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using WAIUA.ViewModels;

namespace WAIUA
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetRequiredService<MainViewModel>();
        }
    }
}