using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HowMuchLonger.Models;
using HowMuchLonger.Services;
using Velopack;

namespace HowMuchLonger.ViewModels;

/// <summary>
/// ViewModel for the settings window
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;

    [ObservableProperty]
    private int _endHour = 17;

    [ObservableProperty]
    private int _endMinute = 0;

    [ObservableProperty]
    private int _startHour = 9;

    [ObservableProperty]
    private int _startMinute = 0;

    [ObservableProperty]
    private bool _enableHourlyNotifications = true;

    [ObservableProperty]
    private bool _enableMilestoneNotifications = true;

    [ObservableProperty]
    private bool _minimizeToTrayOnClose = true;

    [ObservableProperty]
    private bool _isCheckingForUpdates = false;

    [ObservableProperty]
    private string _updateStatusMessage = string.Empty;

    public event EventHandler? SettingsSaved;

    public SettingsViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;
        LoadCurrentSettings();
    }

    private void LoadCurrentSettings()
    {
        var settings = _settingsService.LoadSettings();

        EndHour = settings.EndOfWorkday.Hours;
        EndMinute = settings.EndOfWorkday.Minutes;
        StartHour = settings.StartOfWorkday.Hours;
        StartMinute = settings.StartOfWorkday.Minutes;
        EnableHourlyNotifications = settings.EnableHourlyNotifications;
        EnableMilestoneNotifications = settings.EnableMilestoneNotifications;
        MinimizeToTrayOnClose = settings.MinimizeToTrayOnClose;
    }

    [RelayCommand]
    private void Save()
    {
        var settings = new AppSettings
        {
            EndOfWorkday = new TimeSpan(EndHour, EndMinute, 0),
            StartOfWorkday = new TimeSpan(StartHour, StartMinute, 0),
            WorkDuration = new TimeSpan(EndHour, EndMinute, 0) - new TimeSpan(StartHour, StartMinute, 0),
            EnableHourlyNotifications = EnableHourlyNotifications,
            EnableMilestoneNotifications = EnableMilestoneNotifications,
            MinimizeToTrayOnClose = MinimizeToTrayOnClose
        };

        _settingsService.SaveSettings(settings);
        SettingsSaved?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Cancel()
    {
        LoadCurrentSettings();
    }

    [RelayCommand]
    private async Task CheckForUpdates()
    {
        IsCheckingForUpdates = true;
        UpdateStatusMessage = "Checking for updates...";

        try
        {
            var mgr = new UpdateManager("https://github.com/doubleoh13/HowMuchLonger/releases/latest/download");

            // Check for updates
            var updateInfo = await mgr.CheckForUpdatesAsync();

            if (updateInfo == null)
            {
                UpdateStatusMessage = "You're up to date!";
                MessageBox.Show(
                    "You are running the latest version of How Much Longer.",
                    "No Updates Available",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            UpdateStatusMessage = $"Downloading version {updateInfo.TargetFullRelease.Version}...";

            // Download the update
            await mgr.DownloadUpdatesAsync(updateInfo);

            UpdateStatusMessage = "Update ready to install!";

            // Prompt user to restart
            var result = MessageBox.Show(
                $"Version {updateInfo.TargetFullRelease.Version} has been downloaded and is ready to install.\n\n" +
                "The update will be applied when you restart the application.\n\n" +
                "Would you like to restart now?",
                "Update Ready",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                // Apply updates and restart
                mgr.ApplyUpdatesAndRestart(updateInfo);
            }
            else
            {
                UpdateStatusMessage = "Update will be installed on next restart";
            }
        }
        catch (Exception ex)
        {
            UpdateStatusMessage = "Update check failed";
            MessageBox.Show(
                $"Failed to check for updates:\n\n{ex.Message}",
                "Update Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            IsCheckingForUpdates = false;
            // Clear status message after a delay
            _ = Task.Run(async () =>
            {
                await Task.Delay(5000);
                UpdateStatusMessage = string.Empty;
            });
        }
    }
}
