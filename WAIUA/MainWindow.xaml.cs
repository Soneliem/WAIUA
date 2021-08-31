using System.Windows;
using WAIUA.ViewModels;

namespace WAIUA
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}