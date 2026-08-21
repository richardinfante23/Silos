using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace principal
{
    public class reporteCostoBO
    {
        public int ticket { get; set; }
        public DateTime fecha { get; set; }
        public string producto { get; set; }
        public string productor { get; set; }
        public decimal peso_neto { get; set; }
        public decimal precio_tonelada_usd { get; set; }
        public decimal precio_bs_usd { get; set; }
        public decimal total_usd { get; set; }
        public decimal total_bs { get; set; }
        public string empresa { get; set; }
        public string direccion_empresa { get; set; }
    }

    public partial class reporteCostosForm : Form
    {
        private readonly recepcionData _recepcionData = new recepcionData();
        private readonly tasaData _tasaData = new tasaData();
        private readonly productoData _productoData = new productoData();
        private readonly string _empresa = ConfigurationManager.AppSettings["Empresa"];
        private readonly string _direccion = ConfigurationManager.AppSettings["direccionEmpresa"];
        private static readonly TextInfo TextoEs = CultureInfo.GetCultureInfo("es-VE").TextInfo;

        private List<reporteCostoBO> _lista = new List<reporteCostoBO>();
        private List<reporteCostoBO> _listaFiltrada = new List<reporteCostoBO>();
        private int _idProductorNatural;
        private int _idProductorJuridico;

        private DateTimePicker _dtpDesde;
        private DateTimePicker _dtpHasta;
        private ComboBox _cbProducto;
        private TextBox _txtCedulaProductor;
        private TextBox _txtNombreProductor;
        private Button _btnBuscarProductor;
        private Button _btnFiltrar;
        private Button _btnTodos;
        private Button _btnExportar;
        private DataGridView _dgv;
        private Label _lbTotales;

        public reporteCostosForm()
        {
            InitializeComponent();
            ConstruirUi();
            Load += reporteCostosForm_Load;
        }

        private void reporteCostosForm_Load(object sender, EventArgs e)
        {
            if (MdiParent != null)
                WindowState = FormWindowState.Maximized;

            UiTheme.ApplyForm(this);
            UiTheme.ApplyAppIcon(this);
            UiTheme.StyleDataGridView(_dgv);
            Text = "Reporte de Costos";
            CargarProductos();
            _dtpDesde.Value = DateTime.Today.AddDays(-30);
            _dtpHasta.Value = DateTime.Today;
            LimpiarProductor();
            Buscar();
        }

        private void ConstruirUi()
        {
            SuspendLayout();
            BackColor = UiTheme.Surface;
            Padding = new Padding(12);

            var titulo = new Label
            {
                Text = "Reporte de Costos",
                Font = UiTheme.FontTitle,
                ForeColor = UiTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(16, 8)
            };

            var panel = new Panel
            {
                Location = new Point(16, 42),
                Size = new Size(970, 108),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = UiTheme.Card
            };

            var lblDesde = new Label { Text = "Desde", Location = new Point(12, 8), AutoSize = true, ForeColor = UiTheme.TextMuted };
            _dtpDesde = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(12, 26),
                Size = new Size(100, 26)
            };

            var lblHasta = new Label { Text = "Hasta", Location = new Point(120, 8), AutoSize = true, ForeColor = UiTheme.TextMuted };
            _dtpHasta = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(120, 26),
                Size = new Size(100, 26)
            };

            var lblProd = new Label { Text = "Producto", Location = new Point(232, 8), AutoSize = true, ForeColor = UiTheme.TextMuted };
            _cbProducto = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(232, 26),
                Size = new Size(180, 26)
            };
            UiTheme.StyleComboBox(_cbProducto);

            var lblCedula = new Label { Text = "Cédula / RIF productor", Location = new Point(12, 58), AutoSize = true, ForeColor = UiTheme.TextMuted };
            _txtCedulaProductor = new TextBox
            {
                Location = new Point(12, 76),
                Size = new Size(120, 26)
            };
            UiTheme.StyleTextBox(_txtCedulaProductor);
            _txtCedulaProductor.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnBuscarProductor_Click(s, e);
                }
            };

            _btnBuscarProductor = new Button
            {
                Text = "Buscar",
                Location = new Point(140, 74),
                Size = new Size(90, 30)
            };
            _btnBuscarProductor.Click += btnBuscarProductor_Click;
            UiTheme.StyleCompactPrimaryButton(_btnBuscarProductor, "Buscar", 90, 30);

            var lblNombre = new Label { Text = "Nombre", Location = new Point(244, 58), AutoSize = true, ForeColor = UiTheme.TextMuted };
            _txtNombreProductor = new TextBox
            {
                Location = new Point(244, 76),
                Size = new Size(260, 26),
                ReadOnly = true,
                TabStop = false
            };
            UiTheme.StyleTextBox(_txtNombreProductor);
            _txtNombreProductor.BackColor = UiTheme.FilterStrip;

            _btnFiltrar = new Button { Text = "Filtrar", Location = new Point(620, 26), Size = new Size(90, 32) };
            _btnTodos = new Button { Text = "Todos", Location = new Point(718, 26), Size = new Size(90, 32) };
            _btnExportar = new Button { Text = "Exportar", Location = new Point(816, 26), Size = new Size(100, 32) };
            _btnFiltrar.Click += (s, e) => Buscar();
            _btnTodos.Click += btnTodos_Click;
            _btnExportar.Click += btnExportar_Click;
            UiTheme.StyleCompactPrimaryButton(_btnFiltrar, "Filtrar", 90, 32);
            UiTheme.StyleCompactSecondaryButton(_btnTodos, "Todos", 90, 32);
            UiTheme.StyleCompactPrimaryButton(_btnExportar, "Exportar", 100, 32);

            panel.Controls.Add(lblDesde);
            panel.Controls.Add(_dtpDesde);
            panel.Controls.Add(lblHasta);
            panel.Controls.Add(_dtpHasta);
            panel.Controls.Add(lblProd);
            panel.Controls.Add(_cbProducto);
            panel.Controls.Add(lblCedula);
            panel.Controls.Add(_txtCedulaProductor);
            panel.Controls.Add(_btnBuscarProductor);
            panel.Controls.Add(lblNombre);
            panel.Controls.Add(_txtNombreProductor);
            panel.Controls.Add(_btnFiltrar);
            panel.Controls.Add(_btnTodos);
            panel.Controls.Add(_btnExportar);

            _lbTotales = new Label
            {
                Text = string.Empty,
                Font = UiTheme.FontBold,
                ForeColor = UiTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(16, 158)
            };

            _dgv = new DataGridView
            {
                Location = new Point(16, 182),
                Size = new Size(970, 400),
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

            Controls.Add(titulo);
            Controls.Add(panel);
            Controls.Add(_lbTotales);
            Controls.Add(_dgv);
            ResumeLayout(true);
        }

        private void CargarProductos()
        {
            DataTable dt = _productoData.buscar().Copy();
            DataRow filaTodos = dt.NewRow();
            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName.Equals("id", StringComparison.OrdinalIgnoreCase))
                    filaTodos[col] = 0;
                else if (col.ColumnName.Equals("nombre", StringComparison.OrdinalIgnoreCase))
                    filaTodos[col] = "(Todos)";
                else if (col.DataType == typeof(bool))
                    filaTodos[col] = false;
                else if (col.AllowDBNull)
                    filaTodos[col] = DBNull.Value;
            }
            dt.Rows.InsertAt(filaTodos, 0);
            _cbProducto.DataSource = dt;
            _cbProducto.DisplayMember = "nombre";
            _cbProducto.ValueMember = "id";
            _cbProducto.SelectedIndex = 0;
        }

        private void LimpiarProductor()
        {
            _idProductorNatural = 0;
            _idProductorJuridico = 0;
            _txtCedulaProductor.Text = string.Empty;
            _txtNombreProductor.Text = string.Empty;
            _txtCedulaProductor.Enabled = true;
            _btnBuscarProductor.Text = "Buscar";
            UiTheme.StyleCompactPrimaryButton(_btnBuscarProductor, "Buscar", 90, 30);
        }

        private void AsignarProductor(int idNatural, int idJuridico, string documento, string nombre)
        {
            _idProductorNatural = idNatural;
            _idProductorJuridico = idJuridico;
            _txtCedulaProductor.Text = documento ?? string.Empty;
            _txtNombreProductor.Text = FormatoTitulo(nombre ?? string.Empty);
            _txtCedulaProductor.Enabled = false;
            _btnBuscarProductor.Text = "Cambiar";
            UiTheme.StyleCompactSecondaryButton(_btnBuscarProductor, "Cambiar", 90, 30);
        }

        private void btnBuscarProductor_Click(object sender, EventArgs e)
        {
            if (string.Equals(_btnBuscarProductor.Text, "Cambiar", StringComparison.OrdinalIgnoreCase))
            {
                LimpiarProductor();
                return;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(_txtCedulaProductor.Text))
                {
                    using (var picker = new productorPickerForm())
                    {
                        if (picker.ShowDialog(this) == DialogResult.OK && picker.Seleccionado)
                        {
                            AsignarProductor(picker.IdNatural, picker.IdJuridico, picker.Documento, picker.Nombre);
                        }
                    }
                    return;
                }

                int idNatural;
                int idJuridico;
                string letra;
                string nombre;
                if (productorPickerForm.TryBuscarPorDocumento(_txtCedulaProductor.Text, out idNatural, out idJuridico, out letra, out nombre))
                {
                    AsignarProductor(idNatural, idJuridico, _txtCedulaProductor.Text.Trim(), nombre);
                }
                else
                {
                    Base.msjAdvertencia("No se encontró productor con esa cédula/RIF. Abra el listado dejando el campo vacío y pulse Buscar.");
                }
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "buscar productor");
            }
        }

        private void btnTodos_Click(object sender, EventArgs e)
        {
            _cbProducto.SelectedIndex = 0;
            LimpiarProductor();
            _dtpDesde.Value = DateTime.Today.AddDays(-30);
            _dtpHasta.Value = DateTime.Today;
            Buscar();
        }

        private void Buscar()
        {
            try
            {
                if (_dtpDesde.Value.Date > _dtpHasta.Value.Date)
                {
                    Base.msjAdvertencia("La fecha desde no puede ser mayor que la fecha hasta.");
                    return;
                }

                int idProducto = 0;
                if (_cbProducto.SelectedValue != null && _cbProducto.SelectedValue != DBNull.Value)
                    idProducto = Convert.ToInt32(_cbProducto.SelectedValue);

                DataTable dt = _recepcionData.buscar(
                    0,
                    idProducto,
                    0,
                    _idProductorNatural,
                    _idProductorJuridico,
                    string.Empty,
                    0,
                    5,
                    _dtpDesde.Value.Date,
                    _dtpHasta.Value.Date);

                _lista = new List<reporteCostoBO>();
                foreach (DataRow fila in dt.Rows)
                {
                    var item = MapearFila(fila);
                    if (item != null)
                        _lista.Add(item);
                }

                _listaFiltrada = _lista;
                _dgv.DataSource = null;
                _dgv.DataSource = _listaFiltrada;
                ConfigurarColumnas();
                SumarTotales();
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "consultar el reporte de costos");
            }
        }

        private reporteCostoBO MapearFila(DataRow fila)
        {
            if (fila == null) return null;

            var item = new reporteCostoBO
            {
                ticket = Convert.ToInt32(fila["Nro_Ticket"]),
                fecha = Convert.ToDateTime(fila["fecha"]),
                producto = FormatoTitulo(Convert.ToString(fila["nombre_producto"])),
                peso_neto = Math.Round(Convert.ToDecimal(fila["peso_neto"]), 2),
                empresa = _empresa,
                direccion_empresa = _direccion
            };

            int idProductor = fila["id_productor"] != DBNull.Value ? Convert.ToInt32(fila["id_productor"]) : 0;
            int idJuridico = fila["id_productor_juridico"] != DBNull.Value ? Convert.ToInt32(fila["id_productor_juridico"]) : 0;
            if (idProductor > 0)
            {
                string letra = Convert.ToString(fila["letranatural"]);
                string cedula = Convert.ToString(fila["cedulaproductor"]);
                string nombre = Convert.ToString(fila["nombreproductornatural"]);
                item.productor = FormatoTitulo((letra + "-" + cedula + " " + nombre).Trim());
            }
            else if (idJuridico > 0)
            {
                string letra = Convert.ToString(fila["letrajuridica"]);
                string rif = Convert.ToString(fila["rif"]);
                string nombre = Convert.ToString(fila["nombreproductorjuridico"]);
                item.productor = FormatoTitulo((letra + "-" + rif + " " + nombre).Trim());
            }
            else
            {
                item.productor = string.Empty;
            }

            int idProducto = fila["id_producto"] != DBNull.Value ? Convert.ToInt32(fila["id_producto"]) : 0;
            decimal pesoKg = item.peso_neto;
            if (fila.Table.Columns.Contains("peso_acondicionado")
                && fila["peso_acondicionado"] != DBNull.Value
                && Convert.ToDecimal(fila["peso_acondicionado"]) > 0)
            {
                pesoKg = Math.Round(Convert.ToDecimal(fila["peso_acondicionado"]), 2);
                item.peso_neto = pesoKg;
            }

            AplicarTasa(item, item.fecha, idProducto, pesoKg);
            return item;
        }

        private void AplicarTasa(reporteCostoBO item, DateTime fecha, int idProducto, decimal pesoKg)
        {
            item.precio_tonelada_usd = 0;
            item.precio_bs_usd = 0;
            item.total_usd = 0;
            item.total_bs = 0;
            if (idProducto <= 0) return;

            try
            {
                DataTable dt = _tasaData.buscarPorFechaProducto(fecha.Date, idProducto);
                if (dt == null || dt.Rows.Count == 0) return;

                decimal precioUsd = Convert.ToDecimal(dt.Rows[0]["precio_tonelada_usd"]);
                decimal tasaBs = Convert.ToDecimal(dt.Rows[0]["tasa_dolar_bs"]);
                decimal toneladas = pesoKg / 1000m;
                item.precio_tonelada_usd = precioUsd;
                item.precio_bs_usd = tasaBs;
                item.total_usd = Math.Round(toneladas * precioUsd, 2);
                item.total_bs = Math.Round(item.total_usd * tasaBs, 2);
            }
            catch
            {
                // Sin tasa: montos en cero.
            }
        }

        private void ConfigurarColumnas()
        {
            if (_dgv.Columns.Count == 0) return;

            CatalogGridHelper.HideColumn(_dgv, "empresa");
            CatalogGridHelper.HideColumn(_dgv, "direccion_empresa");
            CatalogGridHelper.SetHeader(_dgv, "ticket", "Ticket");
            CatalogGridHelper.SetHeader(_dgv, "fecha", "Fecha");
            CatalogGridHelper.SetHeader(_dgv, "producto", "Producto");
            CatalogGridHelper.SetHeader(_dgv, "productor", "Productor");
            CatalogGridHelper.SetHeader(_dgv, "peso_neto", "Peso (Kg)");
            CatalogGridHelper.SetHeader(_dgv, "precio_tonelada_usd", "Precio $/Ton");
            CatalogGridHelper.SetHeader(_dgv, "precio_bs_usd", "Precio Bs/$");
            CatalogGridHelper.SetHeader(_dgv, "total_usd", "Total $");
            CatalogGridHelper.SetHeader(_dgv, "total_bs", "Total Bs");

            foreach (DataGridViewColumn col in _dgv.Columns)
            {
                if (col == null || !col.Visible) continue;
                col.HeaderCell.Style.ForeColor = Color.White;
                col.HeaderCell.Style.BackColor = UiTheme.Primary;
            }
        }

        private void SumarTotales()
        {
            decimal peso = _listaFiltrada.Sum(x => x.peso_neto);
            decimal usd = _listaFiltrada.Sum(x => x.total_usd);
            decimal bs = _listaFiltrada.Sum(x => x.total_bs);
            _lbTotales.Text = string.Format(
                CultureInfo.CurrentCulture,
                "Registros: {0}   |   Peso: {1:N2} Kg   |   Total $: {2:N2}   |   Total Bs: {3:N2}",
                _listaFiltrada.Count,
                peso,
                usd,
                bs);
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (_listaFiltrada == null || _listaFiltrada.Count == 0)
            {
                Base.msjAdvertencia("No existen elementos para realizar exportación.");
                return;
            }

            try
            {
                var rf = new reporteForm(new List<analisis>());
                rf.rvReporte.ProcessingMode = ProcessingMode.Local;
                rf.rvReporte.LocalReport.DataSources.Clear();
                rf.rvReporte.LocalReport.DataSources.Add(new ReportDataSource("Boleta", _listaFiltrada));
                rf.rvReporte.LocalReport.ReportEmbeddedResource = "principal.ReporteCostos.rdlc";
                rf.rvReporte.LocalReport.Refresh();
                rf.ShowDialog(this);
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "exportar el reporte de costos");
            }
        }

        private static string FormatoTitulo(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;
            return TextoEs.ToTitleCase(valor.Trim().ToLowerInvariant());
        }
    }
}
