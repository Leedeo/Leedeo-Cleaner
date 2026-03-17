using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Reflection;

// =============================================================================
// MainWindow.cs - Leedeo Cleaner v1.6
// Multi-language support via Strings.cs (EN, ES, PT, IT, FR, DE).
// Language is auto-detected from the OS on startup and can be changed at any
// time using the flag buttons at the bottom of the sidebar.
// =============================================================================

[assembly: AssemblyTitle("Leedeo Cleaner")]
[assembly: AssemblyDescription("Windows optimization and cleaning tool")]
[assembly: AssemblyCompany("Leedeo Studio")]
[assembly: AssemblyProduct("Leedeo Cleaner")]
[assembly: AssemblyCopyright("Copyright © 2025 Leedeo Studio")]
[assembly: AssemblyVersion("1.6.0.0")]
[assembly: AssemblyFileVersion("1.6.0.0")]

public class MainWindow : Form
{
    // ── Color palette ─────────────────────────────────────────────────────────
    private readonly Color cSidebar      = Color.FromArgb(28, 28, 30);
    private readonly Color cSidebarHover = Color.FromArgb(40, 40, 44);
    private readonly Color cBackground   = Color.FromArgb(16, 16, 18);
    private readonly Color cLogBg        = Color.FromArgb(10, 10, 12);
    private readonly Color cBtnQuick     = Color.FromArgb(0, 112, 200);
    private readonly Color cBtnDeep      = Color.FromArgb(196, 52, 52);
    private readonly Color cBtnRepair    = Color.FromArgb(34, 160, 84);
    private readonly Color cBtnUpdate    = Color.FromArgb(220, 120, 0);
    private readonly Color cBrand        = ColorTranslator.FromHtml("#914d97");
    private readonly Color cLogText      = Color.FromArgb(0, 220, 100);
    private readonly Color cDivider      = Color.FromArgb(45, 45, 48);
    private readonly Color cTextDim      = Color.FromArgb(130, 130, 140);

    private readonly Color cBtnRegistry  = Color.FromArgb(100, 80, 160);    // muted purple, distinct from brand

    // ── Controls ──────────────────────────────────────────────────────────────
    private Panel      sidebar, mainContent;
    private PictureBox logo;
    private Label      lblTitle, lblVersion, lblSystemInfo, lblDonate;
    private LinkLabel  linkKofi;
    private TextBox    txtLog;
    private Button     btnQuick, btnDeep, btnRepair, btnUpdate, btnRegistry, btnClose, btnSaveLog;

    // Flag buttons — one per supported language
    private Button btnFlagEn, btnFlagEs, btnFlagPt, btnFlagIt, btnFlagFr, btnFlagDe;

    // ── Window drag state ─────────────────────────────────────────────────────
    private bool  dragging;
    private Point dragCursorPoint, dragFormPoint;

    // ── Constructor ───────────────────────────────────────────────────────────
    public MainWindow()
    {
        InitializeWindow();
        InitializeSidebar();
        InitializeMainContent();
    }

    // ── Borderless window setup ───────────────────────────────────────────────
    private void InitializeWindow()
    {
        FormBorderStyle = FormBorderStyle.None;
        Size            = new Size(780, 520);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = cBackground;
        Padding         = new Padding(1);
        Text            = "Leedeo Cleaner";

        try { Icon = LoadIcon("App.icon.ico"); } catch { }

        // Single-pixel border with a subtle brand tint
        Paint += (s, e) =>
        {
            using (var pen = new Pen(Color.FromArgb(80, 145, 77, 151)))
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        };
    }

