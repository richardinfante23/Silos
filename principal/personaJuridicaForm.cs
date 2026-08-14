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
    public partial class personaJuridicaForm : Form
    {
        public int id = 0;
        public bool actualizar = false;
        ProductorData pd = new ProductorData();
        destinoData dd = new destinoData();
        public bool eliminado=false;
        public bool destino = false;
        public personaJuridicaForm()
        {
            InitializeComponent();
            cargarAsociaciones();
            Load += (s, e) => UiTheme.ApplyForm(this);
        }
        void cargarAsociaciones()
        {
            try
            {
                asiciasionesData ad = new asiciasionesData();
                DataTable productoDT = ad.buscar();
                if (productoDT.Rows.Count > 0)
                {
                    cbAsociaciones.DataSource = productoDT;

                    cbAsociaciones.DisplayMember = "nombre";
                    cbAsociaciones.ValueMember = "id";
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando Conductor. Error Contacte al Administrador del Sistema", ex);

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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {

                if (!validateRegistrarOrEditar("validar"))
                    return;
                if (destino)
                    destinos();
                else
                    productor();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void destinos()
        {
            if (!actualizar)
            {
                dd.insertarPersonaJuridica(txtNombre.Text, Convert.ToDecimal(txtRif.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), false);
                DataTable productorDT = dd.buscarPersonaJuridica(cbLetra.SelectedItem.ToString(), Convert.ToDecimal(txtRif.Text));
                if (productorDT.Rows.Count > 0)
                {

                    foreach (DataRow fila in productorDT.Rows)
                    {
                        id = Convert.ToInt32(fila["id"]);
                        continue;
                    }
                }
                MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            else
            {
                if (id == 0)
                {
                    MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador");
                    return;
                }
                dd.actualizarPersonaJuridica(id, txtNombre.Text, Convert.ToDecimal(txtRif.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), false);
                MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void productor()
        {
            if (!actualizar)
            {
                pd.insertarPersonaJuridica(txtNombre.Text, Convert.ToDecimal(txtRif.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), false);
                DataTable productorDT = pd.buscarPersonaJuridica(cbLetra.SelectedItem.ToString(), Convert.ToDecimal(txtRif.Text));
                if (productorDT.Rows.Count > 0)
                {

                    foreach (DataRow fila in productorDT.Rows)
                    {
                        id = Convert.ToInt32(fila["id"]);
                        continue;
                    }
                }
                MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            else
            {
                if (id == 0)
                {
                    MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador");
                    return;
                }

                pd.actualizarPersonaJuridica(id, txtNombre.Text, Convert.ToDecimal(txtRif.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), false);
                //MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAgregarAsociacion_Click(object sender, EventArgs e)
        {
            try
            {
                asociacionForm tf = new asociacionForm();
                tf.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            cargarAsociaciones();
        }

        private void btnEditarAsociacion_Click(object sender, EventArgs e)
        {
            try
            {
                if (!validateRegistrarOrEditar("validareditar"))
                    return;
                object transporte = cbAsociaciones.SelectedItem;
                asociacionForm tf = new asociacionForm();
                tf.actualizar = true;
                tf.btnEliminar.Visible = true;
                tf.txtRif.Text = Equals(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[2], null) ? string.Empty : ((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[2].ToString();
                tf.id = Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]);
                tf.txtNombre.Text = cbAsociaciones.Text;
                tf.ShowDialog();
                cargarAsociaciones();
                cbAsociaciones.SelectedValue = tf.id;
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
                if (pd.bustramitesxidPorductorJuridico(id).Rows.Count > 0 || dd.bustramitesxidDestinoJuridico(id).Rows.Count>0)
                {

                    MessageBox.Show("Registro no puede ser eliminado ya que se encuentra asociado a Tramites de recepcion o despacho", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                DialogResult result = MessageBox.Show("Recuerde al eliminar registro no podra ser recuperado, ¿Desea realmente eliminar registro?", "Advertencia", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    if (destino) {
                        dd.actualizarPersonaJuridica(id, txtNombre.Text, Convert.ToDecimal(txtRif.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), true);

                    }
                    else
                    {
                        pd.actualizarPersonaJuridica(id, txtNombre.Text, Convert.ToDecimal(txtRif.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), true);

                    }
                  
                    MessageBox.Show("Registro Eliminado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    eliminado = true;

                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void personaJuridicaForm_Load(object sender, EventArgs e)
        {
            if (actualizar)
                btnEliminar.Visible = true;
            if (destino)
            {
                this.Text = "Cliente Jurídico";
                this.lbNombre.Text = "Cliente Jurídico";
            }
                
        }

      


     

    }
}
