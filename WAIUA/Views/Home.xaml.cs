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
    private readonly DispatcherTimer gameTimer = new();
    private readonly Random rand = new();
    private readonly List<Ellipse> removeThis = new();
    private int currentRate;
    private int health = 350;
    private int posX;
    private int posY;
    private int score;
    private int spawnRate = 60;


    public Home()
    {
        InitializeComponent();
        DataContextChanged += DataContextChangedHandler;

        gameTimer.Tick += GameLoop;
        gameTimer.Interval = TimeSpan.FromMilliseconds(20);

        ValorantStatus = ValorantStatusView;
        AccountStatus = AccountStatusView;
        MatchStatus = MatchStatusView;
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

    private void GameLoop(object sender, EventArgs e) // Heavily modified from https://www.mooict.com/wpf-c-tutorial-create-a-simple-clicking-game-in-visual-studio/
    {
        txtScore.Content = score;
        txtLastScore.Content = Properties.Settings.Default.AimScore;
        currentRate -= 2;

        if (currentRate < 1)
        {
            currentRate = spawnRate;

            posX = rand.Next(15, (int) MyCanvas.ActualWidth - 15);
            posY = rand.Next(50, (int) MyCanvas.ActualHeight - 15);

            var circle = new Ellipse
            {
                Tag = "circle",
                Height = 5,
                Width = 5,
                Fill = new SolidColorBrush(Color.FromRgb(255, 70, 84))
            };

            Canvas.SetLeft(circle, posX);
            Canvas.SetTop(circle, posY);
            MyCanvas.Children.Add(circle);
        }

        foreach (var x in MyCanvas.Children.OfType<Ellipse>())
        {
            x.Height += 0.6;
            x.Width += 0.6;
            x.RenderTransformOrigin = new Point(0.5, 0.5);

            if (!(x.Width > 70)) continue;

            removeThis.Add(x);
            health -= 15;
        }

        if (health > 1)
            healthBar.Width = health;
        else
            GameOverFunction();

        foreach (var i in removeThis)
            MyCanvas.Children.Remove(i);

        spawnRate = score switch
        {
            < 5 => 60,
            < 20 => 50,
            < 35 => 40,
            < 50 => 30,
            < 65 => 20,
            _ => spawnRate
        };
    }

    private void ClickOnCanvas(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not Ellipse) return;
        var circle = (Ellipse) e.OriginalSource;
        MyCanvas.Children.Remove(circle);
        score++;
    }

    private void GameOverFunction()
    {
        gameTimer.Stop();
        foreach (var y in MyCanvas.Children.OfType<Ellipse>())
            removeThis.Add(y);
        foreach (var i in removeThis) MyCanvas.Children.Remove(i);
        spawnRate = 60;
        Properties.Settings.Default.AimScore = score;
        Properties.Settings.Default.Save();
        score = 0;
        currentRate = 5;
        health = 350;
        removeThis.Clear();

        TrainerWindow.Visibility = Visibility.Collapsed;
        Grid.RowDefinitions[1].Height = GridLength.Auto;
        Grid.RowDefinitions[0].Height = new GridLength(0.5, GridUnitType.Star);
        TrainerDialog.Visibility = Visibility.Visible;
    }

    private void StartAimTrainerButton(object sender, RoutedEventArgs e)
    {
        gameTimer.Start();
        currentRate = spawnRate;
        TrainerWindow.Visibility = Visibility.Visible;
        Grid.RowDefinitions[0].Height = GridLength.Auto;
        Grid.RowDefinitions[1].Height = new GridLength(0.5, GridUnitType.Star);
        TrainerDialog.Visibility = Visibility.Collapsed;
    }
}