using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Reflection; 

// --- METADATOS DEL EJECUTABLE (Version 1.0 Real) ---
[assembly: AssemblyTitle("Leedeo Cleaner")]
[assembly: AssemblyDescription("Herramienta de optimización y limpieza para Windows")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Leedeo Studio")]
[assembly: AssemblyProduct("Leedeo Cleaner")]
[assembly: AssemblyCopyright("Copyright © 2025 Leedeo Studio")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

public class LimpiadorApp : Form
{
    // COLORES CORPORATIVOS
    Color cSidebar    = Color.FromArgb(32, 32, 32);     
    Color cFondo      = Color.FromArgb(18, 18, 18);     
    Color cBtnNormal  = Color.FromArgb(0, 120, 215);    
    Color cBtnDeep    = Color.FromArgb(210, 60, 60);    
    Color cBtnRepair  = Color.FromArgb(39, 174, 96);    
    Color cLeedeo     = ColorTranslator.FromHtml("#914d97"); 

    private Panel sidebar, mainContent;
    private PictureBox pbLogo;
    private Label lblTitulo, lblVersion, lblSystemInfo, lblDonar;
    private LinkLabel linkKofi;
    private TextBox txtLog;
    private Button btnNormal, btnDeep, btnRepair, btnExit;

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

        this.Icon = CargarIcono("App.icono.ico");

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

        // VERSION TEXTO
        lblVersion = new Label();
        lblVersion.Text = "v1.0";
        lblVersion.Font = new Font("Segoe UI", 9, FontStyle.Regular);
        lblVersion.ForeColor = Color.Gray; 
        lblVersion.Location = new Point(20, 165);
        lblVersion.Size = new Size(200, 30);
        lblVersion.TextAlign = ContentAlignment.TopCenter;
        sidebar.Controls.Add(lblVersion);

        // BOTONES MENU
        btnNormal = CrearBotonGrafico("LIMPIEZA RÁPIDA", "App.btn_rapida.png", 220, cBtnNormal);
        btnDeep   = CrearBotonGrafico("LIMPIEZA PROFUNDA", "App.btn_profunda.png", 275, cBtnDeep);
        btnRepair = CrearBotonGrafico("REPARAR SISTEMA", "App.btn_reparar.png", 330, cBtnRepair);

        // HOVER
        AsignarHover(btnNormal, "► LIMPIEZA RÁPIDA\r\n\r\nBorra temporales y caché DNS al instante.\r\nPerfecto para el mantenimiento diario.");
        AsignarHover(btnDeep, "≡ LIMPIEZA PROFUNDA\r\n\r\nElimina Logs, Updates viejos y basura.\r\nRecomendado usar una vez al mes.");
        AsignarHover(btnRepair, "✚ REPARACIÓN TOTAL\r\n\r\nEjecuta diagnósticos (SFC/DISM).\r\nÚsalo solo si notas errores en Windows.");

        // CLICK
        btnNormal.Click += (s, e) => Ejecutar(1);
        btnDeep.Click += (s, e) => Ejecutar(2);
        btnRepair.Click += (s, e) => Ejecutar(3);

        sidebar.Controls.Add(btnNormal);
        sidebar.Controls.Add(btnDeep);
        sidebar.Controls.Add(btnRepair);

        // --- BOTON CERRAR APP (Ajustado) ---
        btnExit = CrearBotonGrafico("CERRAR LA APP", "App.btn_salir.png", 463, cLeedeo);
        btnExit.Size = new Size(240, 55);
        btnExit.Location = new Point(0, 463);
        btnExit.BackColor = cLeedeo; 
        btnExit.Font = new Font("Segoe UI", 11, FontStyle.Bold); 
        btnExit.ForeColor = Color.White;
        btnExit.FlatAppearance.MouseOverBackColor = ControlPaint.Light(cLeedeo);
        
        // Ocultamos la barrita lateral pequeña
        foreach(Control c in btnExit.Controls) { c.Visible = false; }

        btnExit.Click += (s,e) => Application.Exit();
        sidebar.Controls.Add(btnExit);

        // CONTENIDO PRINCIPAL
        mainContent = new Panel();
        mainContent.Location = new Point(242, 1); 
        mainContent.Size = new Size(537, 518);
        mainContent.BackColor = cFondo;
        mainContent.MouseDown += MouseDownDrag;
        mainContent.MouseMove += MouseMoveDrag;
        mainContent.MouseUp += MouseUpDrag;
        this.Controls.Add(mainContent);

        // 1. INFO SISTEMA
        lblSystemInfo = new Label();
        lblSystemInfo.AutoSize = true;
        lblSystemInfo.Location = new Point(30, 20);
        lblSystemInfo.Font = new Font("Segoe UI", 10, FontStyle.Regular);
        lblSystemInfo.ForeColor = Color.White;
        lblSystemInfo.Text = CargarDatosSistema();
        mainContent.Controls.Add(lblSystemInfo);

        // 2. TEXTO DONACION
        lblDonar = new Label();
        lblDonar.Text = "Si esta herramienta te ha ahorrado tiempo, considera apoyar el proyecto:";
        lblDonar.ForeColor = Color.Gray;
        lblDonar.Font = new Font("Segoe UI", 9, FontStyle.Italic);
        lblDonar.AutoSize = true;
        lblDonar.Location = new Point(30, 75);
        mainContent.Controls.Add(lblDonar);

        // 3. LINK KO-FI
        linkKofi = new LinkLabel();
        linkKofi.Text = "☕ Apoyar en Ko-fi (Leedeo)";
        linkKofi.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        linkKofi.LinkColor = cLeedeo; 
        linkKofi.ActiveLinkColor = ControlPaint.Light(cLeedeo);
        linkKofi.VisitedLinkColor = cLeedeo;
        linkKofi.LinkBehavior = LinkBehavior.HoverUnderline; 
        linkKofi.AutoSize = true;
        linkKofi.Location = new Point(30, 95);
        linkKofi.LinkClicked += (s, e) => {
            try { Process.Start("https://ko-fi.com/leedeo"); } catch {}
        };
        mainContent.Controls.Add(linkKofi);

        // CONSOLA
        txtLog = new TextBox();
        txtLog.Multiline = true;
        txtLog.ReadOnly = true;
        txtLog.BackColor = Color.Black;
        txtLog.ForeColor = Color.Lime; 
        txtLog.Font = new Font("Consolas", 10);
        txtLog.BorderStyle = BorderStyle.None;
        txtLog.Location = new Point(30, 140);
        txtLog.Size = new Size(480, 350);
        txtLog.Text = "Sistema listo. Esperando órdenes...";
        mainContent.Controls.Add(txtLog);
    }

    // --- CARGADORES ---
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

    private string CargarDatosSistema()
    {
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

    // --- CREADOR DE BOTON GRAFICO ---
    private Button CrearBotonGrafico(string texto, string recursoImagen, int top, Color colorBarra)
    {
        Button btn = new Button();
        btn.Text = " " + texto; 
        btn.Location = new Point(10, top);
        btn.Size = new Size(220, 48);
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = cSidebar;
        btn.ForeColor = Color.White;
        
        // Alineacion a la izquierda (ESTANDAR para todos)
        btn.TextAlign = ContentAlignment.MiddleLeft; 
        btn.Font = new Font("Segoe UI", 10);
        btn.Cursor = Cursors.Hand;
        
        Image img = CargarImagen(recursoImagen);
        if (img != null) {
            btn.Image = img;
            btn.ImageAlign = ContentAlignment.MiddleLeft; 
            btn.TextImageRelation = TextImageRelation.ImageBeforeText; 
            btn.Padding = new Padding(15, 0, 0, 0); 
        }

        Panel barrita = new Panel();
        barrita.BackColor = colorBarra;
        barrita.Size = new Size(4, 48);
        barrita.Location = new Point(0, 0);
        btn.Controls.Add(barrita);
        
        return btn;
    }

    private void AsignarHover(Button btn, string desc)
    {
        btn.MouseEnter += (s, e) => {
            btn.BackColor = Color.FromArgb(45, 45, 50);
            if(txtLog.Text.StartsWith("Sistema") || txtLog.Text.StartsWith("►") || txtLog.Text.StartsWith("≡") || txtLog.Text.StartsWith("✚"))
                 txtLog.Text = desc;
        };
        btn.MouseLeave += (s, e) => {
            if (btn.Text != " CERRAR LA APP") btn.BackColor = cSidebar; 
            else btn.BackColor = cLeedeo;

            if(txtLog.Text == desc) 
                txtLog.Text = "Sistema listo. Esperando órdenes...";
        };
    }

    private void Log(string t) {
        if (InvokeRequired) Invoke(new Action<string>(Log), t);
        else { txtLog.AppendText("\r\n" + t); txtLog.ScrollToCaret(); }
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

    private async void Ejecutar(int nivel)
    {
        btnNormal.Enabled = false; btnDeep.Enabled = false; btnRepair.Enabled = false; btnExit.Enabled = false;
        txtLog.Clear();

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
            Log(">>> ✚ REPARACIÓN DE SISTEMA");
            Log("NOTA: Este proceso puede tardar 20-30 minutos.");
            Log("No cierres la ventana hasta que termine.");
            
            Log("[1/5] SFC /SCANNOW...");
            await Cmd("sfc /scannow");
            Log("[2/5] DISM ScanHealth...");
            await Cmd("DISM.exe /Online /Cleanup-Image /ScanHealth");
            Log("[3/5] DISM CheckHealth...");
            await Cmd("DISM.exe /Online /Cleanup-Image /CheckHealth");
            Log("[4/5] DISM RestoreHealth...");
            await Cmd("DISM.exe /Online /Cleanup-Image /RestoreHealth");
            Log("[5/5] DISM ComponentCleanup...");
            await Cmd("DISM.exe /Online /Cleanup-Image /startComponentCleanup");
        }

        Log("");
        Log(">>> ✅ PROCESO FINALIZADO.");
        MessageBox.Show("Operación completada con éxito.", "Leedeo Cleaner");
        btnNormal.Enabled = true; btnDeep.Enabled = true; btnRepair.Enabled = true; btnExit.Enabled = true;
    }

    private void MouseDownDrag(object sender, MouseEventArgs e) { dragging = true; dragCursorPoint = Cursor.Position; dragFormPoint = this.Location; }
    private void MouseMoveDrag(object sender, MouseEventArgs e) { if (dragging) { Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint)); this.Location = Point.Add(dragFormPoint, new Size(dif)); } }
    private void MouseUpDrag(object sender, MouseEventArgs e) { dragging = false; }

    [STAThread]
    static void Main() {
        if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)) {
             Process.Start(new ProcessStartInfo { FileName = Application.ExecutablePath, Verb = "runas", UseShellExecute = true });
             return;
        }
        Application.EnableVisualStyles();
        Application.Run(new LimpiadorApp());
    }
}