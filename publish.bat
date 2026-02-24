@echo off
setlocal enabledelayedexpansion
title ALttP MSU-1 Music Switcher — Build + Package

echo.
echo ============================================================
echo  ALttP MSU-1 Music Switcher — Build ^& Package
echo ============================================================
echo.

:: ── Step 1: dotnet publish ───────────────────────────────────
echo [1/2] Publishing self-contained EXE...
echo.

dotnet publish -c Release -r win-x64 ^
    --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:PublishTrimmed=false ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    -p:DebugType=none ^
    -p:DebugSymbols=false

if %ERRORLEVEL% neq 0 (
    echo.
    echo [ERROR] dotnet publish failed. See output above.
    pause
    exit /b 1
)

echo.
echo  EXE: bin\Release\net8.0-windows\win-x64\publish\LTTPEnhancementTools.exe
echo.

:: ── Step 2: Inno Setup ───────────────────────────────────────
echo [2/2] Building installer...
echo.

set ISCC=
set ISCC_DEFAULT=C:\Program Files (x86)\Inno Setup 6\ISCC.exe

:: Check PATH first, then default install location
where ISCC >nul 2>&1
if %ERRORLEVEL% equ 0 (
    set ISCC=ISCC
) else if exist "%ISCC_DEFAULT%" (
    set ISCC="%ISCC_DEFAULT%"
)

if "!ISCC!"=="" (
    echo  [SKIP] Inno Setup not found.
    echo.
    echo  To build an installer, install Inno Setup 6 (free):
    echo    https://jrsoftware.org/isinfo.php
    echo.
    echo  Then re-run this script, or compile manually:
    echo    "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" setup.iss
    echo.
    echo  The standalone EXE is ready without an installer:
    echo    bin\Release\net8.0-windows\win-x64\publish\LTTPEnhancementTools.exe
) else (
    if not exist installer mkdir installer
    !ISCC! setup.iss
    if %ERRORLEVEL% neq 0 (
        echo.
        echo [ERROR] Inno Setup compilation failed.
        pause
        exit /b 1
    )
    echo.
    echo ============================================================
    echo  Done!
    echo.
    echo  Standalone EXE:
    echo    bin\Release\net8.0-windows\win-x64\publish\LTTPEnhancementTools.exe
    echo.
    echo  Installer:
    echo    installer\LTTPEnhancementToolsSetup-1.0.0-win64.exe
    echo ============================================================
)

echo.
pause
