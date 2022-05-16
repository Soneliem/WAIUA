using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using AutoUpdaterDotNET;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
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

        if (string.IsNullOrEmpty(Settings.Default.Language))
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InstalledUICulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
            Settings.Default.Language = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
            Settings.Default.Save();
        }
        else
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Settings.Default.Language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Language);
        }
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Constants.Log.Error("Unhandeled Exception: {Message}, {Stacktrace}", e.Exception.Message, e.Exception.StackTrace);
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
                //Services
                //.AddSingleton<ISettingsService, SettingsService>()
                // //Page ViewModels
                .AddTransient<HomeViewModel>()
                .AddTransient<InfoViewModel>()
                .AddTransient<NormalmatchViewModel>()
                .AddTransient<SettingsViewModel>()
                //WPF
                .AddSingleton<MainViewModel>()
                //.AddSingleton<IViewFactory>(mappingViewFactory)
                .AddSingleton<IViewFactory>(conventionViewFactory)
                .BuildServiceProvider());

        AutoUpdater.ShowSkipButton = false;
        AutoUpdater.Start("https://raw.githubusercontent.com/Soneliem/WAIUA/master/WAIUA/VersionInfo.xml");

        await CheckAndUpdateJsonAsync().ConfigureAwait(false);
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        Settings.Default.Save();
    }
}