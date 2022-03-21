using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
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
                viewModel.GoMatchEvent += () => {
                    // if (GoMatch.Command.CanExecute(null))
                    // {
                    //     GoMatch.Command.Execute(null);
                    // }
                    this.Dispatcher.Invoke(() =>
                    {
                        ButtonAutomationPeer peer = new ButtonAutomationPeer(GoMatch);
                        IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                        invokeProv.Invoke();
                    });
                    
                };
            }
        }
    }
}