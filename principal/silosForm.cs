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
    public partial class silosForm : Form
    {
        public int id = 0;
        public bool actualizar=false;
        silosData sd= new silosData();
        public silosForm()
        {
            InitializeComponent();
            Load += (s, e) => UiTheme.ApplyForm(this);
        }

        public silosForm(int prmid, string prmnombre)
        {
            InitializeComponent();
            id = prmid;
            txtNombre.Text = prmnombre ?? string.Empty;
            actualizar = true;
            btnEliminar.Visible = true;
            Text = "Editar silo";
            Load += (s, e) => UiTheme.ApplyForm(this);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            id = 0;
            Close();
        }

        private bool validateRegistrarOrEditar(string etiqueta)
        {
            return ValidacionFormulario.validar(this.Controls, etiqueta);
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {

            try
            {

                if (!validateRegistrarOrEditar("validar"))
                    return;
                if (!actualizar)
                {
                    sd.insertar( txtNombre.Text);

                    Base.msjExito("Registro guardado exitosamente.");


                }
                else
                {
                    if (id == 0)
                    {
                        Base.msjError("Ha ocurrido un error. Contacte al administrador del sistema.");
                    }
                    sd.actualizar(id,txtNombre.Text, false);
                    Base.msjExito("Registro guardado exitosamente.");
                }
                Close();
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "guardar el silo");
            }

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = Base.msjPreguntaCancel("Al eliminar este registro no podrá recuperarse. ¿Desea continuar con la eliminación?");
                if (result == DialogResult.Yes)
                {
                    if (sd.buscartramitesxidsilos(id).Rows.Count > 0)
                    {

                        Base.msjError("El registro no puede ser eliminado porque se encuentra asociado a productores.");
                        return;
                    }
                    sd.actualizar(id, txtNombre.Text, true);
                    Base.msjExito("Registro eliminado exitosamente.");


                    Close();
                }
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }

        }
      
    }
}
