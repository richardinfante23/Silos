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
    public partial class productoresNaturalesForm : Form
    {
        ProductorData pd = new ProductorData();
        destinoData dd = new destinoData();
        private readonly string letrasJuridicas = ConfigurationManager.AppSettings["letrasJuridicas"];
        private readonly string letrasNaturales = ConfigurationManager.AppSettings["letrasNaturales"];
       public  bool productor = true;
        public productoresNaturalesForm()
        {
            InitializeComponent();
        }

        void cargarProductor()
        {

            try  {
                dgvProdcutores.DataSource =new DataTable();
                if(productor)
                dgvProdcutores.DataSource=pd.buscarGeneral();
                else 
                    dgvProdcutores.DataSource = dd.buscarGeneral();
                dgvProdcutores.Columns["id"].Visible = false;
                dgvProdcutores.Columns["eliminado"].Visible = false;
                dgvProdcutores.Columns["id_asociacion"].Visible = false;
            
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando productor. Error Contacte al Administrador del Sistema", ex);

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
                    cf.txtCedula.Text = Seleccionado.Cells["Cédula"].Value.ToString();
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
                throw new Exception("Ha ocurrido un buscando productor. Error Contacte al Administrador del Sistema"+ex.Message);

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
