using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HowMuchLonger.Models;
using HowMuchLonger.Services;

namespace HowMuchLonger.ViewModels;

/// <summary>
/// ViewModel for the main countdown window
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly CountdownService _countdownService;
    private readonly NotificationService _notificationService;
    private readonly SettingsService _settingsService;

    [ObservableProperty]
    private string _formattedTime = "00:00:00";

    [ObservableProperty]
    private int _totalMinutesRemaining;

    [ObservableProperty]
    private int _totalSecondsRemaining;

    [ObservableProperty]
    private bool _isOvertime;

    [ObservableProperty]
    private string _statusMessage = "Time until end of workday";

    public AppSettings Settings { get; private set; }

    public event EventHandler? MinimizeToTrayRequested;
    public event EventHandler? SettingsRequested;

    public MainViewModel(
        CountdownService countdownService,
        NotificationService notificationService,
        SettingsService settingsService)
    {
        _countdownService = countdownService;
        _notificationService = notificationService;
        _settingsService = settingsService;

        Settings = _settingsService.LoadSettings();

        // Subscribe to countdown events
        _countdownService.CountdownUpdated += OnCountdownUpdated;
        _countdownService.HourlyNotificationTriggered += OnHourlyNotification;
        _countdownService.MilestoneNotificationTriggered += OnMilestoneNotification;

        // Start the countdown
        _countdownService.Start();
    }

    [RelayCommand]
    private void MinimizeToTray()
    {
        MinimizeToTrayRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void OpenSettings()
    {
        SettingsRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Exit()
    {
        _countdownService.Stop();
        System.Windows.Application.Current.Shutdown();
    }

    public void RefreshSettings()
    {
        Settings = _settingsService.LoadSettings();
        _countdownService.UpdateSettings(Settings);
    }

    private void OnCountdownUpdated(object? sender, CountdownState state)
    {
        FormattedTime = state.FormattedTime;
        TotalMinutesRemaining = state.TotalMinutesRemaining;
        TotalSecondsRemaining = state.TotalSecondsRemaining;
        IsOvertime = state.IsOvertime;

        StatusMessage = state.IsOvertime
            ? "You're in overtime! 🔥"
            : "Time until end of workday";

        // Show notification when workday ends
        if (state.IsWorkdayOver && !IsOvertime)
        {
            _notificationService.ShowWorkdayEndedNotification();
        }
    }

    private void OnHourlyNotification(object? sender, TimeSpan timeRemaining)
    {
        _notificationService.ShowHourlyNotification(timeRemaining);
    }

    private void OnMilestoneNotification(object? sender, string message)
    {
        _notificationService.ShowMilestoneNotification(message);
    }
}
