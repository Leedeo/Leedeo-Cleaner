using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Reflection; 

// LEEDEO CLEANER v1.2
// Diseño limpio (X arriba a la derecha) + Confirmaciones de seguridad

[assembly: AssemblyTitle("Leedeo Cleaner")]
[assembly: AssemblyDescription("Herramienta de optimización y limpieza")]
[assembly: AssemblyCompany("Leedeo Studio")]
[assembly: AssemblyProduct("Leedeo Cleaner")]
[assembly: AssemblyCopyright("Copyright © 2025 Leedeo Studio")]
[assembly: AssemblyVersion("1.2.0.0")]
[assembly: AssemblyFileVersion("1.2.0.0")]

public class LimpiadorApp : Form
{
    // COLORES
    Color cSidebar    = Color.FromArgb(32, 32, 32);     
    Color cFondo      = Color.FromArgb(18, 18, 18);     
    Color cBtnNormal  = Color.FromArgb(0, 120, 215);    
    Color cBtnDeep    = Color.FromArgb(210, 60, 60);    
    Color cBtnRepair  = Color.FromArgb(39, 174, 96);    
    Color cLeedeo     = ColorTranslator.FromHtml("#914d97"); 
    Color cVerde      = Color.Lime;

    private Panel sidebar, mainContent;
    private PictureBox pbLogo;
    private Label lblTitulo, lblVersion, lblSystemInfo, lblDonar;
    private LinkLabel linkKofi;
    private TextBox txtLog;
    private Button btnNormal, btnDeep, btnRepair, btnCerrar;

    private bool dragging = false;
    private Point dragCursorPoint, dragFormPoint;

    public LimpiadorApp()
    {
        // VENTANA
        this.FormBorderStyle = FormBorderStyle.None;
        this.Size = new Size(780, 520);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = cFondo;
        this.Padding = new Padding(1);
        this.Text = "Leedeo Cleaner";

        try { this.Icon = CargarIcono("App.icono.ico"); } catch {}

        this.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(Color.FromArgb(60,60,60)), 0, 0, Width-1, Height-1);

        // SIDEBAR
        sidebar = new Panel();
        sidebar.Location = new Point(1, 1);
        sidebar.Size = new Size(240, 518);
        sidebar.BackColor = cSidebar;
        sidebar.MouseDown += MouseDownDrag;
        sidebar.MouseMove += MouseMoveDrag;
        sidebar.MouseUp += MouseUpDrag;
        this.Controls.Add(sidebar);

        // LOGO
        pbLogo = new PictureBox();
        pbLogo.Size = new Size(160, 80);
        pbLogo.Location = new Point(40, 20);
        pbLogo.SizeMode = PictureBoxSizeMode.Zoom;
        pbLogo.Image = CargarImagen("App.logo.png");
        sidebar.Controls.Add(pbLogo);

        // TITULO
        lblTitulo = new Label();
        lblTitulo.Text = "LEEDEO\nCLEANER";
        lblTitulo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        lblTitulo.ForeColor = Color.White;
        lblTitulo.Location = new Point(20, 110);
        lblTitulo.Size = new Size(200, 55);
        lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
        sidebar.Controls.Add(lblTitulo);

        // VERSION
        lblVersion = new Label();
        lblVersion.Text = "v1.2";
        lblVersion.Font = new Font("Segoe UI", 9, FontStyle.Regular);
        lblVersion.ForeColor = Color.Gray; 
        lblVersion.Location = new Point(20, 165);
        lblVersion.Size = new Size(200, 30);
        lblVersion.TextAlign = ContentAlignment.TopCenter;
        sidebar.Controls.Add(lblVersion);

        // BOTONES (Solo los 3 de acción)
        btnNormal = CrearBotonGrafico("LIMPIEZA RÁPIDA", "App.btn_rapida.png", 220, cBtnNormal);
        btnDeep   = CrearBotonGrafico("LIMPIEZA PROFUNDA", "App.btn_profunda.png", 275, cBtnDeep);
        btnRepair = CrearBotonGrafico("REPARAR SISTEMA", "App.btn_reparar.png", 330, cBtnRepair);

