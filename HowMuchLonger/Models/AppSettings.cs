namespace HowMuchLonger.Models;

/// <summary>
/// Application settings that persist between sessions
/// </summary>
public class AppSettings
{
    /// <summary>
    /// End of workday time (e.g., 17:00 for 5:00 PM)
    /// </summary>
    public TimeSpan EndOfWorkday { get; set; } = new TimeSpan(17, 0, 0); // Default: 5:00 PM

    /// <summary>
    /// Enable hourly notifications
    /// </summary>
    public bool EnableHourlyNotifications { get; set; } = true;

    /// <summary>
    /// Enable milestone notifications (halfway, 1 hour, 15 minutes)
    /// </summary>
    public bool EnableMilestoneNotifications { get; set; } = true;

    /// <summary>
    /// Total work duration for calculating halfway point
    /// </summary>
    public TimeSpan WorkDuration { get; set; } = new TimeSpan(8, 0, 0); // Default: 8 hours

    /// <summary>
    /// Start of workday time (calculated or set)
    /// </summary>
    public TimeSpan StartOfWorkday { get; set; } = new TimeSpan(9, 0, 0); // Default: 9:00 AM

    /// <summary>
    /// Whether to minimize to tray on close
    /// </summary>
    public bool MinimizeToTrayOnClose { get; set; } = true;
}
