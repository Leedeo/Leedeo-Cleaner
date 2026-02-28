using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Reflection;

// LEEDEO CLEANER v1.4
// Refactorización: orden de comandos corregido, limpieza de código, comentarios en español,
// corrección de fugas de memoria en imágenes, protección mejorada en Ejecutar(),
// y apertura de URLs corregida para .NET moderno.

[assembly: AssemblyTitle("Leedeo Cleaner")]
[assembly: AssemblyDescription("Herramienta de optimización y limpieza")]
[assembly: AssemblyCompany("Leedeo Studio")]
[assembly: AssemblyProduct("Leedeo Cleaner")]
[assembly: AssemblyCopyright("Copyright © 2025 Leedeo Studio")]
[assembly: AssemblyVersion("1.4.0.0")]
[assembly: AssemblyFileVersion("1.4.0.0")]

public class LimpiadorApp : Form
{
    // ── Paleta de colores ─────────────────────────────────────────────────────
    private readonly Color cSidebar   = Color.FromArgb(32, 32, 32);
    private readonly Color cFondo     = Color.FromArgb(18, 18, 18);
    private readonly Color cBtnNormal = Color.FromArgb(0, 120, 215);
    private readonly Color cBtnDeep   = Color.FromArgb(210, 60, 60);
    private readonly Color cBtnRepair = Color.FromArgb(39, 174, 96);
    private readonly Color cBtnUpdate = Color.FromArgb(255, 140, 0);
    private readonly Color cLeedeo    = ColorTranslator.FromHtml("#914d97");
    private readonly Color cVerde     = Color.Lime;

    // ── Controles ─────────────────────────────────────────────────────────────
    private Panel     sidebar, mainContent;
    private PictureBox pbLogo;
    private Label     lblTitulo, lblVersion, lblSystemInfo, lblDonar;
    private LinkLabel linkKofi;
    private TextBox   txtLog;
    private Button    btnNormal, btnDeep, btnRepair, btnUpdate, btnCerrar;

    // ── Estado de arrastre de ventana ─────────────────────────────────────────
    private bool  dragging;
    private Point dragCursorPoint, dragFormPoint;

    // ── Constructor ───────────────────────────────────────────────────────────
    public LimpiadorApp()
    {
        InicializarVentana();
        InicializarSidebar();
        InicializarContenidoPrincipal();
    }

    // ── Ventana sin bordes ────────────────────────────────────────────────────
    private void InicializarVentana()
    {
        FormBorderStyle = FormBorderStyle.None;
        Size            = new Size(780, 520);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = cFondo;
        Padding         = new Padding(1);
        Text            = "Leedeo Cleaner";

        // Intenta cargar icono incrustado; si falla, continúa sin él
        try { Icon = CargarIcono("App.icono.ico"); } catch { /* sin icono */ }

        // Borde decorativo de un píxel
        Paint += (s, e) =>
            e.Graphics.DrawRectangle(
                new Pen(Color.FromArgb(60, 60, 60)), 0, 0, Width - 1, Height - 1);
    }

