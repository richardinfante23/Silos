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
    public partial class UnidadArticulosFormcs : Form
    {
        UnidadArticulosData ud = new UnidadArticulosData();
        public bool nuevo = true;
        public int id = 0;
        public UnidadArticulosFormcs()
        {
            InitializeComponent();


        }
        protected override void OnLoad(EventArgs e)
        {
            if (!nuevo)
                txtCodigo.Text = id.ToString();
            UiTheme.ApplyForm(this);
            base.OnLoad(e);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

            try
            {

                if (string.IsNullOrWhiteSpace(txtdescripcion.Text))
                {
                    Base.msjError("Debe colocar descripcion");
                    return;
                }




                if (nuevo)
                {



                    ud.insertar(txtdescripcion.Text);



                }
                else
                {
                    validarunidadarticulos(false);
                    ud.actualizar(id, txtdescripcion.Text, false);




                }
                btnEliminar.Enabled = true;
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

        private void btnEliminar_Click(object sender, EventArgs e)
        {

            try
            {



                if (!nuevo)
                {
                    if (validarunidadarticulos(true))
                        return;
                    ud.actualizar(id, txtdescripcion.Text, true);



                }

                Base.msjExito("Registro eliminado exitosamente");
                Close();

            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un insertando recepcion. Error Contacte al Administrador del Sistema", ex);

            }


        }

        bool validarunidadarticulos(bool eliminar)
        {
            bool bandera = false;
            try
            {

             if(ud.articulosxunidad(id).Rows.Count >0)
                {
                    if(eliminar)
                    {
                        Base.msjError("Unidad asociada a artíclos.No puede ser eliminada");
                        bandera = true;
                    }
                    else
                        Base.msjAdvertencia("Todos los artíclos asociados a esta unidad se veran afectados por esta modificacion. ");
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
