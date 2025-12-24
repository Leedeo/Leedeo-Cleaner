@echo off
cd /d "%~dp0"
color 0A
title Compilando Leedeo Cleaner (Graphic Edition)...
echo.

:: 1. BUSCAR COMPILADOR
set "csc="
for /d %%i in (%windir%\Microsoft.NET\Framework64\v4*) do set "csc=%%i\csc.exe"

:: 2. VERIFICAR QUE TENGAS LAS IMAGENES
echo [Verificando imagenes...]
if not exist "logo.png" ( echo [FALTA] logo.png & pause & exit )
if not exist "icono.ico" ( echo [FALTA] icono.ico & pause & exit )
if not exist "btn_rapida.png" ( echo [FALTA] btn_rapida.png & pause & exit )
if not exist "btn_profunda.png" ( echo [FALTA] btn_profunda.png & pause & exit )
if not exist "btn_reparar.png" ( echo [FALTA] btn_reparar.png & pause & exit )

:: 3. COMPILAR
echo [Compilando...]

"%csc%" /target:winexe /out:"LeedeoCleaner.exe" ^
/win32icon:"icono.ico" ^
/resource:logo.png,App.logo.png ^
/resource:icono.ico,App.icono.ico ^
/resource:btn_rapida.png,App.btn_rapida.png ^
/resource:btn_profunda.png,App.btn_profunda.png ^
/resource:btn_reparar.png,App.btn_reparar.png ^
/r:System.dll,System.Drawing.dll,System.Windows.Forms.dll "CodigoFuente.cs"

if %errorlevel% == 0 (
    echo.
    echo [EXITO] LeedeoCleaner.exe creado.
) else (
    color 0C
    echo [ERROR] Algo fallo. Revisa arriba.
)
pause