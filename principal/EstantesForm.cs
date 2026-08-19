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
    public partial class EstantesForm : Form
    {
        bool nuevo = true;
        EstantesData ad = new EstantesData();
        DataTable dtGeneral = new DataTable();
        public EstantesForm(bool prmnuevo,int prmidarticulo)
        {
            InitializeComponent();
            UiTheme.ApplyForm(this);

            nuevo = prmnuevo;
            if(!nuevo)
            {
                txtCodigo.Enabled = true;

            }
            else
            {

                btnGuardar.Text = "Guardar";
            }
         
           
               
        
        }
        void llenarEstatntes()
        {
            try
            {
                dtGeneral = new DataTable();
                dtGeneral = ad.buscar(!string.IsNullOrWhiteSpace(txtCodigo.Text) ? Convert.ToInt16( txtCodigo.Text):0,txtdescripcion.Text);


                if (dtGeneral.Rows.Count > 0)
                {
                    dgvEstantes.DataSource = new DataTable();
                    dgvEstantes.DataSource = dtGeneral;
                    dgvEstantes.Columns["id"].HeaderText = "Códgo";
                    dgvEstantes.Columns["eliminado"].Visible = false;
                    dgvEstantes.Columns["descripcion"].HeaderText = "Nombre";

                    llenarCampos(dtGeneral.Rows[0]);

                }
                else
                {

                    Base.msjAdvertencia("No se encontraron Estantes.");


                }


            }
            catch (Exception ex)
            {

                Base.msjError(ex.Message);
            }

        }
        void limpiarCampos()
        {

            txtCodigo.Text = string.Empty;
            txtdescripcion.Text = string.Empty;
            dgvSecciones.DataSource = new DataTable();
        }
        void llenarCampos(DataRow fila)
        {
            try { 
            txtCodigo.Enabled = false;
            txtCodigo.Text = fila["id"].ToString();
            txtdescripcion.Text= fila["descripcion"].ToString();
            btnAgregar.Enabled = true;
            btnEliminar.Enabled = true;
                txtCodigo.Enabled = false;
            llenarSeccion();
            btnGuardar.Text = "Guardar";
           nuevo = false;


        }
            catch (Exception ex)
            {

                Base.msjError(ex.Message);
            }

}


        void llenarSeccion()
        {
                    try
                    { 

                        if (string.IsNullOrWhiteSpace(txtCodigo.Text))
                            return;

                        SeccionArticuloData ed = new SeccionArticuloData();
                        DataTable dt = ed.buscar(Convert.ToInt32(txtCodigo.Text));
                        dgvSecciones.DataSource = new DataTable();
                        dgvSecciones.DataSource = dt;
                        dgvSecciones.Columns["id"].HeaderText = "Códgo";
                        dgvSecciones.Columns["eliminado"].Visible = false;
                        dgvSecciones.Columns["id_estante"].Visible = false;
                        dgvSecciones.Columns["descripcion"].HeaderText = "Nombre";

            }
                    catch (Exception ex)
                    {
                        Base.msjError(ex.Message);

                    }

       }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {

              
                SeccionesForm sf = new SeccionesForm();
                sf.id_estante = Convert.ToInt16( txtCodigo.Text);
                sf.nuevo = true;
                sf.ShowDialog();

                llenarSeccion();

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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if(btnGuardar.Text=="Guardar")
                {


               
                        if (string.IsNullOrWhiteSpace(txtdescripcion.Text))
                        {

                            Base.msjAdvertencia("Debe llenar Descripción");
                        return;
                        }

                        if (nuevo)
                        {

                           

                        DataRow drResultado = ad.insertar(txtdescripcion.Text).Rows[0];

                        if (drResultado.ItemArray.Count() > 0)
                        {
                            txtCodigo.Text = drResultado["id"].ToString();

                        }

                    }
                        else
                        {
                            ad.actualizar(Convert.ToInt16(txtCodigo.Text), txtdescripcion.Text, false);

                        }
                        Base.msjExito("Registro guardado exitosamente");
                }
                llenarEstatntes();
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
        }

        private void dgvEstantes_DoubleClick(object sender, EventArgs e)
        {
            if (dgvEstantes.RowCount == 0)
                return;
            try
            {
                var seleccionado = Convert.ToInt32(dgvEstantes.SelectedRows[0].Cells[0].Value);

                DataRow[] rowseleccionado = dtGeneral.Select("id=" + seleccionado);
                llenarCampos(rowseleccionado[0]);
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
        }

        private void dgvSecciones_DoubleClick(object sender, EventArgs e)
        {
            if (dgvSecciones.RowCount == 0)
                return;

            try {
                var seleccionado = Convert.ToInt32(dgvSecciones.SelectedRows[0].Cells[0].Value);
                
                SeccionesForm sf = new SeccionesForm();
                sf.id_estante = Convert.ToInt16(txtCodigo.Text);
                sf.txtNombre.Text = dgvSecciones.SelectedRows[0].Cells[1].Value.ToString() ;
                sf.nuevo = false;
                sf.id = seleccionado;
                sf.ShowDialog();

                llenarSeccion();

            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
}

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {

                if (Base.msjPregunta("¿Desea realmente eliminar la Estante: " + txtdescripcion.Text + "?") == DialogResult.Yes)
                {
                    if (!nuevo)
                    {


                        if (validar(true))
                            return;
                        ad.actualizar(Convert.ToInt16(txtCodigo.Text), txtdescripcion.Text, true);




                    }

                    Base.msjExito("Eliminado  exitosamente");
                    limpiarCampos();
                    llenarEstatntes();
                }

            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
}

        bool validar(bool eliminar)
        {
            seccionData sd = new seccionData();
            bool bandera = false;
            try
            {

                if (sd.validarestanteyseccion(0, Convert.ToInt16(txtCodigo.Text)).Rows.Count > 0)
                {
                    if (eliminar)
                    {
                        Base.msjError("Estante con inventario.No puede ser eliminada");
                        bandera = true;
                    }
                    else
                        Base.msjAdvertencia("Todos los inventarios asociados a este Estante se veran afectados por esta modificacion. ");
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
