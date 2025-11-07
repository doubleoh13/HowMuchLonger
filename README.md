# How Much Longer

A Windows desktop application that counts down to the end of your workday with real-time updates and configurable notifications.

## Features

- **Real-time Countdown**: Displays remaining time in HH:MM:SS format
- **Multiple Time Views**: Shows total minutes and total seconds remaining
- **System Tray Integration**: Minimize to system tray and run in the background
- **Automatic Updates**: Built-in auto-update system powered by Velopack
- **Toast Notifications**:
  - Hourly notifications (e.g., "3 hours left")
  - Milestone notifications (halfway point, 1 hour left, 15 minutes left)
  - All notifications are configurable
- **Customizable Settings**:
  - Set your workday start and end times
  - Toggle notifications on/off
  - Configure minimize-to-tray behavior
- **Clean, Modern UI**: Built with WPF and modern design principles
- **Easy Distribution**: Professional installer with automatic updates

## Requirements

- Windows 10 version 1809 (Build 17763) or later
- .NET 9 Runtime

## Installation

### For End Users

Download and run the installer from the [Releases](https://github.com/doubleoh13/HowMuchLonger/releases) page.

The app includes automatic updates - you'll be notified when new versions are available.

### For Developers

1. Clone or download this repository
2. Open the solution in Visual Studio 2022 or JetBrains Rider
3. Build the solution (Ctrl+Shift+B in Visual Studio)
4. Run the application

## Building from Source

```bash
# Navigate to the project directory
cd "D:\Projects\How Much Longer\HowMuchLonger"

# Build the project
dotnet build

# Run the application
dotnet run
```

## Usage

### First Time Setup

1. Launch the application
2. Click the "Settings" button
3. Configure your workday times:
   - Set your start time (default: 09:00)
   - Set your end time (default: 17:00)
4. Enable or disable notifications as desired
5. Click "Save"

### Main Window

The main window displays:
- Current countdown timer (HH:MM:SS)
- Total minutes remaining
- Total seconds remaining
- Status message

### System Tray

- **Minimize to Tray**: Click the "Minimize to Tray" button or minimize the window
- **Restore Window**: Double-click the system tray icon
- **Context Menu**: Right-click the tray icon for quick actions

## Project Structure

```
HowMuchLonger/
├── Models/
│   ├── AppSettings.cs          # Application settings model
│   └── CountdownState.cs       # Countdown state model
├── Services/
│   ├── CountdownService.cs     # Timer and countdown logic
│   ├── NotificationService.cs  # Windows toast notifications
│   └── SettingsService.cs      # Settings persistence
├── ViewModels/
│   ├── MainViewModel.cs        # Main window view model
│   └── SettingsViewModel.cs    # Settings window view model
├── Views/
│   └── SettingsWindow.xaml     # Settings dialog
├── MainWindow.xaml             # Main application window
├── App.xaml                    # Application startup and DI
└── Resources/
    └── README.md               # Icon resource instructions
```

## Architecture

The application follows the **MVVM (Model-View-ViewModel)** pattern:

- **Models**: Data structures for settings and state
- **Services**: Business logic for countdown, notifications, and persistence
- **ViewModels**: Presentation logic using CommunityToolkit.MVVM
- **Views**: XAML-based user interfaces

## Dependencies

- **CommunityToolkit.Mvvm** (8.4.0): MVVM framework with source generators
- **Hardcodet.NotifyIcon.Wpf** (2.0.1): System tray icon support
- **Microsoft.Toolkit.Uwp.Notifications** (7.1.3): Windows toast notifications
- **Velopack** (0.0.1298): Installer and automatic update framework

## Configuration

Settings are stored in JSON format at:
```
%AppData%\HowMuchLonger\settings.json
```

Example settings file:
```json
{
  "EndOfWorkday": "17:00:00",
  "EnableHourlyNotifications": true,
  "EnableMilestoneNotifications": true,
  "WorkDuration": "08:00:00",
  "StartOfWorkday": "09:00:00",
  "MinimizeToTrayOnClose": true
}
```

## Customization

### Adding a Custom Icon

1. Create or obtain a 256x256 pixel icon (.ico format)
2. Save it as `app.ico` in the `Resources` folder
3. Update `MainWindow.xaml` line 113 to uncomment the IconSource:
   ```xml
   IconSource="pack://application:,,,/Resources/app.ico"
   ```

### Modifying Notification Intervals

Edit `CountdownService.cs` in the `Services` folder to customize:
- Hourly notification timing
- Milestone thresholds
- Custom notification messages

## Troubleshooting

### Notifications Not Showing

1. Ensure Windows notifications are enabled:
   - Settings > System > Notifications
2. Check Focus Assist settings
3. Verify notification permissions for the app

### Application Won't Start

1. Ensure .NET 9 runtime is installed
2. Check Windows version (must be 1809 or later)
3. Try running as administrator

### System Tray Icon Not Visible

1. Check Windows system tray settings
2. Ensure the icon isn't hidden in the overflow area
3. Add a custom icon (see Customization section)

## Building an Installer

To create a distributable installer with auto-update support:

```powershell
# Build installer (creates Setup.exe in Releases folder)
.\build-installer.ps1

# Or with a specific version
.\build-installer.ps1 -Version "1.2.0"
```

For complete deployment instructions, see [DEPLOYMENT.md](DEPLOYMENT.md).

## Future Enhancements

Potential features for future versions:
- Custom notification sounds
- Multiple workday profiles
- Break time tracking
- Weekly work hour statistics
- Dark mode theme
- Manual check for updates button

## License

This project is open source. Feel free to modify and distribute as needed.

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## Support

For issues or questions:
1. Check the Troubleshooting section
2. Review the project documentation
3. Open an issue on the project repository

---

**Enjoy your countdown to freedom!**
