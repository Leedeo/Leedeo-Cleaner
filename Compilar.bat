@echo off
cd /d "%~dp0"
color 0A
title Compilando Leedeo Cleaner v1.4...
echo.

:: ============================================================
::  LEEDEO CLEANER v1.4 - Script de compilacion
::  Requiere .NET Framework 4.x instalado (64 bits)
:: ============================================================

:: 1. BUSCAR COMPILADOR (ultima version de .NET Framework 4.x disponible)
set "csc="
for /d %%i in ("%windir%\Microsoft.NET\Framework64\v4*") do set "csc=%%i\csc.exe"

if not exist "%csc%" (
    color 0C
    echo [ERROR] No se encontro csc.exe en .NET Framework 4.x (64 bits^).
    echo         Asegurate de tener .NET Framework 4.x instalado.
    pause
    exit /b 1
)

echo [Compilador encontrado] %csc%
echo.

:: 2. VERIFICAR IMAGENES Y RECURSOS NECESARIOS
echo [Verificando recursos...]

set "faltan=0"
if not exist "logo.png"           ( echo    [FALTA] logo.png           & set "faltan=1" )
if not exist "icono.ico"          ( echo    [FALTA] icono.ico          & set "faltan=1" )
if not exist "btn_rapida.png"     ( echo    [FALTA] btn_rapida.png     & set "faltan=1" )
if not exist "btn_profunda.png"   ( echo    [FALTA] btn_profunda.png   & set "faltan=1" )
if not exist "btn_reparar.png"    ( echo    [FALTA] btn_reparar.png    & set "faltan=1" )
if not exist "btn_actualizar.png" ( echo    [FALTA] btn_actualizar.png & set "faltan=1" )
if not exist "CodigoFuente.cs"    ( echo    [FALTA] CodigoFuente.cs    & set "faltan=1" )

if "%faltan%"=="1" (
    echo.
    echo [ERROR] Faltan archivos necesarios. Colocalos junto a este .bat y vuelve a intentarlo.
    pause
    exit /b 1
)

echo    Todos los recursos encontrados. OK
echo.

:: 3. COMPILAR E INCRUSTAR RECURSOS
echo [Compilando v1.4...]

"%csc%" /target:winexe ^
        /out:"LeedeoCleaner.exe" ^
        /win32icon:"icono.ico" ^
        /resource:logo.png,App.logo.png ^
        /resource:icono.ico,App.icono.ico ^
        /resource:btn_rapida.png,App.btn_rapida.png ^
        /resource:btn_profunda.png,App.btn_profunda.png ^
        /resource:btn_reparar.png,App.btn_reparar.png ^
        /resource:btn_actualizar.png,App.btn_actualizar.png ^
        /r:System.dll,System.Drawing.dll,System.Windows.Forms.dll ^
        "CodigoFuente.cs"

:: 4. RESULTADO
echo.
if %errorlevel% == 0 (
    color 0A
    echo ============================================================
    echo  [EXITO] LeedeoCleaner.exe v1.4 creado correctamente.
    echo ============================================================
) else (
    color 0C
    echo ============================================================
    echo  [ERROR] La compilacion fallo. Revisa los mensajes de arriba.
    echo ============================================================
)

echo.
pause
