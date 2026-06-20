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
        private DataGridView _grid;
        private Button       _btnRefresh;
        private Button       _btnApply;
        private Label        _lblStatus;
        private ImageList    _imgList;

        private static readonly Color BgColor    = Color.FromArgb(26, 26, 46);
        private static readonly Color CardColor  = Color.FromArgb(22, 33, 62);
        private static readonly Color AccentColor= Color.FromArgb(74, 111, 165);
        private static readonly Color SubColor   = Color.FromArgb(102, 102, 102);
        private static readonly Color GreenColor = Color.FromArgb(100, 200, 120);
        private static readonly Color RedColor   = Color.FromArgb(224, 82, 82);

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
                Dock      = DockStyle.Top,
                Height    = 56,
                BackColor = Color.FromArgb(12, 12, 28),
                SizeMode  = PictureBoxSizeMode.Zoom,
                Padding   = new Padding(10, 6, 10, 6),
                Image     = LoadLogo()
            };

            // ── toolbar ──────────────────────────────────────────
            var toolbar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 40,
                BackColor = BgColor,
            };

            _btnRefresh = MakeButton("  Ανανέωση", 6, MakeRefreshIcon());
            _btnRefresh.Click += OnRefresh;

            _btnApply = MakeButton("  Εφαρμογή", 114, MakeApplyIcon());
            _btnApply.BackColor = Color.FromArgb(22, 50, 30);
            _btnApply.ForeColor = GreenColor;
            _btnApply.FlatAppearance.BorderColor = Color.FromArgb(60, 140, 80);
            _btnApply.Click += OnApply;

            toolbar.Controls.Add(_btnRefresh);
            toolbar.Controls.Add(_btnApply);

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

            // ── image list for grid ──────────────────────────────
            _imgList = new ImageList { ImageSize = new Size(14, 14), ColorDepth = ColorDepth.Depth32Bit };
            _imgList.Images.Add("param", MakeParamIcon());

            // ── grid ─────────────────────────────────────────────
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

            // icon column
            var iconCol = new DataGridViewImageColumn
            {
                Name        = "cIcon",
                HeaderText  = "",
                Width       = 24,
                ReadOnly    = true,
                Resizable   = DataGridViewTriState.False,
                DefaultCellStyle = { NullValue = null, Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            _grid.Columns.Add(iconCol);

            AddTextColumn("cName", "Όνομα", 38, readOnly: true);
            AddTextColumn("cVal",  "Τιμή",  38, readOnly: false);

            Controls.Add(_grid);
            Controls.Add(_lblStatus);
            Controls.Add(toolbar);
            Controls.Add(banner);
        }

        // ── helpers ──────────────────────────────────────────────

        private Button MakeButton(string text, int x, Image icon)
        {
            var btn = new Button
            {
                Text      = text,
                Size      = new Size(102, 28),
                Location  = new Point(x, 6),
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
                Name       = name,
                HeaderText = header,
                FillWeight = weight,
                ReadOnly   = readOnly
            });
        }

        // ── icon generators (GDI+) ───────────────────────────────

        private static Image LoadLogo()
        {
            try
            {
                var asm    = Assembly.GetExecutingAssembly();
                var resName = asm.GetManifestResourceNames();
                foreach (var r in resName)
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
            // arrowhead
            var tip = new Point(12, 7);
            g.FillPolygon(Brushes.SteelBlue, new[] { new Point(10,4), new Point(14,4), new Point(12,8) });
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
            using var brush = new SolidBrush(Color.FromArgb(74, 111, 165));
            g.FillEllipse(brush, 1, 1, 12, 12);
            using var pen = new Pen(Color.FromArgb(170, 196, 255), 1.5f);
            g.DrawLine(pen, 4, 7, 10, 7);  // horizontal
            g.DrawLine(pen, 7, 4, 7, 10);  // vertical
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
                        int row = _grid.Rows.Add(_imgList.Images["param"], (string)p.Name, expr);
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
                        if ((string)p.Expression != newExpr)
                        {
                            p.Expression = newExpr;
                            updated++;
                        }
                    }
                    catch { errors++; }
                }

                doc.Update();

                if (errors > 0)
                    SetStatus($"{errors} σφάλματα κατά την εφαρμογή", RedColor);
                else
                    SetStatus($"{updated} αλλαγές εφαρμόστηκαν", GreenColor);
            }
            catch (Exception ex)
            {
                SetStatus($"Σφάλμα: {ex.Message}", RedColor);
            }
        }

        private void SetStatus(string text, Color color)
        {
            _lblStatus.Text      = text;
            _lblStatus.ForeColor = color;
        }
    }
}
