#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        // ── Tema: Koyu mor / indigo ────────────────────────────────
        readonly Color BG = Color.FromArgb(8, 8, 14);
        readonly Color SURF = Color.FromArgb(12, 12, 20);
        readonly Color CARD = Color.FromArgb(16, 17, 28);
        readonly Color ELEV = Color.FromArgb(22, 23, 38);
        readonly Color HOVER = Color.FromArgb(28, 30, 48);

        readonly Color GOLD = Color.FromArgb(255, 200, 60);
        readonly Color LIME = Color.FromArgb(80, 230, 130);
        readonly Color ROSE = Color.FromArgb(255, 75, 110);
        readonly Color BLUE = Color.FromArgb(80, 160, 255);
        readonly Color PURP = Color.FromArgb(170, 100, 255);
        readonly Color TEAL = Color.FromArgb(0, 210, 190);

        readonly Color TXT = Color.FromArgb(210, 215, 240);
        readonly Color TXT2 = Color.FromArgb(90, 95, 130);
        readonly Color TXT3 = Color.FromArgb(38, 40, 62);
        readonly Color BDR = Color.FromArgb(18, 255, 255, 255);

        // ── State ─────────────────────────────────────────────────
        string selNav = "Smart Money";
        string selPair = "EURUSD";
        string selTF = "H1";
        bool showBOS = true;
        bool showOB = true;
        bool showLiq = true;
        bool showFVG = true;
        double livePrice = 1.08421;
        readonly Random RNG = new Random();
        System.Windows.Forms.Timer ticker;

        // ── Veri ──────────────────────────────────────────────────
        class SMZone
        {
            public string Type = "", Label = "", Pair = "", TF = "";
            public double Price, Top, Bottom;
            public bool IsBull;
            public string Status = "", Time = "";
            public Color Col;
        }

        readonly List<SMZone> zones = new List<SMZone>
        {
            new SMZone{Type="OB",  Label="Bullish OB",  Price=1.08200,Top=1.08240,Bottom=1.08160,IsBull=true, Status="ACTIVE",  Time="14:32",Pair="EURUSD",TF="H1"},
            new SMZone{Type="OB",  Label="Bearish OB",  Price=1.08650,Top=1.08690,Bottom=1.08610,IsBull=false,Status="ACTIVE",  Time="12:15",Pair="EURUSD",TF="H1"},
            new SMZone{Type="BOS", Label="BOS Bullish",  Price=1.08310,Top=1.08310,Bottom=1.08310,IsBull=true, Status="CONFIRMED",Time="11:40",Pair="EURUSD",TF="H1"},
            new SMZone{Type="BOS", Label="BOS Bearish",  Price=1.08580,Top=1.08580,Bottom=1.08580,IsBull=false,Status="CONFIRMED",Time="10:05",Pair="EURUSD",TF="H1"},
            new SMZone{Type="LIQ", Label="Buy Liq.",    Price=1.08720,Top=1.08730,Bottom=1.08710,IsBull=true, Status="INTACT",  Time="09:20",Pair="EURUSD",TF="H1"},
            new SMZone{Type="LIQ", Label="Sell Liq.",   Price=1.08080,Top=1.08090,Bottom=1.08070,IsBull=false,Status="SWEPT",   Time="08:45",Pair="EURUSD",TF="H1"},
            new SMZone{Type="FVG", Label="Bullish FVG", Price=1.08380,Top=1.08420,Bottom=1.08340,IsBull=true, Status="OPEN",    Time="13:10",Pair="EURUSD",TF="H1"},
        };

        // ── Controls ──────────────────────────────────────────────
        Panel pnlSidebar, pnlTopBar, pnlContent, pnlStatus;
        Panel pnlChart, pnlZones, pnlInfo, pnlSettings;
        Label lblPrice, lblPriceChg, lblClock, lblStatus;
        CheckBox chkBOS, chkOB, chkLiq, chkFVG;
        ListView lvZones;

        // ═════════════════════════════════════════════════════════
        public Form1()
        {
            this.Text = "Smart Money Indicator · AutoScripts";
            this.Size = new Size(1200, 750);
            this.MinimumSize = new Size(960, 620);
            this.BackColor = BG;
            this.ForeColor = TXT;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f);
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;

            // SIRALAMA: Top → Bottom → Left → Fill
            MakeTopBar();
            MakeStatusBar();
            MakeSidebar();
            MakeContent();

            StartTicker();
        }

        // ── TopBar ────────────────────────────────────────────────
        void MakeTopBar()
        {
            pnlTopBar = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = SURF };
            pnlTopBar.Paint += (s, e) =>
            {
                var g = e.Graphics;
                // üst altın şerit
                using (var br = new LinearGradientBrush(
                    new Point(0, 0), new Point(pnlTopBar.Width, 0),
                    Color.FromArgb(80, GOLD.R, GOLD.G, GOLD.B), Color.Transparent))
                    g.FillRectangle(br, 0, 0, pnlTopBar.Width, 3);
                using (var p = new Pen(BDR)) g.DrawLine(p, 0, 47, pnlTopBar.Width, 47);

                // ikon
                g.SmoothingMode = SmoothingMode.AntiAlias;
                DrawSMIcon(g, 14, 12);

                // başlık
                using (var f = new Font("Segoe UI", 11f, FontStyle.Bold))
                using (var br = new SolidBrush(TXT))
                    g.DrawString("SMART MONEY INDICATOR", f, br, 44, 12);
                using (var f = new Font("Consolas", 7.5f))
                using (var br = new SolidBrush(TXT3))
                    g.DrawString("Liquidity  ·  Break of Structure  ·  Order Blocks  ·  FVG", f, br, 46, 29);
            };
            pnlTopBar.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                { NativeMethods.ReleaseCapture(); NativeMethods.SendMessage(Handle, 0xA1, (IntPtr)2, IntPtr.Zero); }
            };

            // Win butonları
            var bCl = WinBtn(Color.FromArgb(255, 85, 75)); bCl.Click += (s, e) => Close();
            var bMn = WinBtn(Color.FromArgb(255, 185, 40)); bMn.Click += (s, e) => WindowState = FormWindowState.Minimized;
            var bMx = WinBtn(Color.FromArgb(35, 195, 55)); bMx.Click += (s, e) =>
                WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            pnlTopBar.Controls.AddRange(new Control[] { bMn, bMx, bCl });
            void PosWin() { bCl.Location = new Point(pnlTopBar.Width - 26, 18); bMx.Location = new Point(pnlTopBar.Width - 46, 18); bMn.Location = new Point(pnlTopBar.Width - 66, 18); }
            PosWin(); pnlTopBar.Resize += (s, e) => PosWin();

            // Pair butonları
            string[] pairs = { "EURUSD", "GBPUSD", "USDJPY", "XAUUSD", "AUDUSD" };
            int px = 290;
            foreach (var pr in pairs)
            {
                string p2 = pr;
                var b = new Button
                {
                    Text = p2,
                    Location = new Point(px, 11),
                    Size = new Size(66, 26),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = p2 == selPair ? Color.FromArgb(28, 255, 200, 60) : Color.Transparent,
                    ForeColor = p2 == selPair ? GOLD : TXT2,
                    Font = new Font("Consolas", 8.5f, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                };
                b.FlatAppearance.BorderColor = p2 == selPair ? Color.FromArgb(80, GOLD.R, GOLD.G, GOLD.B) : Color.FromArgb(18, 255, 255, 255);
                b.FlatAppearance.BorderSize = 1;
                b.Click += (s, e) =>
                {
                    selPair = p2;
                    foreach (Control c in pnlTopBar.Controls)
                    {
                        if (!(c is Button btn) || Array.IndexOf(pairs, btn.Text) < 0) continue;
                        bool sel = btn.Text == selPair;
                        btn.BackColor = sel ? Color.FromArgb(28, 255, 200, 60) : Color.Transparent;
                        btn.ForeColor = sel ? GOLD : TXT2;
                        btn.FlatAppearance.BorderColor = sel ? Color.FromArgb(80, GOLD.R, GOLD.G, GOLD.B) : Color.FromArgb(18, 255, 255, 255);
                    }
                    pnlChart?.Invalidate();
                };
                pnlTopBar.Controls.Add(b);
                px += 70;
            }

            // TF butonları
            string[] tfs = { "M5", "M15", "H1", "H4", "D1" };
            int tx = 650;
            foreach (var tf in tfs)
            {
                string t2 = tf;
                var b = new Button
                {
                    Text = t2,
                    Location = new Point(tx, 11),
                    Size = new Size(40, 26),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = t2 == selTF ? Color.FromArgb(28, 170, 100, 255) : Color.Transparent,
                    ForeColor = t2 == selTF ? PURP : TXT2,
                    Font = new Font("Consolas", 8f, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                };
                b.FlatAppearance.BorderColor = t2 == selTF ? Color.FromArgb(80, PURP.R, PURP.G, PURP.B) : Color.FromArgb(18, 255, 255, 255);
                b.FlatAppearance.BorderSize = 1;
                b.Click += (s, e) =>
                {
                    selTF = t2;
                    foreach (Control c in pnlTopBar.Controls)
                    {
                        if (!(c is Button btn) || Array.IndexOf(tfs, btn.Text) < 0) continue;
                        bool sel = btn.Text == selTF;
                        btn.BackColor = sel ? Color.FromArgb(28, 170, 100, 255) : Color.Transparent;
                        btn.ForeColor = sel ? PURP : TXT2;
                        btn.FlatAppearance.BorderColor = sel ? Color.FromArgb(80, PURP.R, PURP.G, PURP.B) : Color.FromArgb(18, 255, 255, 255);
                    }
                    pnlChart?.Invalidate();
                };
                pnlTopBar.Controls.Add(b);
                tx += 44;
            }

            // Canlı fiyat
            lblPrice = new Label { Text = "1.08421", Location = new Point(870, 10), Size = new Size(105, 26), ForeColor = GOLD, Font = new Font("Consolas", 15f, FontStyle.Bold), BackColor = Color.Transparent };
            lblPriceChg = new Label { Text = "▲ +0.00034", Location = new Point(978, 16), Size = new Size(105, 16), ForeColor = LIME, Font = new Font("Consolas", 9f), BackColor = Color.Transparent };
            pnlTopBar.Controls.AddRange(new Control[] { lblPrice, lblPriceChg });

            this.Controls.Add(pnlTopBar);
        }

        void DrawSMIcon(Graphics g, int x, int y)
        {
            // 3 iç içe üçgen (smart money piramit)
            using (var pen = new Pen(GOLD, 1.5f))
            {
                g.DrawPolygon(pen, new Point[] { new Point(x + 12, y), new Point(x + 24, y + 22), new Point(x, y + 22) });
            }
            using (var pen = new Pen(Color.FromArgb(120, PURP.R, PURP.G, PURP.B), 1f))
                g.DrawLine(pen, x + 6, y + 11, x + 18, y + 11);
            using (var br = new SolidBrush(GOLD))
                g.FillEllipse(br, x + 10, y + 9, 5, 5);
        }

        // ── StatusBar ─────────────────────────────────────────────
        void MakeStatusBar()
        {
            pnlStatus = new Panel { Dock = DockStyle.Bottom, Height = 26, BackColor = SURF };
            pnlStatus.Paint += (s, e) =>
            {
                var g = e.Graphics;
                using (var p = new Pen(BDR)) g.DrawLine(p, 0, 0, pnlStatus.Width, 0);
                // bottom accent
                using (var br = new LinearGradientBrush(new Point(0, 24), new Point(pnlStatus.Width, 24),
                    Color.FromArgb(40, GOLD.R, GOLD.G, GOLD.B), Color.Transparent))
                    g.FillRectangle(br, 0, 24, pnlStatus.Width, 2);
                using (var f = new Font("Consolas", 8f)) using (var br = new SolidBrush(TXT3))
                {
                    g.DrawString($"Zones: {zones.Count}   Pair: {selPair}   TF: {selTF}", f, br, 210, 7);
                    g.DrawString("SMC v2.0.0", f, br, pnlStatus.Width - 90, 7);
                }
            };
            lblStatus = new Label { Text = "● Ready", Location = new Point(12, 6), Size = new Size(190, 14), ForeColor = LIME, Font = new Font("Consolas", 8f, FontStyle.Bold), BackColor = Color.Transparent };
            lblClock = new Label { Text = "", Location = new Point(800, 6), Size = new Size(230, 14), ForeColor = TXT3, Font = new Font("Consolas", 8f), BackColor = Color.Transparent };
            pnlStatus.Controls.AddRange(new Control[] { lblStatus, lblClock });
            pnlStatus.Resize += (s, e) => { lblClock.Location = new Point(pnlStatus.Width - 240, 6); pnlStatus.Invalidate(); };
            this.Controls.Add(pnlStatus);
        }

        // ── Sidebar ───────────────────────────────────────────────
        void MakeSidebar()
        {
            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 212, BackColor = SURF };
            pnlSidebar.Paint += (s, e) =>
            {
                var g = e.Graphics;
                using (var p = new Pen(BDR)) g.DrawLine(p, pnlSidebar.Width - 1, 0, pnlSidebar.Width - 1, pnlSidebar.Height);
                using (var br = new LinearGradientBrush(new Point(0, 0), new Point(0, pnlSidebar.Height),
                    Color.FromArgb(60, GOLD.R, GOLD.G, GOLD.B), Color.Transparent))
                    g.FillRectangle(br, 0, 0, 3, pnlSidebar.Height);
            };

            // Logo
            var logo = new Panel { Height = 56, Dock = DockStyle.Top, BackColor = Color.Transparent };
            logo.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                DrawSMIcon(g, 14, 16);
                using (var f = new Font("Segoe UI", 10f, FontStyle.Bold)) using (var br = new SolidBrush(TXT))
                    g.DrawString("SMART MONEY", f, br, 44, 15);
                using (var f = new Font("Consolas", 7.5f)) using (var br = new SolidBrush(TXT3))
                    g.DrawString("SMC Indicator", f, br, 46, 31);
                using (var p = new Pen(BDR)) g.DrawLine(p, 8, 54, 204, 54);
            };
            pnlSidebar.Controls.Add(logo);

            // Nav
            var navDefs = new[]
            {
                ("",   "── MENU ──"),
                ("▤",  "Library"),
                ("⌂",  "Home"),
                ("◈",  "Dashboard"),
                ("",   "── SMC ──"),
                ("◈",  "Smart Money"),
                ("⊛",  "Account Protector"),
                ("⊞",  "Position Sizer"),
                ("🕒", "Session"),
                ("",   "── SYSTEM ──"),
                ("ℹ",  "About"),
                ("✉",  "Contact"),
            };

            int ny = 60;
            foreach (var (icon, label) in navDefs)
            {
                if (icon == "")
                {
                    pnlSidebar.Controls.Add(new Label { Text = label, Location = new Point(10, ny), Size = new Size(192, 18), ForeColor = TXT3, Font = new Font("Consolas", 7.5f, FontStyle.Bold), BackColor = Color.Transparent });
                    ny += 22;
                }
                else
                {
                    string nav = label; bool act = nav == selNav;
                    var row = new Panel { Location = new Point(0, ny), Size = new Size(211, 34), BackColor = act ? Color.FromArgb(22, 255, 200, 60) : Color.Transparent, Cursor = Cursors.Hand, Tag = nav };
                    row.Paint += (s, e) =>
                    {
                        if ((string)row.Tag == selNav)
                            using (var p = new Pen(GOLD, 2)) e.Graphics.DrawLine(p, row.Width - 1, 0, row.Width - 1, row.Height);
                    };
                    row.MouseEnter += (s, e) => { if ((string)row.Tag != selNav) row.BackColor = HOVER; };
                    row.MouseLeave += (s, e) => row.BackColor = (string)row.Tag == selNav ? Color.FromArgb(22, 255, 200, 60) : Color.Transparent;
                    row.Click += (s, e) => NavClick((string)row.Tag);

                    var icL = new Label { Text = icon, Location = new Point(14, 9), Size = new Size(18, 16), ForeColor = act ? GOLD : TXT2, Font = new Font("Segoe UI", 9f), BackColor = Color.Transparent, Tag = nav + "_i" };
                    var txL = new Label { Text = nav, Location = new Point(36, 9), Size = new Size(124, 16), ForeColor = act ? GOLD : TXT2, Font = new Font("Segoe UI", 9f), BackColor = Color.Transparent, Tag = nav + "_t" };
                    icL.Click += (s, e) => NavClick(nav); txL.Click += (s, e) => NavClick(nav);
                    row.Controls.AddRange(new Control[] { icL, txL });

                    if (nav == "Smart Money")
                    {
                        var bdg = new Label { Text = "SMC", Location = new Point(162, 10), Size = new Size(32, 14), BackColor = act ? GOLD : TXT3, ForeColor = BG, Font = new Font("Consolas", 7f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
                        bdg.Click += (s, e) => NavClick(nav); row.Controls.Add(bdg);
                    }
                    pnlSidebar.Controls.Add(row);
                    ny += 36;
                }
            }

            // Footer — zone sayaç
            var foot = new Panel { Dock = DockStyle.Bottom, Height = 100, BackColor = Color.Transparent };
            foot.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var p = new Pen(BDR)) g.DrawLine(p, 8, 0, 204, 0);
                FootStat(g, 14, 8, "ORDER BLOCKS", $"{zones.FindAll(z => z.Type == "OB").Count}  active", GOLD);
                FootStat(g, 14, 34, "BOS SIGNALS", $"{zones.FindAll(z => z.Type == "BOS").Count}  detected", LIME);
                FootStat(g, 14, 60, "LIQUIDITY", $"{zones.FindAll(z => z.Type == "LIQ").Count}  levels", ROSE);
                FootStat(g, 14, 82, "FVG", $"{zones.FindAll(z => z.Type == "FVG").Count}  open", BLUE);
            };
            pnlSidebar.Controls.Add(foot);
            this.Controls.Add(pnlSidebar);
        }

        void FootStat(Graphics g, int x, int y, string lbl, string val, Color col)
        {
            using (var br = new SolidBrush(col)) g.FillEllipse(br, x, y + 4, 5, 5);
            using (var f = new Font("Consolas", 7.5f)) using (var br = new SolidBrush(TXT3)) g.DrawString(lbl, f, br, x + 10, y);
            using (var f = new Font("Consolas", 8f, FontStyle.Bold)) using (var br = new SolidBrush(col)) g.DrawString(val, f, br, x + 10, y + 11);
        }

        void NavClick(string nav)
        {
            selNav = nav;
            foreach (Control c in pnlSidebar.Controls)
            {
                if (!(c is Panel p) || !(p.Tag is string t)) continue;
                bool act = t == nav;
                p.BackColor = act ? Color.FromArgb(22, 255, 200, 60) : Color.Transparent; p.Invalidate();
                foreach (Control ch in p.Controls)
                {
                    if (!(ch is Label l) || l.Text == "SMC") continue;
                    bool mine = l.Tag is string lt && (lt == nav + "_i" || lt == nav + "_t");
                    l.ForeColor = mine ? GOLD : TXT2;
                }
            }
        }

        // ── Content ───────────────────────────────────────────────
        void MakeContent()
        {
            pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = BG, Padding = new Padding(8) };

            // Üst: Chart (sol geniş) + Info (sağ dar)
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 310, BackColor = BG };

            pnlInfo = new Panel { Dock = DockStyle.Right, Width = 242, BackColor = CARD };
            pnlInfo.Paint += Info_Paint;
            BuildInfoPanel(pnlInfo);

            pnlChart = new Panel { Dock = DockStyle.Fill, BackColor = CARD };
            pnlChart.Paint += Chart_Paint;

            var gapR = new Panel { Dock = DockStyle.Right, Width = 6, BackColor = BG };
            pnlTop.Controls.Add(pnlChart);   // Fill — son
            pnlTop.Controls.Add(gapR);
            pnlTop.Controls.Add(pnlInfo);    // Right

            // Alt: Settings (sol) + Zones table (sağ)
            var pnlBot = new Panel { Dock = DockStyle.Fill, BackColor = BG };

            pnlSettings = new Panel { Dock = DockStyle.Left, Width = 242, BackColor = CARD };
            pnlSettings.Paint += (s, e) => CardHdr(e.Graphics, pnlSettings, "Display Settings", PURP);
            BuildSettings(pnlSettings);

            pnlZones = new Panel { Dock = DockStyle.Fill, BackColor = CARD };
            pnlZones.Paint += (s, e) => CardHdr(e.Graphics, pnlZones, "Detected Zones", GOLD);
            BuildZonesLV(pnlZones);

            var gapL = new Panel { Dock = DockStyle.Left, Width = 6, BackColor = BG };
            pnlBot.Controls.Add(pnlZones);    // Fill — son
            pnlBot.Controls.Add(gapL);
            pnlBot.Controls.Add(pnlSettings); // Left

            // pnlContent'e: Top önce, Fill sonra
            var gapMid = new Panel { Dock = DockStyle.Top, Height = 6, BackColor = BG };
            pnlContent.Controls.Add(pnlBot);   // Fill
            pnlContent.Controls.Add(gapMid);
            pnlContent.Controls.Add(pnlTop);   // Top

            this.Controls.Add(pnlContent);    // Form'a en son
        }

        // ── Chart ─────────────────────────────────────────────────
        void Chart_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
            int W = pnlChart.Width, H = pnlChart.Height;
            if (W < 20 || H < 20) return;

            g.Clear(Color.FromArgb(10, 10, 18));
            CardHdr(g, pnlChart, $"{selPair}  ·  {selTF}", GOLD);

            // Grid
            using (var p = new Pen(Color.FromArgb(10, 255, 255, 255), .5f))
            {
                for (int i = 1; i < 10; i++) g.DrawLine(p, i * W / 10, 0, i * W / 10, H);
                for (int i = 1; i < 6; i++) g.DrawLine(p, 0, i * H / 6, W, i * H / 6);
            }

            double pMin = 1.0790, pMax = 1.0880, pRng = pMax - pMin;
            int cL = 8, cR = W - 8, nBar = 70;
            float bW = (float)(cR - cL) / nBar;
            int chartH = H - 30;

            float PY(double price) => (float)(chartH - ((price - pMin) / pRng) * chartH + 28);

            // ── Zone bölgeleri ────────────────────────────────────
            if (showOB)
            {
                // Bullish OB
                float y1 = PY(1.08240), y2 = PY(1.08160);
                using (var br = new SolidBrush(Color.FromArgb(28, LIME.R, LIME.G, LIME.B))) g.FillRectangle(br, cL, y1, cR - cL, y2 - y1);
                using (var p = new Pen(Color.FromArgb(80, LIME.R, LIME.G, LIME.B), .8f)) g.DrawRectangle(p, cL, y1, cR - cL, y2 - y1);
                using (var f = new Font("Consolas", 7.5f, FontStyle.Bold)) using (var br = new SolidBrush(Color.FromArgb(160, LIME.R, LIME.G, LIME.B)))
                    g.DrawString("Bull OB", f, br, cL + 3, y1 + 2);

                // Bearish OB
                y1 = PY(1.08690); y2 = PY(1.08610);
                using (var br = new SolidBrush(Color.FromArgb(28, ROSE.R, ROSE.G, ROSE.B))) g.FillRectangle(br, cL, y1, cR - cL, y2 - y1);
                using (var p = new Pen(Color.FromArgb(80, ROSE.R, ROSE.G, ROSE.B), .8f)) g.DrawRectangle(p, cL, y1, cR - cL, y2 - y1);
                using (var f = new Font("Consolas", 7.5f, FontStyle.Bold)) using (var br = new SolidBrush(Color.FromArgb(160, ROSE.R, ROSE.G, ROSE.B)))
                    g.DrawString("Bear OB", f, br, cL + 3, y1 + 2);
            }

            if (showFVG)
            {
                float y1 = PY(1.08420), y2 = PY(1.08340);
                using (var br = new SolidBrush(Color.FromArgb(20, BLUE.R, BLUE.G, BLUE.B))) g.FillRectangle(br, cL, y1, cR - cL, y2 - y1);
                using (var p = new Pen(Color.FromArgb(60, BLUE.R, BLUE.G, BLUE.B), .8f) { DashStyle = DashStyle.Dot }) g.DrawRectangle(p, cL, y1, cR - cL, y2 - y1);
                using (var f = new Font("Consolas", 7f)) using (var br = new SolidBrush(Color.FromArgb(130, BLUE.R, BLUE.G, BLUE.B)))
                    g.DrawString("FVG", f, br, cL + 3, y1 + 2);
            }

            if (showBOS)
            {
                // BOS bull line
                float bY = PY(1.08310);
                using (var p = new Pen(Color.FromArgb(200, LIME.R, LIME.G, LIME.B), 1.5f) { DashStyle = DashStyle.Dash }) g.DrawLine(p, cL, bY, cR, bY);
                using (var f = new Font("Consolas", 7.5f, FontStyle.Bold)) using (var br = new SolidBrush(LIME))
                    g.DrawString("BOS ▲", f, br, cR - 46, bY - 13);

                // BOS bear line
                bY = PY(1.08580);
                using (var p = new Pen(Color.FromArgb(200, ROSE.R, ROSE.G, ROSE.B), 1.5f) { DashStyle = DashStyle.Dash }) g.DrawLine(p, cL, bY, cR, bY);
                using (var f = new Font("Consolas", 7.5f, FontStyle.Bold)) using (var br = new SolidBrush(ROSE))
                    g.DrawString("BOS ▼", f, br, cR - 46, bY + 2);
            }

            if (showLiq)
            {
                // Buy Liquidity
                float lY = PY(1.08720);
                using (var p = new Pen(Color.FromArgb(180, GOLD.R, GOLD.G, GOLD.B), 1f) { DashStyle = DashStyle.Dot }) g.DrawLine(p, cL, lY, cR, lY);
                using (var f = new Font("Consolas", 7f)) using (var br = new SolidBrush(GOLD)) g.DrawString("Buy Liq.", f, br, cL + 3, lY - 11);

                // Sell Liquidity (swept)
                lY = PY(1.08080);
                using (var p = new Pen(Color.FromArgb(90, ROSE.R, ROSE.G, ROSE.B), 1f) { DashStyle = DashStyle.Dot }) g.DrawLine(p, cL, lY, cR, lY);
                using (var f = new Font("Consolas", 7f)) using (var br = new SolidBrush(Color.FromArgb(120, ROSE.R, ROSE.G, ROSE.B))) g.DrawString("Sell Liq. (swept)", f, br, cL + 3, lY + 2);
            }

            // ── Mumlar ────────────────────────────────────────────
            double p2 = 1.0828;
            for (int i = 0; i < nBar; i++)
            {
                double o = p2, c = p2 + (RNG.NextDouble() - .48) * .0005;
                double h = Math.Max(o, c) + RNG.NextDouble() * .0002;
                double l = Math.Min(o, c) - RNG.NextDouble() * .0002;
                p2 = c;

                float cx = cL + i * bW + bW / 2f;
                float yH = PY(h), yL = PY(l), yO = PY(o), yC = PY(c);
                bool bull = c >= o;
                Color col = bull ? LIME : ROSE;

                using (var pen = new Pen(Color.FromArgb(160, col.R, col.G, col.B), .8f)) g.DrawLine(pen, cx, yH, cx, yL);
                float top = Math.Min(yO, yC), bh = Math.Max(Math.Abs(yO - yC), 1.5f);
                using (var br = new SolidBrush(Color.FromArgb(bull ? 180 : 150, col.R, col.G, col.B)))
                    g.FillRectangle(br, cx - bW * .38f, top, bW * .76f, bh);
            }

            // ── Canlı fiyat ───────────────────────────────────────
            float curY = PY(livePrice);
            using (var pen = new Pen(Color.FromArgb(70, 255, 255, 255), .6f) { DashStyle = DashStyle.Dot }) g.DrawLine(pen, cL, curY, cR, curY);
            var tag = new Rectangle(cR - 68, (int)curY - 10, 68, 20);
            using (var br = new SolidBrush(Color.FromArgb(180, GOLD.R, GOLD.G, GOLD.B))) g.FillRectangle(br, tag);
            using (var f = new Font("Consolas", 8f, FontStyle.Bold)) using (var br = new SolidBrush(BG))
            { var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }; g.DrawString(livePrice.ToString("F5"), f, br, tag, sf); }
        }

        // ── Info Panel ────────────────────────────────────────────
        void Info_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
            int W = pnlInfo.Width, H = pnlInfo.Height;
            CardHdr(g, pnlInfo, "Market Structure", TEAL);

            var items = new[]
            {
                ("Trend",       "Bullish ▲",   LIME),
                ("Structure",   "HH · HL",     LIME),
                ("Last BOS",    "Bullish",      LIME),
                ("Last CHoCH",  "None",         TXT2),
                ("Bias",        "Long",         LIME),
                ("Premium",     "1.08580+",     ROSE),
                ("Discount",    "1.08310-",     LIME),
                ("Equilibrium", "1.08445",      GOLD),
                ("ATR (14)",    "0.00640",      TXT),
                ("Spread",      "0.8 pips",     TXT2),
            };

            int ry = 34;
            foreach (var (lbl, val, col) in items)
            {
                using (var p = new Pen(Color.FromArgb(7, 255, 255, 255))) g.DrawLine(p, 10, ry + 16, W - 10, ry + 16);
                using (var f = new Font("Segoe UI", 8.5f)) using (var br = new SolidBrush(TXT2)) g.DrawString(lbl, f, br, 10, ry);
                using (var f = new Font("Consolas", 9f, FontStyle.Bold)) using (var br = new SolidBrush(col))
                { var sf = new StringFormat { Alignment = StringAlignment.Far }; g.DrawString(val, f, br, new RectangleF(0, ry, W - 10, 16), sf); }
                ry += 20;
            }

            // Premium / Discount görseli
            int visY = ry + 10;
            if (visY + 80 < H)
            {
                using (var p = new Pen(BDR)) g.DrawLine(p, 10, visY, W - 10, visY);
                using (var f = new Font("Consolas", 7.5f, FontStyle.Bold)) using (var br = new SolidBrush(TXT3))
                    g.DrawString("PRICE RANGE VISUAL", f, br, 10, visY + 4);

                int bx = 10, by2 = visY + 20, bw2 = W - 20, bh2 = 50;
                // premium zone (üst %50)
                using (var br = new SolidBrush(Color.FromArgb(22, ROSE.R, ROSE.G, ROSE.B))) g.FillRectangle(br, bx, by2, bw2, bh2 / 2);
                // discount zone (alt %50)
                using (var br = new SolidBrush(Color.FromArgb(22, LIME.R, LIME.G, LIME.B))) g.FillRectangle(br, bx, by2 + bh2 / 2, bw2, bh2 / 2);
                // equilibrium line
                using (var p = new Pen(Color.FromArgb(140, GOLD.R, GOLD.G, GOLD.B), 1.5f)) g.DrawLine(p, bx, by2 + bh2 / 2, bx + bw2, by2 + bh2 / 2);

                using (var f = new Font("Consolas", 7f)) using (var br = new SolidBrush(ROSE)) g.DrawString("Premium", f, br, bx + 4, by2 + 3);
                using (var f = new Font("Consolas", 7f)) using (var br = new SolidBrush(LIME)) g.DrawString("Discount", f, br, bx + 4, by2 + bh2 / 2 + 3);
                using (var f = new Font("Consolas", 7f)) using (var br = new SolidBrush(GOLD)) g.DrawString("EQ", f, br, bx + bw2 - 20, by2 + bh2 / 2 - 10);

                // current price marker
                float priceMarker = (float)Math.Min(Math.Max((livePrice - 1.0790) / (1.0880 - 1.0790), 0), 1);
                int mX = (int)(bx + bw2 * priceMarker);
                using (var br = new SolidBrush(GOLD)) g.FillEllipse(br, mX - 4, by2 + bh2 / 2 - 4, 8, 8);
            }
        }

        void BuildInfoPanel(Panel p)
        {
            // Info panel sadece Paint ile çiziliyor — ek kontrol yok
        }

        // ── Settings Panel ────────────────────────────────────────
        void BuildSettings(Panel p)
        {
            int y = 36;
            chkBOS = MkChk(p, "Show BOS / CHoCH", LIME, ref y, true); chkBOS.CheckedChanged += (s, e) => { showBOS = chkBOS.Checked; pnlChart?.Invalidate(); };
            chkOB = MkChk(p, "Show Order Blocks", GOLD, ref y, true); chkOB.CheckedChanged += (s, e) => { showOB = chkOB.Checked; pnlChart?.Invalidate(); };
            chkLiq = MkChk(p, "Show Liquidity", ROSE, ref y, true); chkLiq.CheckedChanged += (s, e) => { showLiq = chkLiq.Checked; pnlChart?.Invalidate(); };
            chkFVG = MkChk(p, "Show FVG", BLUE, ref y, true); chkFVG.CheckedChanged += (s, e) => { showFVG = chkFVG.Checked; pnlChart?.Invalidate(); };

            y += 8;
            Sep(p, ref y);

            // Legend
            var legDefs = new[]
            {
                (LIME,"Bullish OB / BOS ▲"),
                (ROSE,"Bearish OB / BOS ▼"),
                (GOLD,"Liquidity Level"),
                (BLUE,"Fair Value Gap (FVG)"),
                (TEAL,"CHoCH (Change of Char)"),
            };
            foreach (var (col, lbl) in legDefs)
            {
                var leg = new Panel { Location = new Point(10, y), Size = new Size(220, 18), BackColor = Color.Transparent };
                int yy = y; Color cc = col; string ll = lbl;
                leg.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var br = new SolidBrush(cc)) e.Graphics.FillRectangle(br, 0, 5, 10, 8);
                    using (var f = new Font("Segoe UI", 8.5f)) using (var br = new SolidBrush(TXT2))
                        e.Graphics.DrawString(ll, f, br, 14, 2);
                };
                p.Controls.Add(leg);
                y += 22;
            }
        }

        CheckBox MkChk(Panel p, string lbl, Color col, ref int y, bool chk)
        {
            var c = new CheckBox { Text = lbl, Checked = chk, Location = new Point(10, y), AutoSize = true, ForeColor = col, Font = new Font("Segoe UI", 9f), BackColor = Color.Transparent };
            p.Controls.Add(c); y += 26; return c;
        }

        void Sep(Panel p, ref int y)
        {
            var s = new Panel { Location = new Point(10, y), Size = new Size(222, 1), BackColor = Color.FromArgb(22, 255, 255, 255) };
            p.Controls.Add(s); y += 8;
        }

        // ── Zones ListView ────────────────────────────────────────
        void BuildZonesLV(Panel p)
        {
            lvZones = new ListView { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true, GridLines = false, BackColor = CARD, ForeColor = TXT, BorderStyle = BorderStyle.None, Font = new Font("Consolas", 8.5f), OwnerDraw = true };
            lvZones.Columns.Add("Type", 55);
            lvZones.Columns.Add("Label", 110);
            lvZones.Columns.Add("Price", 78);
            lvZones.Columns.Add("Top", 78);
            lvZones.Columns.Add("Bottom", 78);
            lvZones.Columns.Add("Status", 82);
            lvZones.Columns.Add("Dir", 52);
            lvZones.Columns.Add("Time", 52);

            foreach (var z in zones)
            {
                var it = new ListViewItem(z.Type) { Tag = z };
                it.SubItems.Add(z.Label);
                it.SubItems.Add(z.Price.ToString("F5"));
                it.SubItems.Add(z.Top.ToString("F5"));
                it.SubItems.Add(z.Bottom.ToString("F5"));
                it.SubItems.Add(z.Status);
                it.SubItems.Add(z.IsBull ? "Bull" : "Bear");
                it.SubItems.Add(z.Time);
                lvZones.Items.Add(it);
            }

            lvZones.DrawColumnHeader += (s, e) =>
            {
                e.Graphics.FillRectangle(new SolidBrush(SURF), e.Bounds);
                using (var p2 = new Pen(BDR)) e.Graphics.DrawLine(p2, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
                using (var f = new Font("Consolas", 7.5f, FontStyle.Bold)) using (var br = new SolidBrush(TXT3))
                    e.Graphics.DrawString(e.Header.Text.ToUpper(), f, br, e.Bounds.Left + 5, e.Bounds.Top + 5);
            };
            lvZones.DrawItem += (s, e) => e.DrawBackground();
            lvZones.DrawSubItem += (s, e) =>
            {
                if (!(e.Item.Tag is SMZone z)) return;
                var g = e.Graphics; var rc = e.Bounds;
                if (e.Item.Index % 2 == 0) using (var br = new SolidBrush(Color.FromArgb(8, 255, 255, 255))) g.FillRectangle(br, rc);

                Color fg = TXT;
                if (e.ColumnIndex == 0) fg = z.Type == "OB" ? GOLD : z.Type == "BOS" ? LIME : z.Type == "LIQ" ? ROSE : BLUE;
                if (e.ColumnIndex == 1) fg = TXT;
                if (e.ColumnIndex == 5) fg = z.Status == "ACTIVE" || z.Status == "OPEN" ? LIME : z.Status == "CONFIRMED" ? GOLD : z.Status == "SWEPT" ? Color.FromArgb(100, ROSE.R, ROSE.G, ROSE.B) : TXT2;
                if (e.ColumnIndex == 6) fg = z.IsBull ? LIME : ROSE;

                using (var f = new Font("Consolas", 8.5f)) using (var br = new SolidBrush(fg))
                    g.DrawString(e.SubItem.Text, f, br, rc.X + 5, rc.Y + 4);
                using (var pen = new Pen(Color.FromArgb(7, 255, 255, 255)))
                    g.DrawLine(pen, rc.Left, rc.Bottom - 1, rc.Right, rc.Bottom - 1);
            };

            var wrap = new Panel { Dock = DockStyle.Fill, BackColor = CARD, Padding = new Padding(0, 28, 0, 0) };
            wrap.Controls.Add(lvZones);
            p.Controls.Add(wrap);
        }

        // ── Card Header ───────────────────────────────────────────
        void CardHdr(Graphics g, Panel p, string title, Color acc)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(Color.FromArgb(22, 255, 255, 255))) g.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            if (p.Width > 2) using (var br = new LinearGradientBrush(new Point(0, 0), new Point(p.Width / 2, 0), Color.FromArgb(55, acc.R, acc.G, acc.B), Color.Transparent)) g.FillRectangle(br, 0, 0, p.Width / 2, 3);
            using (var br = new SolidBrush(Color.FromArgb(14, 255, 255, 255))) g.FillRectangle(br, 0, 3, p.Width, 23);
            using (var pen = new Pen(BDR)) g.DrawLine(pen, 0, 26, p.Width, 26);
            using (var br = new SolidBrush(acc)) g.FillEllipse(br, 8, 11, 4, 4);
            using (var f = new Font("Consolas", 7.5f, FontStyle.Bold)) using (var br = new SolidBrush(TXT3))
                g.DrawString(title.ToUpper(), f, br, 16, 8);
        }

        // ── Ticker ────────────────────────────────────────────────
        void StartTicker()
        {
            ticker = new System.Windows.Forms.Timer { Interval = 1600 };
            ticker.Tick += (s, e) =>
            {
                livePrice += (RNG.NextDouble() - .49) * .00008;
                double chg = livePrice - 1.08387;
                if (lblPrice != null) lblPrice.Text = livePrice.ToString("F5");
                if (lblPriceChg != null) { lblPriceChg.Text = (chg >= 0 ? "▲ +" : "▼ ") + chg.ToString("F5"); lblPriceChg.ForeColor = chg >= 0 ? LIME : ROSE; }
                if (lblClock != null) lblClock.Text = DateTime.Now.ToString("HH:mm:ss  ·  dd.MM.yyyy");
                pnlChart?.Invalidate();
                pnlInfo?.Invalidate();
                pnlSidebar?.Invalidate(true);
                pnlStatus?.Invalidate();
            };
            ticker.Start();
        }

        Panel WinBtn(Color col)
        {
            var p = new Panel { Size = new Size(12, 12), BackColor = col, Cursor = Cursors.Hand };
            p.Paint += (s, e) => { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; using (var br = new SolidBrush(p.BackColor)) e.Graphics.FillEllipse(br, 0, 0, 11, 11); };
            p.MouseEnter += (s, e) => p.BackColor = ControlPaint.Light(col, .3f);
            p.MouseLeave += (s, e) => p.BackColor = col;
            return p;
        }

        protected override void OnFormClosed(FormClosedEventArgs e) { ticker?.Stop(); base.OnFormClosed(e); }
    }

    static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern IntPtr SendMessage(IntPtr h, int m, IntPtr w, IntPtr l);
    }
}
