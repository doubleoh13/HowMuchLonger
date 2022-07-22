using System;
using How_Much_Longer.Services;
using How_Much_Longer.Stores;

namespace How_Much_Longer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private NavigationStore _navigationStore;
        private readonly TimeService _timeService;
        private readonly NotificationService _notificationService;
        private readonly NavigationService<CountdownViewModel> _countdownViewNavigationService;
        private readonly NavigationService<AfterHoursViewModel> _afterHoursNavigationService;

        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        public MainViewModel(
            NavigationStore navigationStore,
            TimeService timeService,
            NotificationService notificationService,
            NavigationService<CountdownViewModel> countdownViewNavigationService,
            NavigationService<AfterHoursViewModel> afterHoursNavigationService)
        {
            _navigationStore = navigationStore;
            _timeService = timeService;
            _notificationService = notificationService;
            _countdownViewNavigationService = countdownViewNavigationService;
            _afterHoursNavigationService = afterHoursNavigationService;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _timeService.Tick += OnTick;
        }

        private void OnTick(System.TimeSpan remaining)
        {
            if (DateTime.Now.Hour >= 17 && CurrentViewModel is not AfterHoursViewModel)
            {
                _afterHoursNavigationService.Navigate();
            }

            if (DateTime.Now.Hour < 17 && CurrentViewModel is not CountdownViewModel)
            {
                _countdownViewNavigationService.Navigate();
            }
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}