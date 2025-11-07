using System.Windows;
using HowMuchLonger.Models;
using HowMuchLonger.Services;
using HowMuchLonger.ViewModels;
using HowMuchLonger.Views;
using Velopack;

namespace HowMuchLonger;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private SettingsService? _settingsService;
    private CountdownService? _countdownService;
    private NotificationService? _notificationService;

    /// <summary>
    /// Custom Main method with Velopack bootstrap for updates
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            // Velopack will handle app updates before the main app runs
            VelopackApp.Build().Run();
        }
        catch (Exception ex)
        {
            // If Velopack initialization fails, log but continue
            System.Diagnostics.Debug.WriteLine($"Velopack initialization error: {ex.Message}");
        }

        // Start WPF application
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // Check for updates on startup (runs in background)
        _ = Task.Run(async () =>
        {
            try
            {
                await CheckForUpdatesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update check failed: {ex.Message}");
            }
        });

        // Initialize services
        _settingsService = new SettingsService();
        var settings = _settingsService.LoadSettings();

        _notificationService = new NotificationService();
        _countdownService = new CountdownService(settings);

        // Create main ViewModel
        var mainViewModel = new MainViewModel(_countdownService, _notificationService, _settingsService);

        // Create and show main window
        var mainWindow = new MainWindow(mainViewModel);
        mainWindow.Show();
    }

    /// <summary>
    /// Check for updates and apply them if available
    /// </summary>
    private async Task CheckForUpdatesAsync()
    {
        try
        {
            // Get the update manager
            // Updates are hosted on GitHub Releases
            var mgr = new UpdateManager("https://github.com/doubleoh13/HowMuchLonger/releases/latest/download");

            // Check for updates
            var updateInfo = await mgr.CheckForUpdatesAsync();

            if (updateInfo == null)
            {
                // No updates available
                return;
            }

            // Download the update in the background
            await mgr.DownloadUpdatesAsync(updateInfo);

            // Notify user that update is ready
            Dispatcher.Invoke(() =>
            {
                var result = MessageBox.Show(
                    $"Version {updateInfo.TargetFullRelease.Version} is ready to install.\n\n" +
                    "The update will be applied when you restart the application.\n\n" +
                    "Would you like to restart now?",
                    "Update Available",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    // Apply updates and restart
                    mgr.ApplyUpdatesAndRestart(updateInfo);
                }
            });
        }
        catch (Exception ex)
        {
            // Silently fail - don't interrupt user experience for update failures
            System.Diagnostics.Debug.WriteLine($"Update check error: {ex.Message}");
        }
    }

    public SettingsWindow GetSettingsWindow()
    {
        if (_settingsService == null)
            throw new InvalidOperationException("Settings service not initialized");

        var settingsViewModel = new SettingsViewModel(_settingsService);
        return new SettingsWindow(settingsViewModel);
    }
}

