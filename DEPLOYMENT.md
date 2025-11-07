# Deployment Guide - How Much Longer

This guide explains how to build, package, and deploy the How Much Longer application with Velopack for automatic updates.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Building the Installer](#building-the-installer)
3. [Testing the Installer](#testing-the-installer)
4. [Deploying Updates](#deploying-updates)
5. [Configuring Update URL](#configuring-update-url)
6. [Release Workflow](#release-workflow)
7. [Troubleshooting](#troubleshooting)

## Prerequisites

### Required Tools

1. **.NET 9 SDK** - Download from https://dotnet.microsoft.com/download
2. **Velopack CLI (vpk)** - Will be installed automatically by the build script
3. **PowerShell** (Windows) - Pre-installed on Windows 10/11

### Optional Tools

- **Code signing certificate** - For signing your installers (recommended for production)
- **GitHub Account** - For hosting releases (or use your own web server)

## Building the Installer

### Quick Build

Simply run the build script:

```powershell
# Using PowerShell
.\build-installer.ps1

# Or using the batch file
.\build-installer.bat
```

This will:
1. Clean previous builds
2. Build the application in Release mode
3. Install Velopack CLI if needed
4. Create an installer package
5. Output everything to the `Releases` folder

### Custom Version Build

To build with a specific version number:

```powershell
.\build-installer.ps1 -Version "1.2.0"
```

### Build Output

After building, you'll find these files in the `Releases` folder:

```
Releases/
├── HowMuchLonger-1.0.0-win-Setup.exe    # Full installer
├── HowMuchLonger-1.0.0-win-full.nupkg   # Full package
└── RELEASES                              # Version manifest
```

## Testing the Installer

### First Installation Test

1. Navigate to the `Releases` folder
2. Run `HowMuchLonger-1.0.0-win-Setup.exe`
3. Follow the installation wizard
4. Launch the application

### Installation Locations

The app will be installed to:
```
%LocalAppData%\HowMuchLonger\
```

Settings are stored in:
```
%AppData%\HowMuchLonger\settings.json
```

### Uninstallation

To uninstall:
1. Go to Windows Settings > Apps
2. Search for "How Much Longer"
3. Click Uninstall

Or use the command line:
```powershell
# Find the app's uninstall command
Get-ItemProperty HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\* |
    Where-Object {$_.DisplayName -like "*How Much Longer*"}
```

## Deploying Updates

### Step 1: Prepare Your Update Server

You need a location to host your update files. Options include:

#### Option A: GitHub Releases (Recommended)

1. Create a GitHub repository for your app
2. Create a new Release
3. Upload all files from the `Releases` folder
4. Use this URL format:
   ```
   https://github.com/doubleoh13/HowMuchLonger/releases/latest/download/
   ```

#### Option B: Web Server

1. Set up a web server (Apache, Nginx, IIS, etc.)
2. Create a directory for releases (e.g., `/releases`)
3. Upload all files from the `Releases` folder
4. Ensure files are publicly accessible
5. Use your server URL:
   ```
   https://yourdomain.com/releases
   ```

#### Option C: Azure Blob Storage / AWS S3

1. Create a storage container with public read access
2. Upload all files from the `Releases` folder
3. Use the container's public URL

### Step 2: Configure Update URL

Edit `HowMuchLonger/App.xaml.cs` and update line 82:

```csharp
// Before (line 82):
var mgr = new UpdateManager("https://yourdomain.com/releases");

// After (example for GitHub):
var mgr = new UpdateManager("https://github.com/doubleoh13/HowMuchLonger/releases/latest/download");

// After (example for your server):
var mgr = new UpdateManager("https://updates.yourcompany.com/howmuchlonger");
```

### Step 3: Deploy the Update Files

Upload these files to your update server:

1. **Required files** (from Releases folder):
   - `RELEASES` - Version manifest (must be at root)
   - `*.nupkg` - Package files
   - `Setup.exe` - For new installations

2. **File structure on server**:
   ```
   /releases/
   ├── RELEASES
   ├── HowMuchLonger-1.0.0-win-full.nupkg
   ├── HowMuchLonger-1.0.0-win-Setup.exe
   ├── HowMuchLonger-1.1.0-win-full.nupkg      (newer version)
   ├── HowMuchLonger-1.1.0-win-delta.nupkg     (delta update)
   └── HowMuchLonger-1.1.0-win-Setup.exe
   ```

### Step 4: Create a New Version

1. Update version in `HowMuchLonger.csproj`:
   ```xml
   <Version>1.1.0</Version>
   <AssemblyVersion>1.1.0.0</AssemblyVersion>
   <FileVersion>1.1.0.0</FileVersion>
   ```

2. Make your changes to the code

3. Build the new version:
   ```powershell
   .\build-installer.ps1 -Version "1.1.0"
   ```

4. Upload the new files to your update server:
   - Keep old versions on the server
   - Add new `.nupkg` and `Setup.exe` files
   - **Replace** the `RELEASES` file (this lists all versions)

## Release Workflow

### Initial Release (v1.0.0)

1. Build version 1.0.0
2. Set up your update server
3. Upload initial release files
4. Configure update URL in code
5. Rebuild with the correct URL
6. Distribute Setup.exe to users

### Subsequent Releases (v1.1.0+)

1. Make code changes
2. Update version number in `.csproj`
3. Build new version with `build-installer.ps1`
4. Test the installer locally
5. Upload new files to update server
6. **Keep old versions** on the server
7. Velopack will generate delta updates automatically

Users will be notified of updates automatically when they launch the app.

## How Auto-Updates Work

### Update Check Process

1. **On Startup**: App checks for updates in the background
2. **Download**: If update available, downloads in background
3. **Notification**: User is prompted when update is ready
4. **Install**: User can choose to restart now or later
5. **Apply**: Update is applied on restart

### Delta Updates

Velopack creates delta packages that contain only the changed files:

- First release: `HowMuchLonger-1.0.0-win-full.nupkg` (Full package)
- Second release:
  - `HowMuchLonger-1.1.0-win-full.nupkg` (Full package for new installs)
  - `HowMuchLonger-1.1.0-win-delta.nupkg` (Delta from 1.0.0 to 1.1.0)

Users updating from 1.0.0 to 1.1.0 only download the delta package.

### Update Channels

You can have multiple update channels:

```powershell
# Stable channel (default)
.\build-installer.ps1 -Version "1.0.0" -Channel "stable"

# Beta channel
.\build-installer.ps1 -Version "1.1.0-beta" -Channel "beta"
```

Configure the channel in code:
```csharp
var mgr = new UpdateManager("https://yourdomain.com/releases", channel: "beta");
```

## Advanced Configuration

### Code Signing

To sign your installer (recommended for production):

1. Obtain a code signing certificate
2. Install it on your build machine
3. Add signing parameters to build script:

```powershell
vpk pack `
    --packId "HowMuchLonger" `
    --packVersion $Version `
    --packDir $PublishDir `
    --mainExe "HowMuchLonger.exe" `
    --signParams "/a /t http://timestamp.digicert.com /fd sha256"
```

### Custom Installer Icon

Add an icon to your installer:

```powershell
vpk pack `
    --icon "path\to\icon.ico" `
    # ... other parameters
```

### Splash Screen

Add a splash screen during updates:

```powershell
vpk pack `
    --splashImage "path\to\splash.gif" `
    # ... other parameters
```

## Troubleshooting

### Build Issues

**Problem**: Build script fails with "vpk not found"

**Solution**:
```powershell
dotnet tool install -g vpk
```

**Problem**: Build fails with "Unable to find project"

**Solution**: Make sure you're running the script from the project root directory.

### Update Issues

**Problem**: App doesn't detect updates

**Solution**:
1. Check update URL is correct and accessible
2. Verify `RELEASES` file exists at the URL
3. Check firewall/antivirus isn't blocking the connection
4. Look for errors in debug output

**Problem**: Update downloads but fails to install

**Solution**:
1. Ensure app has write permissions to installation directory
2. Check antivirus isn't blocking the update
3. Try running as administrator

**Problem**: Delta updates not working

**Solution**:
1. Make sure all versions are kept on the server
2. Verify the `RELEASES` file lists all versions
3. Check that users are updating from a version that's still on the server

### Distribution Issues

**Problem**: Windows SmartScreen blocks installer

**Solution**:
1. Code sign your installer with a trusted certificate
2. Build reputation by having many downloads
3. Users can click "More info" → "Run anyway"

**Problem**: Antivirus flags the installer

**Solution**:
1. Submit the installer to antivirus vendors for whitelisting
2. Code sign the executable
3. Scan the build machine for malware

## Best Practices

1. **Always test updates locally** before deploying
2. **Keep all versions on the server** for delta updates
3. **Use semantic versioning** (e.g., 1.2.3)
4. **Code sign your installers** for production
5. **Monitor update success rate** via telemetry
6. **Have a rollback plan** (keep old installers available)
7. **Test on clean machines** before releasing
8. **Document changes** in release notes

## Example GitHub Actions Workflow

Automate builds with GitHub Actions:

```yaml
name: Build and Release

on:
  push:
    tags:
      - 'v*'

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'

      - name: Install vpk
        run: dotnet tool install -g vpk

      - name: Build Installer
        run: .\build-installer.ps1 -Version "${{ github.ref_name }}"

      - name: Create Release
        uses: softprops/action-gh-release@v1
        with:
          files: |
            Releases/*
```

## Support

For issues with:
- **Velopack**: https://github.com/velopack/velopack/issues
- **This Application**: Check the main README.md

---

**Happy Deploying!**
