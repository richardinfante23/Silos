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
    public partial class TercerosForm : Form
    {
        bool nuevo = true;
        bool entrada = true;
        Tercero_ArticulosDatacs td = new Tercero_ArticulosDatacs();
        string tipo = string.Empty;
        public TercerosForm(bool prmNuevo,bool prmEntrada)
        {
            entrada = prmEntrada;
            nuevo = prmNuevo;
            tipo=(entrada ? "Proveedor" : "Usuario");
            InitializeComponent();
            UiTheme.ApplyForm(this);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(txtdescripcion.Text))
                {
                    Base.msjError("Debe colocar nombre");
                    return;
                }




                if (nuevo)
                {



                    td.insertar(txtdescripcion.Text, txtCedula.Text,txtRif.Text,txtDireccion.Text,txtTelefono.Text,entrada?1:2);



                }
                else
                {
                    validar(false);
                 
                    td.actualizar(Convert.ToInt32( txtCodigo.Text),txtdescripcion.Text, txtCedula.Text, txtRif.Text, txtDireccion.Text, txtTelefono.Text, entrada? 1:2,false);



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

        public void soloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {



            //Para obligar a que sólo se introduzcan números 
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                if (Char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso 
            {
                e.Handled = false;
            }
            else
            {
                //el resto de teclas pulsadas se desactivan 
                e.Handled = true;
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

            try
            {

                if (Base.msjPregunta("¿Desea realmente eliminar el " +tipo+": "+ txtdescripcion.Text + "?") == DialogResult.Yes)
                {
                    if (!nuevo)
                    {


                        if (validar(true))
                           return;
                        td.actualizar(Convert.ToInt32(txtCodigo.Text), txtdescripcion.Text, txtCedula.Text, txtRif.Text, txtDireccion.Text, txtTelefono.Text, entrada ? 1 : 2, true);



                    }
                    nuevo = false;
                    btnEliminar.Enabled = true;
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

                if (td.inventarioxtercero(Convert.ToInt32(txtCodigo.Text)).Rows.Count > 0)
                {
                  
                    if (eliminar)
                    {
                        Base.msjError(tipo+" con inventario.No puede ser eliminado");
                        bandera = true;
                    }
                    else
                        Base.msjAdvertencia("Todos los inventarios asociados a este "+tipo+" se veran afectados por esta modificacion. ");
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
