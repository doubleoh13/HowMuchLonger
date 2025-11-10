using Microsoft.Toolkit.Uwp.Notifications;

namespace HowMuchLonger.Services;

/// <summary>
/// Service for displaying Windows toast notifications
/// </summary>
public class NotificationService
{
    public void ShowHourlyNotification(TimeSpan timeRemaining)
    {
        try
        {
            var hours = (int)timeRemaining.TotalHours;
            var hourText = hours == 1 ? "hour" : "hours";

            new ToastContentBuilder()
                .AddText("How Much Longer")
                .AddText($"{hours} {hourText} left until end of workday")
                .Show();

            System.Diagnostics.Debug.WriteLine($"Hourly notification sent: {hours} {hourText} remaining");
        }
        catch (Exception ex)
        {
            // Log but don't crash the app if notifications fail
            System.Diagnostics.Debug.WriteLine($"Failed to show hourly notification: {ex.Message}");
        }
    }

    public void ShowMilestoneNotification(string message)
    {
        try
        {
            new ToastContentBuilder()
                .AddText("How Much Longer")
                .AddText(message)
                .Show();

            System.Diagnostics.Debug.WriteLine($"Milestone notification sent: {message}");
        }
        catch (Exception ex)
        {
            // Log but don't crash the app if notifications fail
            System.Diagnostics.Debug.WriteLine($"Failed to show milestone notification: {ex.Message}");
        }
    }

    public void ShowWorkdayEndedNotification()
    {
        try
        {
            new ToastContentBuilder()
                .AddText("How Much Longer")
                .AddText("Workday is over! Time to go home!")
                .Show();

            System.Diagnostics.Debug.WriteLine("Workday ended notification sent");
        }
        catch (Exception ex)
        {
            // Log but don't crash the app if notifications fail
            System.Diagnostics.Debug.WriteLine($"Failed to show workday ended notification: {ex.Message}");
        }
    }
}
