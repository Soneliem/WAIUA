using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Demo.Controls;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Demo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
            PlayerList = new List<Player>
            {
                new(), new(), new(), new(), new(), new(), new(), new(), new(), new()
            };
        }
        [ObservableProperty] private List<Player> _playerList;
        [ObservableProperty] private MatchDetails _match;
        [INotifyPropertyChanged]
        public partial class MatchDetails
        {
            [ObservableProperty] private string _gameMode;
            [ObservableProperty] private string _map;
            [ObservableProperty] private Uri _mapImage;
            [ObservableProperty] private string _server;
        }
	}
}