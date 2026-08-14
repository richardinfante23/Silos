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
    public partial class nuevoUsuarioForm : Form
    {
        public bool nuevo = true;
        usuarioData ud = new usuarioData();
        public int id;
        public nuevoUsuarioForm()
        {
            InitializeComponent();
            cargarPerfil();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void nuevoUsuarioForm_Load(object sender, EventArgs e)
        {
            UiTheme.ApplyForm(this);
            
         

        }

        void cargarPerfil(){

            try
            {

                DataTable perfilDT = ud.buscarPerfil();
                if (perfilDT.Rows.Count > 0)
                {
                    cbPerfil.DataSource = perfilDT;

                    cbPerfil.DisplayMember = "nombre";
                    cbPerfil.ValueMember = "id";

                }
                if(nuevo)
                cbPerfil.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando Conductor. Error Contacte al Administrador del Sistema", ex);

            }

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txtLogin.Text))
                {
                    Base.msjAdvertencia("Login Obligatorio");
                    return;
                }

                if (string.IsNullOrEmpty(txtClave.Text))
                {
                    Base.msjAdvertencia("Clave Obligatorio");
                    return;
                }

                if (string.IsNullOrEmpty(txtNombre.Text))
                {
                    Base.msjAdvertencia("Nombre Obligatorio");
                    return;
                }
                if (string.IsNullOrEmpty(cbPerfil.Text))
                    {
                    Base.msjAdvertencia("Debe seleccionar Perfil");
                    return;
                    }

                if (nuevo)
                {
                    ud.insertar(txtLogin.Text, Seguridad.Encriptar(txtClave.Text), txtNombre.Text, Convert.ToInt16(((System.Data.DataRowView)(cbPerfil.SelectedItem)).Row.ItemArray[0]));


                }
                else
                {
                    ud.actualizar(id, txtLogin.Text, Seguridad.Encriptar(txtClave.Text), txtNombre.Text, Convert.ToInt16(((System.Data.DataRowView)(cbPerfil.SelectedItem)).Row.ItemArray[0]), false);


                }
                Close();
                // productoDT.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando Conductor. Error Contacte al Administrador del Sistema", ex);

            }

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Recuerde al eliminar registro no podra ser recuperado, ¿Desea realmente eliminar registro?", "Advertencia", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                ud.actualizar(id, txtLogin.Text, Seguridad.Encriptar(txtClave.Text), txtNombre.Text, Convert.ToInt16(((System.Data.DataRowView)(cbPerfil.SelectedItem)).Row.ItemArray[0]), true);
              
                MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            
        }
    }
}