        // HOVER
        AsignarHover(btnNormal, "► LIMPIEZA RÁPIDA\r\n\r\nBorra temporales y caché DNS al instante.\r\nPerfecto para el mantenimiento diario.");
        AsignarHover(btnDeep, "≡ LIMPIEZA PROFUNDA\r\n\r\nElimina Logs, Updates viejos y basura.\r\nRecomendado usar una vez al mes.");
        AsignarHover(btnRepair, "✚ REPARACIÓN TOTAL\r\n\r\nAnaliza y repara Disco, Archivos e Imagen.\r\nSi detecta errores, te ayudará a corregirlos.");

        // CLICK
        btnNormal.Click += (s, e) => Ejecutar(1);
        btnDeep.Click += (s, e) => Ejecutar(2);
        btnRepair.Click += (s, e) => Ejecutar(3);

        sidebar.Controls.Add(btnNormal);
        sidebar.Controls.Add(btnDeep);
        sidebar.Controls.Add(btnRepair);

        // CONTENIDO
        mainContent = new Panel();
        mainContent.Location = new Point(242, 1); 
        mainContent.Size = new Size(537, 518);
        mainContent.BackColor = cFondo;
        mainContent.MouseDown += MouseDownDrag;
        mainContent.MouseMove += MouseMoveDrag;
        mainContent.MouseUp += MouseUpDrag;
        this.Controls.Add(mainContent);

        // --- BOTON CERRAR SUPERIOR DERECHO ---
        btnCerrar = new Button();
        btnCerrar.Text = "\u2715"; // X limpia
        btnCerrar.Font = new Font("Segoe UI", 12, FontStyle.Regular);
        btnCerrar.Size = new Size(45, 30);
        btnCerrar.Location = new Point(mainContent.Width - 45, 0); 
        btnCerrar.FlatStyle = FlatStyle.Flat;
        btnCerrar.FlatAppearance.BorderSize = 0;
        btnCerrar.ForeColor = Color.Gray;
        btnCerrar.BackColor = cFondo; 
        
        // Efecto Hover Rojo
        btnCerrar.FlatAppearance.MouseOverBackColor = Color.Red; 
        btnCerrar.FlatAppearance.MouseDownBackColor = Color.DarkRed;
        btnCerrar.MouseEnter += (s, e) => btnCerrar.ForeColor = Color.White; 
        btnCerrar.MouseLeave += (s, e) => btnCerrar.ForeColor = Color.Gray;  
        
        btnCerrar.Click += (s, e) => Application.Exit();
        mainContent.Controls.Add(btnCerrar);

        // INFO
        lblSystemInfo = new Label();
        lblSystemInfo.AutoSize = true;
        lblSystemInfo.Location = new Point(30, 20);
        lblSystemInfo.Font = new Font("Segoe UI", 10, FontStyle.Regular);
        lblSystemInfo.ForeColor = Color.White;
        lblSystemInfo.Text = CargarDatosSistema();
        mainContent.Controls.Add(lblSystemInfo);

        // DONAR
        lblDonar = new Label();
        lblDonar.Text = "Si esta herramienta te ha ahorrado tiempo, considera apoyar el proyecto:";
        lblDonar.ForeColor = Color.Gray;
        lblDonar.Font = new Font("Segoe UI", 9, FontStyle.Italic);
        lblDonar.AutoSize = true;
        lblDonar.Location = new Point(30, 75);
        mainContent.Controls.Add(lblDonar);

        linkKofi = new LinkLabel();
        linkKofi.Text = "☕ Apoyar en Ko-fi (Leedeo)";
        linkKofi.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        linkKofi.LinkColor = cLeedeo; 
        linkKofi.ActiveLinkColor = ControlPaint.Light(cLeedeo);
        linkKofi.VisitedLinkColor = cLeedeo;
        linkKofi.LinkBehavior = LinkBehavior.HoverUnderline; 
        linkKofi.AutoSize = true;
        linkKofi.Location = new Point(30, 95);
        linkKofi.LinkClicked += (s, e) => { try { Process.Start("https://ko-fi.com/leedeo"); } catch {} };
        mainContent.Controls.Add(linkKofi);

