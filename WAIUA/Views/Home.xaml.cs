using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
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
                    if (GoMatch.Command.CanExecute(null)) GoMatch.Command.Execute(null);
                });
            };
    }
    
}