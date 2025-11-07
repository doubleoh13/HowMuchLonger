namespace HowMuchLonger.Models;

/// <summary>
/// Current state of the countdown timer
/// </summary>
public class CountdownState
{
    /// <summary>
    /// Time remaining until end of workday
    /// </summary>
    public TimeSpan TimeRemaining { get; set; }

    /// <summary>
    /// Total minutes remaining
    /// </summary>
    public int TotalMinutesRemaining => (int)TimeRemaining.TotalMinutes;

    /// <summary>
    /// Total seconds remaining
    /// </summary>
    public int TotalSecondsRemaining => (int)TimeRemaining.TotalSeconds;

    /// <summary>
    /// Formatted time string (HH:MM:SS)
    /// </summary>
    public string FormattedTime => TimeRemaining.TotalSeconds >= 0
        ? $"{(int)TimeRemaining.TotalHours:D2}:{TimeRemaining.Minutes:D2}:{TimeRemaining.Seconds:D2}"
        : "00:00:00";

    /// <summary>
    /// Whether the workday has ended
    /// </summary>
    public bool IsWorkdayOver => TimeRemaining.TotalSeconds <= 0;

    /// <summary>
    /// Whether we're in overtime (past end of workday)
    /// </summary>
    public bool IsOvertime => TimeRemaining.TotalSeconds < 0;

    /// <summary>
    /// Current time
    /// </summary>
    public DateTime CurrentTime { get; set; } = DateTime.Now;

    /// <summary>
    /// End time for today
    /// </summary>
    public DateTime EndTime { get; set; }
}
