using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using FontAwesome6.Fonts;
using WAIUA.ViewModels;

namespace WAIUA.Views;

public partial class Home : UserControl
{
    public static ImageAwesome ValorantStatus;
    public static ImageAwesome AccountStatus;
    public static ImageAwesome MatchStatus;
    private readonly DispatcherTimer _gameTimer = new();
    private readonly Random _rand = new();
    private readonly List<Ellipse> _removeThis = new();
    private int _currentRate;
    private int _health = 350;
    private int _posX;
    private int _posY;
    private int _score;
    private int _spawnRate = 60;


    public Home()
    {
        InitializeComponent();
        DataContextChanged += DataContextChangedHandler;

        _gameTimer.Tick += GameLoop;
        _gameTimer.Interval = TimeSpan.FromMilliseconds(20);

        ValorantStatus = ValorantStatusView;
        AccountStatus = AccountStatusView;
        MatchStatus = MatchStatusView;
    }
    

    private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is HomeViewModel viewModel)
            viewModel.GoMatchEvent += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (GoMatch.Command.CanExecute(null)) GoMatch.Command.Execute(null);
                });
            };
    }

    private void GameLoop(object sender, EventArgs e) // Heavily modified from https://www.mooict.com/wpf-c-tutorial-create-a-simple-clicking-game-in-visual-studio/
    {
        txtScore.Content = _score;
        txtLastScore.Content = Properties.Settings.Default.AimScore;
        _currentRate -= 2;

        if (_currentRate < 1)
        {
            _currentRate = _spawnRate;

            _posX = _rand.Next(15, (int) MyCanvas.ActualWidth - 15);
            _posY = _rand.Next(50, (int) MyCanvas.ActualHeight - 15);

            var circle = new Ellipse
            {
                Tag = "circle",
                Height = 5,
                Width = 5,
                Fill = new SolidColorBrush(Color.FromRgb(255, 70, 84))
            };

            Canvas.SetLeft(circle, _posX);
            Canvas.SetTop(circle, _posY);
            MyCanvas.Children.Add(circle);
        }

        foreach (var x in MyCanvas.Children.OfType<Ellipse>())
        {
            x.RenderTransformOrigin = new Point(0.5, 0.5);
            x.Height += 0.6;
            x.Width += 0.6;

            if (!(x.Width > 70)) continue;

            _removeThis.Add(x);
            _health -= 15;
        }

        if (_health > 1)
            healthBar.Width = _health;
        else
            GameOverFunction();

        foreach (var i in _removeThis)
            MyCanvas.Children.Remove(i);

        _spawnRate = _score switch
        {
            < 5 => 60,
            < 20 => 50,
            < 35 => 40,
            < 50 => 30,
            < 65 => 20,
            _ => _spawnRate
        };
    }

    private void ClickOnCanvas(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not Ellipse)
        {
            _health -= 10;
            return;
        }
        var circle = (Ellipse) e.OriginalSource;
        MyCanvas.Children.Remove(circle);
        _score++;
    }

    private void GameOverFunction()
    {
        _gameTimer.Stop();
        foreach (var y in MyCanvas.Children.OfType<Ellipse>())
            _removeThis.Add(y);
        foreach (var i in _removeThis) MyCanvas.Children.Remove(i);
        _spawnRate = 60;
        Properties.Settings.Default.AimScore = _score;
        Properties.Settings.Default.Save();
        _score = 0;
        _currentRate = 5;
        _health = 350;
        _removeThis.Clear();

        TrainerWindow.Visibility = Visibility.Collapsed;
        Grid.RowDefinitions[1].Height = GridLength.Auto;
        Grid.RowDefinitions[0].Height = new GridLength(0.5, GridUnitType.Star);
        TrainerDialog.Visibility = Visibility.Visible;
    }

    private void StartAimTrainerButton(object sender, RoutedEventArgs e)
    {
        _gameTimer.Start();
        _currentRate = _spawnRate;
        TrainerWindow.Visibility = Visibility.Visible;
        Grid.RowDefinitions[0].Height = GridLength.Auto;
        Grid.RowDefinitions[1].Height = new GridLength(0.5, GridUnitType.Star);
        TrainerDialog.Visibility = Visibility.Collapsed;
    }
}