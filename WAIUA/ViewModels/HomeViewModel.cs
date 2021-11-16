using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WAIUA.Commands;

namespace WAIUA.ViewModels
{
	public class HomeViewModel : ViewModelBase
	{
		public class Player
		{
			public string[] data = null;
			public static Player[] players = null;

			static Player()
			{
				players = new Player[10];
				for (sbyte x = 0; x < 10; x++)
				{
					players[x] = new Player();
				}
			}

			public static string[] Player0 => players[0].data;
			public static string[] Player1 => players[1].data;
			public static string[] Player2 => players[2].data;
			public static string[] Player3 => players[3].data;
			public static string[] Player4 => players[4].data;
			public static string[] Player5 => players[5].data;
			public static string[] Player6 => players[6].data;
			public static string[] Player7 => players[7].data;
			public static string[] Player8 => players[8].data;
			public static string[] Player9 => players[9].data;
		}

		private bool GetPlayerInfo()
		{
			bool output = false;
			try
			{
				var NewMatch = new Main();
				Parallel.For(0, 10, i => { Player.players[i].data = null; });

				if (NewMatch.LiveMatchChecks())
				{
					try
					{
						Parallel.For(0, 10, i =>
						{
							Player.players[i].data = NewMatch.LiveMatchOutput((sbyte)i);
						});

						List<string> colours = new List<string>() { "Red", "Green", "DarkOrange", "White", "DeepSkyBlue", "MediumPurple", "SaddleBrown" };
						for (int i = 0; i < Player.players.Length; i++)
						{
							bool colourused = false;
							string id = Player.players[i].data[28];
							for (int j = i + 1; j < Player.players.Length; j++)
							{
								if (Player.players[j].data[28] == id && Player.players[j].data[28].Length > 8)
								{
									Player.players[i].data[28] = Player.players[j].data[28] = colours[0];
									colourused = true;
								}
							}
							if (colourused)
							{
								colours.RemoveAt(0);
							}
						}
					}
					catch (Exception)
					{
					}

					output = true;
				}
			}
			catch (Exception)
			{
			}

			return output;
		}

		private string[] _player0Prop;

		public string[] Player0
		{
			get => _player0Prop;
			set => SetProperty(ref _player0Prop, value, nameof(Player0));
		}

		private string[] _player1Prop;

		public string[] Player1
		{
			get => _player1Prop;
			set => SetProperty(ref _player1Prop, value, nameof(Player1));
		}

		private string[] _player2Prop;

		public string[] Player2
		{
			get => _player2Prop;
			set => SetProperty(ref _player2Prop, value, nameof(Player2));
		}

		private string[] _player3Prop;

		public string[] Player3
		{
			get => _player3Prop;
			set => SetProperty(ref _player3Prop, value, nameof(Player3));
		}

		private string[] _player4Prop;

		public string[] Player4
		{
			get => _player4Prop;
			set => SetProperty(ref _player4Prop, value, nameof(Player4));
		}

		private string[] _player5Prop;

		public string[] Player5
		{
			get => _player5Prop;
			set => SetProperty(ref _player5Prop, value, nameof(Player5));
		}

		private string[] _player6Prop;

		public string[] Player6
		{
			get => _player6Prop;
			set => SetProperty(ref _player6Prop, value, nameof(Player6));
		}

		private string[] _player7Prop;

		public string[] Player7
		{
			get => _player7Prop;
			set => SetProperty(ref _player7Prop, value, nameof(Player7));
		}

		private string[] _player8Prop;

		public string[] Player8
		{
			get => _player8Prop;
			set => SetProperty(ref _player8Prop, value, nameof(Player8));
		}

		private string[] _player9Prop;

		public string[] Player9
		{
			get => _player9Prop;
			set => SetProperty(ref _player9Prop, value, nameof(Player9));
		}

		public ICommand NavigateHomeCommand { get; }
		public ICommand NavigateInfoCommand { get; }
		public ICommand NavigateSettingsCommand { get; }

		public HomeViewModel(INavigationService homeNavigationService, INavigationService infoNavigationService,
			INavigationService settingsNavigationService)
		{
			Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
			if (GetPlayerInfo())
			{
				_player0Prop = Player.Player0;
				_player1Prop = Player.Player1;
				_player2Prop = Player.Player2;
				_player3Prop = Player.Player3;
				_player4Prop = Player.Player4;
				_player5Prop = Player.Player5;
				_player6Prop = Player.Player6;
				_player7Prop = Player.Player7;
				_player8Prop = Player.Player8;
				_player9Prop = Player.Player9;
			}

			NavigateHomeCommand = new NavigateCommand(homeNavigationService);
			NavigateInfoCommand = new NavigateCommand(infoNavigationService);
			NavigateSettingsCommand = new NavigateCommand(settingsNavigationService);
			Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
		}

		private void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
		{
			field = newValue;
			OnPropertyChanged(propertyName);
		}
	}
}