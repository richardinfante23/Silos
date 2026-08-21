using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace principal
{
    /// <summary>
    /// Listado emergente de productores (naturales y jurídicos) para selección.
    /// Doble clic elige y cierra.
    /// </summary>
    public class productorPickerForm : Form
    {
        private readonly ProductorData _data = new ProductorData();
        private GridHeaderFilter _filtro;
        private DataGridView _dgv;
        private TextBox _txtBuscar;
        private Button _btnCerrar;
        private DataTable _tabla;

        public int IdNatural { get; private set; }
        public int IdJuridico { get; private set; }
        public string Documento { get; private set; }
        public string Nombre { get; private set; }
        public string Letra { get; private set; }
        public bool Seleccionado { get; private set; }

        public productorPickerForm()
        {
            Documento = string.Empty;
            Nombre = string.Empty;
            Letra = string.Empty;
            ConstruirUi();
            Load += productorPickerForm_Load;
        }

        private void productorPickerForm_Load(object sender, EventArgs e)
        {
            UiTheme.ApplyForm(this);
            UiTheme.ApplyAppIcon(this);
            UiTheme.StyleDataGridView(_dgv);
            Text = "Seleccionar productor";
            Cargar();
        }

        private void ConstruirUi()
        {
            SuspendLayout();
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(720, 480);
            BackColor = UiTheme.Surface;

            var lbl = new Label
            {
                Text = "Buscar por cédula / RIF o nombre · doble clic para seleccionar",
                Location = new Point(16, 14),
                AutoSize = true,
                ForeColor = UiTheme.TextMuted
            };

            _txtBuscar = new TextBox
            {
                Location = new Point(16, 36),
                Size = new Size(560, 28)
            };
            UiTheme.StyleTextBox(_txtBuscar);
            _txtBuscar.TextChanged += (s, e) => AplicarFiltroTexto();
            _txtBuscar.KeyDown += txtBuscar_KeyDown;

            _btnCerrar = new Button
            {
                Text = "Cerrar",
                Location = new Point(590, 34),
                Size = new Size(100, 32)
            };
            _btnCerrar.Click += (s, e) => Close();
            UiTheme.StyleSecondaryButton(_btnCerrar);

            _dgv = new DataGridView
            {
                Location = new Point(16, 78),
                Size = new Size(688, 380),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false
            };
            _dgv.CellDoubleClick += dgv_CellDoubleClick;

            Controls.Add(lbl);
            Controls.Add(_txtBuscar);
            Controls.Add(_btnCerrar);
            Controls.Add(_dgv);
            ResumeLayout(true);
        }

        private void Cargar()
        {
            try
            {
                _tabla = new DataTable();
                _tabla.Columns.Add("id_natural", typeof(int));
                _tabla.Columns.Add("id_juridico", typeof(int));
                _tabla.Columns.Add("tipo", typeof(string));
                _tabla.Columns.Add("letra", typeof(string));
                _tabla.Columns.Add("documento", typeof(string));
                _tabla.Columns.Add("nombre", typeof(string));

                foreach (DataRow fila in _data.buscarGeneral().Rows)
                {
                    if (EsEliminado(fila)) continue;
                    _tabla.Rows.Add(
                        Convert.ToInt32(GetCol(fila, "id")),
                        0,
                        "Natural",
                        Convert.ToString(GetCol(fila, "letra")).Trim(),
                        Convert.ToString(GetCol(fila, "Cédula", "cedula", "Cedula")),
                        Convert.ToString(GetCol(fila, "nombre")));
                }

                foreach (DataRow fila in _data.buscarJuridicoGeneral().Rows)
                {
                    if (EsEliminado(fila)) continue;
                    _tabla.Rows.Add(
                        0,
                        Convert.ToInt32(GetCol(fila, "id")),
                        "Jurídico",
                        Convert.ToString(GetCol(fila, "letra")).Trim(),
                        Convert.ToString(GetCol(fila, "R.I.F", "rif", "RIF")),
                        Convert.ToString(GetCol(fila, "nombre")));
                }

                if (_filtro == null)
                    _filtro = new GridHeaderFilter(_dgv);
                _filtro.Bind(_tabla);

                CatalogGridHelper.HideColumn(_dgv, "id_natural");
                CatalogGridHelper.HideColumn(_dgv, "id_juridico");
                CatalogGridHelper.SetHeader(_dgv, "tipo", "Tipo");
                CatalogGridHelper.SetHeader(_dgv, "letra", "Letra");
                CatalogGridHelper.SetHeader(_dgv, "documento", "Cédula / RIF");
                CatalogGridHelper.SetHeader(_dgv, "nombre", "Nombre");
                _filtro.SyncHeaders();

                foreach (DataGridViewColumn col in _dgv.Columns)
                {
                    if (col == null || !col.Visible) continue;
                    col.HeaderCell.Style.ForeColor = Color.White;
                    col.HeaderCell.Style.BackColor = UiTheme.Primary;
                }
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "cargar productores");
            }
        }

        private static bool EsEliminado(DataRow fila)
        {
            return fila.Table.Columns.Contains("eliminado")
                && fila["eliminado"] != DBNull.Value
                && Convert.ToBoolean(fila["eliminado"]);
        }

        private void AplicarFiltroTexto()
        {
            if (_tabla == null || _filtro == null) return;
            string q = (_txtBuscar.Text ?? string.Empty).Trim().Replace("'", "''");
            if (string.IsNullOrEmpty(q))
            {
                _filtro.BindingSource.RemoveFilter();
                return;
            }

            try
            {
                _filtro.BindingSource.Filter =
                    "documento LIKE '%" + q + "%' OR nombre LIKE '%" + q + "%' OR letra LIKE '%" + q + "%'";
            }
            catch
            {
                _filtro.BindingSource.RemoveFilter();
            }
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            if (_dgv.Rows.Count == 1 && _dgv.Rows[0].DataBoundItem is DataRowView)
            {
                SeleccionarFila(_dgv.Rows[0]);
            }
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            SeleccionarFila(_dgv.Rows[e.RowIndex]);
        }

        private void SeleccionarFila(DataGridViewRow row)
        {
            if (row == null) return;
            try
            {
                IdNatural = Convert.ToInt32(row.Cells["id_natural"].Value);
                IdJuridico = Convert.ToInt32(row.Cells["id_juridico"].Value);
                Letra = Convert.ToString(row.Cells["letra"].Value);
                Documento = Convert.ToString(row.Cells["documento"].Value);
                Nombre = Convert.ToString(row.Cells["nombre"].Value);
                Seleccionado = IdNatural > 0 || IdJuridico > 0;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "seleccionar productor");
            }
        }

        /// <summary>
        /// Busca por documento (cédula o RIF) sin abrir el listado.
        /// </summary>
        public static bool TryBuscarPorDocumento(string documento, out int idNatural, out int idJuridico, out string letra, out string nombre)
        {
            idNatural = 0;
            idJuridico = 0;
            letra = string.Empty;
            nombre = string.Empty;
            if (string.IsNullOrWhiteSpace(documento))
                return false;

            decimal numero;
            if (!decimal.TryParse(documento.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out numero)
                && !decimal.TryParse(documento.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out numero))
                return false;

            var data = new ProductorData();
            try
            {
                // Natural: prueba letras comunes vía buscarGeneral filtrado
                foreach (DataRow fila in data.buscarGeneral().Rows)
                {
                    if (EsEliminadoStatic(fila)) continue;
                    object doc = GetCol(fila, "Cédula", "cedula", "Cedula");
                    if (doc == null || doc == DBNull.Value) continue;
                    if (Convert.ToDecimal(doc) != numero) continue;
                    idNatural = Convert.ToInt32(GetCol(fila, "id"));
                    letra = Convert.ToString(GetCol(fila, "letra")).Trim();
                    nombre = Convert.ToString(GetCol(fila, "nombre"));
                    return true;
                }

                foreach (DataRow fila in data.buscarJuridicoGeneral().Rows)
                {
                    if (EsEliminadoStatic(fila)) continue;
                    object doc = GetCol(fila, "R.I.F", "rif", "RIF");
                    if (doc == null || doc == DBNull.Value) continue;
                    if (Convert.ToDecimal(doc) != numero) continue;
                    idJuridico = Convert.ToInt32(GetCol(fila, "id"));
                    letra = Convert.ToString(GetCol(fila, "letra")).Trim();
                    nombre = Convert.ToString(GetCol(fila, "nombre"));
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Los SP aliasan documento como "Cédula" / "R.I.F" (no "cedula"/"rif").
        /// </summary>
        private static object GetCol(DataRow fila, params string[] nombres)
        {
            if (fila == null || nombres == null) return null;
            foreach (string nombre in nombres)
            {
                if (string.IsNullOrEmpty(nombre)) continue;
                if (fila.Table.Columns.Contains(nombre))
                    return fila[nombre];
            }

            foreach (DataColumn col in fila.Table.Columns)
            {
                foreach (string nombre in nombres)
                {
                    if (string.Equals(col.ColumnName, nombre, StringComparison.OrdinalIgnoreCase))
                        return fila[col];
                }
            }
            return null;
        }

        private static bool EsEliminadoStatic(DataRow fila)
        {
            return fila.Table.Columns.Contains("eliminado")
                && fila["eliminado"] != DBNull.Value
                && Convert.ToBoolean(fila["eliminado"]);
        }
    }
}
