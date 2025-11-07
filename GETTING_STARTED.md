# Getting Started with How Much Longer

## Quick Start Guide

### Running the Application

1. **Build the application**:
   ```bash
   cd "D:\Projects\How Much Longer\HowMuchLonger"
   dotnet build
   dotnet run
   ```

2. **First-time setup**:
   - The app will launch with default settings (9:00 AM - 5:00 PM)
   - Click "Settings" to customize your workday times
   - Configure notification preferences
   - Click "Save"

3. **Using the app**:
   - The main window shows your countdown timer
   - Click "Minimize to Tray" to run in the background
   - Double-click the system tray icon to restore the window
   - Right-click the tray icon for quick access to settings and exit

## Key Features Explained

### Countdown Timer
- **HH:MM:SS**: Main countdown display in hours, minutes, and seconds
- **Total Minutes**: Useful for quick mental calculations
- **Total Seconds**: Exact time remaining

### Notifications

#### Hourly Notifications
- Triggered at the top of each hour
- Example: "3 hours left until end of workday"
- Can be disabled in Settings

#### Milestone Notifications
Three key milestones:
1. **Halfway Point**: When you've completed half your workday
2. **One Hour Left**: One hour before end time
3. **Fifteen Minutes**: Final 15-minute countdown

All can be toggled on/off in Settings.

### Settings Configuration

#### Work Hours
- **Start Time**: When your workday begins (24-hour format)
- **End Time**: When your workday ends (24-hour format)
- Example: 17 = 5:00 PM, 09 = 9:00 AM

#### Notification Toggles
- Enable/disable hourly notifications
- Enable/disable milestone notifications
- Independent controls for maximum flexibility

#### Behavior
- **Minimize to tray on close**: When enabled, closing the window minimizes to tray instead of exiting

## Tips and Tricks

1. **Quick Settings Access**: Right-click the system tray icon for immediate access to settings

2. **Stay Focused**: Minimize to tray and let the app notify you of important milestones

3. **Customize Your Schedule**: If you work non-standard hours, update the start/end times in Settings

4. **Disable Distractions**: Turn off notifications if you prefer to check the countdown manually

5. **Persistent Settings**: Your settings are automatically saved and restored when you restart the app

## Technical Notes

### Settings Location
Settings are stored at: `%AppData%\HowMuchLonger\settings.json`

You can manually edit this file if needed, but it's recommended to use the Settings UI.

### Time Calculations
- The app calculates time until the configured end time each day
- If the current time is past the end time, it shows the end time for the next day
- The countdown updates every second for accuracy

### Notifications
- Uses Windows native toast notifications
- Appears in the Windows Notification Center
- Can be customized through Windows notification settings

## Troubleshooting

### Problem: App doesn't start
**Solution**: Ensure .NET 9 runtime is installed. Download from https://dotnet.microsoft.com/download

### Problem: No notifications appearing
**Solution**:
1. Check Windows notification settings (Settings > System > Notifications)
2. Disable Focus Assist if it's blocking notifications
3. Verify notifications are enabled in the app Settings

### Problem: System tray icon missing
**Solution**:
1. Check if it's in the system tray overflow (click the up arrow in the taskbar)
2. Add a custom icon (see Resources/README.md)
3. Restart the application

### Problem: Wrong countdown time
**Solution**:
1. Open Settings and verify your end time is correct
2. Make sure you're using 24-hour format (17 = 5:00 PM)
3. Save the settings and restart the app if needed

## Building for Distribution

To create a release build:

```bash
cd "D:\Projects\How Much Longer\HowMuchLonger"
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

This creates a standalone executable in:
```
bin\Release\net9.0-windows10.0.19041.0\win-x64\publish\
```

## Next Steps

1. **Customize the UI**: Edit the XAML files to change colors, fonts, and layout
2. **Add Features**: Extend the Services or ViewModels to add new functionality
3. **Create an Icon**: Add a custom icon in the Resources folder
4. **Share**: Distribute the app to colleagues who might find it useful

---

**Ready to start counting down? Run `dotnet run` and enjoy!**
