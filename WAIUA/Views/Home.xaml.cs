using System.Windows.Controls;
using WAIUA.ViewModels;

namespace WAIUA.Views
{
    public partial class Home : UserControl
    {
        private readonly HomeViewModel _viewModel;
        public Home()
        {
            InitializeComponent();
            _viewModel = new HomeViewModel();
            DataContext = _viewModel;
            System.Diagnostics.Debug.WriteLine(_viewModel.TestOutput);
        }

    }
}