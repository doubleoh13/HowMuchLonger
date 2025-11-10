using System.IO;
using System.Windows;
using HowMuchLonger.Models;
using HowMuchLonger.Services;
using HowMuchLonger.ViewModels;
using HowMuchLonger.Views;
using Microsoft.Toolkit.Uwp.Notifications;
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
        // Register COM server and activator type for toast notifications
        ToastNotificationManagerCompat.OnActivated += toastArgs =>
        {
            // Handle notification activation if needed
        };

        // Register app for toast notifications
        RegisterAppForNotifications();

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
    /// Register the app for toast notifications by creating a Start Menu shortcut
    /// </summary>
    private void RegisterAppForNotifications()
    {
        try
        {
            const string appId = "HowMuchLonger.CountdownApp";

            // Get Start Menu programs folder
            string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            string shortcutPath = Path.Combine(startMenuPath, "How Much Longer.lnk");

            // Only create if it doesn't exist
            if (!File.Exists(shortcutPath))
            {
                // Get the executable path
                string exePath = Environment.ProcessPath ?? System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;

                if (!string.IsNullOrEmpty(exePath))
                {
                    // Create the shortcut using Windows Script Host
                    var shellType = Type.GetTypeFromProgID("WScript.Shell");
                    if (shellType != null)
                    {
                        dynamic? shell = Activator.CreateInstance(shellType);
                        if (shell != null)
                        {
                            var shortcut = shell.CreateShortcut(shortcutPath);
                            shortcut.TargetPath = exePath;
                            shortcut.WorkingDirectory = Path.GetDirectoryName(exePath);
                            shortcut.Description = "How Much Longer - Workday Countdown";

                            // Set the AppUserModelId for toast notifications
                            try
                            {
                                var propertyStore = (dynamic)shortcut;
                                propertyStore.SetProperty("System.AppUserModel.ID", appId);
                            }
                            catch
                            {
                                // Property setting might not work in all scenarios
                            }

                            shortcut.Save();
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(shortcut);
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(shell);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Don't crash if shortcut creation fails
            System.Diagnostics.Debug.WriteLine($"Failed to register for notifications: {ex.Message}");
        }
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

