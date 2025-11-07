using System.Windows.Threading;
using HowMuchLonger.Models;

namespace HowMuchLonger.Services;

/// <summary>
/// Service for managing the countdown timer logic
/// </summary>
public class CountdownService : IDisposable
{
    private readonly DispatcherTimer _timer;
    private AppSettings _settings;

    public event EventHandler<CountdownState>? CountdownUpdated;
    public event EventHandler<TimeSpan>? HourlyNotificationTriggered;
    public event EventHandler<string>? MilestoneNotificationTriggered;

    private int _lastHourNotified = -1;
    private bool _halfwayNotified = false;
    private bool _oneHourNotified = false;
    private bool _fifteenMinutesNotified = false;

    public CountdownService(AppSettings settings)
    {
        _settings = settings;
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += OnTimerTick;
    }

    public void UpdateSettings(AppSettings settings)
    {
        _settings = settings;
        ResetNotificationFlags();
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        var state = CalculateCurrentState();
        CountdownUpdated?.Invoke(this, state);

        CheckAndTriggerNotifications(state);
    }

    private CountdownState CalculateCurrentState()
    {
        var now = DateTime.Now;
        var endTimeToday = DateTime.Today.Add(_settings.EndOfWorkday);

        // If end time has passed, it might be for tomorrow
        if (now > endTimeToday)
        {
            endTimeToday = endTimeToday.AddDays(1);
        }

        var timeRemaining = endTimeToday - now;

        return new CountdownState
        {
            CurrentTime = now,
            EndTime = endTimeToday,
            TimeRemaining = timeRemaining
        };
    }

    private void CheckAndTriggerNotifications(CountdownState state)
    {
        if (state.IsWorkdayOver)
            return;

        // Hourly notifications
        if (_settings.EnableHourlyNotifications)
        {
            var currentHour = (int)state.TimeRemaining.TotalHours;
            if (currentHour > 0 && currentHour != _lastHourNotified && state.TimeRemaining.Minutes == 0)
            {
                _lastHourNotified = currentHour;
                HourlyNotificationTriggered?.Invoke(this, state.TimeRemaining);
            }
        }

        // Milestone notifications
        if (_settings.EnableMilestoneNotifications)
        {
            // Halfway point
            var halfwayPoint = _settings.WorkDuration.TotalMinutes / 2;
            if (!_halfwayNotified && state.TotalMinutesRemaining <= halfwayPoint &&
                state.TotalMinutesRemaining >= halfwayPoint - 1)
            {
                _halfwayNotified = true;
                MilestoneNotificationTriggered?.Invoke(this, "Halfway there! 🎯");
            }

            // 1 hour left
            if (!_oneHourNotified && state.TotalMinutesRemaining <= 60 && state.TotalMinutesRemaining > 59)
            {
                _oneHourNotified = true;
                MilestoneNotificationTriggered?.Invoke(this, "Just 1 hour left! ⏰");
            }

            // 15 minutes left
            if (!_fifteenMinutesNotified && state.TotalMinutesRemaining <= 15 && state.TotalMinutesRemaining > 14)
            {
                _fifteenMinutesNotified = true;
                MilestoneNotificationTriggered?.Invoke(this, "Only 15 minutes to go! 🏁");
            }
        }
    }

    private void ResetNotificationFlags()
    {
        _lastHourNotified = -1;
        _halfwayNotified = false;
        _oneHourNotified = false;
        _fifteenMinutesNotified = false;
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
    }
}
