@echo off
REM Build and Package Script for How Much Longer (Batch wrapper)
REM This script calls the PowerShell build script

echo ========================================
echo Building How Much Longer Installer
echo ========================================
echo.

REM Check if PowerShell is available
where powershell >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo PowerShell not found! Please install PowerShell to use this build script.
    pause
    exit /b 1
)

REM Run the PowerShell build script
powershell -ExecutionPolicy Bypass -File "%~dp0build-installer.ps1" %*

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Build failed! Check the output above for errors.
    pause
    exit /b 1
)

exit /b 0
