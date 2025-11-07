using System.Windows;
using HowMuchLonger.ViewModels;
using HowMuchLonger.Views;

namespace HowMuchLonger;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        // Subscribe to ViewModel events
        _viewModel.MinimizeToTrayRequested += OnMinimizeToTrayRequested;
        _viewModel.SettingsRequested += OnSettingsRequested;

        // Handle window state changes
        StateChanged += OnStateChanged;
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        if (_viewModel.Settings.MinimizeToTrayOnClose)
        {
            e.Cancel = true;
            MinimizeToTray();
        }
        else
        {
            base.OnClosing(e);
        }
    }

    private void OnMinimizeToTrayRequested(object? sender, EventArgs e)
    {
        MinimizeToTray();
    }

    private void OnSettingsRequested(object? sender, EventArgs e)
    {
        var settingsWindow = ((App)Application.Current).GetSettingsWindow();
        settingsWindow.Owner = this;
        settingsWindow.ShowDialog();
        _viewModel.RefreshSettings();
    }

    private void OnStateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized && _viewModel.Settings.MinimizeToTrayOnClose)
        {
            MinimizeToTray();
        }
    }

    private void MinimizeToTray()
    {
        Hide();
        TrayIcon.Visibility = Visibility.Visible;
    }

    private void ShowWindow()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    // Tray icon event handlers
    private void TrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
    {
        ShowWindow();
    }

    private void ShowWindow_Click(object sender, RoutedEventArgs e)
    {
        ShowWindow();
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        ShowWindow();
        OnSettingsRequested(this, EventArgs.Empty);
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}