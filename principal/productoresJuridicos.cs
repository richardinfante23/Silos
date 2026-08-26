using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace principal
{
    public partial class productoresJuridicos : Form
    {
        private readonly string letrasJuridicas = ConfigurationManager.AppSettings["letrasJuridicas"];
        private readonly string letrasNaturales = ConfigurationManager.AppSettings["letrasNaturales"];
        ProductorData pd = new ProductorData();
        destinoData dd = new destinoData();
        public bool productor = true;
        private GridHeaderFilter _headerFilter;
        private TextBox _txtFiltro;
        private DataTable _tabla;

        public productoresJuridicos()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                personaJuridicaForm cf = new personaJuridicaForm();
                List<string> listaLetras = new List<string>();
                listaLetras.AddRange(letrasJuridicas.Split(','));
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

        private void productoresJuridicos_Load(object sender, EventArgs e)
        {
            if (!productor)
                this.Text = "Clientes Jurídicos";
            UiTheme.ApplyCatalogScreen(this);
            AsegurarFiltroUi();
            cargarProductor();
        }

        void cargarProductor()
        {
            try
            {
                _tabla = productor ? pd.buscarJuridicoGeneral() : dd.buscarDestinoGeneral();
                CatalogGridHelper.EnsureHeaderFilter(dgvProdcutores, ref _headerFilter);
                _headerFilter.Bind(_tabla ?? new DataTable());
                CatalogGridHelper.HideColumn(dgvProdcutores, "id");
                CatalogGridHelper.HideColumn(dgvProdcutores, "eliminado");
                CatalogGridHelper.HideColumn(dgvProdcutores, "id_asociacion");
                if (dgvProdcutores.Columns.Contains("R.I.F"))
                    CatalogGridHelper.SetHeader(dgvProdcutores, "R.I.F", "R.I.F");
                else if (dgvProdcutores.Columns.Contains("rif"))
                    CatalogGridHelper.SetHeader(dgvProdcutores, "rif", "R.I.F");
                if (dgvProdcutores.Columns.Contains("letra"))
                    CatalogGridHelper.SetHeader(dgvProdcutores, "letra", "Letra");
                if (dgvProdcutores.Columns.Contains("nombre"))
                    CatalogGridHelper.SetHeader(dgvProdcutores, "nombre", "Nombre");
                _headerFilter.SyncHeaders();
                AplicarFiltroTexto();

                if (_tabla == null || _tabla.Rows.Count == 0)
                    Base.msjInformacion("No se encontraron resultados.");
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
                Text = "Filtrar por RIF o nombre",
                Location = new Point(8, 10),
                AutoSize = true
            };
            _txtFiltro = new TextBox
            {
                Location = new Point(160, 6),
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
                if (_tabla.Columns.Contains("R.I.F"))
                    partes.Add("CONVERT([R.I.F], 'System.String') LIKE '%" + q + "%'");
                else if (_tabla.Columns.Contains("rif"))
                    partes.Add("CONVERT(rif, 'System.String') LIKE '%" + q + "%'");
                if (partes.Count > 0)
                    _headerFilter.BindingSource.Filter = string.Join(" OR ", partes.ToArray());
            }
            catch
            {
                _headerFilter.BindingSource.RemoveFilter();
            }
        }

        private void dgvProdcutores_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dgvProdcutores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                personaJuridicaForm cf = new personaJuridicaForm();
                List<string> listaLetras = new List<string>();
                listaLetras.AddRange(letrasJuridicas.Split(','));
                cf.cbLetra.DataSource = listaLetras;
                cf.actualizar = true;
                foreach (DataGridViewRow Seleccionado in dgvProdcutores.SelectedRows)
                {
                    cf.txtNombre.Text = Seleccionado.Cells["nombre"].Value.ToString();
                    if (dgvProdcutores.Columns.Contains("R.I.F"))
                        cf.txtRif.Text = Seleccionado.Cells["R.I.F"].Value.ToString();
                    else
                        cf.txtRif.Text = Seleccionado.Cells["rif"].Value.ToString();
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
    }
}
