using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WAIUA.Commands;
using WAIUA.ViewModels;

namespace WAIUA.Views
{
    public partial class Home : UserControl
    {
        private DispatcherTimer dispatcherTimer;
        private DispatcherTimer dispatcherTimer2;
        private Main newMatch;

        private HomeViewModel viewModel;

        public Home()
        {
            InitializeComponent();
        }

        private void PassiveLoad(object sender, RoutedEventArgs e)
        {
            PassiveBtn.Text = "Waiting for match";
            viewModel = (HomeViewModel) DataContext;
            newMatch = new Main();
            if (newMatch.LiveMatchChecks(true))
                if (viewModel.NavigateMatchCommand.CanExecute(null))
                    viewModel.NavigateMatchCommand.Execute(null);
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 15);

            dispatcherTimer2 = new DispatcherTimer();
            dispatcherTimer2.Tick += updateTimers;
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 15);

            dispatcherTimer.Start();
            dispatcherTimer2.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (newMatch.LiveMatchChecks(true))
            {
                dispatcherTimer.Stop();
                if (viewModel.NavigateMatchCommand.CanExecute(null))
                    viewModel.NavigateMatchCommand.Execute(null);
            }

            CommandManager.InvalidateRequerySuggested();
        }

        private void updateTimers(object sender, EventArgs e)
        {
        }
    }
}