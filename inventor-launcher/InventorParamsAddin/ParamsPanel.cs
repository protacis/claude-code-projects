using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace InventorParamsAddin
{
    public class ParamsPanel : UserControl
    {
        // params tab controls
        private DataGridView _grid;
        private Button       _btnRefresh;
        private Button       _btnApply;
        private Label        _lblStatus;
        private ImageList    _imgList;

        private static readonly Color BgColor     = Color.FromArgb(26, 26, 46);
        private static readonly Color CardColor   = Color.FromArgb(22, 33, 62);
        private static readonly Color AccentColor = Color.FromArgb(74, 111, 165);
        private static readonly Color SubColor    = Color.FromArgb(102, 102, 102);
        private static readonly Color GreenColor  = Color.FromArgb(100, 200, 120);
        private static readonly Color RedColor    = Color.FromArgb(224, 82, 82);

        public ParamsPanel()
        {
            BackColor = BgColor;
            Dock      = DockStyle.Fill;
            Font      = new Font("Segoe UI", 9f);
            BuildUI();
        }

        private void BuildUI()
        {
            // ── logo banner ──────────────────────────────────────
            var banner = new PictureBox
            {
                Dock     = DockStyle.Top,
                Height   = 52,
                BackColor= Color.FromArgb(12, 12, 28),
                SizeMode = PictureBoxSizeMode.Zoom,
                Padding  = new Padding(10, 6, 10, 6),
                Image    = LoadLogo()
            };

            // ── menu strip ───────────────────────────────────────
            var menu = new MenuStrip
            {
                BackColor   = Color.FromArgb(18, 17, 42),
                ForeColor   = Color.FromArgb(170, 170, 200),
                RenderMode  = ToolStripRenderMode.Professional,
                Renderer    = new DarkMenuRenderer(),
                Dock        = DockStyle.Top,
                Padding     = new Padding(4, 0, 0, 0)
            };

            var mFile = AddMenu(menu, "Αρχείο");
            AddMenuItem(mFile, "Άνοιγμα σχεδίου...", null);
            AddMenuItem(mFile, "Πρόσφατα", null);
            mFile.DropDownItems.Add(new ToolStripSeparator());
            AddMenuItem(mFile, "Κλείσιμο", null);

            var mView = AddMenu(menu, "Προβολή");
            AddMenuItem(mView, "Ανανέωση", () => OnRefresh(null, null));
            mView.DropDownItems.Add(new ToolStripSeparator());
            AddMenuItem(mView, "Εμφάνιση όλων παραμέτρων", null);
            AddMenuItem(mView, "Μόνο Key", null);

            var mHelp = AddMenu(menu, "Βοήθεια");
            AddMenuItem(mHelp, "Σχετικά με...", null);

            // ── tab control ──────────────────────────────────────
            var tabs = new TabControl
            {
                Dock      = DockStyle.Fill,
                Font      = new Font("Segoe UI", 9f),
                Appearance= TabAppearance.Normal,
            };
            tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabs.DrawItem += DrawTab;
            tabs.ItemSize  = new Size(0, 26);

            tabs.Controls.Add(BuildParamsTab());
            tabs.Controls.Add(BuildDummyTab("Υλικά",    "⬡", "Δεν υπάρχει συνδεδεμένο\nυλικό στο ενεργό σχέδιο."));
            tabs.Controls.Add(BuildDummyTab("iProperties", "☰", "Τίτλος, Σχεδιαστής, Έκδοση\nκαι άλλα metadata."));
            tabs.Controls.Add(BuildDummyTab("Ιστορικό",  "⏱", "Λίστα αλλαγών παραμέτρων\nανά session."));

            // ── status bar ───────────────────────────────────────
            _lblStatus = new Label
            {
                Dock      = DockStyle.Bottom,
                Height    = 22,
                BackColor = Color.FromArgb(12, 12, 28),
                ForeColor = SubColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(6, 0, 0, 0),
                Text      = "Πάτα Ανανέωση"
            };

            Controls.Add(tabs);
            Controls.Add(_lblStatus);
            Controls.Add(menu);
            Controls.Add(banner);
        }

        // ── tab builders ─────────────────────────────────────────

        private TabPage BuildParamsTab()
        {
            var page = MakeTabPage("Παράμετροι");

            // toolbar
            var toolbar = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = BgColor };

            _btnRefresh = MakeButton("  Ανανέωση", 6, MakeRefreshIcon());
            _btnRefresh.Click += OnRefresh;

            _btnApply = MakeButton("  Εφαρμογή", 114, MakeApplyIcon());
            _btnApply.BackColor = Color.FromArgb(22, 50, 30);
            _btnApply.ForeColor = GreenColor;
            _btnApply.FlatAppearance.BorderColor = Color.FromArgb(60, 140, 80);
            _btnApply.Click += OnApply;

            toolbar.Controls.Add(_btnRefresh);
            toolbar.Controls.Add(_btnApply);

            // image list
            _imgList = new ImageList { ImageSize = new Size(14, 14), ColorDepth = ColorDepth.Depth32Bit };
            _imgList.Images.Add("param", MakeParamIcon());

            // grid
            _grid = new DataGridView
            {
                Dock                        = DockStyle.Fill,
                BackgroundColor             = BgColor,
                ForeColor                   = Color.White,
                GridColor                   = Color.FromArgb(30, 30, 56),
                BorderStyle                 = BorderStyle.None,
                ReadOnly                    = false,
                AllowUserToAddRows          = false,
                AllowUserToDeleteRows       = false,
                SelectionMode               = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode         = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible           = false,
                CellBorderStyle             = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight         = 28,
                EnableHeadersVisualStyles   = false,
                RowTemplate                 = { Height = 26 }
            };
            _grid.DefaultCellStyle.BackColor          = CardColor;
            _grid.DefaultCellStyle.ForeColor          = Color.White;
            _grid.DefaultCellStyle.SelectionBackColor = AccentColor;
            _grid.DefaultCellStyle.SelectionForeColor = Color.White;
            _grid.AlternatingRowsDefaultCellStyle.BackColor = BgColor;
            _grid.ColumnHeadersDefaultCellStyle.BackColor   = Color.FromArgb(18, 17, 42);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor   = SubColor;
            _grid.ColumnHeadersDefaultCellStyle.Font        = new Font("Segoe UI", 8f, FontStyle.Bold);

            _grid.Columns.Add(new DataGridViewImageColumn
            {
                Name      = "cIcon",
                HeaderText= "",
                Width     = 24,
                ReadOnly  = true,
                Resizable = DataGridViewTriState.False,
                DefaultCellStyle = { NullValue = null, Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            AddTextColumn("cName", "Όνομα", 38, true);
            AddTextColumn("cVal",  "Τιμή",  38, false);

            page.Controls.Add(_grid);
            page.Controls.Add(toolbar);
            return page;
        }

        private TabPage BuildDummyTab(string title, string icon, string message)
        {
            var page = MakeTabPage(title);

            var pnl = new Panel { Dock = DockStyle.Fill, BackColor = BgColor };

            var ico = new Label
            {
                Text      = icon,
                Font      = new Font("Segoe UI", 28f),
                ForeColor = Color.FromArgb(50, 60, 90),
                AutoSize  = false,
                Size      = new Size(60, 60),
                TextAlign = ContentAlignment.MiddleCenter,
            };

            var lbl = new Label
            {
                Text      = message,
                ForeColor = SubColor,
                Font      = new Font("Segoe UI", 9f),
                AutoSize  = false,
                Size      = new Size(220, 50),
                TextAlign = ContentAlignment.MiddleCenter,
            };

            pnl.Controls.Add(ico);
            pnl.Controls.Add(lbl);

            pnl.Resize += (s, e) =>
            {
                ico.Location = new Point((pnl.Width - ico.Width) / 2, pnl.Height / 2 - 64);
                lbl.Location = new Point((pnl.Width - lbl.Width) / 2, pnl.Height / 2 - 4);
            };

            page.Controls.Add(pnl);
            return page;
        }

        private static TabPage MakeTabPage(string title)
        {
            return new TabPage(title) { BackColor = BgColor, ForeColor = Color.White, UseVisualStyleBackColor = false };
        }

        // ── tab owner-draw (dark style) ───────────────────────────

        private void DrawTab(object sender, DrawItemEventArgs e)
        {
            var tabs = (TabControl)sender;
            var page = tabs.TabPages[e.Index];
            bool selected = e.Index == tabs.SelectedIndex;

            var bg = selected ? CardColor : Color.FromArgb(18, 17, 42);
            using (var brush = new SolidBrush(bg))
                e.Graphics.FillRectangle(brush, e.Bounds);

            if (selected)
            {
                using var pen = new Pen(AccentColor, 2);
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }

            var fg = selected ? Color.White : SubColor;
            TextRenderer.DrawText(e.Graphics, page.Text, new Font("Segoe UI", 8.5f, selected ? FontStyle.Bold : FontStyle.Regular),
                e.Bounds, fg, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // ── menu helpers ─────────────────────────────────────────

        private static ToolStripMenuItem AddMenu(MenuStrip strip, string text)
        {
            var item = new ToolStripMenuItem(text) { ForeColor = Color.FromArgb(170, 170, 200) };
            strip.Items.Add(item);
            return item;
        }

        private static void AddMenuItem(ToolStripMenuItem parent, string text, Action onClick)
        {
            var item = new ToolStripMenuItem(text);
            if (onClick != null) item.Click += (s, e) => onClick();
            parent.DropDownItems.Add(item);
        }

        // ── button / column helpers ───────────────────────────────

        private Button MakeButton(string text, int x, Image icon)
        {
            var btn = new Button
            {
                Text      = text,
                Size      = new Size(102, 28),
                Location  = new Point(x, 5),
                BackColor = CardColor,
                ForeColor = Color.FromArgb(170, 196, 255),
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 8.5f),
                Image     = icon,
                ImageAlign= ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Padding   = new Padding(4, 0, 6, 0)
            };
            btn.FlatAppearance.BorderColor = AccentColor;
            return btn;
        }

        private void AddTextColumn(string name, string header, int weight, bool readOnly)
        {
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = name, HeaderText = header, FillWeight = weight, ReadOnly = readOnly
            });
        }

        // ── GDI+ icons ───────────────────────────────────────────

        private static Image LoadLogo()
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                foreach (var r in asm.GetManifestResourceNames())
                {
                    if (!r.EndsWith("logo.png", StringComparison.OrdinalIgnoreCase)) continue;
                    using var stream = asm.GetManifestResourceStream(r);
                    return Image.FromStream(new MemoryStream(ReadAll(stream)));
                }
            }
            catch { }
            return null;
        }

        private static byte[] ReadAll(Stream s)
        {
            using var ms = new MemoryStream();
            s.CopyTo(ms);
            return ms.ToArray();
        }

        private static Bitmap MakeRefreshIcon()
        {
            var bmp = new Bitmap(16, 16);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);
            using var pen = new Pen(Color.FromArgb(170, 196, 255), 2f);
            g.DrawArc(pen, 2, 2, 11, 11, -30, 270);
            g.FillPolygon(new SolidBrush(Color.FromArgb(170, 196, 255)),
                new[] { new Point(10, 4), new Point(14, 4), new Point(12, 8) });
            return bmp;
        }

        private static Bitmap MakeApplyIcon()
        {
            var bmp = new Bitmap(16, 16);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);
            using var pen = new Pen(GreenColor, 2.2f) { LineJoin = LineJoin.Round };
            g.DrawLines(pen, new[] { new Point(2, 8), new Point(6, 12), new Point(14, 4) });
            return bmp;
        }

        private static Bitmap MakeParamIcon()
        {
            var bmp = new Bitmap(14, 14);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);
            g.FillEllipse(new SolidBrush(Color.FromArgb(74, 111, 165)), 1, 1, 12, 12);
            using var pen = new Pen(Color.FromArgb(170, 196, 255), 1.5f);
            g.DrawLine(pen, 4, 7, 10, 7);
            g.DrawLine(pen, 7, 4, 7, 10);
            return bmp;
        }

        // ── events ───────────────────────────────────────────────

        private void OnRefresh(object sender, EventArgs e)
        {
            _grid.Rows.Clear();
            var app = AddinServer.App;
            if (app == null) { SetStatus("Add-in δεν είναι ενεργό", RedColor); return; }

            try
            {
                dynamic doc    = app.ActiveDocument;
                dynamic mParam = doc.ComponentDefinition.Parameters.ModelParameters;
                int n          = (int)mParam.Count;

                for (int i = 1; i <= n; i++)
                {
                    try
                    {
                        dynamic p = mParam[i];
                        if (!(bool)p.IsKey) continue;
                        string expr = string.IsNullOrEmpty((string)p.Expression) ? "—" : (string)p.Expression;
                        _grid.Rows.Add(_imgList.Images["param"], (string)p.Name, expr);
                    }
                    catch { }
                }

                if (_grid.Rows.Count == 0)
                    SetStatus($"{(string)doc.DisplayName}  —  δεν βρέθηκαν key parameters", SubColor);
                else
                    SetStatus((string)doc.DisplayName, Color.White);
            }
            catch
            {
                SetStatus("Δεν υπάρχει ανοιχτό σχέδιο", RedColor);
            }
        }

        private void OnApply(object sender, EventArgs e)
        {
            var app = AddinServer.App;
            if (app == null) { SetStatus("Add-in δεν είναι ενεργό", RedColor); return; }

            try
            {
                dynamic doc    = app.ActiveDocument;
                dynamic mParam = doc.ComponentDefinition.Parameters.ModelParameters;
                int updated = 0, errors = 0;

                foreach (DataGridViewRow row in _grid.Rows)
                {
                    string pName   = row.Cells["cName"].Value?.ToString();
                    string newExpr = row.Cells["cVal"].Value?.ToString();
                    if (string.IsNullOrEmpty(pName) || string.IsNullOrEmpty(newExpr)) continue;
                    try
                    {
                        dynamic p = mParam[pName];
                        if ((string)p.Expression != newExpr) { p.Expression = newExpr; updated++; }
                    }
                    catch { errors++; }
                }

                doc.Update();

                if (errors > 0) SetStatus($"{errors} σφάλματα", RedColor);
                else            SetStatus($"{updated} αλλαγές εφαρμόστηκαν", GreenColor);
            }
            catch (Exception ex) { SetStatus($"Σφάλμα: {ex.Message}", RedColor); }
        }

        private void SetStatus(string text, Color color)
        {
            _lblStatus.Text      = text;
            _lblStatus.ForeColor = color;
        }
    }

    // ── dark menu renderer ────────────────────────────────────────────
    internal class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        private static readonly Color MenuBg      = Color.FromArgb(18, 17, 42);
        private static readonly Color DropdownBg  = Color.FromArgb(22, 33, 62);
        private static readonly Color HoverBg     = Color.FromArgb(42, 60, 100);
        private static readonly Color BorderColor = Color.FromArgb(42, 42, 74);

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using var b = new SolidBrush(MenuBg);
            e.Graphics.FillRectangle(b, e.AffectedBounds);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var bg = e.Item.Selected ? HoverBg : (e.Item.Owner is ToolStripDropDown ? DropdownBg : MenuBg);
            using var b = new SolidBrush(bg);
            e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is ToolStripDropDown)
            {
                using var pen = new Pen(BorderColor);
                e.Graphics.DrawRectangle(pen, 0, 0, e.AffectedBounds.Width - 1, e.AffectedBounds.Height - 1);
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            int y = e.Item.Height / 2;
            using var pen = new Pen(BorderColor);
            e.Graphics.DrawLine(pen, 4, y, e.Item.Width - 4, y);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Enabled ? Color.FromArgb(200, 200, 220) : Color.FromArgb(80, 80, 100);
            base.OnRenderItemText(e);
        }
    }
}
