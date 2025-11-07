# Velopack Integration Summary

## What Was Added

Velopack has been successfully integrated into the How Much Longer application to provide:
- Professional Windows installer
- Automatic update checking
- Background update downloads
- Delta updates (only downloads changes)
- User-friendly update notifications

## Files Modified

### 1. HowMuchLonger.csproj
**Changes:**
- Added `<StartupObject>` to specify custom entry point
- Added version properties (`Version`, `AssemblyVersion`, `FileVersion`)
- Changed `App.xaml` from `ApplicationDefinition` to `Page`
- Added Velopack NuGet package reference

**Location:** `HowMuchLonger/HowMuchLonger.csproj`

### 2. App.xaml.cs
**Changes:**
- Added custom `Main()` method with Velopack bootstrap
- Added `CheckForUpdatesAsync()` method
- Added update checking on application startup
- Implemented update notification dialog

**Location:** `HowMuchLonger/App.xaml.cs`

**Important:** Line 82 contains the update URL - **you must change this** to your actual update server URL before deploying.

## Files Created

### Build Scripts

1. **build-installer.ps1**
   - PowerShell script to build and package the application
   - Handles version numbering
   - Creates installer in `Releases/` folder
   - Location: Project root

2. **build-installer.bat**
   - Batch wrapper for PowerShell script
   - Location: Project root

### Documentation

1. **DEPLOYMENT.md**
   - Comprehensive deployment guide
   - Covers building, testing, and deploying updates
   - Includes troubleshooting section

2. **VELOPACK_QUICKSTART.md**
   - Quick reference guide
   - Common commands and workflows
   - Testing procedures

3. **VELOPACK_INTEGRATION_SUMMARY.md**
   - This file
   - Overview of integration changes

## How It Works

### Application Startup

1. `Main()` method runs before WPF initialization
2. Velopack checks if an update needs to be applied
3. If update pending, applies it and relaunches
4. Otherwise, continues to normal WPF startup
5. Background thread checks for new updates

### Update Process

1. **Check**: App contacts update server on startup
2. **Download**: New version downloads in background
3. **Notify**: User sees dialog when download complete
4. **Install**: On restart, Velopack applies update
5. **Verify**: App launches with new version

### Update Server Structure

```
https://yourdomain.com/releases/
├── RELEASES                             # Manifest file
├── HowMuchLonger-1.0.0-win-full.nupkg
├── HowMuchLonger-1.0.0-win-Setup.exe
├── HowMuchLonger-1.1.0-win-full.nupkg
├── HowMuchLonger-1.1.0-win-delta.nupkg  # Only changed files
└── HowMuchLonger-1.1.0-win-Setup.exe
```

## Configuration Required

### Before First Release

**CRITICAL:** Update the update URL in `App.xaml.cs` line 82:

```csharp
// Current (placeholder):
var mgr = new UpdateManager("https://yourdomain.com/releases");

// Change to your actual URL:
var mgr = new UpdateManager("https://github.com/doubleoh13/HowMuchLonger/releases/latest/download");
// OR
var mgr = new UpdateManager("https://updates.yoursite.com/howmuchlonger");
```

### Version Updates

For each new release, update `HowMuchLonger.csproj`:

```xml
<Version>1.1.0</Version>
<AssemblyVersion>1.1.0.0</AssemblyVersion>
<FileVersion>1.1.0.0</FileVersion>
```

## Testing

### Local Testing

1. Build version 1.0.0:
   ```powershell
   .\build-installer.ps1 -Version "1.0.0"
   ```

2. Set update URL to local folder:
   ```csharp
   var mgr = new UpdateManager("file:///C:/TestReleases");
   ```

3. Install v1.0.0 from `Releases/Setup.exe`

4. Build v1.1.0 and copy to test folder:
   ```powershell
   .\build-installer.ps1 -Version "1.1.0"
   Copy-Item .\Releases\* C:\TestReleases\ -Force
   ```

5. Launch app - should detect and offer update

### Production Testing

1. Upload releases to your server
2. Install from Setup.exe on clean machine
3. Release new version to server
4. Launch app and verify update works

## Deployment Workflow