    // ── Panel lateral izquierdo ───────────────────────────────────────────────
    private void InicializarSidebar()
    {
        sidebar = new Panel
        {
            Location  = new Point(1, 1),
            Size      = new Size(240, 518),
            BackColor = cSidebar
        };
        // El sidebar también permite arrastrar la ventana
        sidebar.MouseDown += MouseDownDrag;
        sidebar.MouseMove += MouseMoveDrag;
        sidebar.MouseUp   += MouseUpDrag;
        Controls.Add(sidebar);

        // Logo
        pbLogo = new PictureBox
        {
            Size     = new Size(160, 80),
            Location = new Point(40, 20),
            SizeMode = PictureBoxSizeMode.Zoom,
            Image    = CargarImagen("App.logo.png")
        };
        sidebar.Controls.Add(pbLogo);

        // Título
        lblTitulo = new Label
        {
            Text      = "LEEDEO\nCLEANER",
            Font      = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            Location  = new Point(20, 110),
            Size      = new Size(200, 55),
            TextAlign = ContentAlignment.MiddleCenter
        };
        sidebar.Controls.Add(lblTitulo);

        // Versión
        lblVersion = new Label
        {
            Text      = "v1.4",
            Font      = new Font("Segoe UI", 9, FontStyle.Regular),
            ForeColor = Color.Gray,
            Location  = new Point(20, 165),
            Size      = new Size(200, 30),
            TextAlign = ContentAlignment.TopCenter
        };
        sidebar.Controls.Add(lblVersion);

        // Botones del menú
        btnNormal = CrearBotonGrafico("LIMPIEZA RÁPIDA",    "App.btn_rapida.png",     220, cBtnNormal);
        btnDeep   = CrearBotonGrafico("LIMPIEZA PROFUNDA",  "App.btn_profunda.png",   275, cBtnDeep);
        btnRepair = CrearBotonGrafico("REPARAR SISTEMA",    "App.btn_reparar.png",    330, cBtnRepair);
        btnUpdate = CrearBotonGrafico("ACTUALIZAR APPS",    "App.btn_actualizar.png", 385, cBtnUpdate);

        // Texto descriptivo al pasar el ratón
        AsignarHover(btnNormal, "► LIMPIEZA RÁPIDA\r\n\r\nBorra temporales y caché DNS al instante.\r\nPerfecto para el mantenimiento diario.");
        AsignarHover(btnDeep,   "≡ LIMPIEZA PROFUNDA\r\n\r\nElimina Logs, Updates viejos y basura.\r\nRecomendado usar una vez al mes.");
        AsignarHover(btnRepair, "✚ REPARACIÓN TOTAL\r\n\r\nAnaliza y repara Disco, Archivos e Imagen.\r\nSi detecta errores, te ayudará a corregirlos.");
        AsignarHover(btnUpdate, "⚡ ACTUALIZAR APPS (WINGET)\r\n\r\nBusca actualizaciones para tus programas.\r\nTe mostrará una lista para que confirmes antes de instalar nada.");

        // Eventos de clic
        btnNormal.Click += (s, e) => Ejecutar(1);
        btnDeep.Click   += (s, e) => Ejecutar(2);
        btnRepair.Click += (s, e) => Ejecutar(3);
        btnUpdate.Click += (s, e) => Ejecutar(4);

        sidebar.Controls.Add(btnNormal);
        sidebar.Controls.Add(btnDeep);
        sidebar.Controls.Add(btnRepair);
        sidebar.Controls.Add(btnUpdate);
    }

