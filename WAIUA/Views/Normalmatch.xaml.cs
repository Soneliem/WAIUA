using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WAIUA.ViewModels;

namespace WAIUA.Views;

/// <summary>
///     Interaction logic for Normalmatch.xaml
/// </summary>
public partial class Normalmatch : UserControl
{
    public Normalmatch()
    {
        InitializeComponent();
        DataContextChanged += DataContextChangedHandler;
    }

    private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
    {
        var viewModel = e.NewValue as NormalmatchViewModel;

        if (viewModel != null)
            viewModel.GoHomeEvent += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (GoHome.Command.CanExecute(null)) GoHome.Command.Execute(null);
                });
            };
    }
}