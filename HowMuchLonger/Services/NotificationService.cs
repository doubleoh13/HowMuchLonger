using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

namespace HowMuchLonger.Services;

/// <summary>
/// Service for displaying Windows toast notifications
/// </summary>
public class NotificationService
{
    public void ShowHourlyNotification(TimeSpan timeRemaining)
    {
        var hours = (int)timeRemaining.TotalHours;
        var hourText = hours == 1 ? "hour" : "hours";

        var toast = new ToastContentBuilder()
            .AddText("How Much Longer")
            .AddText($"{hours} {hourText} left until end of workday")
            .GetToastContent();

        var toastNotification = new ToastNotification(toast.GetXml());
        ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
    }

    public void ShowMilestoneNotification(string message)
    {
        var toast = new ToastContentBuilder()
            .AddText("How Much Longer")
            .AddText(message)
            .GetToastContent();

        var toastNotification = new ToastNotification(toast.GetXml());
        ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
    }

    public void ShowWorkdayEndedNotification()
    {
        var toast = new ToastContentBuilder()
            .AddText("How Much Longer")
            .AddText("Workday is over! Time to go home!")
            .GetToastContent();

        var toastNotification = new ToastNotification(toast.GetXml());
        ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
    }
}
