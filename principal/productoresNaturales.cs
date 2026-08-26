using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace principal
{
    public partial class productoresNaturalesForm : Form
    {
        ProductorData pd = new ProductorData();
        destinoData dd = new destinoData();
        private readonly string letrasJuridicas = ConfigurationManager.AppSettings["letrasJuridicas"];
        private readonly string letrasNaturales = ConfigurationManager.AppSettings["letrasNaturales"];
        public bool productor = true;
        private GridHeaderFilter _headerFilter;
        private TextBox _txtFiltro;
        private DataTable _tabla;

        public productoresNaturalesForm()
        {
            InitializeComponent();
        }

        void cargarProductor()
        {
            try
            {
                _tabla = productor ? pd.buscarGeneral() : dd.buscarGeneral();
                CatalogGridHelper.EnsureHeaderFilter(dgvProdcutores, ref _headerFilter);
                _headerFilter.Bind(_tabla);
                CatalogGridHelper.HideColumn(dgvProdcutores, "id");
                CatalogGridHelper.HideColumn(dgvProdcutores, "eliminado");
                CatalogGridHelper.HideColumn(dgvProdcutores, "id_asociacion");
                if (dgvProdcutores.Columns.Contains("Cédula"))
                    CatalogGridHelper.SetHeader(dgvProdcutores, "Cédula", "Cédula");
                else if (dgvProdcutores.Columns.Contains("cedula"))
                    CatalogGridHelper.SetHeader(dgvProdcutores, "cedula", "Cédula");
                if (dgvProdcutores.Columns.Contains("letra"))
                    CatalogGridHelper.SetHeader(dgvProdcutores, "letra", "Letra");
                if (dgvProdcutores.Columns.Contains("nombre"))
                    CatalogGridHelper.SetHeader(dgvProdcutores, "nombre", "Nombre");
                _headerFilter.SyncHeaders();
                AplicarFiltroTexto();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando productor. Error Contacte al Administrador del Sistema", ex);
            }
        }

        private void AsegurarFiltroUi()
        {
            if (_txtFiltro != null) return;

            dgvProdcutores.Top = 44;
            dgvProdcutores.Height = Math.Max(200, dgvProdcutores.Height - 42);

            var lbl = new Label
            {
                Text = "Filtrar por cédula o nombre",
                Location = new Point(8, 10),
                AutoSize = true
            };
            _txtFiltro = new TextBox
            {
                Location = new Point(180, 6),
                Size = new Size(360, 26),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            _txtFiltro.TextChanged += (s, e) => AplicarFiltroTexto();
            Controls.Add(lbl);
            Controls.Add(_txtFiltro);
            UiTheme.StyleTextBox(_txtFiltro);
        }

        private void AplicarFiltroTexto()
        {
            if (_headerFilter == null || _tabla == null) return;
            string q = (_txtFiltro == null ? string.Empty : _txtFiltro.Text ?? string.Empty).Trim().Replace("'", "''");
            if (string.IsNullOrEmpty(q))
            {
                _headerFilter.BindingSource.RemoveFilter();
                return;
            }

            try
            {
                var partes = new List<string>();
                if (_tabla.Columns.Contains("nombre"))
                    partes.Add("nombre LIKE '%" + q + "%'");
                if (_tabla.Columns.Contains("letra"))
                    partes.Add("letra LIKE '%" + q + "%'");
                if (_tabla.Columns.Contains("Cédula"))
                    partes.Add("CONVERT([Cédula], 'System.String') LIKE '%" + q + "%'");
                else if (_tabla.Columns.Contains("cedula"))
                    partes.Add("CONVERT(cedula, 'System.String') LIKE '%" + q + "%'");
                if (partes.Count > 0)
                    _headerFilter.BindingSource.Filter = string.Join(" OR ", partes.ToArray());
            }
            catch
            {
                _headerFilter.BindingSource.RemoveFilter();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void productoresNaturalesForm_Load(object sender, EventArgs e)
        {
            if (!productor)
                this.Text = "Clientes Naturales";

            UiTheme.ApplyCatalogScreen(this);
            AsegurarFiltroUi();
            cargarProductor();
        }

        private void dgvProdcutores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                productorNatural cf = new productorNatural();
                List<string> listaLetras = new List<string>();
                listaLetras.AddRange(letrasNaturales.Split(','));
                cf.cbLetra.DataSource = listaLetras;
                cf.actualizar = true;
                foreach (DataGridViewRow Seleccionado in dgvProdcutores.SelectedRows)
                {
                    cf.txtNombre.Text = Seleccionado.Cells["nombre"].Value.ToString();
                    if (dgvProdcutores.Columns.Contains("Cédula"))
                        cf.txtCedula.Text = Seleccionado.Cells["Cédula"].Value.ToString();
                    else
                        cf.txtCedula.Text = Seleccionado.Cells["cedula"].Value.ToString();
                    cf.cbLetra.SelectedItem = Seleccionado.Cells["letra"].Value.ToString();
                    cf.cbAsociaciones.SelectedItem = Seleccionado.Cells["id_asociacion"].Value.ToString();
                    cf.id = (int)Seleccionado.Cells["id"].Value;
                }
                cf.destino = !productor;
                cf.ShowDialog();

                cargarProductor();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando productor. Error Contacte al Administrador del Sistema" + ex.Message);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                productorNatural cf = new productorNatural();
                List<string> listaLetras = new List<string>();
                listaLetras.AddRange(letrasNaturales.Split(','));
                cf.cbLetra.DataSource = listaLetras;
                cf.actualizar = false;
                cf.destino = !productor;
                cf.ShowDialog();

                cargarProductor();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando productor. Error Contacte al Administrador del Sistema" + ex.Message);
            }
        }
    }
}
