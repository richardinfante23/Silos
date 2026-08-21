using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace principal
{
    /// <summary>
    /// Filtro por encabezado de columna para DataGridView (BindingSource + popup).
    /// Clic en el encabezado abre el filtro; clic derecho limpia el de esa columna.
    /// </summary>
    public sealed class GridHeaderFilter
    {
        private readonly DataGridView _grid;
        private readonly BindingSource _binding = new BindingSource();
        private readonly Dictionary<string, string> _filters =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _baseHeaders =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private bool _attached;

        public GridHeaderFilter(DataGridView grid)
        {
            if (grid == null) throw new ArgumentNullException("grid");
            _grid = grid;
            Attach();
        }

        public BindingSource BindingSource
        {
            get { return _binding; }
        }

        public void Bind(DataTable table)
        {
            string previousFilter = BuildFilterExpression();
            _binding.DataSource = table ?? new DataTable();
            _grid.DataSource = _binding;

            // Reaplicar filtros si las columnas siguen existiendo.
            if (!string.IsNullOrEmpty(previousFilter))
            {
                try { _binding.Filter = previousFilter; }
                catch { _filters.Clear(); _binding.RemoveFilter(); }
            }

            CaptureBaseHeaders();
            RefreshHeaderCaptions();
        }

        public void ClearAllFilters()
        {
            _filters.Clear();
            _binding.RemoveFilter();
            RefreshHeaderCaptions();
        }

        private void Attach()
        {
            if (_attached) return;
            _attached = true;

            _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            if (_grid.ColumnHeadersHeight < 32)
                _grid.ColumnHeadersHeight = 32;
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            _grid.ColumnHeaderMouseClick += Grid_ColumnHeaderMouseClick;
            _grid.DataBindingComplete += (s, e) =>
            {
                CaptureBaseHeaders();
                RefreshHeaderCaptions();
            };
        }

        public void SyncHeaders()
        {
            // Forzar recaptura (p. ej. después de SetHeader).
            foreach (DataGridViewColumn col in _grid.Columns)
            {
                if (col == null || string.IsNullOrEmpty(col.Name)) continue;
                string header = col.HeaderText ?? col.Name;
                header = header.Replace(" ▼", string.Empty).Replace(" ▽", string.Empty).Trim();
                _baseHeaders[col.Name] = header;
            }
            RefreshHeaderCaptions();
        }

        private void CaptureBaseHeaders()
        {
            foreach (DataGridViewColumn col in _grid.Columns)
            {
                if (col == null || string.IsNullOrEmpty(col.Name)) continue;

                string header = col.HeaderText ?? col.Name;
                header = header.Replace(" ▼", string.Empty).Replace(" ▽", string.Empty).Trim();

                string existing;
                if (_baseHeaders.TryGetValue(col.Name, out existing)
                    && !string.IsNullOrEmpty(existing)
                    && (col.HeaderText ?? string.Empty).Contains("▼"))
                {
                    continue;
                }

                _baseHeaders[col.Name] = header;
            }
        }

        private void RefreshHeaderCaptions()
        {
            foreach (DataGridViewColumn col in _grid.Columns)
            {
                if (col == null || string.IsNullOrEmpty(col.Name)) continue;

                string baseText;
                if (!_baseHeaders.TryGetValue(col.Name, out baseText) || string.IsNullOrEmpty(baseText))
                    baseText = (col.HeaderText ?? col.Name).Replace(" ▼", string.Empty).Trim();

                bool active = _filters.ContainsKey(col.Name) && !string.IsNullOrWhiteSpace(_filters[col.Name]);
                col.HeaderText = active ? (baseText + " ▼") : baseText;
                col.HeaderCell.Style.BackColor = UiTheme.Primary;
                col.HeaderCell.Style.SelectionBackColor = UiTheme.Primary;
                col.HeaderCell.Style.ForeColor = active ? UiTheme.Accent : Color.White;
                col.HeaderCell.Style.SelectionForeColor = Color.White;
                col.HeaderCell.ToolTipText = active
                    ? ("Filtro: " + _filters[col.Name] + "  |  Clic derecho para limpiar")
                    : "Clic para filtrar esta columna";
            }
        }

        private void Grid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0) return;
            DataGridViewColumn column = _grid.Columns[e.ColumnIndex];
            if (column == null || !column.Visible) return;

            if (e.Button == MouseButtons.Right)
            {
                if (_filters.Remove(column.Name))
                {
                    ApplyCombinedFilter();
                    RefreshHeaderCaptions();
                }
                return;
            }

            if (e.Button != MouseButtons.Left) return;
            ShowFilterPopup(column);
        }

        private void ShowFilterPopup(DataGridViewColumn column)
        {
            string current;
            _filters.TryGetValue(column.Name, out current);
            current = current ?? string.Empty;

            var host = new ToolStripControlHost(BuildFilterPanel(column.Name, current))
            {
                AutoSize = false,
                Size = new Size(320, 110),
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            var drop = new ToolStripDropDown
            {
                AutoClose = true,
                AutoSize = true,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };
            drop.Items.Add(host);

            Rectangle headerRect = _grid.GetCellDisplayRectangle(column.Index, -1, true);
            Point screen = _grid.PointToScreen(new Point(headerRect.Left, headerRect.Bottom));
            drop.Show(screen);
        }

        private Control BuildFilterPanel(string columnName, string currentValue)
        {
            var panel = new Panel
            {
                Width = 320,
                Height = 110,
                BackColor = UiTheme.Card,
                Padding = new Padding(10)
            };

            var lbl = new Label
            {
                Text = "Filtrar columna",
                AutoSize = true,
                Location = new Point(12, 10),
                ForeColor = UiTheme.TextMuted,
                Font = UiTheme.FontRegular
            };

            var txt = new TextBox
            {
                Text = currentValue,
                Location = new Point(12, 34),
                Width = 294,
                Font = UiTheme.FontRegular
            };
            UiTheme.StyleTextBox(txt);

            var btnAplicar = new Button
            {
                Text = "Aplicar",
                Location = new Point(12, 68),
                Size = new Size(120, 32),
                AutoSize = false
            };
            var btnLimpiar = new Button
            {
                Text = "Limpiar",
                Location = new Point(142, 68),
                Size = new Size(120, 32),
                AutoSize = false
            };

            // Sin iconos: el ancho del popup es limitado.
            UiTheme.StyleActionButton(btnAplicar, UiTheme.Accent, Color.White, UiTheme.AccentHover, 0, null);
            UiTheme.StyleActionButton(btnLimpiar, UiTheme.Card, UiTheme.TextPrimary, Color.FromArgb(230, 236, 241), 1, null, UiTheme.Border);
            btnAplicar.Size = new Size(120, 32);
            btnLimpiar.Size = new Size(120, 32);
            btnAplicar.Padding = new Padding(6, 0, 6, 0);
            btnLimpiar.Padding = new Padding(6, 0, 6, 0);

            Action apply = () =>
            {
                string value = (txt.Text ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(value))
                    _filters.Remove(columnName);
                else
                    _filters[columnName] = value;

                ApplyCombinedFilter();
                RefreshHeaderCaptions();
                CloseParentDropDown(panel);
            };

            btnAplicar.Click += (s, e) => apply();
            btnLimpiar.Click += (s, e) =>
            {
                txt.Text = string.Empty;
                _filters.Remove(columnName);
                ApplyCombinedFilter();
                RefreshHeaderCaptions();
                CloseParentDropDown(panel);
            };
            txt.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    apply();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    CloseParentDropDown(panel);
                }
            };

            panel.Controls.Add(lbl);
            panel.Controls.Add(txt);
            panel.Controls.Add(btnAplicar);
            panel.Controls.Add(btnLimpiar);
            panel.HandleCreated += (s, e) => txt.Focus();
            return panel;
        }

        private static void CloseParentDropDown(Control control)
        {
            Control current = control;
            while (current != null)
            {
                var drop = current as ToolStripDropDown;
                if (drop != null)
                {
                    drop.Close();
                    return;
                }
                current = current.Parent;
            }
        }

        private void ApplyCombinedFilter()
        {
            string expression = BuildFilterExpression();
            try
            {
                if (string.IsNullOrEmpty(expression))
                    _binding.RemoveFilter();
                else
                    _binding.Filter = expression;
            }
            catch
            {
                _binding.RemoveFilter();
            }
        }

        private string BuildFilterExpression()
        {
            if (_filters.Count == 0) return string.Empty;

            var parts = new List<string>();
            foreach (var pair in _filters)
            {
                if (string.IsNullOrWhiteSpace(pair.Value)) continue;
                if (_binding.DataSource is DataTable table && !table.Columns.Contains(pair.Key))
                    continue;

                string escaped = EscapeForRowFilter(pair.Value);
                // Convert evita problemas con numéricos/fechas.
                parts.Add(string.Format("Convert([{0}], 'System.String') LIKE '%{1}%'", pair.Key, escaped));
            }
            return string.Join(" AND ", parts.ToArray());
        }

        private static string EscapeForRowFilter(string value)
        {
            if (value == null) return string.Empty;
            return value
                .Replace("'", "''")
                .Replace("[", "[[]")
                .Replace("]", "[]]")
                .Replace("*", "[*]")
                .Replace("%", "[%]");
        }
    }
}
