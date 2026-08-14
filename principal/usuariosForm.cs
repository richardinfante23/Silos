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
    public partial class usuariosForm : Form
    {
        usuarioData ud = new usuarioData();
        public usuariosForm()
        {
            InitializeComponent();
        }

        private void usuariosForm_Load(object sender, EventArgs e)
        {
            
            cargarUsuarios();

        }


        void cargarUsuarios()
        {
            try
            {
                
                dgvUsuarios.DataSource = new DataTable();
                DataTable analisisDT =ud.buscar();
                dgvUsuarios.DataSource = analisisDT;
                dgvUsuarios.Columns["id"].Visible = false;
                dgvUsuarios.Columns["eliminado"].Visible = false;
                dgvUsuarios.Columns["id_perfil"].Visible = false;
                dgvUsuarios.Columns["pasword"].Visible = false;
                
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
                nuevoUsuarioForm nuf = new nuevoUsuarioForm();
                nuf.ShowDialog();


                cargarUsuarios();

             
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un agregando Usuario. Error Contacte al Administrador del Sistema", ex);

            }

        }

        private void dgvUsuarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            nuevoUsuarioForm nuf = new nuevoUsuarioForm();
            nuf.nuevo = false;

            foreach (DataGridViewRow filaSeleccionado in dgvUsuarios.SelectedRows)
            {
                nuf.id = Convert.ToInt16(filaSeleccionado.Cells["id"].Value);
                nuf.txtNombre.Text = filaSeleccionado.Cells["nombre"].Value.ToString();
                nuf.txtLogin.Text = Equals(filaSeleccionado.Cells["login"].Value, DBNull.Value) ? string.Empty : filaSeleccionado.Cells["login"].Value.ToString();
                nuf.txtClave.Text = Equals(filaSeleccionado.Cells["pasword"].Value, DBNull.Value) ? string.Empty : Seguridad.DesEncriptar( filaSeleccionado.Cells["pasword"].Value.ToString());
                nuf.cbPerfil.SelectedValue = Convert.ToInt16(filaSeleccionado.Cells["id_perfil"].Value);
                

            }
            nuf.ShowDialog();
            cargarUsuarios();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

       

    }
}
