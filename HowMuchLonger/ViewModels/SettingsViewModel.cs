using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HowMuchLonger.Models;
using HowMuchLonger.Services;

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
}
