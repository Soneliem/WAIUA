using System.Windows;
using WAIUA.ViewModels;

namespace WAIUA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}