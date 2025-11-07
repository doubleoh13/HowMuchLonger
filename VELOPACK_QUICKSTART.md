# Velopack Quick Start Guide

Quick reference for building and deploying updates with Velopack.

## One-Time Setup

### 1. Set Your Update URL

Edit `HowMuchLonger/App.xaml.cs` line 82:

```csharp
var mgr = new UpdateManager("YOUR_UPDATE_URL_HERE");
```

Examples:
- GitHub: `"https://github.com/doubleoh13/HowMuchLonger/releases/latest/download"`
- Your server: `"https://yoursite.com/releases"`
- Local testing: `"file:///C:/releases"`

### 2. Configure Version

Edit `HowMuchLonger/HowMuchLonger.csproj`:

```xml
<Version>1.0.0</Version>
<AssemblyVersion>1.0.0.0</AssemblyVersion>
<FileVersion>1.0.0.0</FileVersion>
```

## Build and Release Workflow

### First Release (v1.0.0)

1. **Build installer**:
   ```powershell
   .\build-installer.ps1 -Version "1.0.0"
   ```

2. **Upload to your server**:
   - Upload all files from `Releases/` folder
   - Make sure `RELEASES` file is accessible

3. **Distribute**:
   - Share `HowMuchLonger-1.0.0-win-Setup.exe` with users

### Releasing Updates (v1.1.0+)

1. **Update version** in `HowMuchLonger.csproj`:
   ```xml
   <Version>1.1.0</Version>
   ```

2. **Build new version**:
   ```powershell
   .\build-installer.ps1 -Version "1.1.0"
   ```

3. **Upload new files**:
   - Add new `.nupkg` files to server
   - Add new `Setup.exe` to server
   - **Replace** `RELEASES` file
   - **Keep** old version files on server

4. **Users auto-update**:
   - App checks for updates on startup
   - Downloads and installs automatically

## Testing Updates Locally

### Setup Local Test Environment

1. **Create local release folder**:
   ```powershell
   mkdir C:\TestReleases
   ```

2. **Copy first release**:
   ```powershell
   Copy-Item -Path ".\Releases\*" -Destination "C:\TestReleases"
   ```

3. **Update App.xaml.cs** for testing:
   ```csharp
   var mgr = new UpdateManager("file:///C:/TestReleases");
   ```

4. **Install v1.0.0**:
   - Run `HowMuchLonger-1.0.0-win-Setup.exe`

### Test an Update

1. **Create new version**:
   ```powershell
   # Update version in .csproj to 1.1.0
   .\build-installer.ps1 -Version "1.1.0"
   ```

2. **Copy to test folder**:
   ```powershell
   Copy-Item -Path ".\Releases\*" -Destination "C:\TestReleases" -Force
   ```

3. **Launch app**:
   - App should detect and offer to install update
   - Accept and restart
   - Verify version updated

## Common Commands

### Build Commands

```powershell
# Standard build
.\build-installer.ps1

# Specific version
.\build-installer.ps1 -Version "2.0.1"

# Beta channel
.\build-installer.ps1 -Version "2.1.0-beta" -Channel "beta"
```

### Check Installation

```powershell
# Find installed version
Get-ItemProperty "HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*" |
    Where-Object {$_.DisplayName -like "*How Much Longer*"} |
    Select-Object DisplayName, DisplayVersion, InstallLocation
```

### Manual Uninstall

```powershell
# Remove installation
Remove-Item "$env:LOCALAPPDATA\HowMuchLonger" -Recurse -Force

# Remove settings
Remove-Item "$env:APPDATA\HowMuchLonger" -Recurse -Force
```

## File Structure on Update Server

Your update server should have this structure:

```
/releases/                               # Your update URL points here
├── RELEASES                             # Version manifest (required!)
├── HowMuchLonger-1.0.0-win-full.nupkg  # v1.0.0 full package
├── HowMuchLonger-1.0.0-win-Setup.exe   # v1.0.0 installer
├── HowMuchLonger-1.1.0-win-full.nupkg  # v1.1.0 full package
├── HowMuchLonger-1.1.0-win-delta.nupkg # v1.1.0 delta update
└── HowMuchLonger-1.1.0-win-Setup.exe   # v1.1.0 installer
```

## Update Check Behavior

- **When**: On app startup (background check)
- **Download**: Automatic in background
- **Prompt**: User chooses to restart now or later
- **Install**: On next restart (seamless)

## Troubleshooting Quick Fixes

### Update Not Detected

1. Check update URL is accessible
2. Verify `RELEASES` file exists
3. Check firewall/antivirus

### Update Download Fails

1. Check server has all required files
2. Verify network connection
3. Look at debug output (Visual Studio Output window)

### Update Install Fails

1. Close all instances of the app
2. Run as administrator
3. Check antivirus isn't blocking

### Rollback

If update has issues:
1. Users can reinstall previous version using old Setup.exe
2. Or manually delete: `%LocalAppData%\HowMuchLonger`
3. Reinstall with working version

## GitHub Releases Setup

### 1. Create Release on GitHub

```bash
# Tag the version
git tag v1.0.0
git push origin v1.0.0
```

### 2. Upload Release Files

1. Go to GitHub > Releases > Create Release
2. Upload all files from `Releases/` folder
3. Publish release

### 3. Get Release URL

Format: `https://github.com/USERNAME/REPO/releases/latest/download/`

Update in App.xaml.cs:
```csharp
var mgr = new UpdateManager("https://github.com/doubleoh13/HowMuchLonger/releases/latest/download");
```

## Version Numbering

Follow semantic versioning:

- **1.0.0** → **1.0.1**: Bug fixes (patch)
- **1.0.0** → **1.1.0**: New features (minor)
- **1.0.0** → **2.0.0**: Breaking changes (major)

Beta versions: `1.1.0-beta`, `1.1.0-rc1`

## Best Practices Checklist

- [ ] Test locally before deploying
- [ ] Keep old versions on server (for delta updates)
- [ ] Update version in `.csproj` before building
- [ ] Code sign installers for production
- [ ] Test on clean machine before release
- [ ] Document changes in release notes
- [ ] Backup releases folder
- [ ] Monitor update success rate

## Need More Help?

- Full deployment guide: [DEPLOYMENT.md](DEPLOYMENT.md)
- Velopack docs: https://docs.velopack.io
- Project README: [README.md](README.md)

---

**Quick Command Reference:**

```powershell
# Build v1.0.0
.\build-installer.ps1 -Version "1.0.0"

# Test locally
$mgr = new UpdateManager("file:///C:/TestReleases")

# Check version
[System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:LOCALAPPDATA\HowMuchLonger\current\HowMuchLonger.exe").FileVersion
```
