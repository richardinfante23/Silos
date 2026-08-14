using System;
using System.Data;
using System.Windows.Forms;

namespace principal
{
    public partial class asociacionGridForm : Form
    {
        private readonly asiciasionesData td = new asiciasionesData();
        private GridHeaderFilter headerFilter;

        public asociacionGridForm()
        {
            InitializeComponent();
        }

        private void asociacionGridForm_Load(object sender, EventArgs e)
        {
            UiTheme.ApplyForm(this);
            UiTheme.StylePrimaryButton(btnNuevo);
            UiTheme.StyleSecondaryButton(btnCancelar);
            Icon = UiTheme.GetAppIcon();
            Text = "Asociaciones";
            CatalogGridHelper.EnsureHeaderFilter(dgvCatalogo, ref headerFilter);
            cargar();
        }

        private void cargar()
        {
            try
            {
                DataTable dt = td.buscar();
                headerFilter.Bind(dt);
                CatalogGridHelper.HideColumn(dgvCatalogo, "id");
                CatalogGridHelper.HideColumn(dgvCatalogo, "eliminado");
                CatalogGridHelper.SetHeader(dgvCatalogo, "rif", "RIF");
                CatalogGridHelper.SetHeader(dgvCatalogo, "nombre", "Nombre");
                headerFilter.SyncHeaders();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un error buscando asociaciones. Contacte al Administrador del Sistema", ex);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new asociacionForm())
                {
                    form.actualizar = false;
                    form.ShowDialog(this);
                }
                cargar();
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dgvCatalogo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = dgvCatalogo.Rows[e.RowIndex];
                int id = Convert.ToInt32(row.Cells["id"].Value);
                string rif = Convert.ToString(row.Cells["rif"].Value);
                string nombre = Convert.ToString(row.Cells["nombre"].Value);

                using (var form = new asociacionForm(id, rif, nombre))
                {
                    form.ShowDialog(this);
                }
                cargar();
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }
        }
    }
}
