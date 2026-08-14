using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace principal
{
    public partial class MovimientoArticulosForm : Form
    {
        int id_usuario = 0;
        bool  nuevo,entrada= false;
        int id_conductor=0;
        DataTable productoDT = new DataTable();
        DataTable detalleDT = new DataTable();
        DataTable GeneralDT = new DataTable();
        string terceros = string.Empty;
        int id_tipo_movi = 0;
        bool anulado = false;
        MovimientoBO movimientoBo = new MovimientoBO();
        List<detalle_articuloBO> listaDetalle = new List<detalle_articuloBO>();
        List<detalle_articuloBO> listaDetalleR = new List<detalle_articuloBO>();
        private readonly string empresa = ConfigurationManager.AppSettings["Empresa"];
        private readonly string direccionempresa = ConfigurationManager.AppSettings["direccionEmpresa"];
        public bool desdearticulos = false;
        public int id_articulo=0;
        historicoarticulosDatacs hd = new historicoarticulosDatacs();
        int cantidadTemporal = 0;
        public MovimientoArticulosForm(bool prmentrada, bool prmnuevo, int prmid_usuarios)
        {
            nuevo = prmnuevo;
            id_usuario = prmid_usuarios;
            entrada = prmentrada;
            InitializeComponent();
           id_tipo_movi= entrada ? 1 : 2;
            if (entrada)
            {

                terceros = lbprovcli.Text = "Proveedor";
                this.Text = "Ajuste entrada artículos";
            }
            else
            {
                dgvDetalle.Columns["cbEstante"].Visible = false;
                dgvDetalle.Columns["txtestante"].Visible = true;
                dgvDetalle.Columns["txtExistencia"].Visible = true;
                dgvDetalle.Columns["txtPRecio"].ReadOnly = true;
                terceros = lbprovcli.Text = "Clientes";
                this.Text = "Ajuste salida artículos"; 

            }
            if (nuevo)
            {
                lbfechahasta.Visible = false;
                dtpFecha.Visible = false;

            }else
            {
                txtCodigo.Enabled = true;
                btnGuardar.Text = "Buscar";
                dtpFecha.Format = DateTimePickerFormat.Custom;
                dtpFecha.CustomFormat = " ";

                dtpfechahasta.Format = DateTimePickerFormat.Custom;
                dtpfechahasta.CustomFormat = " ";

            }


            llenarProvCli();
            llenararticulos();
            llenarEstatntes();
        }

        int existencia(int id_articulo, int id_seccion)
        {
            int existencia = 0;
            try
            {
           if(hd.existenciaarticulo(id_articulo, id_seccion).Rows.Count!=0)
                existencia = Convert.ToInt16((hd.existenciaarticulo(id_articulo, id_seccion).Rows[0].ItemArray[0]));

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return existencia;
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
           
            
            reporteForm rf = new reporteForm(new List<analisis>());
            rf.inventario = true;
            rf.listaDetalle = listaDetalle;
            List<MovimientoBO> listaMovimiento = new List<MovimientoBO>();
            //objRecepcion.listaAnalisis = listaAnalisis;
            listaMovimiento.Add(movimientoBo);

            rf.rvReporte.ProcessingMode = ProcessingMode.Local;


          
            if(entrada)
                rf.rvReporte.LocalReport.ReportEmbeddedResource = "principal.movimientoEntrada.rdlc";
            else
                rf.rvReporte.LocalReport.ReportEmbeddedResource = "principal.movimientoSalida.rdlc";
            ReportDataSource rds = new ReportDataSource("boleta", listaMovimiento);
            rf.rvReporte.LocalReport.DataSources.Clear();

            rf.rvReporte.LocalReport.DataSources.Add(rds);
        
            rf.rvReporte.LocalReport.Refresh();
            rf.ShowDialog();
        }

        private void dtpFecha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)Keys.Back)
            {


                ((DateTimePicker)(((Control)sender))).Value = DateTime.Today;
                ((DateTimePicker)(((Control)sender))).Format = DateTimePickerFormat.Custom;
                ((DateTimePicker)(((Control)sender))).CustomFormat = " ";


            }
            else
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
        void Buscar()
        {
            try
            {

                if (desdearticulos)
                {
                    GeneralDT = hd.buscarxarticulo(id_articulo,id_tipo_movi);
                }else
                {
                    GeneralDT = hd.buscar(string.IsNullOrWhiteSpace(txtCodigo.Text) ? 0 : Convert.ToInt16(txtCodigo.Text), id_conductor, id_tipo_movi, !string.IsNullOrWhiteSpace(cbProvCli.Text) ? Convert.ToInt32(((System.Data.DataRowView)(cbProvCli.SelectedItem)).Row.ItemArray[0]) : 0, string.IsNullOrWhiteSpace(dtpFecha.Text) ? DateTimePicker.MinimumDateTime : dtpFecha.Value, string.IsNullOrWhiteSpace(dtpfechahasta.Text) ? DateTimePicker.MinimumDateTime : dtpfechahasta.Value);

                }
             

                if (GeneralDT.Rows.Count > 0)
                {

                    llenarGrid(GeneralDT);
                }
               else {

                    Base.msjAdvertencia("No se encontraron resultados");
                    if (desdearticulos)
                        Close();
                }

            }
            catch (Exception ex)
            {

                Base.msjError(ex.Message);
            }
         }
                
        void llenarGrid(DataTable dt)
        {
            dgvMovimientos.DataSource = new DataTable();
            dgvMovimientos.DataSource = dt;

            dgvMovimientos.Columns["id"].HeaderText = "Códgo";
            dgvMovimientos.Columns["observacion"].HeaderText = "Observacion";
            dgvMovimientos.Columns["fecha"].HeaderText = "Fecha";
            dgvMovimientos.Columns["Tercero"].HeaderText = entrada?"Proveedor":"Cliente";
            dgvMovimientos.Columns["id_tipo_mov"].Visible = false;
            dgvMovimientos.Columns["id_cli_prov"].Visible = false;
            dgvMovimientos.Columns["id_usuario"].Visible = false;
            dgvMovimientos.Columns["Conductor"].Visible = false;
            dgvMovimientos.Columns["cedula"].Visible = false;
            dgvMovimientos.Columns["eliminado"].Visible = false;
            dgvMovimientos.Columns["id_conductor"].Visible = false;


            llenarCampos(dt.Rows[0]);






        }

        void llenarCampos( DataRow fila)
        {

                txtCodigo.Enabled = false;
           txtCodigo.Text = fila["id"].ToString();
            cbProvCli.SelectedValue = fila["id_cli_prov"].ToString();
            txtObservacion.Text = !Equals(fila["observacion"], null) ? fila["observacion"] .ToString():string.Empty;
            dtpFecha.Value = Convert.ToDateTime(fila["fecha"]);
            anulado =Convert.ToBoolean( fila["eliminado"]);
            id_conductor = Convert.ToInt16(fila["id_conductor"]);
            txtNombreConductor.Text = fila["Conductor"].ToString();
            txtConductor.Text = fila["cedula"].ToString();
            btnAgregarDetalle.Enabled = true;
            btnQuitarDetalle.Enabled = true;
            txtNombreConductor.Enabled = false;
            txtConductor.Enabled = false;

            btnBuscarConductor.Text = "Cambiar";
            dtpFecha.Format = DateTimePickerFormat.Long;
            btnAnular.Text = anulado ?"Confirmar":"Anular";
            lbanulado.Text = anulado ? "Anulado" : string.Empty;
            btnGuardar.Enabled = !anulado;
           // btnGuardar.Enabled = anulado ? Base.administrador() : true;
            btnAnular.Visible = btnExportar.Visible=btnImprimir.Visible= true;
            btnExportar.Enabled = true;
            btnAnular.Enabled = anulado ? Base.administrador():true ;
            buscarDetalle();
            btnGuardar.Text = "Guardar";
            movimientoBo.codigo = Convert.ToInt16(fila["id"]);
            movimientoBo.conductor = txtNombreConductor.Text;
            movimientoBo.empresa = empresa;
            movimientoBo.fecha = dtpFecha.Value;
            movimientoBo.observacion = txtObservacion.Text;
            movimientoBo.proveedor = cbProvCli.Text;
            movimientoBo.usuario = Base.usuario;
            movimientoBo.direccion_empresa = direccionempresa;

            dtpfechahasta.Visible = false;
            lbfechahasta.Visible = false;

        }
       void  buscarDetalle()
        {
          try  {

             llenarDetalle(   hd.buscardetalle(Convert.ToInt16(txtCodigo.Text)));
                
           }
            catch (Exception ex)
            {

                Base.msjError(ex.Message);
            }
}




        void llenararticulos()
        {
            try
            {
                ArticulosData ad = new ArticulosData();
                DataTable resultadodt = new DataTable();
                if (entrada)
                {
                    resultadodt = ad.buscar(0, string.Empty, 0,
             DateTimePicker.MinimumDateTime);
                }else
                {
                    resultadodt = ad.buscararticulossalida();

                }
                
             

                if (resultadodt.Rows.Count > 0)
                {
                    cbArticulo.DataSource = resultadodt;

                    cbArticulo.DisplayMember = "Descripción";
                    cbArticulo.ValueMember = "id";





                }
                else
                {

                    Base.msjAdvertencia("No se encontraron articulos.");

                }


            }
            catch (Exception ex)
            {

            Base.msjError(ex.Message);
           }

        }

        void llenarEstatntes()
        {
            try
            {
                EstantesData ad = new EstantesData();
                DataTable resultadodt = ad.buscar(0,string.Empty);


                if (resultadodt.Rows.Count > 0)
                {
                    cbEstante.DataSource = resultadodt;

                    cbEstante.DisplayMember = "descripcion";
                    cbEstante.ValueMember = "id";





                }
                else
                {

                    Base.msjAdvertencia("No se encontraron Estantes.");

                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        #region Conductor
        private void btnBuscarConductor_Click(object sender, EventArgs e)
        {

            if (btnBuscarConductor.Text == "Cambiar")
            {
                txtConductor.Enabled = true;
                txtNombreConductor.Enabled = true;
                btnBuscarConductor.Text = "Buscar";
                txtConductor.Text = string.Empty;
                txtNombreConductor.Text = string.Empty;
                id_conductor = 0;
                return;
            }
            if (string.IsNullOrWhiteSpace(txtConductor.Text))
            {
                conductoresGridForm cgf = new conductoresGridForm(true);
                
                cgf.ShowDialog();
                txtConductor.Text = cgf.cedula;

                if (string.IsNullOrWhiteSpace(txtConductor.Text))
                    return;
            }
               
            try
            {
                conductoresData cd = new conductoresData();
                DataTable conductorDT = cd.buscar(Convert.ToDecimal(txtConductor.Text));
                if (conductorDT.Rows.Count > 0)
                {
                    int idTransporte = 0;
                    string cedula = string.Empty;
                    string nombre = string.Empty;
                    foreach (DataRow fila in conductorDT.Rows)
                    {
                        id_conductor = Convert.ToInt32(fila["id"]);
                        cedula = fila["cedula"].ToString();
                        nombre = fila["nombre"].ToString();
                        idTransporte = Convert.ToInt32(fila["id_transporte"]);
                    }

                    conductoresForm cf = new conductoresForm(id_conductor, cedula, nombre, idTransporte);
                    cf.actualizar = true;
                    cf.ShowDialog();
                    if (cf.eliminado)
                    {
                        txtConductor.Text = string.Empty;
                        txtNombreConductor.Text = string.Empty;

                        return;
                    }
                    if (cf.id > 0)
                    {
                        txtConductor.Text = cf.txtCedula.Text;
                        txtNombreConductor.Text = cf.txtNombre.Text;
                        txtNombreConductor.Enabled = false;
                        txtConductor.Enabled = false;
                        btnBuscarConductor.Text = "Cambiar";
                    }
                }
                else
                {

                    conductoresForm cf = new conductoresForm(txtConductor.Text);

                    cf.ShowDialog();
                    txtConductor.Text = cf.txtCedula.Text;
                    txtNombreConductor.Text = cf.txtNombre.Text;
                    if (cf.id != 0)
                    {
                       id_conductor = cf.id;
                        txtNombreConductor.Enabled = false;

                        txtConductor.Enabled = false;
                        btnBuscarConductor.Text = "Cambiar";
                    }

                }
                conductorDT.Dispose();


            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message); ;

            }
        }

        #endregion


        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                TercerosForm ud = new TercerosForm(true,entrada);
                if (entrada)
                    ud.Text = "Proveedor";
                else
                    ud.Text = "Usuario";

                ud.ShowDialog();
                llenarProvCli();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }
        void llenarProvCli()
        {

            try
            {
                Tercero_ArticulosDatacs cd = new Tercero_ArticulosDatacs();
                 productoDT = cd.buscar(id_tipo_movi);
                if (productoDT.Rows.Count > 0)
                {
                    cbProvCli.DataSource = productoDT;

                    cbProvCli.DisplayMember = "nombre";
                    cbProvCli.ValueMember = "id";

                }
                // productoDT.Dispose();
            }
            catch (Exception ex)
            {
                Base.msjError("Error llenado unidades: " + ex.Message);

            }
            cbProvCli.SelectedIndex = -1;
            cbProvCli.Text = string.Empty;

        }









        #region Eventos  Grid
        private void dgvDetalle_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {

                if (e.ColumnIndex == 1 && !entrada)
                {
                    cantidaTemporal(e);




                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void dgvDetalle_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            try
            {

                if (e.ColumnIndex == 1 || e.ColumnIndex == 2)
                {
                    calcularTotal(e);

                    if (e.ColumnIndex == 1 && !entrada)
                    {
                        int cantidad = Convert.ToInt32(dgvDetalle.Rows[e.RowIndex].Cells["txtCantidad"].Value);
                        if (cantidad > cantidadTemporal && !Convert.ToBoolean(dgvDetalle.Rows[e.RowIndex].Cells["filanueva"].Value))
                        {
                            int diferencia = cantidad - cantidadTemporal;
                            int exist = existencia(Convert.ToInt16(dgvDetalle.Rows[e.RowIndex].Cells["cbArticulo"].Value), Convert.ToInt16(dgvDetalle.Rows[e.RowIndex].Cells["id_seccion"].Value));
                            int resultado = (exist - diferencia);
                            if (resultado < 0)
                            {
                                dgvDetalle.Rows[e.RowIndex].Cells["txtCantidad"].Value = cantidadTemporal;
                                Base.msjError("Error no se permite inventario negativoe n Nro. " + (e.RowIndex + 1));
                            }

                        }
                        if (Convert.ToBoolean(dgvDetalle.Rows[e.RowIndex].Cells["filanueva"].Value))
                        {
                            if ((existencia(Convert.ToInt16(dgvDetalle.Rows[e.RowIndex].Cells["cbArticulo"].Value), Convert.ToInt16(dgvDetalle.Rows[e.RowIndex].Cells["id_seccion"].Value)) - cantidad) < 0)
                            {
                                dgvDetalle.Rows[e.RowIndex].Cells["txtCantidad"].Value = 0;
                                Base.msjError("Error no se permite inventario negativo");
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cantidaTemporal(DataGridViewCellCancelEventArgs e)
        {
            cantidadTemporal = 0;
             cantidadTemporal = Convert.ToInt32(dgvDetalle.Rows[e.RowIndex].Cells["txtCantidad"].Value);

            
        }

        private void calcularTotal(DataGridViewCellEventArgs e)
        {
            int cantidad = Convert.ToInt32(dgvDetalle.Rows[e.RowIndex].Cells["txtCantidad"].Value);
            Decimal precio = Convert.ToDecimal(dgvDetalle.Rows[e.RowIndex].Cells["txtPRecio"].Value);

            if (precio > 0 && cantidad > 0)
                dgvDetalle.Rows[e.RowIndex].Cells["txtTotal"].Value = precio * cantidad;
        }

        private void dgvDetalle_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvDetalle.CurrentCell.ColumnIndex == 1|| dgvDetalle.CurrentCell.ColumnIndex == 2)
            {

                TextBox txt = e.Control as TextBox;

                if (txt != null)
                {
                    txt.KeyPress -= Txt_KeyPress;
                    txt.KeyPress += Txt_KeyPress;
                }
              
            }
            if (dgvDetalle.CurrentCell.ColumnIndex == 3 || dgvDetalle.CurrentCell.ColumnIndex == 0)
            {
                ComboBox cbx = e.Control as ComboBox;

                if (cbx != null)
                {
                    cbx.SelectionChangeCommitted -= Cbx_SelectionChangeCommitted;
                    cbx.SelectionChangeCommitted += Cbx_SelectionChangeCommitted;
                    //seleccionoEstante(sender);
                }
            }

        }
  

        private void Cbx_SelectionChangeCommitted(object sender, EventArgs e)
        {

            try
            {
                if ( Equals( ((ListControl)sender).SelectedValue,null))
                 return;

                if(dgvDetalle.CurrentCell.ColumnIndex == 0)
                {


                    if (!entrada)
                    {
                        int id_articulo = Convert.ToInt32(((ListControl)sender).SelectedValue);
                        DataTable dtDatosArticulo = new DataTable();
                          dtDatosArticulo= hd.buscarpreciosxarticulos(id_articulo);
                        List<detalle_articuloBO> listaDatoArticulo = new List<detalle_articuloBO>();

                        int cantidadExistencia = 0;
                        decimal totalcostoentrada = decimal.Zero;

                        foreach (DataRow fila in dtDatosArticulo.Rows)
                        {
                          
                            detalle_articuloBO deta = new detalle_articuloBO();

                            deta.codigo = Convert.ToInt16(fila["id"]);
                            deta.descripcion = fila["descripcion"].ToString();
                           // deta.cantidad = Convert.ToInt16(fila["cantidad"]);
                             deta.estante = fila["Estante"].ToString();
                            deta.seccion = fila["Sección"].ToString();
                            deta.Precio = Convert.ToDecimal(fila["precio"]);
                          
                            deta.id_seccion = Convert.ToInt16(fila["id_seccion"]);
                            deta.id_estante = Convert.ToInt16(fila["id_estante"]);
                            deta.Total = existencia(deta.codigo,deta.id_seccion);

                            if (deta.Total > 0)
                            {
                                cantidadExistencia += Convert.ToInt16(fila["Total Entrada"]);
                                totalcostoentrada +=  Convert.ToInt16(fila["Total Entrada"]) * deta.Precio;
                            }

                            listaDatoArticulo.Add(deta);
                        }
                        if(cantidadExistencia>0)
                        {
                          decimal costoPromedio = Math.Round(totalcostoentrada / cantidadExistencia,2);
                          dgvDetalle.Rows[dgvDetalle.CurrentCell.RowIndex].Cells["txtPRecio"].Value = costoPromedio.ToString();
                        }else
                        {
                            Base.msjAdvertencia("Artículo sin Existencia");
                            return;

                        }
                       
                        //preciosForm sf = new preciosForm(listaDatoArticulo);
                        //sf.ShowDialog();
                
                       

                        EstantesData ed = new EstantesData();
                       
                        DataTable dt = ed.buscarxarticulo(Convert.ToInt32(((ListControl)sender).SelectedValue));
                        if (dt.Rows.Count == 0)
                            return;


                        llenarEstanteSalida(listaDatoArticulo, id_articulo);
                      
                        //llenarEstanteSalida(listaDatoArticulo.Where(p=>p.Precio==Convert.ToDecimal( sf.cbPrecios.Text)&& p.Total>0).ToList(),id_articulo);
                    }
                  

                }

                if (dgvDetalle.CurrentCell.ColumnIndex == 3)
                {

                    SeccionArticuloData ed = new SeccionArticuloData();
                    DataTable dt = ed.buscar(Convert.ToInt32(((ListControl)sender).SelectedValue));
                    if (dt.Rows.Count == 0)
                        return;
                    List<detalle_articuloBO> listaDatoArticulo = new List<detalle_articuloBO>();
                    foreach (DataRow fila in dt.Rows)
                    {

                        detalle_articuloBO deta = new detalle_articuloBO();

                        
                      
                        deta.seccion = fila["descripcion"].ToString();
                      
                        deta.id_seccion = Convert.ToInt16(fila["id"]);
                       



                        listaDatoArticulo.Add(deta);
                    }

                    llenarSecciones(listaDatoArticulo, 0);
                }

            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }
        }
        void llenarPrecios()
        {
            try
            {


            }
            catch (Exception ex)
            {
                Base.msjError("Error llenado precios: " + ex.Message);

            }
}
        void llenarEstanteSalida(List<detalle_articuloBO> dt,int prmid_articulo)
        {
            try
            {

                if (dt.Count > 0)
                {

                    EstanteForm sf = new EstanteForm(dt.Where(p => p.Total > 0).GroupBy(r => new { r.id_estante, r.estante }).Select(r => r.First()).ToList());
                    sf.ShowDialog();
                    dgvDetalle.Rows[dgvDetalle.CurrentCell.RowIndex].Cells["txtestante"].Value = sf.cbEstante.Text;
                    dgvDetalle.Rows[dgvDetalle.CurrentCell.RowIndex].Cells["cbEstante"].Value = sf.cbEstante.SelectedValue;


                    SeccionArticuloData sd = new SeccionArticuloData();
                    //DataTable dts = new DataTable();
                    
               //  dts=   sd.buscarxarticulo(prmid_articulo,Convert.ToInt16( sf.cbEstante.SelectedValue));

                    if (dt.Count > 0)
                    {
                       
                        llenarSecciones(dt.Where(r=>r.id_estante== Convert.ToInt16( sf.cbEstante.SelectedValue) && r.Total>0).GroupBy(r => new { r.id_seccion,r.seccion }).Select(r=>r.First()).ToList(), prmid_articulo);
                    }


                }
                // productoDT.Dispose();
            }
            catch (Exception ex)
            {
                Base.msjError("Error llenado secciones: " + ex.Message);

            }



        }
        void llenarSecciones(List<detalle_articuloBO> dt,int prm_idArt)
        {
            try
            {
               
                if (dt.Count > 0)
                {


                    SeccionForm sf = new SeccionForm(dt);
                    sf.ShowDialog();

                    dgvDetalle.Rows[dgvDetalle.CurrentCell.RowIndex].Cells["cbSeccion"].Value = sf.cbseccion.Text ;
                    dgvDetalle.Rows[dgvDetalle.CurrentCell.RowIndex].Cells["id_seccion"].Value = sf.cbseccion.SelectedValue;
                    if(!entrada)
                    dgvDetalle.Rows[dgvDetalle.CurrentCell.RowIndex].Cells["txtExistencia"].Value= existencia(prm_idArt, Convert.ToInt16(sf.cbseccion.SelectedValue));

                }
                // productoDT.Dispose();
            }
            catch (Exception ex)
            {
                Base.msjError("Error llenado secciones: " + ex.Message);

            }
       


        }
      
        private void btnQuitarDetalle_Click(object sender, EventArgs e)
        {
            if (dgvDetalle.Rows.Count == 0)
                return;

            if (Base.msjPregunta("¿Desea realmente eliminar el detalle de la fila número: " + (dgvDetalle.CurrentCell.RowIndex + 1) + "?") == DialogResult.Yes)
            {
                DataGridViewRow fila = dgvDetalle.Rows[dgvDetalle.CurrentCell.RowIndex];
                if ( !Convert.ToBoolean( fila.Cells["filanueva"].Value))
                {
                   int id = Convert.ToInt32(fila.Cells["id"].Value);
                    int  prmcantidad = Convert.ToInt32(fila.Cells["txtCantidad"].Value);
                 int   pmrid_articulo = Convert.ToInt32(fila.Cells["cbArticulo"].Value);
                   decimal  prmiprecio = Convert.ToDecimal(fila.Cells["txtPRecio"].Value);
                   int  pmrid_seccion = Convert.ToInt32(fila.Cells["id_seccion"].Value);

                    hd.actualzardetalle(id, prmcantidad,prmiprecio, pmrid_articulo, pmrid_seccion,Convert.ToInt16( txtCodigo.Text),true,id_usuario);
                }
               
                dgvDetalle.Rows.RemoveAt(dgvDetalle.CurrentCell.RowIndex);
                //if(dgvDetalle.RowCount>0)
                //dgvDetalle.Rows[dgvDetalle.CurrentCell.RowIndex].Cells["eliminado"].Value = true;
            }
        }
        private void btnAgregarDetalle_Click(object sender, EventArgs e)
        {

            if (!validarGridVacio())
                return;

            dgvDetalle.Rows.Insert(dgvDetalle.Rows.Count, new DataGridViewRow());
            dgvDetalle.Rows[dgvDetalle.Rows.Count - 1].Cells["filanueva"].Value = true;
            dgvDetalle.Rows[dgvDetalle.Rows.Count - 1].Cells["eliminado"].Value = false;
            dgvDetalle.Rows[dgvDetalle.Rows.Count - 1].Cells["id"].Value = 0;
            dgvDetalle.Rows[dgvDetalle.Rows.Count - 1].Cells["txtExistencia"].Value = 0;
            dgvDetalle.Rows[dgvDetalle.Rows.Count - 1].Cells["txtestante"].Value = 0;
            dgvDetalle.Rows[dgvDetalle.Rows.Count - 1].Cells["id_seccion"].Value = 0;


        }
        bool  validarGridVacio()
        {
            foreach (DataGridViewRow fila in dgvDetalle.Rows)
            {
                for (int i = 0; i < fila.Cells.Count; i++)
                {
                    if (Equals(fila.Cells[i].Value, null))
                    {
                        Base.msjAdvertencia("Debe completar la totalidad del detalle de la fila Nro. " + (fila.Index + 1));

                        return false;
                    }
                    else
                    {
                        if(Convert.ToInt16( fila.Cells["txtCantidad"].Value)==0)
                        {
                            Base.msjAdvertencia("No se permite ingresar cantidad 0 en la fila Nro. " + (fila.Index + 1));

                            return false;
                        }
                       

                    }

                }



            }
            return true;
        }

        private void dgvUserDetails_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(dgvDetalle.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4);
            }
        }
        private void dgvDetalle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)

            {
                e.SuppressKeyPress = true;
                
            }
        }
        private void Txt_KeyPress(object sender, KeyPressEventArgs e)
        {

            //Para obligar a que sólo se introduzcan números 
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                 if (Char.IsControl(e.KeyChar) || e.KeyChar == 44) //permitir teclas de control como retroceso 
            {
                e.Handled = false;
            }

            else
            {
                //el resto de teclas pulsadas se desactivan 
                e.Handled = true;
            }
        }

        #endregion

        #region Eveentos
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

        private void txtConductor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                btnBuscarConductor.PerformClick();
            }
        }

     

        private void btnEditarSilos_Click(object sender, EventArgs e)
        {

            try
            {

                if (string.IsNullOrWhiteSpace(cbProvCli.Text))
                    return;

                TercerosForm ud = new TercerosForm(false, entrada);
                DataRow[] fila = productoDT.Select("id=" + Convert.ToInt32(((System.Data.DataRowView)(cbProvCli.SelectedItem)).Row.ItemArray[0]));
                if (fila.Count() == 0)
                    return;
                ud.txtCodigo.Text = fila[0]["id"].ToString();
                ud.txtdescripcion.Text = fila[0]["nombre"].ToString();
                ud.txtCedula.Text = !Equals(fila[0]["cedula"], null) ? fila[0]["cedula"].ToString() : string.Empty;
                ud.txtRif.Text = !Equals(fila[0]["rif"], null) ? fila[0]["rif"].ToString() : string.Empty;
                ud.txtTelefono.Text = !Equals(fila[0]["telefono"], null) ? fila[0]["telefono"].ToString() : string.Empty;
                ud.txtDireccion.Text = !Equals(fila[0]["direccion"], null) ? fila[0]["direccion"].ToString() : string.Empty;
                ud.btnEliminar.Enabled = true;


                if (entrada)
                    ud.Text = "Proveedor";
                else
                    ud.Text = "Usuario";

                ud.ShowDialog();
                llenarProvCli();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion


        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (btnGuardar.Text == "Guardar")
            {

                if (!validarGridVacio())
                    return;

                if (string.IsNullOrWhiteSpace(txtConductor.Text))
                {

                    Base.msjAdvertencia("Debe llenar campo conductor");
                    return;

                }
            

                if (string.IsNullOrWhiteSpace(cbProvCli.Text))
                {

                    Base.msjAdvertencia("Debe llenar campo " + terceros);
                    return;

                }


                if (nuevo)
                {


                    DataRow drResultado = hd.insertar(id_tipo_movi, Convert.ToInt32(((System.Data.DataRowView)(cbProvCli.SelectedItem)).Row.ItemArray[0]), txtObservacion.Text, id_usuario, id_conductor, dtpFecha.Value).Rows[0];

                    if (drResultado.ItemArray.Count() > 0)
                    {
                        txtCodigo.Text = drResultado["id"].ToString();

                    }



                }
                else
                {
                    // validar(false);
                    hd.actualizar(Convert.ToInt32(txtCodigo.Text), id_tipo_movi, Convert.ToInt32(((System.Data.DataRowView)(cbProvCli.SelectedItem)).Row.ItemArray[0]), txtObservacion.Text, id_usuario, id_conductor, dtpFecha.Value, anulado);




                }
                btnAgregarDetalle.Enabled = btnQuitarDetalle.Enabled = true;

                #region Guardar Detalle
                if (!nuevo)
                {
                    detalleDT = new DataTable();
                    detalleDT = hd.insertardetalle(dgvDetalle, Convert.ToInt32(txtCodigo.Text),id_usuario);

                    llenarDetalle(detalleDT);

                }
                #endregion
                Base.msjExito("Registro guardado exitosamente");
            }
            Buscar();
           
            nuevo = false;

        
           // llenarGrid();
        }
        void guardar()
        {



        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAnular_Click(object sender, EventArgs e)
        {
            try
            {
                string msj = anulado ?"confirmar":"anular";

                if (Base.msjPregunta("¿Desea realmente "+msj+" el movimiento codigo " + txtCodigo.Text + "?") == DialogResult.Yes)
                {
                    if (!nuevo)
                    {
                        txtObservacion.Text = " Movimiento anulado la fecha " + DateTime.Now.ToLongDateString() + " por el usuario " + Base.usuario; ;
                        anulado = !anulado;
                        btnGuardar.Enabled = true; 
                        btnGuardar.PerformClick();
                      

                    }
                    nuevo = false;
                 
                }

            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
        }

        private void dgvMovimientos_CellDoubleClick(object sender, EventArgs e)
        {
         
            var seleccionado = Convert.ToInt32(dgvMovimientos.SelectedRows[0].Cells[0].Value);

            DataRow[] rowseleccionado = GeneralDT.Select("id=" + seleccionado);
            llenarCampos(rowseleccionado[0]);
        }

        private void dgvDetalle_DoubleClick(object sender, EventArgs e)
        {

        }
        void abrirARticulos(bool nuevo,int id_articulo)
        {

            try
            {
                Cursor = Cursors.WaitCursor;
                ArticulosForm pf = new ArticulosForm(nuevo, id_usuario);
                pf.txtCodigo.Text = id_articulo.ToString();
                pf.desdeMovimientos = true;
                pf.ShowDialog();
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void MovimientoArticulosForm_Load(object sender, EventArgs e)
        {
            if (desdearticulos)
                Buscar();
        }

        private void dgvDetalle_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {

                    

                    ContextMenuStrip menu = new ContextMenuStrip();
                    menu.Items.Add("Ver Artículo").Name = "articulo";

                    menu.Click += Menu_Click;

                    //Obtienes las coordenadas de la celda seleccionada. 
                    Rectangle coordenada = dgvDetalle.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                    int anchoCelda = coordenada.Location.X; //Ancho de la localizacion de la celda
                    int altoCelda = coordenada.Location.Y;  //Alto de la localizacion de la celda

                    //Y para mostrar el menú lo haces de esta forma:  
                    int X = anchoCelda + dgvDetalle.Location.X;
                    int Y = altoCelda + dgvDetalle.Location.Y;

                    menu.Show(Cursor.Position);
                }

               
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
        }

        private void Menu_Click(object sender, EventArgs e)
        {
            if (dgvDetalle.RowCount == 0 || Equals(dgvDetalle.SelectedRows[0].Cells[0].Value, null) || desdearticulos)
                return;
            var seleccionado = Convert.ToInt32(dgvDetalle.SelectedRows[0].Cells[0].Value);

            abrirARticulos(false, seleccionado);
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                listaDetalleR = new List<detalle_articuloBO>();

                foreach(DataGridViewRow fila in dgvMovimientos.Rows)
                {



                    llenarDetalleR(hd.buscardetalle(Convert.ToInt16(fila.Cells["id"].Value)));  

                }
           


                List<detalle_articuloBO> lista = listaDetalleR
                             .GroupBy(l => new { l.codigo,l.id_seccion})
                             .Select(cl => new detalle_articuloBO
                             {
                                  codigo = cl.First().codigo,
                                  descripcion = cl.First().descripcion.ToString(),
                                  cantidad = cl.Sum(c => c.cantidad),
                                  Precio=cl.First().Precio,
                                  estante= cl.First().estante.ToString(),
                                 seccion = cl.First().seccion.ToString()
                             }).OrderBy(s=>s.codigo).ToList();
         reporteForm rf = new reporteForm(new List<analisis>());
                rf.inventario = true;
                rf.listaDetalle = lista;
                List<MovimientoBO> listaMovimiento = new List<MovimientoBO>();
              
                listaMovimiento.Add(movimientoBo);

                rf.rvReporte.ProcessingMode = ProcessingMode.Local;



                if (entrada)
                    rf.rvReporte.LocalReport.ReportEmbeddedResource = "principal.movimientoEntradaR.rdlc";
                else
                    rf.rvReporte.LocalReport.ReportEmbeddedResource = "principal.movSalidaR.rdlc";
                ReportDataSource rds = new ReportDataSource("boleta", listaMovimiento);
                rf.rvReporte.LocalReport.DataSources.Clear();

                rf.rvReporte.LocalReport.DataSources.Add(rds);

                rf.rvReporte.LocalReport.Refresh();
                rf.ShowDialog();

            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
        }

        void llenarDetalle(DataTable dt)
        {
           // dgvDetalle.DataSource = dt;
            int i = 0;
            listaDetalle = new List<detalle_articuloBO>();
            dgvDetalle.Rows.Clear();
            foreach(DataRow fila in dt.Rows)
            {
                dgvDetalle.Rows.Insert(dgvDetalle.Rows.Count, new DataGridViewRow());
                detalle_articuloBO deta = new detalle_articuloBO();

                dgvDetalle.Rows[i].Cells["id"].Value= fila["id"];
                deta.codigo = Convert.ToInt16( fila["id_articulo"]);
               dgvDetalle.Rows[i].Cells["filanueva"].Value = false;
                dgvDetalle.Rows[i].Cells["cbArticulo"].Value = fila["id_articulo"];

                deta.descripcion = fila["articulo"].ToString();
                dgvDetalle.Rows[i].Cells["txtCantidad"].Value = fila["cantidad"] ;

                deta.cantidad = Convert.ToInt16( fila["cantidad"]);
                deta.estante = fila["estante"].ToString();
                deta.seccion = fila["seccion"].ToString();
               deta.Precio=Convert.ToDecimal( fila["precio"]);
                dgvDetalle.Rows[i].Cells["cbEstante"].Value = fila["id_estante"];
                dgvDetalle.Rows[i].Cells["txtPRecio"].Value = fila["precio"];
                dgvDetalle.Rows[i].Cells["cbSeccion"].Value = fila["seccion"];
                dgvDetalle.Rows[i].Cells["id_seccion"].Value = fila["id_seccion"];
                dgvDetalle.Rows[i].Cells["eliminado"].Value = fila["eliminado"];
                dgvDetalle.Rows[i].Cells["txtTotal"].Value =Convert.ToInt32( fila["cantidad"]) * Convert.ToDecimal(fila["precio"]);
               
                    dgvDetalle.Rows[i].Cells["txtestante"].Value = fila["estante"].ToString();
                    dgvDetalle.Rows[dgvDetalle.Rows.Count - 1].Cells["txtExistencia"].Value = existencia(Convert.ToInt16( fila["id_articulo"]), Convert.ToInt16(fila["id_seccion"]));

             
                i++;
                listaDetalle.Add(deta);
            }




        }


        void llenarDetalleR(DataTable dt)
        {
           
            int i = 0;
           // dgvDetalle.Rows.Clear();
            //listaDetalleR = new List<detalle_articuloBO>();
            foreach (DataRow fila in dt.Rows)
            {
               
                detalle_articuloBO deta = new detalle_articuloBO();

                deta.codigo = Convert.ToInt16(fila["id_articulo"]);
                deta.descripcion = fila["articulo"].ToString();
                deta.cantidad = Convert.ToInt16(fila["cantidad"]);
                deta.estante = fila["estante"].ToString();
                deta.seccion = fila["seccion"].ToString();
                deta.id_seccion =Convert.ToInt32( fila["id_seccion"]);
                deta.Precio= Convert.ToDecimal(fila["precio"]);
                i++;
                listaDetalleR.Add(deta);
            }




        }
    }
}
