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
    public partial class ArticulosForm : Form
    {
        ArticulosData ad = new ArticulosData();
        DataTable resultadodt = new DataTable();
        bool nuevo = false;
        int id_usuario = 0;
        public bool desdeMovimientos = false;
        public ArticulosForm(bool prmnuevo,int prmid_usuario)
        {

            InitializeComponent();
            llenarUnidades();
            nuevo = prmnuevo;
            id_usuario = prmid_usuario;
            if(nuevo)
            {
             
                btnGuardar.Text = "Guardar";

            }else
            {
                txtCodigo.Enabled = true;
                dtpFecha.Format = DateTimePickerFormat.Custom;
                dtpFecha.CustomFormat = " ";
            }
         
          
        }
       void llenarGrid()
        {
            try {
                 resultadodt = ad.buscar(string.IsNullOrWhiteSpace( txtCodigo.Text)?0:Convert.ToInt32(txtCodigo.Text),txtdescripcion.Text,
                     !string.IsNullOrWhiteSpace(cbUnidad.Text) ?Convert.ToInt32(((System.Data.DataRowView)(cbUnidad.SelectedItem)).Row.ItemArray[0]):0,
                     string.IsNullOrWhiteSpace(dtpFecha.Text)?DateTimePicker.MinimumDateTime:dtpFecha.Value);
                dgvArticulos.DataSource = new DataTable();

                if(resultadodt.Rows.Count>0)
                {
                            dgvArticulos.DataSource = resultadodt;

                         

                            lbcantidadtotal.Visible = true;
                            lbcantidadtotal.Text = " Total= " + resultadodt.Rows.Count;
                            dgvArticulos.Columns["id"].HeaderText = "Código" ;
                            dgvArticulos.Columns["id_unidad"].Visible = false;
                            llenarCampos(resultadodt.Rows[0]);

                          
                }
                else
                {

                    Base.msjAdvertencia("No se encontraron resultados.");

                }
               
           
               }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

}

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                UnidadArticulosFormcs ud = new UnidadArticulosFormcs();
                ud.ShowDialog();
                llenarUnidades();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

       void  llenarUnidades()
        {

            try
            {
                UnidadArticulosData cd = new UnidadArticulosData();
                DataTable productoDT = cd.buscar();
                if (productoDT.Rows.Count > 0)
                {
                    cbUnidad.DataSource = productoDT;

                    cbUnidad.DisplayMember = "decripcion";
                    cbUnidad.ValueMember = "id";

                }
                // productoDT.Dispose();
            }
            catch (Exception ex)
            {
                Base.msjError("Error llenado unidades: " + ex.Message);

            }
            cbUnidad.SelectedIndex = -1;
            cbUnidad.Text = string.Empty;

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {


                if (string.IsNullOrWhiteSpace(cbUnidad.Text))
                    return;


               
                UnidadArticulosFormcs tf = new UnidadArticulosFormcs();

                tf.nuevo = false;
                tf.id = Convert.ToInt32(((System.Data.DataRowView)(cbUnidad.SelectedItem)).Row.ItemArray[0]);
               
                tf.txtdescripcion.Text = cbUnidad.Text;
                tf.btnEliminar.Enabled = true;
                tf.ShowDialog();
                llenarUnidades();
                cbUnidad.SelectedValue = tf.id;
              
              
           
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void dgvArticulos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var seleccionado = Convert.ToInt32(dgvArticulos.SelectedRows[0].Cells[0].Value);

            DataRow[] rowseleccionado = resultadodt.Select("id=" + seleccionado);
            llenarCampos(rowseleccionado[0]);

        }

        void llenarCampos(DataRow fila)
        {
            try
            {
                txtCodigo.Text = fila["id"].ToString();
                txtdescripcion.Text = fila["Descripción"].ToString();
                cbUnidad.SelectedValue = Convert.ToInt32(fila["id_unidad"]);
                dtpFecha.Format = DateTimePickerFormat.Long;
                dtpFecha.Value = Convert.ToDateTime(fila["fecha"]);
              btnEntradas.Enabled=btnSalidas.Enabled= btnEliminar.Enabled = true;
                txtCodigo.Enabled = false;
                btnGuardar.Text = "Guardar";

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }





        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

            try
            {

                if (btnGuardar.Text == "Guardar")
                {




                                if (string.IsNullOrWhiteSpace(txtdescripcion.Text))
                                {
                                    Base.msjAdvertencia("Debe ingresar descripcion");
                                    return;
                                }
                                if (string.IsNullOrWhiteSpace(dtpFecha.Text))
                                {
                                    Base.msjAdvertencia("Debe ingresar fecha creacion");
                                    return;
                                }
                                if (string.IsNullOrWhiteSpace(cbUnidad.Text))
                                {
                                    Base.msjAdvertencia("Debe ingresar unidad");
                                    return;
                                }


                                if (nuevo)
                                {



                                    ad.insertar(txtdescripcion.Text,dtpFecha.Value,id_usuario,  Convert.ToInt32(((System.Data.DataRowView)(cbUnidad.SelectedItem)).Row.ItemArray[0]));

                    

                                }
                                else
                                {
                                    validar(false);
                                    ad.actualizar(Convert.ToInt16( txtCodigo.Text), txtdescripcion.Text,dtpFecha.Value,id_usuario, Convert.ToInt32(((System.Data.DataRowView)(cbUnidad.SelectedItem)).Row.ItemArray[0]), false);




                                }
                                nuevo = false;
                              
                                Base.msjExito("Registro guardado exitosamente");
                                llenarGrid();
                }
              else
                {
                    llenarGrid();

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

                if (ad.articuloconinventario(Convert.ToInt32( txtCodigo.Text)).Rows.Count > 0)
                {
                    if (eliminar)
                    {
                        Base.msjError("Artíclo con inventario.No puede ser eliminada");
                        bandera = true;
                    }
                    else
                        Base.msjAdvertencia("Todos los inventarios asociados a este artíclo se veran afectados por esta modificacion. ");
                }

            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
            return bandera;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

            try
            {

            if(Base.msjPregunta("¿Desea realmente eliminar el artículo: " + txtdescripcion.Text+"?")== DialogResult.Yes)
                {
                            if (!nuevo)
                            {


                                if (validar(true))
                                    return;
                                ad.actualizar(Convert.ToInt16(txtCodigo.Text), txtdescripcion.Text, dtpFecha.Value, id_usuario, Convert.ToInt32(((System.Data.DataRowView)(cbUnidad.SelectedItem)).Row.ItemArray[0]), true);




                            }
                            nuevo = false;
                            btnEliminar.Enabled = true;
                            Base.msjExito("Eliminado  exitosamente");
                         limpiarCampos();
                            llenarGrid();
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
            cbUnidad.SelectedIndex = -1;
            dtpFecha.Value = DateTime.Today;
            dtpFecha.Format = DateTimePickerFormat.Custom;
            dtpFecha.CustomFormat = " ";

        }

        private void dtpFecha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)Keys.Back)
            {

               
                    ((DateTimePicker)(((Control)sender))).Value = DateTime.Today;
                    ((DateTimePicker)(((Control)sender))).Format = DateTimePickerFormat.Custom;
                    ((DateTimePicker)(((Control)sender))).CustomFormat = " ";


            }else
            {
                formatoFecha(sender);
            }
        }

        private static void formatoFecha(object sender)
        {
            ((DateTimePicker)(((Control)sender))).Format = DateTimePickerFormat.Long;
           // ((DateTimePicker)(((Control)sender))).CustomFormat = "dddd MMMM yyyy";
        }

        private void dtpFecha_DropDown(object sender, EventArgs e)
        {
            formatoFecha(sender);
        }

        private void ArticulosForm_Load(object sender, EventArgs e)
        {
            if (desdeMovimientos)
            {

                llenarGrid();
            }
        }

        private void btnEntradas_Click(object sender, EventArgs e)
        {
            try
            {

                abrirMovimientos(true,false);
            }
       
            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
         }

        void abrirMovimientos(bool entrada, bool nuevo)
        {

            try
            {
                Cursor = Cursors.WaitCursor;
                MovimientoArticulosForm pf = new MovimientoArticulosForm(entrada, nuevo, id_usuario);
                pf.desdearticulos = true;
                pf.id_articulo = Convert.ToInt16( txtCodigo.Text);
                
                
                pf.Show();
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnSalidas_Click(object sender, EventArgs e)
        {
            try
            {

                abrirMovimientos(false, false);
            }

            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
        }
    }
}
