using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace principal
{
    public partial class productosFrom : Form
    {

        public productosFrom()
        {
            InitializeComponent();
        }
        
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void productosFrom_Load(object sender, EventArgs e)
        {
            cargarProducto();
            
        }
       
             void cargarProducto()
        {
            try
            {
                productoData cd = new productoData();
                dgvProductos.DataSource = new DataTable();
                DataTable productoDT = cd.buscar();
                if (productoDT.Rows.Count > 0)
                {
                    dgvProductos.DataSource = productoDT;
                    dgvProductos.Columns["id"].Visible = false;
                    dgvProductos.Columns["eliminado"].Visible = false;
                   
                   
                }
               // productoDT.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando Conductor. Error Contacte al Administrador del Sistema", ex);

            }

        }

             private void btnNuevo_Click(object sender, EventArgs e)
             {

                 try
                 {
                     productoNuevo nuf = new productoNuevo();
                     nuf.ShowDialog();


                     cargarProducto();


                 }
                 catch (Exception ex)
                 {
                     throw new Exception("Ha ocurrido un agregando Usuario. Error Contacte al Administrador del Sistema", ex);

                 }
             }

             private void dgvProductos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
             {
                 productoNuevo nuf = new productoNuevo();
                 nuf.actualizar = true;

                 foreach (DataGridViewRow filaSeleccionado in dgvProductos.SelectedRows)
                 {
                     nuf.id = Convert.ToInt16(filaSeleccionado.Cells["id"].Value);
                     nuf.txtNombre.Text = filaSeleccionado.Cells["nombre"].Value.ToString();
                    


                 }

                 nuf.ShowDialog();
                 cargarProducto();
             }
    }
}
