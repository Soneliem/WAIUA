using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using WAIUA.Commands;
using WAIUA.ViewModels;

namespace WAIUA.Views
{
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
            DataContextChanged += DataContextChangedHandler;
        }

        void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = e.NewValue as HomeViewModel;

            if (viewModel != null)
            {
                viewModel.GoMatchEvent += () => { GoMatch.Command.Execute(""); };
            }
        }
    }
}