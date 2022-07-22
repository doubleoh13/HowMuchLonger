using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using How_Much_Longer.Services;
using How_Much_Longer.Stores;
using How_Much_Longer.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace How_Much_Longer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<NavigationStore>();
                    services.AddSingleton<TimeService>();
                    services.AddSingleton<NotificationService>();

                    services.AddSingleton<NavigationService<CountdownViewModel>>();
                    services.AddTransient<CountdownViewModel>();
                    services.AddSingleton<Func<CountdownViewModel>>(s =>
                        () => s.GetRequiredService<CountdownViewModel>());

                    services.AddSingleton<NavigationService<AfterHoursViewModel>>();
                    services.AddTransient<AfterHoursViewModel>();
                    services.AddSingleton<Func<AfterHoursViewModel>>(s =>
                        () => s.GetRequiredService<AfterHoursViewModel>());

                    
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>(s => new MainWindow()
                    {
                        DataContext = s.GetRequiredService<MainViewModel>()
                    });
                })
                .Build();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            // var navigationService = _host.Services.GetRequiredService<NavigationService<CountdownViewModel>>();
            // navigationService.Navigate();

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.Dispose();

            base.OnExit(e);
        }
    }
}
