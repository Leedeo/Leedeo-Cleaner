@echo off
cd /d "%~dp0"
color 0A
title Building Leedeo Cleaner v1.6...
echo.

:: =============================================================================
::  Leedeo Cleaner v1.6 - Build script
::  Requires .NET Framework 4.x (64-bit) to be installed.
:: =============================================================================

:: 1. LOCATE COMPILER
set "csc="
for /d %%i in ("%windir%\Microsoft.NET\Framework64\v4*") do set "csc=%%i\csc.exe"

if not exist "%csc%" (
    color 0C
    echo [ERROR] csc.exe not found under .NET Framework 4.x (64-bit^).
    echo         Make sure .NET Framework 4.x is installed.
    pause
    exit /b 1
)

echo [Compiler found] %csc%
echo.

:: 2. VERIFY SOURCE FILES AND ASSETS
echo [Checking assets...]

set "missing=0"
if not exist "logo.png"       ( echo    [MISSING] logo.png       & set "missing=1" )
if not exist "icon.ico"       ( echo    [MISSING] icon.ico       & set "missing=1" )
if not exist "btn_quick.png"  ( echo    [MISSING] btn_quick.png  & set "missing=1" )
if not exist "btn_deep.png"   ( echo    [MISSING] btn_deep.png   & set "missing=1" )
if not exist "btn_repair.png" ( echo    [MISSING] btn_repair.png & set "missing=1" )
if not exist "btn_update.png"    ( echo    [MISSING] btn_update.png    & set "missing=1" )
if not exist "btn_registry.png" ( echo    [MISSING] btn_registry.png & set "missing=1" )
if not exist "flag_en.png"    ( echo    [MISSING] flag_en.png    & set "missing=1" )
if not exist "flag_es.png"    ( echo    [MISSING] flag_es.png    & set "missing=1" )
if not exist "flag_pt.png"    ( echo    [MISSING] flag_pt.png    & set "missing=1" )
if not exist "flag_it.png"    ( echo    [MISSING] flag_it.png    & set "missing=1" )
if not exist "flag_fr.png"    ( echo    [MISSING] flag_fr.png    & set "missing=1" )
if not exist "flag_de.png"    ( echo    [MISSING] flag_de.png    & set "missing=1" )
if not exist "MainWindow.cs"  ( echo    [MISSING] MainWindow.cs  & set "missing=1" )
if not exist "Strings.cs"     ( echo    [MISSING] Strings.cs     & set "missing=1" )

if "%missing%"=="1" (
    echo.
    echo [ERROR] Missing files. Place them next to this script and try again.
    pause
    exit /b 1
)

echo    All assets found. OK
echo.

:: 3. COMPILE
echo [Building v1.6...]

"%csc%" /target:winexe ^
        /platform:anycpu ^
        /out:"LeedeoCleaner.exe" ^
        /win32icon:"icon.ico" ^
        /resource:logo.png,App.logo.png ^
        /resource:icon.ico,App.icon.ico ^
        /resource:btn_quick.png,App.btn_quick.png ^
        /resource:btn_deep.png,App.btn_deep.png ^
        /resource:btn_repair.png,App.btn_repair.png ^
        /resource:btn_update.png,App.btn_update.png ^
        /resource:btn_registry.png,App.btn_registry.png ^
        /resource:flag_en.png,App.flag_en.png ^
        /resource:flag_es.png,App.flag_es.png ^
        /resource:flag_pt.png,App.flag_pt.png ^
        /resource:flag_it.png,App.flag_it.png ^
        /resource:flag_fr.png,App.flag_fr.png ^
        /resource:flag_de.png,App.flag_de.png ^
        /r:System.dll,System.Drawing.dll,System.Windows.Forms.dll ^
        "MainWindow.cs" "Strings.cs"

:: 4. RESULT
echo.
if %errorlevel% == 0 (
    color 0A
    echo ============================================================
    echo  [SUCCESS] LeedeoCleaner.exe v1.6 built successfully.
    echo ============================================================
) else (
    color 0C
    echo ============================================================
    echo  [ERROR] Build failed. Check the messages above.
    echo ============================================================
)

echo.
pause