        // LOG
        txtLog = new TextBox();
        txtLog.Multiline = true;
        txtLog.ReadOnly = true;
        txtLog.BackColor = Color.Black;
        txtLog.ForeColor = cVerde; 
        txtLog.Font = new Font("Consolas", 10);
        txtLog.BorderStyle = BorderStyle.None;
        txtLog.Location = new Point(30, 140);
        txtLog.Size = new Size(480, 350);
        txtLog.Text = "Sistema listo. Esperando órdenes...";
        mainContent.Controls.Add(txtLog);
    }

    // --- LOGICA ---
    private async void Ejecutar(int nivel)
    {
        // 1. CONFIRMACIÓN PREVIA
        string tituloConfirm = "";
        string msgConfirm = "";

        if (nivel == 1) {
            tituloConfirm = "Confirmar Limpieza Rápida";
            msgConfirm = "¿Estás seguro de iniciar la Limpieza Rápida?\n\nSe borrarán archivos temporales y se reiniciará el explorador.";
        }
        else if (nivel == 2) {
            tituloConfirm = "Confirmar Limpieza Profunda";
            msgConfirm = "¿Deseas iniciar la Limpieza Profunda?\n\nSe eliminarán historiales, logs de sistema y caché de actualizaciones.";
        }
        else if (nivel == 3) {
            tituloConfirm = "Confirmar Reparación de Sistema";
            msgConfirm = "ATENCIÓN: Este proceso es exhaustivo y puede tardar entre 20 y 30 minutos.\n\n¿Quieres comenzar el diagnóstico y reparación?";
        }

        if (MessageBox.Show(msgConfirm, tituloConfirm, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

        // 2. INICIO
        btnNormal.Enabled = false; btnDeep.Enabled = false; btnRepair.Enabled = false; btnCerrar.Enabled = false;
        txtLog.Clear();

        long espacioInicio = ObtenerEspacioLibre();
        Log("Espacio libre inicial: " + FormatearTamano(espacioInicio));
        Log("------------------------------------------------");

        if (nivel == 1) {
            Log(">>> ► INICIANDO LIMPIEZA RÁPIDA...");
            await Cmd("del /s /q %TEMP%\\* >nul 2>&1");
            await Cmd("del /s /q C:\\Windows\\Temp\\* >nul 2>&1");
            await Cmd("ipconfig /flushdns");
            Log("> Reiniciando Explorer...");
            await Cmd("taskkill /f /im explorer.exe >nul 2>&1");
            await Cmd("del /q %LOCALAPPDATA%\\IconCache.db >nul 2>&1");
            Process.Start("explorer.exe");
        }

        if (nivel == 2) {
            Log(">>> ≡ INICIANDO LIMPIEZA PROFUNDA...");
            await Cmd("del /s /q %TEMP%\\* >nul 2>&1");
            await Cmd("del /s /q C:\\Windows\\Temp\\* >nul 2>&1");
            Log("> Gestionando Windows Update...");
            await Cmd("net stop wuauserv >nul 2>&1");
            await Cmd("net stop bits >nul 2>&1");
            await Cmd("del /s /q C:\\Windows\\SoftwareDistribution\\Download\\* >nul 2>&1");
            await Cmd("net start wuauserv >nul 2>&1");
            Log("> Eliminando Logs y Rastros...");
            await Cmd("for /f \"tokens=*\" %i in ('wevtutil el') do wevtutil cl \"%i\"");
            await Cmd("del /s /q \"C:\\ProgramData\\Microsoft\\Windows Defender\\Scans\\History\\*\" >nul 2>&1");
            await Cmd("del /s /q C:\\Windows\\Prefetch\\* >nul 2>&1");
        }

        if (nivel == 3) {
            Log(">>> ✚ REPARACIÓN DE SISTEMA (ANÁLISIS INTELIGENTE)");
            Log("Iniciando diagnóstico...");
            Log("");

            // 1. DISCO
            Log("[1/6] Analizando salud del Disco (CHKDSK)..."); 
            string resDisco = await CmdCapturar("chkdsk C: /scan");
            
            if (resDisco.Contains("no ha encontrado problemas") || resDisco.Contains("found no problems"))
            {
                Log("   ✅ ESTADO DISCO: Sano. Sin errores físicos.");
            }
            else
            {
                Log("   ⚠️ ESTADO DISCO: Se detectaron anomalías.");
                DialogResult preg = MessageBox.Show(
                    "Leedeo Cleaner ha detectado errores en tu disco.\n\nWindows no puede repararlos mientras se usa.\n¿Deseas programar una reparación automática para el próximo reinicio?", 
                    "Errores de Disco Detectados", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (preg == DialogResult.Yes)
                {
                    await Cmd("fsutil dirty set C:");
                    Log("   --> Reparación programada. Se ejecutará al reiniciar Windows.");
                    MessageBox.Show("Listo. La reparación se iniciará automáticamente la próxima vez que reinicies el PC.", "Programado");
                }
                else { Log("   --> El usuario omitió la reparación de disco."); }
            }

            // 2. ARCHIVOS SFC
            Log("[2/6] Verificando integridad de archivos (SFC)...");
            string resSfc = await CmdCapturar("sfc /scannow");
            if (resSfc.Contains("did not find any integrity") || resSfc.Contains("no encontró ninguna infracción"))
                Log("   ✅ ESTADO ARCHIVOS: Integridad verificada. Todo correcto.");
            else if (resSfc.Contains("successfully repaired") || resSfc.Contains("reparó correctamente"))
                Log("   🛠️ ESTADO ARCHIVOS: Se encontraron errores y FUERON REPARADOS.");
            else
                Log("   ❌ ESTADO ARCHIVOS: Errores complejos. DISM intentará repararlos ahora...");

            // 3. IMAGEN DISM
            Log("[3/6] DISM ScanHealth...");
            await Cmd("DISM.exe /Online /Cleanup-Image /ScanHealth");
            
            Log("[4/6] DISM CheckHealth...");
            await Cmd("DISM.exe /Online /Cleanup-Image /CheckHealth");
            
            Log("[5/6] DISM RestoreHealth (Reparando Imagen)...");
            string resDism = await CmdCapturar("DISM.exe /Online /Cleanup-Image /RestoreHealth");
            if (resDism.Contains("successfully") || resDism.Contains("correctamente"))
                Log("   ✅ ESTADO IMAGEN: Restauración completada.");
            else
                Log("   ⚠️ ESTADO IMAGEN: Hubo un problema al restaurar la imagen.");

            Log("[6/6] Limpiando componentes obsoletos...");
            await Cmd("DISM.exe /Online /Cleanup-Image /startComponentCleanup");
        }

        long espacioFin = ObtenerEspacioLibre();
        long bytesLiberados = espacioFin - espacioInicio;
        if (bytesLiberados < 0) bytesLiberados = 0; 
        string textoLiberado = FormatearTamano(bytesLiberados);

        Log("");
        Log("------------------------------------------------");
        Log(">>> ✅ PROCESO FINALIZADO.");
        Log(">>> ESPACIO LIBERADO: " + textoLiberado);
        
        MessageBox.Show("Operación completada con éxito.\n\nEspacio recuperado: " + textoLiberado, "Leedeo Cleaner");
        
        btnNormal.Enabled = true; btnDeep.Enabled = true; btnRepair.Enabled = true; btnCerrar.Enabled = true;
    }

    // --- FUNCIONES ---
    private async Task<string> CmdCapturar(string c) {
        return await Task.Run(() => {
            try {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/c " + c;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false; 
                p.StartInfo.RedirectStandardOutput = true; 
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.Start();
                string output = p.StandardOutput.ReadToEnd(); 
                p.WaitForExit();
                return output;
            } catch { return "Error"; }
        });
    }

    private async Task Cmd(string c) {
        await Task.Run(() => {
            try {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/c " + c;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.Start();
                p.WaitForExit();
            } catch {}
        });
    }

    private long ObtenerEspacioLibre() { try { return new DriveInfo("C").TotalFreeSpace; } catch { return 0; } }
    private string FormatearTamano(long bytes) {
        string[] sufijos = { "B", "KB", "MB", "GB", "TB" };
        int contador = 0;
        decimal numero = (decimal)bytes;
        while (Math.Round(numero / 1024) >= 1) { numero = numero / 1024; contador++; }
        return string.Format("{0:n1} {1}", numero, sufijos[contador]);
    }
    private Image CargarImagen(string recurso) {
        try {
            var asm = Assembly.GetExecutingAssembly();
            using (var s = asm.GetManifestResourceStream(recurso)) { return s != null ? Image.FromStream(s) : null; }
        } catch { return null; }
    }
    private Icon CargarIcono(string recurso) {
        try {
            var asm = Assembly.GetExecutingAssembly();
            using (var s = asm.GetManifestResourceStream(recurso)) { return s != null ? new Icon(s) : null; }
        } catch { return null; }
    }
    private string CargarDatosSistema() {
        try {
            string nombreWindows = "Windows";
            try {
                string key = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";
                nombreWindows = Registry.GetValue(key, "ProductName", "Windows").ToString();
                string currentBuildObj = Registry.GetValue(key, "CurrentBuild", "0").ToString();
                int buildNumber = 0;
                int.TryParse(currentBuildObj, out buildNumber);
                if (buildNumber >= 22000) nombreWindows = nombreWindows.Replace("Windows 10", "Windows 11");
            } catch { nombreWindows = "Windows Detectado"; }
            string pcName = Environment.MachineName;
            string userName = Environment.UserName;
            return "PC: " + pcName + "   |   USUARIO: " + userName + "\r\nSISTEMA: " + nombreWindows;
        } catch { return "Leedeo Cleaner Ready"; }
    }
    private Button CrearBotonGrafico(string texto, string recursoImagen, int top, Color colorBarra) {
        Button btn = new Button();
        btn.Text = " " + texto; btn.Location = new Point(10, top); btn.Size = new Size(220, 48);
        btn.FlatStyle = FlatStyle.Flat; btn.FlatAppearance.BorderSize = 0; btn.BackColor = cSidebar; btn.ForeColor = Color.White;
        btn.TextAlign = ContentAlignment.MiddleLeft; btn.Font = new Font("Segoe UI", 10); btn.Cursor = Cursors.Hand;
        Image img = CargarImagen(recursoImagen);
        if (img != null) {
            btn.Image = img; btn.ImageAlign = ContentAlignment.MiddleLeft; btn.TextImageRelation = TextImageRelation.ImageBeforeText; btn.Padding = new Padding(15, 0, 0, 0); 
        }
        Panel barrita = new Panel(); barrita.BackColor = colorBarra; barrita.Size = new Size(4, 48); barrita.Location = new Point(0, 0);
        btn.Controls.Add(barrita);
        return btn;
    }
    private void AsignarHover(Button btn, string desc) {
        btn.MouseEnter += (s, e) => {
            btn.BackColor = Color.FromArgb(45, 45, 50);
            if(txtLog.Text.StartsWith("Sistema") || txtLog.Text.StartsWith("►") || txtLog.Text.StartsWith("≡") || txtLog.Text.StartsWith("✚")) txtLog.Text = desc;
        };
        btn.MouseLeave += (s, e) => {
            btn.BackColor = cSidebar;
            if(txtLog.Text == desc) txtLog.Text = "Sistema listo. Esperando órdenes...";
        };
    }
    private void Log(string t) {
        if (InvokeRequired) Invoke(new Action<string>(Log), t);
        else { txtLog.AppendText("\r\n" + t); txtLog.ScrollToCaret(); }
    }
    private void MouseDownDrag(object sender, MouseEventArgs e) { dragging = true; dragCursorPoint = Cursor.Position; dragFormPoint = this.Location; }
    private void MouseMoveDrag(object sender, MouseEventArgs e) { if (dragging) { Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint)); this.Location = Point.Add(dragFormPoint, new Size(dif)); } }
    private void MouseUpDrag(object sender, MouseEventArgs e) { dragging = false; }

    [STAThread]
    static void Main() {
        try {
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)) {
                 Process.Start(new ProcessStartInfo { FileName = Application.ExecutablePath, Verb = "runas", UseShellExecute = true });
                 return;
            }
            Application.EnableVisualStyles();
            Application.Run(new LimpiadorApp());
        } catch (Exception ex) {
            MessageBox.Show("Error al iniciar: " + ex.Message);
        }
    }
}