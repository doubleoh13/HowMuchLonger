using System;
using How_Much_Longer.Services;

namespace How_Much_Longer.ViewModels
{
    public class CountdownViewModel : ViewModelBase
    {
        private readonly TimeService _timeService;
        private string _hoursLeft = "0:00";

        public string HoursLeft
        {
            get => _hoursLeft;
            set
            {
                _hoursLeft = value;
                OnPropertyChanged(nameof(HoursLeft));
            }
        }

        private string _minutesLeft = "00000";

        public string MinutesLeft
        {
            get => _minutesLeft;
            set
            {
                _minutesLeft = value;
                OnPropertyChanged(nameof(MinutesLeft));
            }
        }

        public string _secondsLeft = "000000";

        public string SecondsLeft
        {
            get => _secondsLeft;
            set
            {
                _secondsLeft = value;
                OnPropertyChanged(nameof(SecondsLeft));
            }
        }

        public CountdownViewModel(TimeService timeService)
        {
            _timeService = timeService;
            _timeService.Tick += OnTick;
        }

        private void OnTick(TimeSpan remaining)
        {
            HoursLeft = $"{remaining.Hours:D2}:{remaining.Minutes.ToString("D2")}:{remaining.Seconds.ToString("D2")}";
            MinutesLeft = Convert.ToInt32(remaining.TotalMinutes).ToString("D5");
            SecondsLeft = Convert.ToInt32(remaining.TotalSeconds).ToString("D6");
        }

        public override void Dispose()
        {
            _timeService.Tick -= OnTick;
            base.Dispose();
        }
    }
}
