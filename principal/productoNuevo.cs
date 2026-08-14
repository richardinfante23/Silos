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
    public partial class productoNuevo : Form
    {
        public int id = 0;
        public bool actualizar = false;
        productoData sd = new productoData();
        
        public productoNuevo()
        {
            InitializeComponent();
            Load += (s, e) => UiTheme.ApplyForm(this);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {

                if (!validateRegistrarOrEditar("validar"))
                    return;
                if (!actualizar)
                {
                    sd.insertar(txtNombre.Text);

                    MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }
                else
                {
                    if (id == 0)
                    {
                        MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador");
                    }
                    sd.actualizar(id, txtNombre.Text, false);
                    MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private bool validateRegistrarOrEditar(string etiqueta)
        {
            return ValidacionFormulario.validar(this.Controls, etiqueta);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            id = 0;
            Close();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Recuerde al eliminar registro no podra ser recuperado, ¿Desea realmente eliminar registro?", "Advertencia", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    if (sd.buscartramitesxidproducto(id).Rows.Count > 0)
                    {

                        MessageBox.Show("Registro no puede ser eliminado ya que se encuentra asociado a productores", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    sd.actualizar(id, txtNombre.Text, true);
                    MessageBox.Show("Registro Eliminado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void productoNuevo_Load(object sender, EventArgs e)
        {
            btnEliminar.Visible = actualizar;
        }
    }
}
