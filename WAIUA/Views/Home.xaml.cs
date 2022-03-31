using System.Windows;
using System.Windows.Controls;
using WAIUA.ViewModels;

namespace WAIUA.Views;

public partial class Home : UserControl
{
    public Home()
    {
        InitializeComponent();
        DataContextChanged += DataContextChangedHandler;
    }

    private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
    {
        var viewModel = e.NewValue as HomeViewModel;

        if (viewModel != null)
            viewModel.GoMatchEvent += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    // ButtonAutomationPeer peer = new ButtonAutomationPeer(GoMatch);
                    // IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    // invokeProv.Invoke();
                    if (GoMatch.Command.CanExecute(null)) GoMatch.Command.Execute(null);
                });
            };
    }
}