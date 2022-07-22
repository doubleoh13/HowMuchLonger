using System;
using Microsoft.Toolkit.Uwp.Notifications;

namespace How_Much_Longer.Services
{
    public class NotificationService
    {
        private TimeService _timeService;

        public NotificationService(TimeService timeService)
        {
            _timeService = timeService;
            _timeService.Tick += OnTick;
        }

        private void OnTick(TimeSpan remaining)
        {
            if (remaining.Seconds != 0) return;

            if (Convert.ToInt32(remaining.TotalSeconds) == 0)
            {
                SendToast("The day is over! GO HOME!!");
                return;
            }

            if (remaining.Minutes == 0) SendToast($"Only {remaining.Hours} Hours Left!");

            if(remaining.Hours == 1 && remaining.Minutes == 9) SendToast("69 Minutes Left!!");

            if(remaining.Hours == 0 && remaining.Minutes == 40) SendToast("4:20!");
        }

        private void SendToast(string message)
        {
            new ToastContentBuilder()
                .AddText(message)
                .Show(toast =>
                {
                    toast.ExpirationTime = DateTime.Now.AddMinutes(10);
                });
        }
    }
}