### Initial Release (v1.0.0)

1. ✅ Set update URL in `App.xaml.cs`
2. ✅ Set version to 1.0.0 in `.csproj`
3. ✅ Build: `.\build-installer.ps1 -Version "1.0.0"`
4. ✅ Test installer locally
5. ✅ Upload `Releases/*` to update server
6. ✅ Distribute `Setup.exe` to users

### Subsequent Releases (v1.1.0+)

1. ✅ Make code changes
2. ✅ Update version in `.csproj`
3. ✅ Build: `.\build-installer.ps1 -Version "1.1.0"`
4. ✅ Test installer
5. ✅ Upload new files to server (keep old files!)
6. ✅ Users auto-update next time they launch

## Features Enabled

### Auto-Update Features

- ✅ Automatic update checking on startup
- ✅ Background downloads (non-blocking)
- ✅ User notification when ready
- ✅ Restart prompt (user can defer)
- ✅ Delta updates (smaller downloads)
- ✅ Rollback capability

### Installer Features

- ✅ Professional Windows installer
- ✅ Start menu shortcuts
- ✅ Desktop shortcut
- ✅ Uninstall support
- ✅ Per-user installation
- ✅ No admin rights required

## Benefits

### For Users

- One-click installation
- Automatic updates
- Faster update downloads (delta updates)
- No manual downloading
- Always up-to-date

### For Developers

- Simple build process
- One command to create installer
- Automated version management
- Delta package generation
- Easy rollback if needed

## Customization Options

### Change Update Check Frequency

Currently checks on startup. To add periodic checks:

```csharp
// In Application_Startup
var timer = new DispatcherTimer { Interval = TimeSpan.FromHours(1) };
timer.Tick += async (s, e) => await CheckForUpdatesAsync();
timer.Start();
```

### Silent Updates

To update without prompting:

```csharp
// In CheckForUpdatesAsync
await mgr.DownloadUpdatesAsync(updateInfo);
// Remove MessageBox.Show
mgr.ApplyUpdatesAndRestart(updateInfo); // Restart immediately
```

### Manual Update Check

Add a button to Settings window:

```csharp
[RelayCommand]
private async Task CheckForUpdates()
{
    await CheckForUpdatesAsync();
}
```

## Known Limitations

1. **Update URL must be set before building** - Can't change after distribution
2. **Requires server** - Need somewhere to host update files
3. **Windows only** - Velopack supports macOS/Linux but needs different builds
4. **No delta updates on first release** - Only full package available initially

## Support Resources

- **Full Guide**: [DEPLOYMENT.md](DEPLOYMENT.md)
- **Quick Reference**: [VELOPACK_QUICKSTART.md](VELOPACK_QUICKSTART.md)
- **Velopack Docs**: https://docs.velopack.io
- **Velopack GitHub**: https://github.com/velopack/velopack

## Next Steps

1. **Choose hosting** - GitHub Releases, your server, or cloud storage
2. **Set update URL** - Update `App.xaml.cs` line 82
3. **Build v1.0.0** - Run `.\build-installer.ps1 -Version "1.0.0"`
4. **Test locally** - Install and verify it works
5. **Deploy** - Upload to your hosting
6. **Distribute** - Share Setup.exe with users

## Troubleshooting

### Build Issues

**"vpk not found"**
```powershell
dotnet tool install -g vpk
```

**Build fails**
- Check .NET 9 SDK is installed
- Verify all files are saved
- Try `dotnet clean` then rebuild

### Update Issues

**Updates not detected**
- Verify update URL is correct and accessible
- Check `RELEASES` file exists at URL
- Look for errors in debug output

**Update downloads but won't install**
- Close all instances of the app
- Check antivirus isn't blocking
- Try running as administrator

### Distribution Issues

**Windows SmartScreen blocks installer**
- Code sign the installer (recommended for production)
- Users can click "More info" → "Run anyway"

## Summary

Velopack integration is **complete and functional**. The application now has:

- ✅ Professional installer
- ✅ Automatic updates
- ✅ Build scripts ready to use
- ✅ Comprehensive documentation

**Just update the URL in App.xaml.cs and you're ready to deploy!**
