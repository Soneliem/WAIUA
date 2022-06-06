using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using AutoUpdaterDotNET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using RestoreWindowPlace;
using Serilog;
using WAIUA.Helpers;
using WAIUA.Properties;
using WAIUA.ViewModels;
using static WAIUA.Helpers.ValApi;

namespace WAIUA;

public partial class App : Application
{
    public App()
    {
        Dispatcher.UnhandledException += OnDispatcherUnhandledException;

        WindowPlace = new WindowPlace("placement.config");

        if (string.IsNullOrEmpty(Settings.Default.Language))
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InstalledUICulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
            Settings.Default.Language = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
        }
        else
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Settings.Default.Language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Language);
        }

        Settings.Default.Save();
    }

    public WindowPlace WindowPlace { get; }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Constants.Log.Error("Unhandled Exception: {Message}, {Stacktrace}", e.Exception.Message, e.Exception.StackTrace);
        e.Handled = true;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Constants.LocalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WAIUA";
        Constants.Log = new LoggerConfiguration()
            .WriteTo.Async(a => a.File(Constants.LocalAppDataPath + "\\logs\\log.txt", shared: true, rollingInterval: RollingInterval.Day))
            .CreateLogger();
        Constants.Log.Information("Application Start");

        var conventionViewFactory = new NamingConventionViewFactory();

        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddTransient<HomeViewModel>()
                .AddTransient<InfoViewModel>()
                .AddTransient<MatchViewModel>()
                .AddTransient<SettingsViewModel>()
                .AddSingleton<MainViewModel>()
                .AddSingleton<IViewFactory>(conventionViewFactory)
                .BuildServiceProvider());

        AutoUpdater.ShowSkipButton = false;
        AutoUpdater.Start("https://raw.githubusercontent.com/Soneliem/WAIUA/master/WAIUA/VersionInfo.xml");

        await CheckAndUpdateJsonAsync().ConfigureAwait(false);
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        Settings.Default.Save();
        WindowPlace.Save();
    }
}