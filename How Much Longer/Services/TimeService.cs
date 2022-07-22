using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace How_Much_Longer.Services
{
    public class TimeService
    {
        private Timer _timer;

        public event Action<TimeSpan> Tick;

        public TimeService()
        {
            SetTimer();
        }

        private void SetTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            var remaining = DateTime.Today.AddHours(17).Subtract(DateTime.Now);
            Tick?.Invoke(remaining);
        }
    }
}
