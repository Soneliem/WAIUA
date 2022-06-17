using System.Windows;
using System.Windows.Controls;
using WAIUA.ViewModels;

namespace WAIUA.Views;

/// <summary>
///     Interaction logic for Match.xaml
/// </summary>
public partial class Match : UserControl
{
    public Match()
    {
        InitializeComponent();
        DataContextChanged += DataContextChangedHandler;
    }

    private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
    {
        var viewModel = e.NewValue as MatchViewModel;

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