    // ── Left sidebar ──────────────────────────────────────────────────────────
    private void InitializeSidebar()
    {
        sidebar = new Panel
        {
            Location  = new Point(1, 1),
            Size      = new Size(240, 518),
            BackColor = cSidebar
        };
        sidebar.MouseDown += OnMouseDown;
        sidebar.MouseMove += OnMouseMove;
        sidebar.MouseUp   += OnMouseUp;
        Controls.Add(sidebar);

        // App logo — reduced from 80 to 65px to recover vertical space for the 5th button
        logo = new PictureBox
        {
            Size     = new Size(150, 65),
            Location = new Point(45, 14),
            SizeMode = PictureBoxSizeMode.Zoom,
            Image    = LoadImage("App.logo.png")
        };
        sidebar.Controls.Add(logo);

        // App title
        lblTitle = new Label
        {
            Text      = "LEEDEO\nCLEANER",
            Font      = new Font("Segoe UI Semibold", 13, FontStyle.Regular),
            ForeColor = Color.White,
            Location  = new Point(20, 88),
            Size      = new Size(200, 48),
            TextAlign = ContentAlignment.MiddleCenter
        };
        sidebar.Controls.Add(lblTitle);

        // Version label
        lblVersion = new Label
        {
            Text      = "v1.6",
            Font      = new Font("Segoe UI", 8, FontStyle.Regular),
            ForeColor = cTextDim,
            Location  = new Point(20, 137),
            Size      = new Size(200, 18),
            TextAlign = ContentAlignment.TopCenter
        };
        sidebar.Controls.Add(lblVersion);

        // Thin divider between header and menu buttons
        var dividerTop = new Panel
        {
            Location  = new Point(20, 160),
            Size      = new Size(200, 1),
            BackColor = cDivider
        };
        sidebar.Controls.Add(dividerTop);

        // Menu buttons — grouped by category: cleaning, repair, updates
        // Quick(168) → Deep(212) → Registry(256) → Repair(300) → Update(344)
        btnQuick    = CreateMenuButton(Strings.Get("btn_quick"),    "App.btn_quick.png",    168, cBtnQuick);
        btnDeep     = CreateMenuButton(Strings.Get("btn_deep"),     "App.btn_deep.png",     212, cBtnDeep);
        btnRegistry = CreateMenuButton(Strings.Get("btn_registry"), "App.btn_registry.png", 256, cBtnRegistry);
        btnRepair   = CreateMenuButton(Strings.Get("btn_repair"),   "App.btn_repair.png",   300, cBtnRepair);
        btnUpdate   = CreateMenuButton(Strings.Get("btn_update"),   "App.btn_update.png",   344, cBtnUpdate);

        AssignHover(btnQuick,    "hover_quick");
        AssignHover(btnDeep,     "hover_deep");
        AssignHover(btnRegistry, "hover_registry");
        AssignHover(btnRepair,   "hover_repair");
        AssignHover(btnUpdate,   "hover_update");

        btnQuick.Click    += (s, e) => Run(1);
        btnDeep.Click     += (s, e) => Run(2);
        btnRegistry.Click += (s, e) => Run(5);
        btnRepair.Click   += (s, e) => Run(3);
        btnUpdate.Click   += (s, e) => Run(4);

        sidebar.Controls.Add(btnQuick);
        sidebar.Controls.Add(btnDeep);
        sidebar.Controls.Add(btnRegistry);
        sidebar.Controls.Add(btnRepair);
        sidebar.Controls.Add(btnUpdate);

        // Thin divider between menu buttons and language flags
        var dividerBottom = new Panel
        {
            Location  = new Point(20, 396),
            Size      = new Size(200, 1),
            BackColor = cDivider
        };
        sidebar.Controls.Add(dividerBottom);

        // ── Language flag buttons (bottom of sidebar) ─────────────────────────
        int flagY       = 412;
        int flagSize    = 24;
        int flagSpacing = 4;
        int totalWidth  = (flagSize * 6) + (flagSpacing * 5);   // 164px
        int flagStartX  = (240 - totalWidth) / 2;               // centered in 240px sidebar

        btnFlagEn = CreateFlagButton("App.flag_en.png", "English",    flagStartX + (flagSize + flagSpacing) * 0, flagY, flagSize);
        btnFlagEs = CreateFlagButton("App.flag_es.png", "Español",    flagStartX + (flagSize + flagSpacing) * 1, flagY, flagSize);
        btnFlagPt = CreateFlagButton("App.flag_pt.png", "Português",  flagStartX + (flagSize + flagSpacing) * 2, flagY, flagSize);
        btnFlagIt = CreateFlagButton("App.flag_it.png", "Italiano",   flagStartX + (flagSize + flagSpacing) * 3, flagY, flagSize);
        btnFlagFr = CreateFlagButton("App.flag_fr.png", "Français",   flagStartX + (flagSize + flagSpacing) * 4, flagY, flagSize);
        btnFlagDe = CreateFlagButton("App.flag_de.png", "Deutsch",    flagStartX + (flagSize + flagSpacing) * 5, flagY, flagSize);

        btnFlagEn.Click += (s, e) => ChangeLanguage("en");
        btnFlagEs.Click += (s, e) => ChangeLanguage("es");
        btnFlagPt.Click += (s, e) => ChangeLanguage("pt");
        btnFlagIt.Click += (s, e) => ChangeLanguage("it");
        btnFlagFr.Click += (s, e) => ChangeLanguage("fr");
        btnFlagDe.Click += (s, e) => ChangeLanguage("de");

        sidebar.Controls.Add(btnFlagEn);
        sidebar.Controls.Add(btnFlagEs);
        sidebar.Controls.Add(btnFlagPt);
        sidebar.Controls.Add(btnFlagIt);
        sidebar.Controls.Add(btnFlagFr);
        sidebar.Controls.Add(btnFlagDe);

        // Highlight the active language flag on startup
        UpdateFlagHighlight();
    }

