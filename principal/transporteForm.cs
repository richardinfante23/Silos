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
    public partial class transporteForm : Form
    {
        transportesData td = new transportesData();
        conductoresData cd = new conductoresData();
        public int id = 0;
        public bool actualizar = false;
        public transporteForm()
        {
            InitializeComponent();
            Load += (s, e) => UiTheme.ApplyForm(this);
        }

        public transporteForm(bool prmactualizar)
        {
            InitializeComponent();

            actualizar = prmactualizar;
            Load += (s, e) => UiTheme.ApplyForm(this);
        }

        public transporteForm(int prmid, string prmrif, string prmnombre)
        {
            InitializeComponent();
            id = prmid;
            txtRif.Text = prmrif ?? string.Empty;
            txtNombre.Text = prmnombre ?? string.Empty;
            actualizar = true;
            btnEliminar.Visible = true;
            Text = "Editar transporte";
            Load += (s, e) => UiTheme.ApplyForm(this);
        }
        private bool validateRegistrarOrEditar(string etiqueta)
        {
            return ValidacionFormulario.validar(this.Controls,etiqueta);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {

                if (!validateRegistrarOrEditar("validar"))
                    return;
                if (!actualizar)
                {
                    td.insertar(txtRif.Text, txtNombre.Text);
                   
                    MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }
                else
                {
                    if (id == 0)
                    {
                        MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador");
                    }
                    td.actualizar(id, txtRif.Text, txtNombre.Text, false);
                    MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
              try
            {
                if (cd.buscarconductorxidtransporte(id).Rows.Count > 0)
                {

                    MessageBox.Show("Registro no puede ser eliminado ya que se encuentra asociado a conductores", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                DialogResult result = MessageBox.Show("Recuerde al eliminar registro no podra ser recuperado, ¿Desea realmente eliminar registro?", "Advertencia", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                   
                    td.actualizar(id, txtRif.Text, txtNombre.Text, true);
                    MessageBox.Show("Registro Eliminado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    Close(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
