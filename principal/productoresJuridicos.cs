using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace principal
{
    public partial class productoresJuridicos : Form
    {
        private readonly string letrasJuridicas = ConfigurationManager.AppSettings["letrasJuridicas"];
        private readonly string letrasNaturales = ConfigurationManager.AppSettings["letrasNaturales"];
        ProductorData pd = new ProductorData();
        destinoData dd = new destinoData();
        public bool productor = true;
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
            cargarProductor();
        }
        void cargarProductor()
        {
            try
            {
                DataTable dtProductor = new DataTable();
                dgvProdcutores.DataSource = new DataTable();
                if (productor)
                    dtProductor = pd.buscarJuridicoGeneral();
                else
                    dtProductor = dd.buscarDestinoGeneral();
                if (dtProductor.Rows.Count > 0)
                {
                    dgvProdcutores.DataSource = dtProductor;
                    dgvProdcutores.Columns["id"].Visible = false;
                    dgvProdcutores.Columns["eliminado"].Visible = false;
                    dgvProdcutores.Columns["id_asociacion"].Visible = false;
                }
                else
                {

                    MessageBox.Show("No se Encontraron resultados","Información",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
               

            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando productor. Error Contacte al Administrador del Sistema", ex);

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
                    cf.txtRif.Text = Seleccionado.Cells["R.I.F"].Value.ToString();
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
