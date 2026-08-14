using System;
using System.Data;
using System.Windows.Forms;

namespace principal
{
    public partial class silosGridForm : Form
    {
        private readonly silosData sd = new silosData();
        private GridHeaderFilter headerFilter;

        public silosGridForm()
        {
            InitializeComponent();
        }

        private void silosGridForm_Load(object sender, EventArgs e)
        {
            UiTheme.ApplyForm(this);
            UiTheme.StylePrimaryButton(btnNuevo);
            UiTheme.StyleSecondaryButton(btnCancelar);
            Icon = UiTheme.GetAppIcon();
            Text = "Silos";
            CatalogGridHelper.EnsureHeaderFilter(dgvCatalogo, ref headerFilter);
            cargar();
        }

        private void cargar()
        {
            try
            {
                DataTable dt = sd.buscar();
                headerFilter.Bind(dt);
                CatalogGridHelper.HideColumn(dgvCatalogo, "id");
                CatalogGridHelper.HideColumn(dgvCatalogo, "eliminado");
                CatalogGridHelper.SetHeader(dgvCatalogo, "nombre", "Nombre");
                headerFilter.SyncHeaders();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un error buscando silos. Contacte al Administrador del Sistema", ex);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new silosForm())
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
                string nombre = Convert.ToString(row.Cells["nombre"].Value);

                using (var form = new silosForm(id, nombre))
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
