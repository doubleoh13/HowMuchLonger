# Build and Package Script for How Much Longer
# This script builds the application and creates an installer using Velopack

param(
    [string]$Version = "1.0.0",
    [string]$Channel = "stable"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building How Much Longer v$Version" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Set paths
$ProjectDir = Join-Path $PSScriptRoot "HowMuchLonger"
$ProjectFile = Join-Path $ProjectDir "HowMuchLonger.csproj"
$PublishDir = Join-Path $PSScriptRoot "publish"
$ReleasesDir = Join-Path $PSScriptRoot "Releases"

# Clean previous builds
Write-Host "`nCleaning previous builds..." -ForegroundColor Yellow
if (Test-Path $PublishDir) {
    Remove-Item $PublishDir -Recurse -Force
}

# Build the application
Write-Host "`nBuilding application..." -ForegroundColor Yellow
dotnet publish $ProjectFile `
    -c Release `
    --self-contained true `
    -r win-x64 `
    -o $PublishDir `
    /p:PublishSingleFile=false `
    /p:Version=$Version

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nBuild failed!" -ForegroundColor Red
    exit 1
}

Write-Host "`nBuild completed successfully!" -ForegroundColor Green

# Check if vpk is installed
Write-Host "`nChecking for Velopack CLI (vpk)..." -ForegroundColor Yellow
$vpkInstalled = $null
try {
    $vpkInstalled = Get-Command vpk -ErrorAction Stop
} catch {
    Write-Host "Velopack CLI (vpk) not found. Installing..." -ForegroundColor Yellow
    dotnet tool install -g vpk
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`nFailed to install vpk!" -ForegroundColor Red
        exit 1
    }
}

Write-Host "vpk found: $($vpkInstalled.Source)" -ForegroundColor Green

# Create installer using Velopack
Write-Host "`nCreating installer package..." -ForegroundColor Yellow

# Create releases directory if it doesn't exist
if (-not (Test-Path $ReleasesDir)) {
    New-Item -ItemType Directory -Path $ReleasesDir | Out-Null
}

vpk pack `
    --packId "HowMuchLonger" `
    --packVersion $Version `
    --packDir $PublishDir `
    --mainExe "HowMuchLonger.exe" `
    --outputDir $ReleasesDir `
    --packTitle "How Much Longer" `
    --packAuthors "Your Name" `
    --channel $Channel

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nPackaging failed!" -ForegroundColor Red
    exit 1
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Build Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "`nInstaller created in: $ReleasesDir" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Test the installer: Run the Setup.exe in the Releases folder" -ForegroundColor White
Write-Host "2. Deploy: Upload the contents of the Releases folder to your update server" -ForegroundColor White
Write-Host "3. Update the URL in App.xaml.cs (line 82) to point to your update server" -ForegroundColor White
Write-Host "`nPress any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
