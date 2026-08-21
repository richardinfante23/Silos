using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace principal
{
    public partial class tasaForm : Form
    {
        private readonly tasaData _data = new tasaData();
        private readonly productoData _productoData = new productoData();
        private GridHeaderFilter _filtro;
        private bool _editando;
        private int _idActual;

        private Panel _panelForm;
        private DateTimePicker _dtpFecha;
        private ComboBox _cbProducto;
        private TextBox _txtPrecioUsd;
        private TextBox _txtTasaBs;
        private Button _btnGuardar;
        private Button _btnNuevo;
        private Button _btnCancelar;
        private DataGridView _dgv;
        private Label _lbTitulo;

        public tasaForm()
        {
            InitializeComponent();
            ConstruirUi();
            Load += tasaForm_Load;
        }

        private void tasaForm_Load(object sender, EventArgs e)
        {
            if (MdiParent != null)
                WindowState = FormWindowState.Maximized;

            UiTheme.ApplyForm(this);
            UiTheme.ApplyAppIcon(this);
            UiTheme.StyleDataGridView(_dgv);
            _dgv.ColumnHeadersHeight = 36;
            _dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            Text = "Configuración Tasa";
            CargarProductos();
            ModoNuevo();
            CargarGrid();
        }

        private void ConstruirUi()
        {
            SuspendLayout();
            BackColor = UiTheme.Surface;
            Padding = new Padding(12);

            _lbTitulo = new Label
            {
                Text = "Configuración Tasa",
                Font = UiTheme.FontTitle,
                ForeColor = UiTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(16, 8)
            };

            _panelForm = new Panel
            {
                Location = new Point(16, 42),
                Size = new Size(920, 118),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = UiTheme.Card,
                Padding = new Padding(12, 8, 12, 8)
            };

            var lblFecha = CrearEtiqueta("Fecha", 12, 8);
            _dtpFecha = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(12, 26),
                Size = new Size(120, 26),
                Enabled = false
            };

            var lblProducto = CrearEtiqueta("Producto", 148, 8);
            _cbProducto = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(148, 26),
                Size = new Size(200, 26)
            };
            UiTheme.StyleComboBox(_cbProducto);

            var lblPrecio = CrearEtiqueta("Precio tonelada (USD)", 364, 8);
            _txtPrecioUsd = CrearTexto(364, 26, 160);
            _txtPrecioUsd.KeyPress += SoloDecimal_KeyPress;

            var lblTasa = CrearEtiqueta("Precio Bs/$", 540, 8);
            _txtTasaBs = CrearTexto(540, 26, 160);
            _txtTasaBs.KeyPress += SoloDecimal_KeyPress;

            _btnGuardar = new Button { Text = "Guardar", Location = new Point(12, 68), Size = new Size(100, 34) };
            _btnNuevo = new Button { Text = "Nuevo", Location = new Point(122, 68), Size = new Size(100, 34) };
            _btnCancelar = new Button { Text = "Cerrar", Location = new Point(232, 68), Size = new Size(100, 34) };
            _btnGuardar.Click += btnGuardar_Click;
            _btnNuevo.Click += (s, e) => ModoNuevo();
            _btnCancelar.Click += (s, e) => Close();

            UiTheme.StylePrimaryButton(_btnGuardar);
            UiTheme.StyleNewButton(_btnNuevo);
            UiTheme.StyleSecondaryButton(_btnCancelar);

            _panelForm.Controls.Add(lblFecha);
            _panelForm.Controls.Add(_dtpFecha);
            _panelForm.Controls.Add(lblProducto);
            _panelForm.Controls.Add(_cbProducto);
            _panelForm.Controls.Add(lblPrecio);
            _panelForm.Controls.Add(_txtPrecioUsd);
            _panelForm.Controls.Add(lblTasa);
            _panelForm.Controls.Add(_txtTasaBs);
            _panelForm.Controls.Add(_btnGuardar);
            _panelForm.Controls.Add(_btnNuevo);
            _panelForm.Controls.Add(_btnCancelar);

            var lblListado = new Label
            {
                Text = "Últimas tasas · doble clic para editar",
                Font = UiTheme.FontSubtitle,
                ForeColor = UiTheme.TextMuted,
                AutoSize = true,
                Location = new Point(16, 170)
            };

            _dgv = new DataGridView
            {
                Location = new Point(16, 192),
                Size = new Size(920, 410),
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
            _dgv.DataBindingComplete += (s, e) => ConfigurarColumnas();

            Controls.Add(_lbTitulo);
            Controls.Add(_panelForm);
            Controls.Add(lblListado);
            Controls.Add(_dgv);
            ResumeLayout(true);
        }

        private static Label CrearEtiqueta(string texto, int x, int y)
        {
            return new Label
            {
                Text = texto,
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = UiTheme.TextMuted,
                Font = UiTheme.FontRegular
            };
        }

        private static TextBox CrearTexto(int x, int y, int width)
        {
            var txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 28)
            };
            UiTheme.StyleTextBox(txt);
            return txt;
        }

        private void CargarProductos()
        {
            try
            {
                DataTable dt = _productoData.buscar();
                _cbProducto.DataSource = dt;
                _cbProducto.DisplayMember = "nombre";
                _cbProducto.ValueMember = "id";
                _cbProducto.SelectedIndex = dt.Rows.Count > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "cargar productos");
            }
        }

        private int ObtenerIdProductoSeleccionado()
        {
            if (_cbProducto.SelectedValue == null || _cbProducto.SelectedValue == DBNull.Value)
                return 0;
            return Convert.ToInt32(_cbProducto.SelectedValue);
        }

        private void ModoNuevo()
        {
            _editando = false;
            _idActual = 0;
            _dtpFecha.Value = DateTime.Today;
            _dtpFecha.Enabled = false;
            _cbProducto.Enabled = true;
            if (_cbProducto.Items.Count > 0)
                _cbProducto.SelectedIndex = 0;
            _txtPrecioUsd.Text = string.Empty;
            _txtTasaBs.Text = string.Empty;
            _btnGuardar.Text = "Guardar";
            _txtPrecioUsd.Focus();
        }

        private void ModoEdicion(DataRow fila)
        {
            _editando = true;
            _idActual = Convert.ToInt32(fila["id"]);
            _dtpFecha.Value = Convert.ToDateTime(fila["fecha"]).Date;
            _dtpFecha.Enabled = false;
            if (fila.Table.Columns.Contains("id_producto") && fila["id_producto"] != DBNull.Value)
                _cbProducto.SelectedValue = Convert.ToInt32(fila["id_producto"]);
            _cbProducto.Enabled = true;
            _txtPrecioUsd.Text = Convert.ToDecimal(fila["precio_tonelada_usd"]).ToString("N4", CultureInfo.CurrentCulture);
            _txtTasaBs.Text = Convert.ToDecimal(fila["tasa_dolar_bs"]).ToString("N4", CultureInfo.CurrentCulture);
            _btnGuardar.Text = "Actualizar";
            _txtPrecioUsd.Focus();
        }

        private void CargarGrid()
        {
            try
            {
                DataTable dt = _data.buscar();
                if (_filtro == null)
                    _filtro = new GridHeaderFilter(_dgv);
                _filtro.Bind(dt);
                ConfigurarColumnas();
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "consultar las tasas");
            }
        }

        private void ConfigurarColumnas()
        {
            if (_dgv.Columns.Count == 0) return;

            CatalogGridHelper.HideColumn(_dgv, "id");
            CatalogGridHelper.HideColumn(_dgv, "id_producto");
            CatalogGridHelper.HideColumn(_dgv, "creado_en");
            CatalogGridHelper.HideColumn(_dgv, "actualizado_en");

            CatalogGridHelper.SetHeader(_dgv, "fecha", "Fecha");
            CatalogGridHelper.SetHeader(_dgv, "producto", "Producto");
            CatalogGridHelper.SetHeader(_dgv, "precio_tonelada_usd", "Precio tonelada (USD)");
            CatalogGridHelper.SetHeader(_dgv, "tasa_dolar_bs", "Precio Bs/$");

            foreach (DataGridViewColumn col in _dgv.Columns)
            {
                if (col == null || !col.Visible) continue;
                col.HeaderCell.Style.ForeColor = Color.White;
                col.HeaderCell.Style.BackColor = UiTheme.Primary;
                col.HeaderCell.Style.Font = UiTheme.FontBold;
            }

            if (_dgv.Columns.Contains("fecha"))
                _dgv.Columns["fecha"].FillWeight = 90;
            if (_dgv.Columns.Contains("producto"))
                _dgv.Columns["producto"].FillWeight = 120;
            if (_dgv.Columns.Contains("precio_tonelada_usd"))
                _dgv.Columns["precio_tonelada_usd"].FillWeight = 140;
            if (_dgv.Columns.Contains("tasa_dolar_bs"))
                _dgv.Columns["tasa_dolar_bs"].FillWeight = 130;

            if (_filtro != null)
                _filtro.SyncHeaders();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int idProducto = ObtenerIdProductoSeleccionado();
                if (idProducto <= 0)
                {
                    Base.msjAdvertencia("Seleccione un producto.");
                    _cbProducto.Focus();
                    return;
                }

                decimal precioUsd;
                decimal tasaBs;
                if (!decimal.TryParse(_txtPrecioUsd.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out precioUsd)
                    && !decimal.TryParse(_txtPrecioUsd.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out precioUsd))
                {
                    Base.msjAdvertencia("Indique un precio de tonelada válido en USD.");
                    _txtPrecioUsd.Focus();
                    return;
                }
                if (!decimal.TryParse(_txtTasaBs.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out tasaBs)
                    && !decimal.TryParse(_txtTasaBs.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out tasaBs))
                {
                    Base.msjAdvertencia("Indique un precio Bs/$ válido.");
                    _txtTasaBs.Focus();
                    return;
                }
                if (precioUsd < 0 || tasaBs < 0)
                {
                    Base.msjAdvertencia("Los valores no pueden ser negativos.");
                    return;
                }

                if (!_editando)
                {
                    DataTable existente = _data.buscarPorFechaProducto(DateTime.Today, idProducto);
                    if (existente != null && existente.Rows.Count > 0)
                    {
                        Base.msjAdvertencia("Ya existe una tasa para hoy y ese producto. Selecciónela en el listado para editarla.");
                        ModoEdicion(existente.Rows[0]);
                        return;
                    }

                    _data.insertar(DateTime.Today, idProducto, precioUsd, tasaBs);
                    Base.msjExito("Tasa del día guardada exitosamente.");
                }
                else
                {
                    _data.actualizar(_idActual, idProducto, precioUsd, tasaBs);
                    Base.msjExito("Tasa actualizada exitosamente.");
                }

                CargarGrid();
                ModoNuevo();
            }
            catch (Exception ex)
            {
                string msg = ex.Message ?? string.Empty;
                if (msg.IndexOf("23505", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("Ya existe una tasa", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Base.msjAdvertencia("Ya existe una tasa para esa fecha y producto. Edítela desde el listado.");
                    return;
                }
                Base.msjErrorOperacion(ex, "guardar la tasa");
            }
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _dgv.Rows.Count == 0) return;
            try
            {
                DataGridViewRow row = _dgv.Rows[e.RowIndex];
                if (row.DataBoundItem is DataRowView drv)
                    ModoEdicion(drv.Row);
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "cargar la tasa");
            }
        }

        private static void SoloDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            char sep = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (char.IsControl(e.KeyChar))
                return;
            if (char.IsDigit(e.KeyChar))
                return;
            if (e.KeyChar == sep || e.KeyChar == '.' || e.KeyChar == ',')
            {
                TextBox tb = sender as TextBox;
                if (tb != null && (tb.Text.Contains(sep.ToString()) || tb.Text.Contains(".") || tb.Text.Contains(",")))
                    e.Handled = true;
                return;
            }
            e.Handled = true;
        }
    }
}