    // ── Right panel (log + info) ──────────────────────────────────────────────
    private void InitializeMainContent()
    {
        mainContent = new Panel
        {
            Location  = new Point(242, 1),
            Size      = new Size(537, 518),
            BackColor = cBackground
        };
        mainContent.MouseDown += OnMouseDown;
        mainContent.MouseMove += OnMouseMove;
        mainContent.MouseUp   += OnMouseUp;
        Controls.Add(mainContent);

        // Close button
        btnClose = new Button
        {
            Text      = "\u2715",
            Font      = new Font("Segoe UI", 11, FontStyle.Bold),
            Size      = new Size(45, 30),
            Location  = new Point(mainContent.Width - 45, 0),
            FlatStyle = FlatStyle.Flat,
            BackColor = cBrand,
            ForeColor = Color.White,
            Cursor    = Cursors.Hand
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.Red;
        btnClose.MouseLeave += (s, e) => btnClose.BackColor = cBrand;
        btnClose.Click      += (s, e) => Application.Exit();
        mainContent.Controls.Add(btnClose);

        // System info — smaller and dimmer, it's secondary information
        lblSystemInfo = new Label
        {
            AutoSize  = true,
            Location  = new Point(30, 22),
            Font      = new Font("Segoe UI", 9, FontStyle.Regular),
            ForeColor = cTextDim,
            Text      = GetSystemInfo()
        };
        mainContent.Controls.Add(lblSystemInfo);

        // Donation label — italic, muted
        lblDonate = new Label
        {
            Text      = Strings.Get("lbl_donate"),
            ForeColor = Color.FromArgb(90, 90, 100),
            Font      = new Font("Segoe UI", 8, FontStyle.Italic),
            AutoSize  = true,
            Location  = new Point(30, 70)
        };
        mainContent.Controls.Add(lblDonate);

        // Ko-fi link — brand color, slightly smaller
        linkKofi = new LinkLabel
        {
            Text             = Strings.Get("lbl_kofi"),
            Font             = new Font("Segoe UI", 8, FontStyle.Bold),
            LinkColor        = cBrand,
            ActiveLinkColor  = ControlPaint.Light(cBrand),
            VisitedLinkColor = cBrand,
            LinkBehavior     = LinkBehavior.HoverUnderline,
            AutoSize         = true,
            Location         = new Point(30, 88)
        };
        linkKofi.LinkClicked += (s, e) =>
        {
            try { Process.Start(new ProcessStartInfo("https://ko-fi.com/leedeo") { UseShellExecute = true }); }
            catch { }
        };
        mainContent.Controls.Add(linkKofi);

        // Thin divider between header info and log
        var logDivider = new Panel
        {
            Location  = new Point(30, 110),
            Size      = new Size(480, 1),
            BackColor = cDivider
        };
        mainContent.Controls.Add(logDivider);

        // Terminal-style log area — scroll bar visible when content overflows
        txtLog = new TextBox
        {
            Multiline    = true,
            ReadOnly     = true,
            BackColor    = cLogBg,
            ForeColor    = cLogText,
            Font         = new Font("Consolas", 9),
            BorderStyle  = BorderStyle.None,
            Location     = new Point(30, 118),
            Size         = new Size(480, 332),
            Text         = Strings.Get("log_ready")
        };
        mainContent.Controls.Add(txtLog);

        // Save log button — slim, integrated below the log, right-aligned
        btnSaveLog = new Button
        {
            Text      = Strings.Get("btn_save_log"),
            Font      = new Font("Segoe UI", 8, FontStyle.Regular),
            Size      = new Size(120, 24),
            Location  = new Point(390, 458),
            FlatStyle = FlatStyle.Flat,
            BackColor = cBackground,
            ForeColor = cTextDim,
            Cursor    = Cursors.Hand
        };
        btnSaveLog.FlatAppearance.BorderColor = cDivider;
        btnSaveLog.FlatAppearance.BorderSize  = 1;
        btnSaveLog.MouseEnter += (s, e) => { btnSaveLog.ForeColor = Color.White; btnSaveLog.FlatAppearance.BorderColor = cBrand; };
        btnSaveLog.MouseLeave += (s, e) => { btnSaveLog.ForeColor = cTextDim;   btnSaveLog.FlatAppearance.BorderColor = cDivider; };
        btnSaveLog.Click      += (s, e) => SaveLog();
        mainContent.Controls.Add(btnSaveLog);
    }

    // =========================================================================
    // LANGUAGE SWITCHING
    // =========================================================================

    // Reloads all UI strings into every control without recreating the form
    private void ChangeLanguage(string lang)
    {
        Strings.LoadLanguage(lang);

        // Menu button labels (hover handlers don't need updating — they use keys)
        btnQuick.Text    = "  " + Strings.Get("btn_quick");
        btnDeep.Text     = "  " + Strings.Get("btn_deep");
        btnRepair.Text   = "  " + Strings.Get("btn_repair");
        btnUpdate.Text   = "  " + Strings.Get("btn_update");
        btnRegistry.Text = "  " + Strings.Get("btn_registry");

        // Right panel labels
        lblDonate.Text     = Strings.Get("lbl_donate");
        linkKofi.Text      = Strings.Get("lbl_kofi");
        btnSaveLog.Text    = Strings.Get("btn_save_log");
        lblSystemInfo.Text = GetSystemInfo();

        // Reset log only if it currently shows the idle message (don't wipe active output)
        if (txtLog.Text == Strings.Get("log_ready") || IsIdleLog())
            txtLog.Text = Strings.Get("log_ready");

        UpdateFlagHighlight();
    }

    // Returns true if the log contains only the previous idle message
    // (used to decide whether it is safe to replace it after a language change)
    private bool IsIdleLog()
    {
        string[] idleMessages = { "System ready. Waiting for orders...", "Sistema listo. Esperando órdenes...",
                                  "Sistema pronto. Aguardando ordens...", "Sistema pronto. In attesa di ordini...",
                                  "Système prêt. En attente d'ordres...", "System bereit. Warte auf Befehle..." };
        foreach (string msg in idleMessages)
            if (txtLog.Text == msg) return true;
        return false;
    }

    // Highlights the active language flag with a brand-color border; resets the rest
    private void UpdateFlagHighlight()
    {
        Button[] flags  = { btnFlagEn, btnFlagEs, btnFlagPt, btnFlagIt, btnFlagFr, btnFlagDe };
        string[] codes  = { "en",      "es",      "pt",      "it",      "fr",      "de"      };
        string   active = Strings.CurrentLanguage;

        for (int i = 0; i < flags.Length; i++)
        {
            flags[i].FlatAppearance.BorderColor = (codes[i] == active)
                ? cBrand
                : cDivider;
        }
    }

    // =========================================================================
    // MAIN LOGIC
    // =========================================================================

    private async void Run(int level)
    {
        string title, message;
        switch (level)
        {
            case 1: title = Strings.Get("confirm_qc_title");  message = Strings.Get("confirm_qc_msg");  break;
            case 2: title = Strings.Get("confirm_dc_title");  message = Strings.Get("confirm_dc_msg");  break;
            case 3: title = Strings.Get("confirm_rep_title"); message = Strings.Get("confirm_rep_msg"); break;
            case 4: title = Strings.Get("confirm_upd_title"); message = Strings.Get("confirm_upd_msg"); break;
            case 5: title = Strings.Get("confirm_reg_title"); message = Strings.Get("confirm_reg_msg"); break;
            default: return;
        }

        if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            return;

        if (level == 4) { RunWinget(); return; }
        if (level == 5) { await RunRegistryCleaner(); SetButtonsEnabled(true); return; }

        SetButtonsEnabled(false);
        txtLog.Clear();

        // Repair and Registry cleaner do not free measurable disk space — skip the space counter
        bool trackSpace  = (level != 3);
        long spaceBefore = trackSpace ? GetFreeSpace() : 0;

        if (trackSpace)
        {
            Log(Strings.Get("log_space_before") + FormatSize(spaceBefore));
            Log(Strings.Get("log_separator"));
        }

        try
        {
            if (level == 1) await RunQuickClean();
            if (level == 2) await RunDeepClean();
            if (level == 3) await RunRepair();
        }
        catch (Exception ex)
        {
            Log(Strings.Get("log_error") + ex.Message);
        }
        finally
        {
            Log("");
            Log(Strings.Get("log_separator"));

            if (trackSpace)
            {
                long   freed     = Math.Max(0, GetFreeSpace() - spaceBefore);
                string freedText = FormatSize(freed);
                Log(Strings.Get("log_done"));
                Log(Strings.Get("log_freed") + freedText);
                MessageBox.Show(Strings.Get("mb_done_msg") + freedText, Strings.Get("mb_done_title"));
            }
            else
            {
                Log(Strings.Get("log_rep_done"));
                MessageBox.Show(Strings.Get("mb_rep_done_msg"), Strings.Get("mb_rep_done_title"));
            }

            SetButtonsEnabled(true);
        }
    }

    // ── Level 1: Quick Clean ──────────────────────────────────────────────────
    private async Task RunQuickClean()
    {
        Log(Strings.Get("log_qc_start"));
        await Cmd(@"del /s /q %TEMP%\* >nul 2>&1");
        Log(Strings.Get("log_qc_user_tmp"));
        await Cmd(@"del /s /q C:\Windows\Temp\* >nul 2>&1");
        Log(Strings.Get("log_qc_sys_tmp"));
        await Cmd("ipconfig /flushdns >nul 2>&1");
        Log(Strings.Get("log_qc_dns"));
        Log(Strings.Get("log_qc_explorer"));
        await Cmd("taskkill /f /im explorer.exe >nul 2>&1");
        await Cmd(@"del /q %LOCALAPPDATA%\IconCache.db >nul 2>&1");
        Process.Start(new ProcessStartInfo("explorer.exe") { UseShellExecute = true });
    }

    // ── Level 2: Deep Clean ───────────────────────────────────────────────────
    private async Task RunDeepClean()
    {
        Log(Strings.Get("log_dc_start"));
        await Cmd(@"del /s /q %TEMP%\* >nul 2>&1");
        await Cmd(@"del /s /q C:\Windows\Temp\* >nul 2>&1");
        Log(Strings.Get("log_dc_tmp"));
        Log(Strings.Get("log_dc_wu"));
        await Cmd("net stop wuauserv >nul 2>&1");
        await Cmd("net stop bits >nul 2>&1");
        await Cmd(@"del /s /q C:\Windows\SoftwareDistribution\Download\* >nul 2>&1");
        Log(Strings.Get("log_dc_wu_cache"));
        await Cmd("net start wuauserv >nul 2>&1");
        Log(Strings.Get("log_dc_wu_start"));
        Log(Strings.Get("log_dc_logs"));
        await Cmd("for /f \"tokens=*\" %i in ('wevtutil el') do @wevtutil cl \"%i\" >nul 2>&1");
        await Cmd(@"del /s /q ""C:\ProgramData\Microsoft\Windows Defender\Scans\History\*"" >nul 2>&1");
        Log(Strings.Get("log_dc_defender"));
        await Cmd(@"del /s /q C:\Windows\Prefetch\* >nul 2>&1");
        Log(Strings.Get("log_dc_prefetch"));
    }

    // ── Level 3: System Repair ────────────────────────────────────────────────
    private async Task RunRepair()
    {
        Log(Strings.Get("log_rep_start"));
        Log(Strings.Get("log_rep_diag"));
        Log("");

        // [1/6] CHKDSK — exit code 0 = clean, anything else = issues found.
        // Using exit code is more reliable than parsing text because CHKDSK
        // writes to stderr and its phrasing varies by Windows language and build.
        Log(Strings.Get("log_rep_chk"));
        CmdResult chkdsk = await CmdRun("chkdsk C: /scan");
        if (chkdsk.ExitCode == 0)
        {
            Log(Strings.Get("log_rep_chk_ok"));
        }
        else
        {
            Log(Strings.Get("log_rep_chk_warn"));
            if (MessageBox.Show(Strings.Get("mb_chkdsk_msg"), Strings.Get("mb_chkdsk_title"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                await Cmd("fsutil dirty set C:");
                Log(Strings.Get("log_rep_chk_sched"));
                MessageBox.Show(Strings.Get("mb_chkdsk_sched"), Strings.Get("mb_chkdsk_ok"));
            }
        }

        // [2/6] SFC — single execution via CmdRun gives us exit code and output together.
        // Exit code != 0 means SFC found problems it could not fix.
        // Exit code 0 + "repaired" text means it fixed something.
        // Exit code 0 + no repair text means the system was already clean.
        Log(Strings.Get("log_rep_sfc"));
        CmdResult sfc = await CmdRun("sfc /scannow");

        if (sfc.ExitCode != 0)
        {
            Log(Strings.Get("log_rep_sfc_err"));
        }
        else if (sfc.Output.Contains("successfully repaired") ||
                 sfc.Output.Contains("reparó correctamente")  ||
                 sfc.Output.Contains("riparati")              ||
                 sfc.Output.Contains("réparés")               ||
                 sfc.Output.Contains("erfolgreich repariert") ||
                 sfc.Output.Contains("reparados com êxito"))
        {
            Log(Strings.Get("log_rep_sfc_fixed"));
        }
        else
        {
            Log(Strings.Get("log_rep_sfc_ok"));
        }

        Log(Strings.Get("log_rep_scan"));
        await Cmd("DISM.exe /Online /Cleanup-Image /ScanHealth >nul 2>&1");

        Log(Strings.Get("log_rep_check"));
        await Cmd("DISM.exe /Online /Cleanup-Image /CheckHealth >nul 2>&1");

        Log(Strings.Get("log_rep_restore"));
        CmdResult dism = await CmdRun("DISM.exe /Online /Cleanup-Image /RestoreHealth");
        if (dism.ExitCode == 0)
            Log(Strings.Get("log_rep_rest_ok"));
        else
            Log(Strings.Get("log_rep_rest_warn"));

        Log(Strings.Get("log_rep_cleanup"));
        await Cmd("DISM.exe /Online /Cleanup-Image /StartComponentCleanup >nul 2>&1");
    }

    // ── Level 5: Registry Cleaner ─────────────────────────────────────────────
    // Safe scope only: orphaned uninstall entries, broken Run/RunOnce autostart
    // entries, and stale MRU (recently used files) lists.
    // Never touches: COM, shell extensions, file associations, services, drivers,
    // or anything under HKLM\SYSTEM.
    // Flow: scan → show findings → ask confirmation → delete only confirmed entries.
    private async Task RunRegistryCleaner()
    {
        SetButtonsEnabled(false);
        txtLog.Clear();
        Log(Strings.Get("log_reg_start"));
        Log(Strings.Get("log_separator"));

        // Run the scan on a background thread to keep the UI responsive
        var toDelete = await Task.Run<System.Collections.Generic.List<string[]>>(
            new Func<System.Collections.Generic.List<string[]>>(ScanRegistry));

        // ── Show findings ─────────────────────────────────────────────────────
        if (toDelete.Count == 0)
        {
            Log(Strings.Get("log_reg_clean"));
            Log(Strings.Get("log_separator"));
            Log(Strings.Get("log_done"));
            MessageBox.Show(Strings.Get("mb_reg_clean"), Strings.Get("mb_done_title"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            SetButtonsEnabled(true);
            return;
        }

        Log(string.Format(Strings.Get("log_reg_found"), toDelete.Count));
        Log("");
        foreach (string[] entry in toDelete)
            Log("  [" + entry[0] + "] " + entry[3]);

        Log("");
        Log(Strings.Get("log_separator"));
        Log(Strings.Get("log_reg_confirm"));

        // ── Confirmation before deleting ──────────────────────────────────────
        if (MessageBox.Show(
                string.Format(Strings.Get("mb_reg_confirm_msg"), toDelete.Count),
                Strings.Get("mb_reg_confirm_title"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information) == DialogResult.No)
        {
            Log(Strings.Get("log_reg_cancelled"));
            SetButtonsEnabled(true);
            return;
        }

        // ── Delete confirmed entries ──────────────────────────────────────────
        int deleted = 0, failed = 0;
        Log(Strings.Get("log_reg_deleting"));

        foreach (string[] entry in toDelete)
        {
            try
            {
                string hive    = entry[0];
                string keyPath = entry[1];
                string valName = entry[2];

                var rootKey = hive == "HKLM"
                    ? Microsoft.Win32.Registry.LocalMachine
                    : Microsoft.Win32.Registry.CurrentUser;

                if (string.IsNullOrEmpty(valName))
                {
                    // Delete entire subkey (orphaned uninstall entry)
                    string parent = Path.GetDirectoryName(keyPath);
                    string child  = Path.GetFileName(keyPath);
                    if (parent == null) { failed++; continue; }
                    using (var parentKey = rootKey.OpenSubKey(parent, true))
                    {
                        if (parentKey != null)
                        {
                            parentKey.DeleteSubKeyTree(child, false);
                            Log("  ✓ " + entry[3]);
                            deleted++;
                        }
                        else { failed++; }
                    }
                }
                else
                {
                    // Delete single value (Run entry or MRU entry)
                    using (var key = rootKey.OpenSubKey(keyPath, true))
                    {
                        if (key != null)
                        {
                            key.DeleteValue(valName, false);
                            Log("  ✓ " + entry[3]);
                            deleted++;
                        }
                        else { failed++; }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("  ✗ " + entry[3] + " — " + ex.Message);
                failed++;
            }
        }

        Log("");
        Log(Strings.Get("log_separator"));
        Log(string.Format(Strings.Get("log_reg_done"), deleted, failed));

        string summary = failed == 0
            ? string.Format(Strings.Get("mb_reg_done_msg"), deleted)
            : string.Format(Strings.Get("mb_reg_done_partial"), deleted, failed);

        MessageBox.Show(summary, Strings.Get("mb_done_title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // Synchronous scan method — called on a background thread from RunRegistryCleaner.
    // Separated from the async method to avoid C# 5 limitations with await inside
    // complex Task.Run lambdas.
    // Returns a list of entries to delete: [0]=hive, [1]=key path, [2]=value name, [3]=display label
    private System.Collections.Generic.List<string[]> ScanRegistry()
    {
        var toDelete = new System.Collections.Generic.List<string[]>();

        // ── ZONE 1: Orphaned uninstall entries ────────────────────────────────
        // Checks both 32-bit and 64-bit uninstall hives.
        // An entry is flagged only when BOTH UninstallString AND DisplayIcon
        // point to paths that do not exist — requiring both conditions avoids
        // false positives from entries that use non-standard install layouts.
        string[] uninstallHives = {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        foreach (string hive in uninstallHives)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(hive))
                {
                    if (key == null) continue;
                    foreach (string subName in key.GetSubKeyNames())
                    {
                        try
                        {
                            using (var sub = key.OpenSubKey(subName))
                            {
                                if (sub == null) continue;

                                // Skip Windows system components and update entries — never touch these
                                object sysComp = sub.GetValue("SystemComponent");
                                if (sysComp != null && sysComp.ToString() == "1") continue;
                                if (sub.GetValue("ReleaseType") != null) continue;

                                string uninstStr   = (sub.GetValue("UninstallString")  ?? "").ToString().Trim('"', ' ');
                                string displayIcon = (sub.GetValue("DisplayIcon")       ?? "").ToString().Split(',')[0].Trim('"', ' ');
                                string displayName = (sub.GetValue("DisplayName")       ?? subName).ToString();

                                // Skip MSI-managed entries — they may not have a direct exe path
                                if (uninstStr.StartsWith("MsiExec",  StringComparison.OrdinalIgnoreCase)) continue;
                                if (uninstStr.StartsWith("msiexec",  StringComparison.OrdinalIgnoreCase)) continue;

                                string exeDir  = Path.GetDirectoryName(uninstStr)   ?? "";
                                string iconDir = Path.GetDirectoryName(displayIcon) ?? "";

                                bool uninstMissing = !string.IsNullOrEmpty(uninstStr)   && !File.Exists(uninstStr)   && !Directory.Exists(exeDir);
                                bool iconMissing   = !string.IsNullOrEmpty(displayIcon) && !File.Exists(displayIcon) && !Directory.Exists(iconDir);

                                if (uninstMissing && iconMissing)
                                    toDelete.Add(new string[] { "HKLM", hive + @"\" + subName, "", displayName });
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        // ── ZONE 2: Broken Run / RunOnce autostart entries ────────────────────
        // Only HKCU — HKLM Run entries often belong to drivers and services
        // that load differently and would generate too many false positives.
        string[] runKeys = {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce"
        };

        foreach (string runKey in runKeys)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(runKey))
                {
                    if (key == null) continue;
                    foreach (string valueName in key.GetValueNames())
                    {
                        try
                        {
                            string raw     = (key.GetValue(valueName) ?? "").ToString();
                            // Extract executable path — strip quotes and arguments
                            string exePath = raw.Trim().TrimStart('"');
                            int    endQuote = exePath.IndexOf('"');
                            if (endQuote > 0) exePath = exePath.Substring(0, endQuote);
                            exePath = exePath.Trim();

                            if (!string.IsNullOrEmpty(exePath) && !File.Exists(exePath))
                                toDelete.Add(new string[] { "HKCU", runKey, valueName, valueName + " → " + exePath });
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        // ── ZONE 3: Stale MRU (recently used files) string entries ────────────
        // Only string values with a rooted path — binary PIDL values are skipped
        // entirely because they cannot be decoded safely without Shell APIs.
        string[] mruPaths = {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRU"
        };

        foreach (string mruPath in mruPaths)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(mruPath))
                {
                    if (key == null) continue;
                    foreach (string valueName in key.GetValueNames())
                    {
                        if (valueName.Equals("MRUListEx", StringComparison.OrdinalIgnoreCase) ||
                            valueName.Equals("MRUList",   StringComparison.OrdinalIgnoreCase)) continue;
                        try
                        {
                            object val = key.GetValue(valueName);
                            // C# 5 compatible type check — no pattern matching
                            string strVal = val as string;
                            if (strVal != null)
                            {
                                string filePath = strVal.Trim();
                                if (!string.IsNullOrEmpty(filePath)
                                    && Path.IsPathRooted(filePath)
                                    && !File.Exists(filePath)
                                    && !Directory.Exists(filePath))
                                    toDelete.Add(new string[] { "HKCU", mruPath, valueName, filePath });
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        return toDelete;
    }

    // ── Level 4: Winget ───────────────────────────────────────────────────────
    private async void RunWinget()
    {
        txtLog.AppendText(Strings.Get("log_wg_start"));
        txtLog.AppendText(Strings.Get("log_wg_search"));

        string wingetPath = string.Empty;
        string candidate  = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            @"Microsoft\WindowsApps\winget.exe");

        if (File.Exists(candidate))
        {
            wingetPath = candidate;
        }
        else
        {
            CmdResult where = await CmdRun("where winget");
            if (where.ExitCode == 0 && where.Output.Trim().Length > 4)
                wingetPath = "winget";
        }

        if (string.IsNullOrEmpty(wingetPath))
        {
            MessageBox.Show(Strings.Get("mb_wg_notfound"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtLog.AppendText(Strings.Get("log_wg_notfound"));
            return;
        }

        txtLog.AppendText(Strings.Get("log_wg_found"));
        txtLog.AppendText(Strings.Get("log_wg_open"));

        try
        {
            string wg   = "\"" + wingetPath + "\"";
            string args = "/c \"" +
                wg + " upgrade " +
                "& echo. & echo ======================================================= " +
                "& echo   " + Strings.Get("log_wg_attention") + " " +
                "& echo ======================================================= " +
                "& echo. & echo   " + Strings.Get("log_wg_warning") + " " +
                "& echo   " + Strings.Get("log_wg_confirm") + " " +
                "& pause " +
                "& " + wg + " upgrade --all --include-unknown --accept-source-agreements --accept-package-agreements " +
                "& echo. & echo " + Strings.Get("log_wg_finished") + " & pause\"";

            Process.Start(new ProcessStartInfo { FileName = "cmd.exe", Arguments = args, UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show(Strings.Get("mb_wg_error") + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── Save log ──────────────────────────────────────────────────────────────
    private void SaveLog()
    {
        string content = txtLog.Text.Trim();

        if (string.IsNullOrEmpty(content) || content == Strings.Get("log_ready"))
        {
            MessageBox.Show(Strings.Get("save_log_empty"), "Leedeo Cleaner", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using (var dialog = new SaveFileDialog())
        {
            dialog.Title            = Strings.Get("save_log_title");
            dialog.Filter           = "Text file (*.txt)|*.txt";
            dialog.FileName         = Strings.Get("save_log_default") + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (dialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                // Header with timestamp and basic system info
                string header =
                    "Leedeo Cleaner v1.6 - Log\r\n" +
                    "Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n" +
                    "PC:   " + Environment.MachineName + " / " + Environment.UserName + "\r\n" +
                    "================================================\r\n\r\n";

                File.WriteAllText(dialog.FileName, header + content, System.Text.Encoding.UTF8);
                MessageBox.Show(Strings.Get("save_log_success"), "Leedeo Cleaner", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Strings.Get("save_log_error") + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // =========================================================================
    // HELPER METHODS
    // =========================================================================

    private void SetButtonsEnabled(bool enabled)
    {
        btnQuick.Enabled    = enabled;
        btnDeep.Enabled     = enabled;
        btnRepair.Enabled   = enabled;
        btnUpdate.Enabled   = enabled;
        btnRegistry.Enabled = enabled;
        btnClose.Enabled    = enabled;
        btnSaveLog.Enabled  = enabled;
    }

    private async Task Cmd(string command)
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
                        Arguments       = "/c " + command,
                        CreateNoWindow  = true,
                        UseShellExecute = false,
                        WindowStyle     = ProcessWindowStyle.Hidden
                    };
                    p.Start();
                    p.WaitForExit();
                }
            }
            catch { }
        });
    }

    // Runs a command and returns both its exit code and merged stdout+stderr output.
    // Using a single execution avoids the inconsistency of running the same command
    // twice to get exit code and text separately (as was the case with SFC).
    private struct CmdResult { public int ExitCode; public string Output; }

    private async Task<CmdResult> CmdRun(string command)
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
                        Arguments              = "/c " + command,
                        CreateNoWindow         = true,
                        UseShellExecute        = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError  = true,
                        WindowStyle            = ProcessWindowStyle.Hidden
                    };
                    p.Start();
                    string stdout = p.StandardOutput.ReadToEnd();
                    string stderr = p.StandardError.ReadToEnd();
                    p.WaitForExit();
                    return new CmdResult { ExitCode = p.ExitCode, Output = stdout + stderr };
                }
            }
            catch { return new CmdResult { ExitCode = -1, Output = string.Empty }; }
        });
    }

    private long GetFreeSpace()
    {
        try { return new DriveInfo("C").TotalFreeSpace; }
        catch { return 0; }
    }

    private string FormatSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int i = 0;
        decimal value = bytes;
        while (Math.Round(value / 1024) >= 1 && i < suffixes.Length - 1) { value /= 1024; i++; }
        return string.Format("{0:n1} {1}", value, suffixes[i]);
    }

    // Loads an embedded image; MemoryStream is kept open because Image.FromStream
    // requires the stream to stay alive for the lifetime of the Image object.
    private Image LoadImage(string resource)
    {
        try
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var s = asm.GetManifestResourceStream(resource))
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

    private Icon LoadIcon(string resource)
    {
        try
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var s = asm.GetManifestResourceStream(resource))
            {
                return s != null ? new Icon(s) : null;
            }
        }
        catch { return null; }
    }

    private string GetSystemInfo()
    {
        try
        {
            string osName = "Windows";
            try
            {
                const string key = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";
                object productName = Registry.GetValue(key, "ProductName", "Windows");
                osName = productName != null ? productName.ToString() : "Windows";
                object buildObj = Registry.GetValue(key, "CurrentBuild", "0");
                int build = 0;
                if (buildObj != null) int.TryParse(buildObj.ToString(), out build);
                if (build >= 22000) osName = osName.Replace("Windows 10", "Windows 11");
            }
            catch { osName = Strings.Get("lbl_win_detected"); }

            return Strings.Get("lbl_pc") + Environment.MachineName +
                   Strings.Get("lbl_user") + Environment.UserName +
                   Strings.Get("lbl_system") + osName;
        }
        catch { return Strings.Get("lbl_ready"); }
    }

    // Creates a sidebar menu button with a colored accent bar and optional icon
    private Button CreateMenuButton(string label, string imageResource, int top, Color accentColor)
    {
        var btn = new Button
        {
            Text      = "  " + label,
            Location  = new Point(10, top),
            Size      = new Size(220, 40),
            FlatStyle = FlatStyle.Flat,
            BackColor = cSidebar,
            ForeColor = Color.FromArgb(210, 210, 215),
            TextAlign = ContentAlignment.MiddleLeft,
            Font      = new Font("Segoe UI", 9),
            Cursor    = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;

        Image img = LoadImage(imageResource);
        if (img != null)
        {
            btn.Image             = img;
            btn.ImageAlign        = ContentAlignment.MiddleLeft;
            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn.Padding           = new Padding(14, 0, 0, 0);
        }

        // Left accent bar
        var accent = new Panel { BackColor = accentColor, Size = new Size(3, 40), Location = new Point(0, 0) };
        btn.Controls.Add(accent);

        btn.MouseEnter += (s, e) =>
        {
            btn.BackColor    = cSidebarHover;
            btn.ForeColor    = Color.White;
            accent.BackColor = ControlPaint.Light(accentColor, 0.15f);
        };
        btn.MouseLeave += (s, e) =>
        {
            btn.BackColor    = cSidebar;
            btn.ForeColor    = Color.FromArgb(210, 210, 215);
            accent.BackColor = accentColor;
        };

        return btn;
    }

    // Creates a flag button: square, flat, image-only, with a subtle border
    private Button CreateFlagButton(string imageResource, string tooltip, int x, int y, int size)
    {
        var btn = new Button
        {
            Location  = new Point(x, y),
            Size      = new Size(size, size),
            FlatStyle = FlatStyle.Flat,
            BackColor = cSidebar,
            Cursor    = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize  = 1;
        btn.FlatAppearance.BorderColor = cDivider;

        Image img = LoadImage(imageResource);
        if (img != null)
        {
            btn.Image      = img;
            btn.ImageAlign = ContentAlignment.MiddleCenter;
        }
        else
        {
            btn.Text      = tooltip.Substring(0, 2).ToUpper();
            btn.Font      = new Font("Segoe UI", 6, FontStyle.Bold);
            btn.ForeColor = cTextDim;
        }

        var tip = new ToolTip { InitialDelay = 400, ReshowDelay = 200 };
        tip.SetToolTip(btn, tooltip);

        btn.MouseEnter += (s, e) => { btn.BackColor = cSidebarHover; btn.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 95); };
        btn.MouseLeave += (s, e) => { btn.BackColor = cSidebar; };  // border reset handled by UpdateFlagHighlight
        btn.MouseLeave += (s, e) => UpdateFlagHighlight();
        return btn;
    }

    // Shows a description in the log area on hover; restores on leave.
    // Receives the Strings key (e.g. "hover_quick"), NOT the translated string,
    // so the text is always resolved at event time against the active language.
    // Handlers are registered once at startup and never replaced.
    private void AssignHover(Button btn, string hoverKey)
    {
        btn.MouseEnter += (s, e) =>
        {
            btn.BackColor = cSidebarHover;
            if (IsIdleLog() ||
                txtLog.Text == Strings.Get("hover_quick")  ||
                txtLog.Text == Strings.Get("hover_deep")   ||
                txtLog.Text == Strings.Get("hover_repair") ||
                txtLog.Text == Strings.Get("hover_update"))
                txtLog.Text = Strings.Get(hoverKey);
        };
        btn.MouseLeave += (s, e) =>
        {
            btn.BackColor = cSidebar;
            // Clear only if we were the ones who set the text
            if (txtLog.Text == Strings.Get(hoverKey))
                txtLog.Text = Strings.Get("log_ready");
        };
    }

    private void Log(string text)
    {
        if (InvokeRequired) Invoke(new Action<string>(Log), text);
        else { txtLog.AppendText("\r\n" + text); txtLog.ScrollToCaret(); }
    }

    // ── Window drag ───────────────────────────────────────────────────────────
    private void OnMouseDown(object sender, MouseEventArgs e) { dragging = true;  dragCursorPoint = Cursor.Position; dragFormPoint = Location; }
    private void OnMouseMove(object sender, MouseEventArgs e) { if (dragging) Location = Point.Add(dragFormPoint, new Size(Point.Subtract(Cursor.Position, new Size(dragCursorPoint)))); }
    private void OnMouseUp  (object sender, MouseEventArgs e) { dragging = false; }

    // ── Entry point ───────────────────────────────────────────────────────────
    [STAThread]
    static void Main()
    {
        try
        {
            Strings.Load();

            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName        = Application.ExecutablePath,
                    Verb            = "runas",
                    UseShellExecute = true
                });
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
        catch (Exception ex)
        {
            MessageBox.Show(Strings.Get("mb_start_error") + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}