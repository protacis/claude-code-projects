using System;
using System.Drawing;
using System.Windows.Forms;

namespace InventorParamsAddin
{
    public class ParamsPanel : UserControl
    {
        private DataGridView _grid;
        private Button       _btnRefresh;
        private Button       _btnApply;
        private Label        _lblStatus;

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
            // ── toolbar ──────────────────────────────────────────
            var toolbar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 40,
                BackColor = BgColor,
                Padding   = new Padding(6, 5, 6, 0)
            };

            _btnRefresh = MakeButton("⟳  Ανανέωση", 0);
            _btnRefresh.Click += OnRefresh;

            _btnApply = MakeButton("✔  Εφαρμογή", 108);
            _btnApply.BackColor = Color.FromArgb(22, 50, 30);
            _btnApply.ForeColor = GreenColor;
            _btnApply.FlatAppearance.BorderColor = Color.FromArgb(60, 140, 80);
            _btnApply.Click += OnApply;

            toolbar.Controls.Add(_btnRefresh);
            toolbar.Controls.Add(_btnApply);

            // ── status label ─────────────────────────────────────
            _lblStatus = new Label
            {
                Dock      = DockStyle.Bottom,
                Height    = 22,
                BackColor = BgColor,
                ForeColor = SubColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(6, 0, 0, 0),
                Text      = "Πάτα Ανανέωση"
            };

            // ── grid ─────────────────────────────────────────────
            _grid = new DataGridView
            {
                Dock                          = DockStyle.Fill,
                BackgroundColor               = BgColor,
                ForeColor                     = Color.White,
                GridColor                     = Color.FromArgb(30, 30, 56),
                BorderStyle                   = BorderStyle.None,
                ReadOnly                      = false,
                AllowUserToAddRows            = false,
                AllowUserToDeleteRows         = false,
                SelectionMode                 = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode           = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible             = false,
                CellBorderStyle               = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersHeightSizeMode   = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight           = 28,
                EnableHeadersVisualStyles     = false,
            };

            _grid.DefaultCellStyle.BackColor          = CardColor;
            _grid.DefaultCellStyle.ForeColor          = Color.White;
            _grid.DefaultCellStyle.SelectionBackColor = AccentColor;
            _grid.DefaultCellStyle.SelectionForeColor = Color.White;
            _grid.AlternatingRowsDefaultCellStyle.BackColor = BgColor;
            _grid.ColumnHeadersDefaultCellStyle.BackColor   = Color.FromArgb(18, 17, 42);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor   = SubColor;
            _grid.ColumnHeadersDefaultCellStyle.Font        = new Font("Segoe UI", 8f, FontStyle.Bold);

            AddColumn("cName", "Όνομα",  40, readOnly: true);
            AddColumn("cVal",  "Τιμή",   60, readOnly: false);

            Controls.Add(_grid);
            Controls.Add(toolbar);
            Controls.Add(_lblStatus);
        }

        private Button MakeButton(string text, int x)
        {
            var btn = new Button
            {
                Text      = text,
                Size      = new Size(100, 28),
                Location  = new Point(x, 4),
                BackColor = CardColor,
                ForeColor = Color.FromArgb(170, 196, 255),
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 8.5f)
            };
            btn.FlatAppearance.BorderColor = AccentColor;
            return btn;
        }

        private void AddColumn(string name, string header, int weight, bool readOnly)
        {
            var col = new DataGridViewTextBoxColumn
            {
                Name       = name,
                HeaderText = header,
                FillWeight = weight,
                ReadOnly   = readOnly
            };
            _grid.Columns.Add(col);
        }

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
                        _grid.Rows.Add((string)p.Name, expr);
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
