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
    public partial class SeccionesForm : Form
    {
        seccionData sd = new seccionData();
        public bool nuevo = true;
        public int id=0;
        public int id_estante = 0;
        public SeccionesForm()
        {
            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {

                    Base.msjAdvertencia("Debe llenar Seccion");
                    return;
                }

                if (nuevo)
                {

                    sd.insertar(txtNombre.Text,id_estante);

                }
                else
                {
                    validar(false);
                    sd.actualizar(id,txtNombre.Text,id_estante,false);

                }
                Base.msjExito("Registro guardado exitosamente");
                Close();
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

        private void SeccionesForm_Load(object sender, EventArgs e)
        {
            if (!nuevo)
                btnEliminar.Enabled = true;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {

                if (Base.msjPregunta("¿Desea realmente eliminar la Sección: " + txtNombre.Text + "?") == DialogResult.Yes)
                {
                    if (!nuevo)
                    {


                        if (validar(true))
                            return;
                        sd.actualizar(id, txtNombre.Text, id_estante, true);




                    }
             
                    Base.msjExito("Eliminado  exitosamente");
                    Close();
                }

            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
        }
        bool validar(bool eliminar)
        {
            bool bandera = false;
            try
            {

                if (sd.validarestanteyseccion(id,0).Rows.Count > 0)
                {
                    if (eliminar)
                    {
                        Base.msjError("Sección con inventario.No puede ser eliminada");
                        bandera = true;
                    }
                    else
                        Base.msjAdvertencia("Todos los inventarios asociados a esta sección se veran afectados por esta modificacion. ");
                }

            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
            return bandera;
        }

    }
}