    // ── Panel derecho (log + información) ────────────────────────────────────
    private void InicializarContenidoPrincipal()
    {
        mainContent = new Panel
        {
            Location  = new Point(242, 1),
            Size      = new Size(537, 518),
            BackColor = cFondo
        };
        mainContent.MouseDown += MouseDownDrag;
        mainContent.MouseMove += MouseMoveDrag;
        mainContent.MouseUp   += MouseUpDrag;
        Controls.Add(mainContent);

        // Botón cerrar (esquina superior derecha)
        btnCerrar = new Button
        {
            Text      = "\u2715",
            Font      = new Font("Segoe UI", 11, FontStyle.Bold),
            Size      = new Size(45, 30),
            Location  = new Point(mainContent.Width - 45, 0),
            FlatStyle = FlatStyle.Flat,
            BackColor = cLeedeo,
            ForeColor = Color.White,
            Cursor    = Cursors.Hand
        };
        btnCerrar.FlatAppearance.BorderSize = 0;
        btnCerrar.MouseEnter += (s, e) => btnCerrar.BackColor = Color.Red;
        btnCerrar.MouseLeave += (s, e) => btnCerrar.BackColor = cLeedeo;
        btnCerrar.Click      += (s, e) => Application.Exit();
        mainContent.Controls.Add(btnCerrar);

        // Info del sistema (PC, usuario, versión Windows)
        lblSystemInfo = new Label
        {
            AutoSize  = true,
            Location  = new Point(30, 20),
            Font      = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = Color.White,
            Text      = CargarDatosSistema()
        };
        mainContent.Controls.Add(lblSystemInfo);

        // Texto de donación
        lblDonar = new Label
        {
            Text      = "Si esta herramienta te ha ahorrado tiempo, considera apoyar el proyecto:",
            ForeColor = Color.Gray,
            Font      = new Font("Segoe UI", 9, FontStyle.Italic),
            AutoSize  = true,
            Location  = new Point(30, 75)
        };
        mainContent.Controls.Add(lblDonar);

        // Enlace Ko-fi
        linkKofi = new LinkLabel
        {
            Text           = "☕ Apoyar en Ko-fi (Leedeo)",
            Font           = new Font("Segoe UI", 9, FontStyle.Bold),
            LinkColor      = cLeedeo,
            ActiveLinkColor= ControlPaint.Light(cLeedeo),
            VisitedLinkColor = cLeedeo,
            LinkBehavior   = LinkBehavior.HoverUnderline,
            AutoSize       = true,
            Location       = new Point(30, 95)
        };
        // FIX: Process.Start con URL requiere UseShellExecute = true en .NET Core / .NET 5+
        linkKofi.LinkClicked += (s, e) =>
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://ko-fi.com/leedeo") { UseShellExecute = true });
            }
            catch { /* Si el navegador no se puede abrir, se ignora */ }
        };
        mainContent.Controls.Add(linkKofi);

        // Área de log (estilo terminal)
        txtLog = new TextBox
        {
            Multiline   = true,
            ReadOnly    = true,
            BackColor   = Color.Black,
            ForeColor   = cVerde,
            Font        = new Font("Consolas", 10),
            BorderStyle = BorderStyle.None,
            Location    = new Point(30, 140),
            Size        = new Size(480, 350),
            Text        = "Sistema listo. Esperando órdenes..."
        };
        mainContent.Controls.Add(txtLog);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // LÓGICA PRINCIPAL
    // ═════════════════════════════════════════════════════════════════════════

    private async void Ejecutar(int nivel)
    {
        // Construir el mensaje de confirmación según el nivel
        string titulo, mensaje;
        switch (nivel)
        {
            case 1:
                titulo  = "Confirmar Limpieza Rápida";
                mensaje = "¿Estás seguro de iniciar la Limpieza Rápida?\n\nSe borrarán archivos temporales y se vaciará la caché DNS.";
                break;
            case 2:
                titulo  = "Confirmar Limpieza Profunda";
                mensaje = "¿Deseas iniciar la Limpieza Profunda?\n\nSe eliminarán historiales, logs de sistema y caché de actualizaciones.";
                break;
            case 3:
                titulo  = "Confirmar Reparación de Sistema";
                mensaje = "ATENCIÓN: Este proceso es exhaustivo y puede tardar entre 20 y 30 minutos.\n\n¿Quieres comenzar el diagnóstico y reparación?";
                break;
            case 4:
                titulo  = "Actualizar Aplicaciones";
                mensaje = "Se abrirá el gestor Winget para buscar actualizaciones.\n\nSe mostrará una lista y podrás confirmar si quieres instalar todo o cancelar.\n¿Continuar?";
                break;
            default:
                return;
        }

        if (MessageBox.Show(mensaje, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            return;

        // Winget se gestiona en su propio método (abre ventana CMD externa)
        if (nivel == 4)
        {
            EjecutarWinget();
            return;
        }

        // Bloquear controles mientras se ejecuta
        ToggleBotones(false);
        txtLog.Clear();

        long espacioInicio = ObtenerEspacioLibre();
        Log("Espacio libre inicial: " + FormatearTamano(espacioInicio));
        Log("------------------------------------------------");

        try
        {
            if (nivel == 1) await EjecutarLimpiezaRapida();
            if (nivel == 2) await EjecutarLimpiezaProfunda();
            if (nivel == 3) await EjecutarReparacion();
        }
        catch (Exception ex)
        {
            Log("❌ Error inesperado: " + ex.Message);
        }
        finally
        {
            // Calcular espacio recuperado
            long espacioFin     = ObtenerEspacioLibre();
            long bytesLiberados = Math.Max(0, espacioFin - espacioInicio);
            string textoLiberado = FormatearTamano(bytesLiberados);

            Log("");
            Log("------------------------------------------------");
            Log(">>> ✅ PROCESO FINALIZADO.");
            Log(">>> ESPACIO LIBERADO: " + textoLiberado);

            MessageBox.Show(
                "Operación completada.\n\nEspacio recuperado: " + textoLiberado,
                "Leedeo Cleaner");

            ToggleBotones(true);
        }
    }

    // ── Nivel 1: Limpieza Rápida ──────────────────────────────────────────────
    // Orden correcto: temporales → DNS → reinicio del Explorer
    private async Task EjecutarLimpiezaRapida()
    {
        Log(">>> ► INICIANDO LIMPIEZA RÁPIDA...");

        // 1. Borrar temporales del usuario
        await Cmd(@"del /s /q %TEMP%\* >nul 2>&1");
        Log("> Temporales de usuario eliminados.");

        // 2. Borrar temporales del sistema
        await Cmd(@"del /s /q C:\Windows\Temp\* >nul 2>&1");
        Log("> Temporales del sistema eliminados.");

        // 3. Limpiar caché DNS
        await Cmd("ipconfig /flushdns >nul 2>&1");
        Log("> Caché DNS vaciada.");

        // 4. Reiniciar Explorer para refrescar caché de iconos
        Log("> Reiniciando Explorer...");
        await Cmd("taskkill /f /im explorer.exe >nul 2>&1");
        await Cmd(@"del /q %LOCALAPPDATA%\IconCache.db >nul 2>&1");
        // FIX: Process.Start necesita UseShellExecute = true para ejecutables en .NET Core
        Process.Start(new ProcessStartInfo("explorer.exe") { UseShellExecute = true });
    }

    // ── Nivel 2: Limpieza Profunda ────────────────────────────────────────────
    // Orden correcto: temporales → detener WU → limpiar caché WU → reiniciar WU → logs → Prefetch
    private async Task EjecutarLimpiezaProfunda()
    {
        Log(">>> ≡ INICIANDO LIMPIEZA PROFUNDA...");

        // 1. Temporales (mismo que el nivel rápido, pero sin reiniciar Explorer)
        await Cmd(@"del /s /q %TEMP%\* >nul 2>&1");
        await Cmd(@"del /s /q C:\Windows\Temp\* >nul 2>&1");
        Log("> Temporales eliminados.");

        // 2. Detener Windows Update antes de tocar su caché
        Log("> Gestionando Windows Update...");
        await Cmd("net stop wuauserv >nul 2>&1");
        await Cmd("net stop bits >nul 2>&1");

        // 3. Borrar caché de descargas de actualizaciones
        await Cmd(@"del /s /q C:\Windows\SoftwareDistribution\Download\* >nul 2>&1");
        Log("> Caché de actualizaciones eliminada.");

        // 4. Volver a arrancar el servicio de Windows Update
        await Cmd("net start wuauserv >nul 2>&1");
        Log("> Servicio de Windows Update reiniciado.");

        // 5. Limpiar logs del visor de eventos
        // NOTA: El comando original usaba %i sin comillas, lo que falla con nombres con espacios.
        //       Se usa /c con comillas en el bucle para mayor compatibilidad.
        Log("> Eliminando logs del sistema...");
        await Cmd("for /f \"tokens=*\" %i in ('wevtutil el') do @wevtutil cl \"%i\" >nul 2>&1");

        // 6. Historial de análisis de Windows Defender
        await Cmd(@"del /s /q ""C:\ProgramData\Microsoft\Windows Defender\Scans\History\*"" >nul 2>&1");
        Log("> Historial de Defender eliminado.");

        // 7. Prefetch — se deja al final porque Windows lo regenera en el próximo arranque
        await Cmd(@"del /s /q C:\Windows\Prefetch\* >nul 2>&1");
        Log("> Prefetch limpiado.");
    }

    // ── Nivel 3: Reparación ───────────────────────────────────────────────────
    // Orden correcto y técnicamente justificado:
    //   CHKDSK → SFC → DISM ScanHealth → DISM CheckHealth → DISM RestoreHealth → ComponentCleanup
    // SFC va antes de DISM porque si el sistema de archivos tiene errores de disco,
    // DISM puede fallar. Y DISM RestoreHealth se necesita ANTES de un segundo SFC en caso de errores.
    private async Task EjecutarReparacion()
    {
        Log(">>> ✚ REPARACIÓN DE SISTEMA (ANÁLISIS INTELIGENTE)");
        Log("Iniciando diagnóstico...");
        Log("");

        // [1/6] CHKDSK: análisis del disco físico
        Log("[1/6] Analizando salud del Disco (CHKDSK)...");
        string resDisco = await CmdCapturar("chkdsk C: /scan");

        if (resDisco.Contains("no ha encontrado problemas") || resDisco.Contains("found no problems"))
        {
            Log("   ✅ ESTADO DISCO: Sano. Sin errores físicos.");
        }
        else
        {
            Log("   ⚠️ ESTADO DISCO: Se detectaron anomalías.");
            if (MessageBox.Show(
                    "Se han detectado errores en el disco. ¿Programar reparación al reiniciar?",
                    "Error de Disco",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                // Marca el volumen como sucio para que CHKDSK /F se ejecute en el arranque
                await Cmd("fsutil dirty set C:");
                Log("   --> Reparación programada para el próximo reinicio.");
                MessageBox.Show("Reparación programada para el próximo reinicio.", "Listo");
            }
        }

        // [2/6] SFC: verificación de archivos del sistema
        Log("[2/6] Verificando integridad de archivos (SFC)...");
        string resSfc = await CmdCapturar("sfc /scannow");

        if (resSfc.Contains("did not find any integrity") || resSfc.Contains("no encontró ninguna infracción"))
            Log("   ✅ ESTADO ARCHIVOS: Integridad verificada.");
        else if (resSfc.Contains("successfully repaired") || resSfc.Contains("reparó correctamente"))
            Log("   🛠️ ESTADO ARCHIVOS: Errores detectados y reparados.");
        else
            Log("   ❌ ESTADO ARCHIVOS: Errores complejos. Continuando con DISM para reparar la imagen...");

        // [3/6] DISM ScanHealth: análisis de la imagen del sistema
        Log("[3/6] DISM ScanHealth...");
        await Cmd("DISM.exe /Online /Cleanup-Image /ScanHealth >nul 2>&1");

        // [4/6] DISM CheckHealth: comprobación rápida de estado
        Log("[4/6] DISM CheckHealth...");
        await Cmd("DISM.exe /Online /Cleanup-Image /CheckHealth >nul 2>&1");

        // [5/6] DISM RestoreHealth: reparación real de la imagen (puede tardar)
        Log("[5/6] DISM RestoreHealth (puede tardar varios minutos)...");
        string resDism = await CmdCapturar("DISM.exe /Online /Cleanup-Image /RestoreHealth");

        if (resDism.Contains("successfully") || resDism.Contains("correctamente"))
            Log("   ✅ ESTADO IMAGEN: Restauración completada.");
        else
            Log("   ⚠️ ESTADO IMAGEN: Hubo un problema al restaurar la imagen.");

        // [6/6] ComponentCleanup: libera espacio de componentes obsoletos del sistema
        Log("[6/6] Limpiando componentes obsoletos (ComponentCleanup)...");
        await Cmd("DISM.exe /Online /Cleanup-Image /StartComponentCleanup >nul 2>&1");
    }

    // ── Winget (nivel 4) ──────────────────────────────────────────────────────
    // Se abre en una ventana CMD separada para que el usuario pueda revisar y confirmar
    private async void EjecutarWinget()
    {
        txtLog.AppendText("\r\n>>> ⚡ INICIANDO GESTOR DE ACTUALIZACIONES...\r\n");
        txtLog.AppendText("Buscando Winget...\r\n");

        string rutaWinget = string.Empty;

        // Ruta habitual de Winget en Windows 10/11
        string posibleRuta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            @"Microsoft\WindowsApps\winget.exe");

        if (File.Exists(posibleRuta))
        {
            rutaWinget = posibleRuta;
        }
        else
        {
            // Intento alternativo con PATH del sistema
            string check = await CmdCapturar("where winget");
            if (!check.Contains("no se pudo") && check.Length > 2)
                rutaWinget = "winget";
        }

        if (string.IsNullOrEmpty(rutaWinget))
        {
            MessageBox.Show("No se encontró Winget en este PC.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtLog.AppendText("❌ Error: Winget no encontrado.\r\n");
            return;
        }

        txtLog.AppendText("✅ Winget localizado.\r\n");
        txtLog.AppendText("Abriendo lista de actualizaciones. Revisa antes de aceptar.\r\n");

        try
        {
            // Flujo seguro: muestra lista → avisa → pausa → actualiza solo si el usuario no cierra
            string cmdWinget = "\"" + rutaWinget + "\"";
            string args = "/c \"" +
                cmdWinget + " upgrade " +
                "& echo. & echo ======================================================= " +
                "& echo   ATENCION: REVISA LA LISTA DE ARRIBA " +
                "& echo ======================================================= " +
                "& echo. & echo   Si NO quieres actualizar algun programa, CIERRA ESTA VENTANA. " +
                "& echo   Si quieres actualizar TODO, presiona una tecla... " +
                "& pause " +
                "& " + cmdWinget + " upgrade --all --include-unknown --accept-source-agreements --accept-package-agreements " +
                "& echo. & echo PROCESO FINALIZADO & pause\"";

            Process.Start(new ProcessStartInfo
            {
                FileName       = "cmd.exe",
                Arguments      = args,
                UseShellExecute = true   // Necesario para que abra ventana visible
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al lanzar Winget: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    // FUNCIONES AUXILIARES
    // ═════════════════════════════════════════════════════════════════════════

    // Habilita o deshabilita todos los botones (evita ejecutar dos procesos a la vez)
    private void ToggleBotones(bool estado)
    {
        btnNormal.Enabled = estado;
        btnDeep.Enabled   = estado;
        btnRepair.Enabled = estado;
        btnUpdate.Enabled = estado;
        btnCerrar.Enabled = estado;
    }

    // Ejecuta un comando en CMD sin capturar salida (fire-and-forget async)
    private async Task Cmd(string comando)
    {
        await Task.Run(() =>
        {
            try
            {
                using (var p = new Process())
                {
                    p.StartInfo = new ProcessStartInfo
                    {
                        FileName        = "cmd.exe",
                        Arguments       = "/c " + comando,
                        CreateNoWindow  = true,
                        UseShellExecute = false,
                        WindowStyle     = ProcessWindowStyle.Hidden
                    };
                    p.Start();
                    p.WaitForExit();
                }
            }
            catch { /* Se ignoran errores individuales de comandos de limpieza */ }
        });
    }

    // Ejecuta un comando y devuelve su salida estándar como string
    private async Task<string> CmdCapturar(string comando)
    {
        return await Task.Run(() =>
        {
            try
            {
                using (var p = new Process())
                {
                    p.StartInfo = new ProcessStartInfo
                    {
                        FileName               = "cmd.exe",
                        Arguments              = "/c " + comando,
                        CreateNoWindow         = true,
                        UseShellExecute        = false,
                        RedirectStandardOutput = true,
                        WindowStyle            = ProcessWindowStyle.Hidden
                    };
                    p.Start();
                    string salida = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();
                    return salida;
                }
            }
            catch { return string.Empty; }
        });
    }

    // Espacio libre en C: en bytes
    private long ObtenerEspacioLibre()
    {
        try { return new DriveInfo("C").TotalFreeSpace; }
        catch { return 0; }
    }

    // Convierte bytes a una cadena legible (KB, MB, GB…)
    private string FormatearTamano(long bytes)
    {
        string[] sufijos = { "B", "KB", "MB", "GB", "TB" };
        int i = 0;
        decimal valor = bytes;
        while (Math.Round(valor / 1024) >= 1 && i < sufijos.Length - 1)
        {
            valor /= 1024;
            i++;
        }
        return string.Format("{0:n1} {1}", valor, sufijos[i]);
    }

    // FIX: el Stream se cierra después de crear la imagen, lo que provoca que
    //       Image.FromStream falle en versiones modernas de .NET si el stream se cierra.
    //       Solución: copiar a MemoryStream y mantenerlo vivo.
    private Image CargarImagen(string recurso)
    {
        try
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var s = asm.GetManifestResourceStream(recurso))
            {
                if (s == null) return null;
                var ms = new MemoryStream();
                s.CopyTo(ms);
                ms.Position = 0;
                return Image.FromStream(ms);
            }
        }
        catch { return null; }
    }

    // Carga el icono incrustado
    private Icon CargarIcono(string recurso)
    {
        try
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var s = asm.GetManifestResourceStream(recurso))
            {
                return s != null ? new Icon(s) : null;
            }
        }
        catch { return null; }
    }

    // Obtiene nombre del SO y datos básicos del equipo
    private string CargarDatosSistema()
    {
        try
        {
            string nombreWindows = "Windows";
            try
            {
                const string key = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";
                object prodName = Registry.GetValue(key, "ProductName", "Windows");
                nombreWindows = prodName != null ? prodName.ToString() : "Windows";

                // A partir del build 22000, el nombre del registro todavía dice "Windows 10"
                object buildObj = Registry.GetValue(key, "CurrentBuild", "0");
                int build = 0;
                if (buildObj != null) int.TryParse(buildObj.ToString(), out build);
                if (build >= 22000)
                    nombreWindows = nombreWindows.Replace("Windows 10", "Windows 11");
            }
            catch { nombreWindows = "Windows Detectado"; }

            return "PC: " + Environment.MachineName + "   |   USUARIO: " + Environment.UserName + "\r\nSISTEMA: " + nombreWindows;
        }
        catch { return "Leedeo Cleaner Ready"; }
    }

    // Crea un botón de menú con barra de color lateral e imagen opcional
    private Button CrearBotonGrafico(string texto, string recursoImagen, int top, Color colorBarra)
    {
        var btn = new Button
        {
            Text      = " " + texto,
            Location  = new Point(10, top),
            Size      = new Size(220, 48),
            FlatStyle = FlatStyle.Flat,
            BackColor = cSidebar,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Font      = new Font("Segoe UI", 10),
            Cursor    = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;

        Image img = CargarImagen(recursoImagen);
        if (img != null)
        {
            btn.Image             = img;
            btn.ImageAlign        = ContentAlignment.MiddleLeft;
            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn.Padding           = new Padding(15, 0, 0, 0);
        }

        // Barra de color lateral (decorativa)
        var barra = new Panel
        {
            BackColor = colorBarra,
            Size      = new Size(4, 48),
            Location  = new Point(0, 0)
        };
        btn.Controls.Add(barra);

        return btn;
    }

    // Muestra descripción en el log al pasar el ratón por encima de un botón
    private void AsignarHover(Button btn, string descripcion)
    {
        btn.MouseEnter += (s, e) =>
        {
            btn.BackColor = Color.FromArgb(45, 45, 50);
            // Solo sobreescribe el log si está mostrando el mensaje de espera o una descripción anterior
            if (txtLog.Text.StartsWith("Sistema") ||
                txtLog.Text.StartsWith("►")       ||
                txtLog.Text.StartsWith("≡")       ||
                txtLog.Text.StartsWith("✚")       ||
                txtLog.Text.StartsWith("⚡"))
                txtLog.Text = descripcion;
        };
        btn.MouseLeave += (s, e) =>
        {
            btn.BackColor = cSidebar;
            if (txtLog.Text == descripcion)
                txtLog.Text = "Sistema listo. Esperando órdenes...";
        };
    }

    // Escribe una línea en el log de forma thread-safe
    private void Log(string texto)
    {
        if (InvokeRequired)
            Invoke(new Action<string>(Log), texto);
        else
        {
            txtLog.AppendText("\r\n" + texto);
            txtLog.ScrollToCaret();
        }
    }

    // ── Arrastre de ventana ───────────────────────────────────────────────────
    private void MouseDownDrag(object sender, MouseEventArgs e) { dragging = true;  dragCursorPoint = Cursor.Position; dragFormPoint = Location; }
    private void MouseMoveDrag(object sender, MouseEventArgs e) { if (dragging) Location = Point.Add(dragFormPoint, new Size(Point.Subtract(Cursor.Position, new Size(dragCursorPoint)))); }
    private void MouseUpDrag  (object sender, MouseEventArgs e) { dragging = false; }

    // ── Punto de entrada ──────────────────────────────────────────────────────
    [STAThread]
    static void Main()
    {
        try
        {
            // Si no es administrador, relanza el proceso solicitando elevación
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName       = Application.ExecutablePath,
                    Verb           = "runas",
                    UseShellExecute = true
                });
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false); // Buena práctica en WinForms
            Application.Run(new LimpiadorApp());
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al iniciar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
