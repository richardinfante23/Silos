using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace principal
{
  
    public partial class DespachoForm : Form
    {
        //recepcionData rd = new recepcionData();
        despachoData dd = new despachoData();
        public int id_perfil;
        public int id_usuario;
        public bool supervisor=false;
        public string nombre_Usuario;
        despacho objRecepcion = new despacho();
        List<string> listaLetras = new List<string>();
        DateTime horadefecto = new DateTime(1, 1, 1, 0, 0, 0);
        private readonly string letrasJuridicas = ConfigurationManager.AppSettings["letrasJuridicas"];
        private readonly string letrasNaturales = ConfigurationManager.AppSettings["letrasNaturales"];
        private readonly string empresa = ConfigurationManager.AppSettings["Empresa"];
        private readonly string direccionempresa = ConfigurationManager.AppSettings["direccionEmpresa"];
        public bool nuevo = true;
        DataTable resultadodt = new DataTable();
        List<analisis> listaAnalisis = new List<analisis>();
        int id_producto_original = 0;
        bool fechahasta = false;
        bool analisis = false;
        DataTable silosDT = new DataTable();
        bool desdeBoton = false;
        bool _omitirAdvertenciaInventario = false;
        readonly historicoData _historicoData = new historicoData();
        public DespachoForm()
        {
            InitializeComponent();
            AplicarUiDespachoModerna();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-VE");
            Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;

            cargarPaso();
            cargarProducto();
            cargarSilos();
            SincronizarCombosFiltroListado();


            dtpHoraentrada.Format = DateTimePickerFormat.Custom;
            dtpHoraentrada.CustomFormat = " ";

            dtpHoraSalida.Format = DateTimePickerFormat.Custom;
            dtpHoraSalida.CustomFormat = " ";
            formatoFecha();
            
        }

        void cargarAnalisis()
        {

            try
            {
              
                dgvAnalisis.DataSource = new DataTable();
                DataTable analisisDT = dd.buscaranalisis(objRecepcion.id);
                if (dd.buscaranalisis(objRecepcion.id).Rows.Count == 0)
                {
                    analisisDT = dd.buscarItemanalisxodproducto(objRecepcion.id_producto, objRecepcion.id);
                    analisis = false;
                    objRecepcion.listaAnalisis = new List<analisis>();
                }
                else
                {
                    analisis = true;
                   
                    if (objRecepcion.id_producto <=3)
                    {
                        objRecepcion.listaAnalisis = new List<analisis>();
                        analisis objAnalisis = new analisis();
                        foreach (DataRow fila in analisisDT.Rows)
                        {
                            objAnalisis = new analisis();
                            fila["valor_resultado"] = Math.Round(Convert.ToDecimal(fila["valor_resultado"]), 2);

                            
                            objAnalisis.nombre = fila["nombre"].ToString();
                            objAnalisis.valor_resultado = Convert.ToDecimal(fila["valor_resultado"]);
                            objRecepcion.listaAnalisis.Add(objAnalisis);


                        }
                        
                    }

                    btnEliminarAnalisis.Visible = true;
                }
                if (analisisDT.Rows.Count > 0)
                {
                    dgvAnalisis.DataSource = analisisDT;
                    ConfigurarColumnasAnalisis();
                    dgvAnalisis.Columns["nombre"].ReadOnly = true;
                    dgvAnalisis.Columns["menor"].ReadOnly = true;
                    dgvAnalisis.Columns["mayor"].ReadOnly = true;
                    if (analisis)
                    {
                        //txtObservaciones.Text = string.Empty;


                        lbAdvertencia.Text = string.Empty; ;

                        foreach (DataGridViewRow row in dgvAnalisis.Rows)
                        {
                            if (Convert.ToDecimal(row.Cells["valor_resultado"].Value) < Convert.ToDecimal(row.Cells["menor"].Value) || Convert.ToDecimal(row.Cells["valor_resultado"].Value) > Convert.ToDecimal(row.Cells["mayor"].Value))
                            {
                                row.DefaultCellStyle.BackColor = Color.Cyan;
                                row.DefaultCellStyle.ForeColor = Color.Black;
                                lbAdvertencia.Text += row.Cells["nombre"].Value.ToString() + "\n";
                            }
                        }

                        //txtObservaciones.Text = "\n Los Siguiente resultados estan fuera de los umbrales permitidos:\n" + lbAdvertencia.Text; 
                        if (!string.IsNullOrWhiteSpace(lbAdvertencia.Text))
                            lbAdvertencia.Text = "Los Siguiente resultados estan fuera de los umbrales permitidos:\n" + lbAdvertencia.Text;

                    }

                }
                // productoDT.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando Conductor. Error Contacte al Administrador del Sistema", ex);

            }



        }

        private void DespachoForm_Load(object sender, EventArgs e)
        {
            if (this.MdiParent != null)
                this.WindowState = FormWindowState.Maximized;

            ActualizarPasosDespacho();
            cbProducto.SelectedItem = null;
            if (nuevo)
            {
                rbTipoNatural.Checked = true;
                AplicarModoDespacho(false);
            }
            if (!nuevo)
            {
                cbEstatus.Enabled = true;
                cbEstatus.SelectedItem = null;
                cbSilos.SelectedItem = null;
                cbEstatus.Text = string.Empty;
                cbSilos.Text = string.Empty;
                rbtFechaHasta.Visible = true;
                AplicarModoDespacho(true);
            }

            AplicarModoPesajeSegunPerfil();
            AplicarPoliticaFechaDocumento();
        }

        /// <summary>
        /// Fecha documento: formato dd/MM/yyyy; solo administrador puede modificarla.
        /// No admin: deshabilitada; en nuevo carga hoy.
        /// </summary>
        private void AplicarPoliticaFechaDocumento()
        {
            formatoFecha();
            if (nuevo)
                dtpFecha.Value = DateTime.Today;

            bool puedeEditar = Base.EsAdministrador();
            if (objRecepcion != null && (objRecepcion.id_estatus == 5 || objRecepcion.id_estatus == 6))
                puedeEditar = false;
            dtpFecha.Enabled = puedeEditar;
        }

        private void AplicarModoPesajeSegunPerfil()
        {
            bool puedeManual = supervisor && !Base.EsRomana();
            if (rbManual != null)
                rbManual.Visible = puedeManual;

            if (!puedeManual)
            {
                if (rbAutomatic != null)
                    rbAutomatic.Checked = true;
                if (rbManual != null)
                    rbManual.Checked = false;
            }

            ActualizarModoPesaje();
        }

        void cargarPaso()
        {
            try
            {

                DataTable productoDT = dd.cargarPasos();
                if (productoDT.Rows.Count > 0)
                {
                    cbEstatus.DataSource = productoDT;

                    cbEstatus.DisplayMember = "nombre";
                    cbEstatus.ValueMember = "id";

                }
                // productoDT.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando Conductor. Error Contacte al Administrador del Sistema", ex);

            }


        }



        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        #region Producto
        void cargarProducto()
        {
            try
            {
                productoData cd = new productoData();
                DataTable productoDT = cd.buscar();
                if (productoDT.Rows.Count > 0)
                {
                    cbProducto.DataSource = productoDT;

                    cbProducto.DisplayMember = "nombre";
                    cbProducto.ValueMember = "id";

                }
                // productoDT.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando Conductor. Error Contacte al Administrador del Sistema", ex);

            }


        }
        #endregion

        #region Silos
        void cargarSilos()
        {
            try
            {
                _omitirAdvertenciaInventario = true;
                silosData cd = new silosData();
                 silosDT = cd.buscar();
                if (silosDT.Rows.Count > 0)
                {
                    cbSilos.DataSource = silosDT;

                    cbSilos.DisplayMember = "nombre";
                    cbSilos.ValueMember = "id";

                }
                // productoDT.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando Conductor. Error Contacte al Administrador del Sistema", ex);

            }
            finally
            {
                _omitirAdvertenciaInventario = false;
            }


        }
        #endregion

        #region Buscar Conductor
        private void btnBuscarConductor_Click(object sender, EventArgs e)
        {

            if (btnBuscarConductor.Text == "Cambiar")
            {
                txtConductor.Enabled = true;
                txtNombreConductor.Enabled = true;
                btnBuscarConductor.Text = "Buscar";
                txtConductor.Text = string.Empty;
                txtNombreConductor.Text = string.Empty;
                objRecepcion.id_conductor = 0;
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
                        objRecepcion.id_conductor = Convert.ToInt32(fila["id"]);
                        cedula = fila["cedula"].ToString();
                        nombre = fila["nombre"].ToString();
                        idTransporte = Convert.ToInt32(fila["id_transporte"]);
                    }


                    if (objRecepcion.id_conductor > 0)
                    {
                        txtConductor.Text = cedula;
                        txtNombreConductor.Text = nombre;
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
                        objRecepcion.id_conductor = cf.id;
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


        private bool validateRegistrarOrEditar(string etiqueta)
        {
            return ValidacionFormulario.validar(this.Controls, etiqueta);
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
        public void soloNumerosconcoma_KeyPress(object sender, KeyPressEventArgs e)
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
        private void soloLetras_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Para obligar a que sólo se introduzcan letras 
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
                if (Char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso 
                {
                    e.Handled = false;
                }
                else
                {
                    //el resto de teclas pulsadas se desactivan 
                    e.Handled = false;
                }
        }

        private void dtpHoraentrada_KeyDown(object sender, KeyEventArgs e)
        {
            #region Borrar
            if (e.KeyValue == (char)Keys.Back)
            {

                if (((Control)sender).Name == "dtpHoraentrada")
                {
                    dtpHoraentrada.Value = DateTime.Now;
                    dtpHoraentrada.Format = DateTimePickerFormat.Custom;
                    dtpHoraentrada.CustomFormat = " ";
                }

                if (((Control)sender).Name == "dtpHoraSalida")
                {
                    dtpHoraSalida.Value = DateTime.Now;
                    dtpHoraSalida.Format = DateTimePickerFormat.Custom;
                    dtpHoraSalida.CustomFormat = " ";
                }
                if (((Control)sender).Name == "dtpFecha")
                {
                    if (!Base.EsAdministrador())
                        return;
                    dtpFecha.Value = DateTime.Today;
                    formatoFecha();
                }
                if (((Control)sender).Name == "dtpFechaHasta")
                {
                    dtpFechaHasta.Value = DateTime.Now;
                    dtpFechaHasta.Format = DateTimePickerFormat.Custom;
                    dtpFechaHasta.CustomFormat = " ";
                }
            }
            else
            {
                if (((Control)sender).Name == "dtpHoraentrada")
                    formatoHoraentrada();
                if (((Control)sender).Name == "dtpHoraSalida")
                    formatoHoraSalida();
                if (((Control)sender).Name == "dtpFecha")
                    formatoFecha();
                if (((Control)sender).Name == "dtpFechaHasta")
                    formatoFechaHasta();
            }
           
            #endregion
        }

        private void formatoHoraSalida()
        {
            //if (string.IsNullOrWhiteSpace(dtpHoraSalida.Text))
            //    dtpHoraSalida.Value = DateTime.Now;
            dtpHoraSalida.Format = DateTimePickerFormat.Custom;
            dtpHoraSalida.CustomFormat = "dd/MM/yyyy hh:mm tt";
        }

        private void formatoHoraentrada()
        {
            
            dtpHoraentrada.Format = DateTimePickerFormat.Custom;
            dtpHoraentrada.CustomFormat = "dd/MM/yyyy hh:mm tt";
            //if (string.IsNullOrWhiteSpace(dtpHoraentrada.Text.Trim()))
            //    dtpHoraentrada.Value = DateTime.Now;
        }
        private void formatoFecha()
        {
        //    if (string.IsNullOrWhiteSpace(dtpFecha.Text))
        //        dtpFecha.Value = DateTime.Now;
            dtpFecha.Format = DateTimePickerFormat.Custom;
            dtpFecha.CustomFormat = "dd/MM/yyyy";
        }
        private void formatoFechaHasta()
        {
            //if (string.IsNullOrWhiteSpace(dtpFechaHasta.Text))
            //    dtpFechaHasta.Value = DateTime.Now;
            dtpFechaHasta.Format = DateTimePickerFormat.Custom;
            dtpFechaHasta.CustomFormat = "dd/MM/yyyy";
        }

        #region Productor
        private void rbJuridica_CheckedChanged(object sender, EventArgs e)
        {
            listaLetras = new List<string>();
            listaLetras.AddRange(letrasJuridicas.Split(','));
            cbLetras.Enabled = true;
            txtDIProductor.Enabled = true;
            txtNombreProductor.Enabled = true;
            txtDIProductor.Text = string.Empty;
            txtNombreProductor.Text = string.Empty;
            objRecepcion.id_destino = 0;
            btnBuscarProductor.Text = "Buscar";

            lbEjemploletraConductor.Text = "Ejem: G";
            lbEjemploCedulaCondcutor.Text = "Ejem: 123456789";
            cbLetras.DataSource = new DataTable();
            cbLetras.DataSource = listaLetras;

        }


        private void btnBuscarProductor_Click(object sender, EventArgs e)
        {
            if (btnBuscarProductor.Text == "Cambiar")
            {
                txtDIProductor.Enabled = true;
                txtNombreProductor.Enabled = true;
                btnBuscarProductor.Text = "Buscar";
                EstilarBotonCampo(btnBuscarProductor, "Buscar");
                txtDIProductor.Text = string.Empty;
                txtNombreProductor.Text = string.Empty;
                objRecepcion.id_destino = 0;
                objRecepcion.id_destino_natural = 0;
                cbLetras.Enabled = true;
                return;
            }

            // Sin cédula/RIF: listado de clientes (igual que productor en recepción)
            if (string.IsNullOrWhiteSpace(txtDIProductor.Text))
            {
                try
                {
                    string tipo = null;
                    if (rbTipoNatural.Checked)
                        tipo = "Natural";
                    else if (rbJuridica.Checked)
                        tipo = "Jurídico";

                    using (var picker = new productorPickerForm(tipo, CatalogoPersonaModo.Cliente))
                    {
                        if (picker.ShowDialog(this) == DialogResult.OK && picker.Seleccionado)
                            AplicarClienteDesdePicker(picker);
                    }
                }
                catch (Exception ex)
                {
                    Base.msjErrorOperacion(ex, "listar clientes");
                }
                return;
            }

            if (!rbTipoNatural.Checked && !rbJuridica.Checked)
            {
                UiDialog.Show("Debe seleccionar Persona Natural o Persona Jurídica antes de buscar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rbTipoNatural.Checked = true;
                rbJuridica.Checked = false;
                rbTipoNatural_CheckedChanged(rbTipoNatural, EventArgs.Empty);
                return;
            }

            if (!validateRegistrarOrEditar("validarcedula"))
                return;

            try
            {
                if (rbTipoNatural.Checked)
                    productorNatural();
                else
                    productorJuridico();

            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando productor. Error Contacte al Administrador del Sistema", ex);

            }
        }

        private void AplicarClienteDesdePicker(productorPickerForm picker)
        {
            if (picker == null || !picker.Seleccionado)
                return;

            bool esNatural = picker.IdNatural > 0;

            this.rbTipoNatural.CheckedChanged -= new System.EventHandler(this.rbTipoNatural_CheckedChanged);
            this.rbJuridica.CheckedChanged -= new System.EventHandler(this.rbJuridica_CheckedChanged);
            try
            {
                rbTipoNatural.Checked = esNatural;
                rbJuridica.Checked = !esNatural;

                listaLetras = new List<string>();
                if (esNatural)
                    listaLetras.AddRange(letrasNaturales.Split(','));
                else
                    listaLetras.AddRange(letrasJuridicas.Split(','));

                cbLetras.DataSource = new DataTable();
                cbLetras.DataSource = listaLetras;

                if (!string.IsNullOrWhiteSpace(picker.Letra))
                {
                    string letra = picker.Letra.Trim();
                    string match = listaLetras.Find(x => string.Equals(x.Trim(), letra, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                        cbLetras.SelectedItem = match;
                }

                if (esNatural)
                {
                    lbEjemploletraConductor.Text = "Ejem: V";
                    lbEjemploCedulaCondcutor.Text = "Ejem: 12345678";
                    objRecepcion.id_destino_natural = picker.IdNatural;
                    objRecepcion.id_destino = 0;
                }
                else
                {
                    lbEjemploletraConductor.Text = "Ejem: G";
                    lbEjemploCedulaCondcutor.Text = "Ejem: 123456789";
                    objRecepcion.id_destino = picker.IdJuridico;
                    objRecepcion.id_destino_natural = 0;
                }

                txtDIProductor.Text = picker.Documento ?? string.Empty;
                txtNombreProductor.Text = picker.Nombre ?? string.Empty;
                txtDIProductor.Enabled = false;
                txtNombreProductor.Enabled = false;
                cbLetras.Enabled = false;
                btnBuscarProductor.Text = "Cambiar";
                EstilarBotonCampo(btnBuscarProductor, "Cambiar");
            }
            finally
            {
                this.rbTipoNatural.CheckedChanged += new System.EventHandler(this.rbTipoNatural_CheckedChanged);
                this.rbJuridica.CheckedChanged += new System.EventHandler(this.rbJuridica_CheckedChanged);
            }
        }
        private void productorJuridico()
        {
            try
            {
                destinoData dd = new destinoData();
                DataTable productorDT = dd.buscarPersonaJuridica(cbLetras.SelectedItem.ToString(), Convert.ToDecimal(txtDIProductor.Text));
                if (productorDT.Rows.Count > 0)
                {


                    personaJuridicaForm cf = new personaJuridicaForm();
                    cf.destino = true;
                    listaLetras = new List<string>();
                    listaLetras.AddRange(letrasJuridicas.Split(','));
                    cf.cbLetra.DataSource = listaLetras;
                    foreach (DataRow fila in productorDT.Rows)
                    {
                        objRecepcion.id_destino = Convert.ToInt32(fila["id"]);
                        cf.txtNombre.Text = fila["nombre"].ToString();
                        cf.txtRif.Text = fila["rif"].ToString();
                        cf.cbLetra.SelectedItem = cbLetras.SelectedItem;
                        cf.cbAsociaciones.SelectedItem = fila["id_asociacion"].ToString();
                        cf.id = (int)objRecepcion.id_destino;

                    }
                    cf.btnEliminar.Visible = true;
                    cf.actualizar = true;
                    cf.ShowDialog();
                    if (cf.eliminado)
                    {

                        txtDIProductor.Text = string.Empty;
                        txtNombreProductor.Text = string.Empty;

                        return;
                    }
                    if (cf.id > 0)
                    {
                        txtDIProductor.Text = cf.txtRif.Text;
                        txtNombreProductor.Text = cf.txtNombre.Text;
                        txtDIProductor.Enabled = false;
                        cbLetras.SelectedItem = cf.cbLetra.SelectedItem;
                        txtNombreProductor.Enabled = false;
                        cbLetras.Enabled = false;
                        btnBuscarProductor.Text = "Cambiar";
                    EstilarBotonCampo(btnBuscarProductor, "Cambiar");
                        objRecepcion.id_destino = cf.id;
                    }
                }
                else
                {

                    personaJuridicaForm cf = new personaJuridicaForm();
                    cf.destino = true;
                    listaLetras = new List<string>();
                    listaLetras.AddRange(letrasJuridicas.Split(','));
                    cf.cbLetra.DataSource = listaLetras;
                    cf.txtRif.Text = txtDIProductor.Text;
                    cf.cbLetra.SelectedItem = cbLetras.SelectedItem;


                    cf.ShowDialog();

                    if (cf.id != 0)
                    {
                        objRecepcion.id_destino = cf.id;
                        txtNombreProductor.Enabled = false;
                        txtNombreProductor.Text = cf.txtNombre.Text;
                        txtDIProductor.Text = cf.txtRif.Text;
                        cbLetras.SelectedItem = cf.cbLetra.SelectedItem;
                        txtDIProductor.Enabled = false;
                        cbLetras.Enabled = false;
                        btnBuscarProductor.Text = "Cambiar";
                    EstilarBotonCampo(btnBuscarProductor, "Cambiar");
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando productor. Error Contacte al Administrador del Sistema", ex);

            }
        }

        private void productorNatural()
        {
            try
            {
                destinoData cd = new destinoData();
                DataTable productorDT = cd.buscar(cbLetras.SelectedItem.ToString(), Convert.ToDecimal(txtDIProductor.Text));
                if (productorDT.Rows.Count > 0)
                {


                    productorNatural cf = new productorNatural();
                    cf.destino = true;
                    listaLetras = new List<string>();
                    listaLetras.AddRange(letrasNaturales.Split(','));
                    cf.cbLetra.DataSource = listaLetras;
                    foreach (DataRow fila in productorDT.Rows)
                    {
                        objRecepcion.id_destino_natural = Convert.ToInt32(fila["id"]);
                        cf.txtNombre.Text = fila["nombre"].ToString();
                        cf.txtCedula.Text = fila["cedula"].ToString();
                        cf.cbLetra.SelectedItem = cbLetras.SelectedItem;
                        cf.cbAsociaciones.SelectedItem = fila["id_asociacion"].ToString();
                        cf.id = (int)objRecepcion.id_destino_natural;

                    }
                    cf.btnEliminar.Visible = true;
                    cf.actualizar = true;
                    cf.ShowDialog();
                    if (cf.eliminado)
                    {

                        txtDIProductor.Text = string.Empty;
                        txtNombreProductor.Text = string.Empty;

                        return;
                    }
                    if (cf.id > 0)
                    {
                        txtDIProductor.Text = cf.txtCedula.Text;
                        txtNombreProductor.Text = cf.txtNombre.Text;
                        txtDIProductor.Enabled = false;
                        txtNombreProductor.Enabled = false;
                        cbLetras.SelectedItem = cf.cbLetra.SelectedItem;
                        cbLetras.Enabled = false;
                        btnBuscarProductor.Text = "Cambiar";
                    EstilarBotonCampo(btnBuscarProductor, "Cambiar");
                    }
                }
                else
                {

                    productorNatural cf = new productorNatural();
                    cf.destino = true;
                    listaLetras = new List<string>();
                    listaLetras.AddRange(letrasNaturales.Split(','));
                    cf.cbLetra.DataSource = listaLetras;
                    cf.txtCedula.Text = txtDIProductor.Text;
                    cf.cbLetra.SelectedItem = cbLetras.SelectedItem;


                    cf.ShowDialog();

                    if (cf.id != 0)
                    {
                        objRecepcion.id_destino_natural = cf.id;
                        txtNombreProductor.Enabled = false;
                        txtNombreProductor.Text = cf.txtNombre.Text;
                        txtDIProductor.Text = cf.txtCedula.Text;
                        cbLetras.SelectedItem = cf.cbLetra.SelectedItem;
                        txtDIProductor.Enabled = false;
                        cbLetras.Enabled = false;
                        btnBuscarProductor.Text = "Cambiar";
                    EstilarBotonCampo(btnBuscarProductor, "Cambiar");
                    }

                }
                productorDT.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando productor. Error Contacte al Administrador del Sistema", ex);

            }
        }

        private void rbTipoNatural_CheckedChanged(object sender, EventArgs e)
        {
            listaLetras = new List<string>();
            listaLetras.AddRange(letrasNaturales.Split(','));
            cbLetras.Enabled = true;
            txtDIProductor.Enabled = true;
            txtNombreProductor.Enabled = true;
            txtDIProductor.Text = string.Empty;
            txtNombreProductor.Text = string.Empty;
            objRecepcion.id_destino_natural = 0;
            btnBuscarProductor.Text = "Buscar";

            lbEjemploletraConductor.Text = "Ejem: V";
            lbEjemploCedulaCondcutor.Text = "Ejem: 12345678";
            cbLetras.DataSource = new DataTable();
            cbLetras.DataSource = listaLetras;
        }


        #endregion

        private void btnAgregarSilos_Click(object sender, EventArgs e)
        {
            try
            {
                silosForm tf = new silosForm();
                tf.ShowDialog();
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }
            cargarSilos();

        }

        private void btnEditarSilos_Click(object sender, EventArgs e)
        {
            try
            {
                if (!validateRegistrarOrEditar("validareditar"))
                    return;
                object silos = cbSilos.SelectedItem;

                silosForm tf = new silosForm();
                tf.actualizar = true;
                tf.id = Convert.ToInt32(((System.Data.DataRowView)(cbSilos.SelectedItem)).Row.ItemArray[0]);
                tf.txtNombre.Text = cbSilos.Text;
                tf.btnEliminar.Visible = true;
                tf.ShowDialog();
                cargarSilos();
                cbSilos.SelectedValue = tf.id;
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_modoListado)
                    return;

                if (objRecepcion.id_conductor == 0)
                {
                    txtNombreConductor.Text = string.Empty;
                    txtConductor.Text = string.Empty;
                    txtConductor.AccessibleDescription = "validar";

                }

                // if (objRecepcion.id_conductor == 0 )
                //{
                //    txtDIProductor.Text = string.Empty;
                //    txtNombreProductor.Text = string.Empty;
                //    txtNombreProductor.AccessibleDescription = "validar";

                //}
                if (!_anulando)
                {
                    txtDIProductor.AccessibleDescription = "validar";
                    if (!validateRegistrarOrEditar("validar"))
                        return;
                }
                objRecepcion.fecha = dtpFecha.Value;
                objRecepcion.nro_placa = txtPlaca.Text;

                if (cbProducto.SelectedItem is DataRowView)
                    objRecepcion.id_producto = Convert.ToInt32(((System.Data.DataRowView)(cbProducto.SelectedItem)).Row.ItemArray[0]);

                objRecepcion.nro_guia = txtNroguia.Text;
                objRecepcion.peso_guia = !string.IsNullOrWhiteSpace(txtPesoGuia.Text) ? Convert.ToDecimal(txtPesoGuia.Text) : 0;
                objRecepcion.observaciones = txtObservaciones.Text;
                objRecepcion.peso_bruto = !string.IsNullOrWhiteSpace(txtPesoSalida.Text) ? Convert.ToDecimal(txtPesoSalida.Text) : 0;
                objRecepcion.peso_tara = !string.IsNullOrWhiteSpace(txtPesoEntrada.Text) ? Convert.ToDecimal(txtPesoEntrada.Text) : 0;
                if (!String.IsNullOrWhiteSpace(cbSilos.Text))
                    objRecepcion.id_silos = Convert.ToInt32(((System.Data.DataRowView)(cbSilos.SelectedItem)).Row.ItemArray[0]);
                if (!String.IsNullOrWhiteSpace(dtpHoraentrada.Text))
                {

                    objRecepcion.hora_entrada = dtpHoraentrada.Value;
                }
                if (!String.IsNullOrWhiteSpace(dtpHoraSalida.Text))
                {

                    objRecepcion.hora_salida = dtpHoraSalida.Value;
                }

                if (nuevo && !ValidarInventarioDespacho())
                    return;

                if (nuevo)
                {

                    objRecepcion.id_estatus = 1;

                    resultadodt = dd.insertar(objRecepcion.nro_placa, objRecepcion.id_conductor, objRecepcion.fecha, objRecepcion.nro_guia, objRecepcion.peso_guia, objRecepcion.id_producto, objRecepcion.id_destino,
                        objRecepcion.observaciones, objRecepcion.id_estatus, objRecepcion.peso_bruto, objRecepcion.peso_tara, objRecepcion.id_silos, objRecepcion.hora_entrada, objRecepcion.hora_salida, objRecepcion.id_destino_natural, rbAnalisis.Checked);
                    Base.msjExito("Registro guardado exitosamente.");
                    llenarGrid();
                    ActualizarPasosDespacho();

                }
                else
                {
                    if ((id_producto_original <= 3 && objRecepcion.id_producto != id_producto_original))
                    {
                        DialogResult result = Base.msjPreguntaCancel("Ha cambiado el tipo de producto. Se eliminará el análisis guardado. ¿Desea continuar?");
                        if (result == DialogResult.Yes)
                        {
                            btnEliminarAnalisis_Click(sender, e);
                        }
                        else
                        {

                            return;
                        }

                    }
                    resultadodt = dd.actualizar(objRecepcion.id, objRecepcion.nro_placa, objRecepcion.id_conductor, objRecepcion.fecha, objRecepcion.nro_guia, objRecepcion.peso_guia, objRecepcion.id_producto, objRecepcion.id_destino,
                       objRecepcion.observaciones, objRecepcion.id_estatus, objRecepcion.peso_bruto, objRecepcion.peso_tara, objRecepcion.id_silos, objRecepcion.hora_entrada, objRecepcion.hora_salida,objRecepcion.id_destino_natural,rbAnalisis.Checked);
                    Base.msjExito("Registro guardado exitosamente.");
                    llenarGrid();
                    ActualizarPasosDespacho();
                    resultadodt.Dispose();


                }


            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "guardar el despacho");
            }

        }

        private void EjecutarBusquedaListado()
        {
            try
            {
                if (_txtFiltroTicket == null)
                    return;

                int nroTicket = 0;
                if (!string.IsNullOrWhiteSpace(_txtFiltroTicket.Text))
                {
                    if (!int.TryParse(_txtFiltroTicket.Text.Trim(), out nroTicket))
                    {
                        UiDialog.Show("El nro de ticket debe ser numérico.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        _txtFiltroTicket.Focus();
                        return;
                    }
                }

                DateTime fechaDesde = string.IsNullOrWhiteSpace(_dtpFiltroDesde.Text)
                    ? DateTimePicker.MinimumDateTime
                    : _dtpFiltroDesde.Value.Date;
                DateTime fechaHasta = string.IsNullOrWhiteSpace(_dtpFiltroHasta.Text)
                    ? DateTimePicker.MinimumDateTime
                    : _dtpFiltroHasta.Value.Date;

                if (fechaDesde != DateTimePicker.MinimumDateTime
                    && fechaHasta != DateTimePicker.MinimumDateTime
                    && fechaDesde > fechaHasta)
                {
                    UiDialog.Show("La fecha desde debe ser menor o igual a la fecha hasta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idConductor = ResolverIdConductorFiltro(_txtFiltroConductor.Text);
                if (!string.IsNullOrWhiteSpace(_txtFiltroConductor.Text) && idConductor == 0)
                {
                    UiDialog.Show("No se encontró conductor con esa cédula.", "Fallido", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    _txtFiltroConductor.Focus();
                    return;
                }

                int idDestino = 0;
                int idDestinoNatural = 0;
                if (!string.IsNullOrWhiteSpace(_txtFiltroCliente.Text))
                {
                    if (!ResolverIdsDestinoFiltro(_txtFiltroCliente.Text, out idDestinoNatural, out idDestino))
                    {
                        UiDialog.Show("No se encontró cliente/destino con ese documento.", "Fallido", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        _txtFiltroCliente.Focus();
                        return;
                    }
                }

                int idProducto = 0;
                if (_cbFiltroProducto != null && !string.IsNullOrWhiteSpace(_cbFiltroProducto.Text) && _cbFiltroProducto.SelectedItem is DataRowView drvProd)
                    idProducto = Convert.ToInt32(drvProd.Row["id"]);

                int idEstatus = 0;
                if (_cbFiltroEstatus != null && !string.IsNullOrWhiteSpace(_cbFiltroEstatus.Text) && _cbFiltroEstatus.SelectedItem is DataRowView drvEst)
                    idEstatus = Convert.ToInt32(drvEst.Row["id"]);

                resultadodt = dd.buscar(
                    nroTicket,
                    idProducto,
                    idConductor,
                    idDestino,
                    idDestinoNatural,
                    string.Empty,
                    0,
                    idEstatus,
                    fechaDesde,
                    fechaHasta);

                if (resultadodt == null || resultadodt.Rows.Count == 0)
                {
                    UiDialog.Show("No se encontraron resultados", "Fallido", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                btnExportar.Enabled = true;
                llenarGrid();
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "consultar el listado");
            }
        }

        private int ResolverIdConductorFiltro(string cedulaTexto)
        {
            if (string.IsNullOrWhiteSpace(cedulaTexto))
                return 0;

            decimal cedula;
            if (!decimal.TryParse(cedulaTexto.Trim(), out cedula))
                return 0;

            conductoresData cd = new conductoresData();
            DataTable dt = cd.buscar(cedula);
            try
            {
                if (dt.Rows.Count == 0)
                    return 0;
                return Convert.ToInt32(dt.Rows[0]["id"]);
            }
            finally
            {
                dt.Dispose();
            }
        }

        private bool ResolverIdsDestinoFiltro(string documentoTexto, out int idNatural, out int idJuridico)
        {
            idNatural = 0;
            idJuridico = 0;
            if (string.IsNullOrWhiteSpace(documentoTexto))
                return true;

            decimal documento;
            if (!decimal.TryParse(documentoTexto.Trim(), out documento))
                return false;

            string letra = _cbFiltroLetraCliente != null && _cbFiltroLetraCliente.SelectedItem != null
                ? _cbFiltroLetraCliente.SelectedItem.ToString().Trim()
                : "V";

            destinoData dest = new destinoData();

            DataTable natural = dest.buscar(letra, documento);
            try
            {
                if (natural.Rows.Count > 0)
                {
                    idNatural = Convert.ToInt32(natural.Rows[0]["id"]);
                    return true;
                }
            }
            finally
            {
                natural.Dispose();
            }

            DataTable juridico = dest.buscarPersonaJuridica(letra, documento);
            try
            {
                if (juridico.Rows.Count > 0)
                {
                    idJuridico = Convert.ToInt32(juridico.Rows[0]["id"]);
                    return true;
                }
            }
            finally
            {
                juridico.Dispose();
            }

            var letrasProbar = new List<string>();
            if (!string.IsNullOrWhiteSpace(letrasNaturales))
                letrasProbar.AddRange(letrasNaturales.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            if (!string.IsNullOrWhiteSpace(letrasJuridicas))
                letrasProbar.AddRange(letrasJuridicas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            foreach (string l in letrasProbar)
            {
                string t = l.Trim();
                if (string.Equals(t, letra, StringComparison.OrdinalIgnoreCase))
                    continue;

                DataTable n2 = dest.buscar(t, documento);
                try
                {
                    if (n2.Rows.Count > 0)
                    {
                        idNatural = Convert.ToInt32(n2.Rows[0]["id"]);
                        return true;
                    }
                }
                finally
                {
                    n2.Dispose();
                }

                DataTable j2 = dest.buscarPersonaJuridica(t, documento);
                try
                {
                    if (j2.Rows.Count > 0)
                    {
                        idJuridico = Convert.ToInt32(j2.Rows[0]["id"]);
                        return true;
                    }
                }
                finally
                {
                    j2.Dispose();
                }
            }

            return false;
        }

        private void llenarGrid()
        {
            if (_filtroRecepciones == null)
                _filtroRecepciones = new GridHeaderFilter(dgvRecepciones);

            DataTable vista = resultadodt != null ? resultadodt.Copy() : new DataTable();
            _filtroRecepciones.Bind(vista);
            ConfigurarColumnasDespachoVisibles();
            ActualizarEspacioGrilla();

            decimal totalNeto = decimal.Zero;
            decimal totalAcondicionado = decimal.Zero;
            int cantidad = 0;
            foreach (DataGridViewRow registro in dgvRecepciones.Rows)
            {
                if (registro.IsNewRow) continue;
                cantidad++;
                if (registro.Cells["peso_acondicionado"].Value != null && registro.Cells["peso_acondicionado"].Value != DBNull.Value)
                    totalAcondicionado += Convert.ToDecimal(registro.Cells["peso_acondicionado"].Value);
                if (registro.Cells["peso_neto"].Value != null && registro.Cells["peso_neto"].Value != DBNull.Value)
                    totalNeto += Convert.ToDecimal(registro.Cells["peso_neto"].Value);
            }

            ActualizarTotalesListado(totalNeto, totalAcondicionado, cantidad);
            AplicarModoDespacho(true);

            if (resultadodt != null && resultadodt.Rows.Count == 1)
            {
                llenarObjeto(resultadodt.Rows[0]);
                IrAPasoCapturaDatos();
            }
        }



        void llenarObjeto(DataRow fila)
        {
            try
            {
                nuevo = false;
                
                objRecepcion.id = Convert.ToInt32(fila["Nro_Ticket"]);
                objRecepcion.nro_placa = !Equals(fila["nro_placa"], DBNull.Value) ? fila["nro_placa"].ToString() : string.Empty;
                objRecepcion.fecha = Convert.ToDateTime(fila["fecha"]);
                objRecepcion.nro_guia = fila["nro_guia"].ToString();
                objRecepcion.peso_guia = Convert.ToDecimal(fila["peso_guia"]);
                objRecepcion.id_producto = Convert.ToInt32(fila["id_producto"]);
                id_producto_original = objRecepcion.id_producto;
                objRecepcion.id_destino = !Equals(fila["id_destino"], DBNull.Value) ? Convert.ToInt32(fila["id_destino"]) : 0;
                objRecepcion.observaciones = fila["observaciones"].ToString();
                objRecepcion.id_estatus = Convert.ToInt32(fila["id_estatus"]);
                objRecepcion.peso_bruto = Math.Round(Convert.ToDecimal(fila["peso_bruto"]), 2);
                objRecepcion.peso_tara = Math.Round(Convert.ToDecimal(fila["peso_tara"]), 2);
                objRecepcion.peso_neto = Math.Round(Convert.ToDecimal(fila["peso_neto"]), 2);
                objRecepcion.peso_acondicionado = Math.Round(Convert.ToDecimal(fila["peso_acondicionado"]), 2);
                objRecepcion.id_silos = !Equals(fila["id_silos"], DBNull.Value) ? Convert.ToInt32(fila["id_silos"]) : 0;
                objRecepcion.id_estatus = Convert.ToInt32(fila["id_estatus"]);

                objRecepcion.hora_entrada = !Equals(fila["hora_entrada"], DBNull.Value) ? Convert.ToDateTime(fila["hora_entrada"]) : DateTimePicker.MinimumDateTime;
                objRecepcion.hora_salida = !Equals(fila["hora_salida"], DBNull.Value) ? Convert.ToDateTime(fila["hora_salida"]) : DateTimePicker.MinimumDateTime;


                objRecepcion.id_destino_natural = !Equals(fila["id_destino_natural"], DBNull.Value) ? Convert.ToInt32(fila["id_destino_natural"]) : 0;
                objRecepcion.nombreconductor = fila["nombreconductor"].ToString();
                objRecepcion.cedulaconductor = Convert.ToInt32(fila["cedulaconductor"]);
                objRecepcion.nombre_producto = !Equals(fila["nombre_producto"], DBNull.Value) ? fila["nombre_producto"].ToString() : string.Empty;
                objRecepcion.nombre_asociacion = !Equals(fila["nombre_asociacion"], DBNull.Value) ? fila["nombre_asociacion"].ToString() : string.Empty;
                objRecepcion.nombre_trasnporte = !Equals(fila["nombre_transporte"], DBNull.Value) ? fila["nombre_transporte"].ToString() : string.Empty;
                objRecepcion.id_conductor = Convert.ToInt32(fila["id_conductor"]);
                if (objRecepcion.id_destino_natural > 0)
                {
                    objRecepcion.letranatural = fila["letranatural"].ToString();
                    objRecepcion.nombreproductornatural = fila["nombreclienternatural"].ToString();
                    objRecepcion.cedulaproductor = Convert.ToInt32(fila["cedulacliente"]);
                    objRecepcion.nombreproductorjuridico = string.Empty;
                    objRecepcion.rif = string.Empty;
                    objRecepcion.letrajuridica = string.Empty;
                }
                if (objRecepcion.id_destino > 0)
                {

                    objRecepcion.letrajuridica = fila["letrajuridica"].ToString();
                    objRecepcion.nombreproductorjuridico = fila["clientejuridico"].ToString();
                    objRecepcion.rif = fila["rif"].ToString();
                    objRecepcion.nombreproductornatural = string.Empty;
                    objRecepcion.cedulaproductor = 0;
                    objRecepcion.letranatural = fila["letranatural"].ToString();
                }
                objRecepcion.resumen = Convert.ToBoolean(fila["resumen"]);
                llenarCampos(objRecepcion);



            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un llenando objeto recepcion. Error Contacte al Administrador del Sistema", ex);

            }

        }

        void llenarCampos(despacho prmRecepcion)
        {
            try
            {

                btnGuardar.Text = "Guardar";
                IrAPasoCapturaDatos();
                ActualizarPasosDespacho();
                AplicarModoPesajeSegunPerfil();

                btnAnular.Visible = true;
                txtNroTcicket.Text = objRecepcion.id.ToString();
                ConfigurarTicketAutogenerado();
                _omitirAdvertenciaInventario = true;
                cbProducto.SelectedValue = prmRecepcion.id_producto;
                if (objRecepcion.id_destino_natural > 0)
                {
                    listaLetras = new List<string>();
                    listaLetras.AddRange(letrasNaturales.Split(','));
                    cbLetras.DataSource = new DataTable();
                    cbLetras.DataSource = listaLetras;
                    this.rbJuridica.CheckedChanged -= new System.EventHandler(this.rbJuridica_CheckedChanged);
                    this.rbTipoNatural.CheckedChanged -= new System.EventHandler(this.rbTipoNatural_CheckedChanged);
                    rbTipoNatural.Checked = true;
                    this.rbTipoNatural.CheckedChanged += new System.EventHandler(this.rbTipoNatural_CheckedChanged);
                    this.rbJuridica.CheckedChanged += new System.EventHandler(this.rbJuridica_CheckedChanged);
                    cbLetras.SelectedItem = objRecepcion.letranatural;
                    txtDIProductor.Text = objRecepcion.cedulaproductor.ToString();
                    txtNombreProductor.Text = objRecepcion.nombreproductornatural;

                    txtDIProductor.Enabled = false;
                    txtNombreProductor.Enabled = false;
                    btnBuscarProductor.Text = "Cambiar";
                    EstilarBotonCampo(btnBuscarProductor, "Cambiar");
                }
                if (objRecepcion.id_destino > 0)
                {
                    listaLetras = new List<string>();
                    listaLetras.AddRange(letrasJuridicas.Split(','));
                    cbLetras.DataSource = new DataTable();
                    cbLetras.DataSource = listaLetras;
                    this.rbTipoNatural.CheckedChanged -= new System.EventHandler(this.rbTipoNatural_CheckedChanged);
                    this.rbJuridica.CheckedChanged -= new System.EventHandler(this.rbJuridica_CheckedChanged);
                    rbJuridica.Checked = true;
                    this.rbJuridica.CheckedChanged += new System.EventHandler(this.rbJuridica_CheckedChanged);
                    this.rbTipoNatural.CheckedChanged += new System.EventHandler(this.rbTipoNatural_CheckedChanged);
                    cbLetras.SelectedItem = objRecepcion.letrajuridica;
                    txtDIProductor.Text = objRecepcion.rif;
                    txtNombreProductor.Text = objRecepcion.nombreproductorjuridico;

                    txtDIProductor.Enabled = false;
                    txtNombreProductor.Enabled = false;
                    btnBuscarProductor.Text = "Cambiar";
                    EstilarBotonCampo(btnBuscarProductor, "Cambiar");

                }
                if (objRecepcion.id_destino_natural == 0 && objRecepcion.id_destino == 0)
                {

                    txtDIProductor.Text = string.Empty;
                    txtNombreProductor.Text = string.Empty;
                    txtDIProductor.Enabled = true;
                    txtNombreProductor.Enabled = true;
                    btnBuscarProductor.Text = "Buscar";
                    EstilarBotonCampo(btnBuscarProductor, "Buscar");
                }
                if (objRecepcion.id_conductor > 0)
                {
                    txtConductor.Text = objRecepcion.cedulaconductor.ToString();
                    txtNombreConductor.Text = objRecepcion.nombreconductor.ToString();
                    txtConductor.Enabled = false;
                    txtNombreConductor.Enabled = false;
                    btnBuscarConductor.Text = "Cambiar";
                    EstilarBotonCampo(btnBuscarConductor, "Cambiar");
                }
                this.rbAnalisis.CheckedChanged -= new System.EventHandler(this.rbResumen_CheckedChanged);
                if (objRecepcion.resumen)
                    rbAnalisis.Checked = true;
                else
                    rbAnalisisNo.Checked = true;
                this.rbAnalisis.CheckedChanged += new System.EventHandler(this.rbResumen_CheckedChanged);
                dgvAnalisis.Enabled = rbAnalisis.Checked;
                btnGuardarAnalisis.Enabled = rbAnalisis.Checked;
                dtpFecha.Value = objRecepcion.fecha;
                txtPlaca.Text = objRecepcion.nro_placa;
                txtNroguia.Text = objRecepcion.nro_guia;
                txtPesoGuia.Text = objRecepcion.peso_guia.ToString();

                //cbLetras.SelectedItem = objRecepcion.id_silos;
                txtObservaciones.Text = objRecepcion.observaciones;
                if ( objRecepcion.id_estatus>=2)
                cargarAnalisis();
                txtPesoEntrada.Text = objRecepcion.peso_tara.ToString();
                txtPesoSalida.Text = objRecepcion.peso_bruto.ToString();
                txtPesoNeto.Text = objRecepcion.peso_neto.ToString();
                txtPesoAcondicionado.Text = objRecepcion.peso_acondicionado.ToString();
                cbSilos.SelectedValue = objRecepcion.id_silos;
                cbEstatus.SelectedValue = objRecepcion.id_estatus;
                cbEstatus.SelectedItem = objRecepcion.id_estatus;
                if (objRecepcion.hora_entrada.Date != horadefecto.Date)
                {
                    formatoHoraentrada();
                    dtpHoraentrada.Value = Convert.ToDateTime(objRecepcion.hora_entrada);


                }
                else
                {
                    dtpHoraentrada.Format = DateTimePickerFormat.Custom;
                    dtpHoraentrada.CustomFormat = " ";
                }
                if (objRecepcion.hora_salida.Date != horadefecto.Date)
                {
                    formatoHoraSalida();
                    dtpHoraSalida.Value = Convert.ToDateTime(objRecepcion.hora_salida);
                }
                else
                {

                    dtpHoraSalida.Format = DateTimePickerFormat.Custom;
                    dtpHoraSalida.CustomFormat = " ";

                }
                formatoFecha();
                dtpFecha.Value = Convert.ToDateTime(objRecepcion.fecha);

                bloquearControles();
                AplicarPoliticaFechaDocumento();

                    if (objRecepcion.id_estatus >= 4 && objRecepcion.id_estatus <= 6)
                    {
                        btnImprimir.Visible = true;
                    }
                    else { 
                        btnImprimir.Visible = false; 
                    }
                if(id_perfil!=1)
                cbEstatus.Enabled = false;
                dtpFechaHasta.Visible = false;
                rbtFechaHasta.Visible = false;
                ActualizarPasosDespacho();


            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un llenando campos recepcion. Error Contacte al Administrador del Sistema", ex);

            }
            finally
            {
                _omitirAdvertenciaInventario = false;
            }

        }

       

        private void tcRecepcion_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (nuevo)
                return;
            if (tcRecepcion.SelectedTab == tpAnalisis)
            {
                //tcRecepcion.SelectedIndex = 1;
                //cargarAnalisis();
            }

        }

        private void rbAutomatic_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarModoPesaje();
        }

        private void ActualizarModoPesaje()
        {
            bool automatico = rbAutomatic.Checked;
            txtPesoEntrada.Enabled = !automatico;
            txtPesoSalida.Enabled = !automatico;
            dtpHoraentrada.Enabled = !automatico;
            dtpHoraSalida.Enabled = !automatico;
            btnPesoEntrada.Visible = automatico;
            btnPesoSalida.Visible = automatico;
            btnPesoEntrada.Enabled = automatico;
            btnPesoSalida.Enabled = automatico;
            if (automatico)
            {
                btnPesoEntrada.ForeColor = Color.White;
                btnPesoSalida.ForeColor = Color.White;
            }
        }
        private void btnGuardarRomana_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPesoEntrada.Text) || !TienePeso(txtPesoEntrada.Text))
            {
                Base.msjAdvertencia("El campo peso de entrada es obligatorio.");
                return;
            }

            if (!HoraCapturada(dtpHoraentrada))
            {
                Base.msjAdvertencia("El campo hora de entrada es obligatorio.");
                return;
            }

            if (TienePeso(txtPesoSalida.Text) && !HoraCapturada(dtpHoraSalida))
            {
                Base.msjAdvertencia("El campo hora de salida es obligatorio.");
                return;
            }

            decimal pesoEntrada = Convert.ToDecimal(string.IsNullOrWhiteSpace(txtPesoEntrada.Text) ? "0" : txtPesoEntrada.Text);
            decimal pesoSalida = Convert.ToDecimal(string.IsNullOrWhiteSpace(txtPesoSalida.Text) ? "0" : txtPesoSalida.Text);
            if (pesoSalida > 0 && pesoEntrada > pesoSalida)
            {
                Base.msjAdvertencia("El peso de salida no puede ser menor que el peso de entrada.");
                return;
            }

            if (HoraCapturada(dtpHoraSalida) && HoraCapturada(dtpHoraentrada) && dtpHoraSalida.Value < dtpHoraentrada.Value)
            {
                Base.msjAdvertencia("La hora de entrada no puede ser mayor que la hora de salida.");
                return;
            }

            if (objRecepcion.id_estatus <= 2)
                objRecepcion.id_estatus = 2;
            if (TienePeso(txtPesoSalida.Text) && HoraCapturada(dtpHoraSalida))
            {
                if (objRecepcion.id_estatus <= 4)
                    objRecepcion.id_estatus = 4;
            }

            btnGuardar_Click(sender, e);
        }

        private static bool TienePeso(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            decimal valor;
            if (!decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out valor)
                && !decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out valor))
                return false;

            return valor > 0;
        }

        private static bool HoraCapturada(DateTimePicker picker)
        {
            if (picker == null)
                return false;
            if (picker.CustomFormat == " ")
                return false;
            return !string.IsNullOrWhiteSpace(picker.Text);
        }

        private void rbManual_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarModoPesaje();
        }

        private void txtPesoEntrada_TextChanged(object sender, EventArgs e)
        {
            cambiosPeso();
        }

        private void cambiosPeso()
        {
            //decimal pesoBruto = !string.IsNullOrWhiteSpace(txtPesoEntrada.Text) ? Math.Round( Convert.ToDecimal(txtPesoEntrada.Text),2) : 0;
            //decimal pesoSalida = !string.IsNullOrWhiteSpace(txtPesoSalida.Text)? Math.Round(Convert.ToDecimal(txtPesoSalida.Text),2):0 ;




            //objRecepcion.peso_neto = pesoBruto - pesoSalida;
            //objRecepcion.peso_acondicionado = objRecepcion.factorMema == 0 ? 1 :Math.Round(objRecepcion.factorMema * objRecepcion.peso_neto,2);


            //txtPesoNeto.Text = objRecepcion.peso_neto.ToString();
            //txtPesoAcondicionado.Text = objRecepcion.peso_acondicionado.ToString();
        }

        private void dgvAnalisis_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {


            DataGridViewTextBoxEditingControl dText = (DataGridViewTextBoxEditingControl)e.Control;
            dText.KeyPress -= new KeyPressEventHandler(text_KeyUp);
            dText.KeyPress += new KeyPressEventHandler(text_KeyUp);
        }

        void text_KeyUp(object sender, KeyPressEventArgs e)
        {
            int rowIndex = ((System.Windows.Forms.DataGridViewTextBoxEditingControl)(sender)).EditingControlRowIndex;

           int id_Item =Convert.ToInt16( dgvAnalisis.Rows[rowIndex].Cells["id_item"].Value);
           if (id_Item == 7)
           {
               Base.msjInformacion("Campo no editable. Será llenado como resultado de la sumatoria de granos dañados luego de ser guardado el análisis.");
              

           }
          
            //Para obligar a que sólo se introduzcan números 
           if (Char.IsDigit(e.KeyChar) && id_Item != 7)
            
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

        private void cbProducto_SelectedValueChanged(object sender, EventArgs e)
        {
            if (Equals(cbProducto.SelectedItem, null))
                return;

            objRecepcion.id_producto = Convert.ToInt32(((System.Data.DataRowView)(cbProducto.SelectedItem)).Row.ItemArray[0]);
            AdvertirInventarioSiloSeleccionado();
        }

        private void btnCancelarAnalisis_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnGuardarAnalisis_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgvAnalisis.Rows)
                {


                   
                    DataGridViewCell id = row.Cells["id_item"];
                    DataGridViewCell cell = row.Cells["valor_resultado"];
                    if ((cell.Value == DBNull.Value || string.IsNullOrWhiteSpace(!Equals(cell.Value, null) ? cell.Value.ToString().Trim() : string.Empty)) && Convert.ToInt32(id.Value) != 7)
                    {
                        DataGridViewCell cellnombre = row.Cells["nombre"];
                        Base.msjAdvertencia("Debe llenar el resultado de " + cellnombre.Value + ".");
                        return;

                    }

                }

                if (dd.buscaranalisis(objRecepcion.id).Rows.Count == 0)
                {
                    foreach (DataGridViewRow row in dgvAnalisis.Rows)
                    {
                        DataGridViewCell id = row.Cells["id_item"];
                        decimal valor;
                        if (Convert.ToInt32(id.Value) == 7)
                        {
                            valor = sumarGranosdañados();
                        }
                        else
                        {
                            valor = Convert.ToDecimal(row.Cells["valor_resultado"].Value);
                        }

                        dd.insertarAnalisis(objRecepcion.id_producto, objRecepcion.id, valor, Convert.ToInt32(id.Value));


                    }
                    if(objRecepcion.id_estatus<=3)
                    objRecepcion.id_estatus = 3;

                    btnGuardar_Click(sender, e);


                }
                else
                {

                    foreach (DataGridViewRow row in dgvAnalisis.Rows)
                    {

                        DataGridViewCell id_item = row.Cells["id_item"];
                        decimal valor;
                        if (Convert.ToInt32(id_item.Value) == 7)
                        {
                            valor = sumarGranosdañados();
                        }
                        else
                        {
                            valor = Convert.ToDecimal(row.Cells["valor_resultado"].Value);
                        }



                        DataGridViewCell id = row.Cells["id"];
                        dd.actualizarAnalisis(Convert.ToInt32(id.Value), objRecepcion.id,
                            valor, Convert.ToInt32(id_item.Value), false);


                    }
                    cargarAnalisis();
                    Base.msjExito("Registro guardado exitosamente.");

                }

            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un Erros guardando Análisis. Error Contacte al Administrador del Sistema", ex);

            }



        }

        decimal sumarGranosdañados()
        {
            decimal suma = decimal.Zero;
            foreach (DataGridViewRow row in dgvAnalisis.Rows)
            {


                int id_Item = Convert.ToInt16(row.Cells["id_item"].Value);

                if (id_Item == 3 || id_Item == 4 || id_Item == 5 || id_Item == 6)
                {
                    suma += Convert.ToDecimal(row.Cells["valor_resultado"].Value);

                }




            }
            return suma;

        }

        private void btnEliminarAnalisis_Click(object sender, EventArgs e)
        {
            if (!desdeBoton)
            {
                DialogResult result = Base.msjPreguntaCancel("Al eliminar este registro no podrá recuperarse. ¿Desea continuar con la eliminación?");
                if (result == DialogResult.Yes)
                {

                    foreach (DataGridViewRow row in dgvAnalisis.Rows)
                    {


                        DataGridViewCell valor = row.Cells["valor_resultado"];
                        DataGridViewCell id_item = row.Cells["id_item"];
                        DataGridViewCell id = row.Cells["id"];

                        dd.actualizarAnalisis(Convert.ToInt32(id.Value), objRecepcion.id, Convert.ToDecimal(valor.Value), Convert.ToInt32(id_item.Value), true);


                    }

                    objRecepcion.listaAnalisis = new List<analisis>();
                    Base.msjExito("Registro guardado exitosamente.");
                }
                else
                {
                    return;
                }
            }
            else
            {

                foreach (DataGridViewRow row in dgvAnalisis.Rows)
                {


                    DataGridViewCell valor = row.Cells["valor_resultado"];
                    DataGridViewCell id_item = row.Cells["id_item"];
                    DataGridViewCell id = row.Cells["id"];

                    dd.actualizarAnalisis(Convert.ToInt32(id.Value), objRecepcion.id, Convert.ToDecimal(valor.Value), Convert.ToInt32(id_item.Value), true);


                }
                objRecepcion.listaAnalisis = new List<analisis>();
                desdeBoton = false;

            }
            objRecepcion.id_estatus = 2;
            cargarAnalisis();
        }

        private bool _anulando;

        private void btnAnular_Click(object sender, EventArgs e)
        {
            if (objRecepcion == null || objRecepcion.id <= 0)
            {
                Base.msjAdvertencia("No hay despacho para anular.");
                return;
            }
            if (objRecepcion.id_estatus == 6)
            {
                Base.msjAdvertencia("El despacho ya está anulado.");
                return;
            }

            if (txtObservaciones != null)
            {
                txtObservaciones.Enabled = true;
                txtObservaciones.AccessibleDescription = "validarAnular";
                txtObservaciones.Focus();
            }

            if (!validateRegistrarOrEditar("validarAnular"))
            {
                Base.msjAdvertencia("Indique el motivo de anulación en observaciones.");
                return;
            }

            DialogResult confirm = UiDialog.Show(
                "¿Desea anular este despacho?",
                "Anular",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            int estatusAnterior = objRecepcion.id_estatus;
            objRecepcion.id_estatus = 6;
            _anulando = true;
            try
            {
                btnGuardar_Click(sender, e);
            }
            finally
            {
                _anulando = false;
            }

            if (objRecepcion.id_estatus == 6)
            {
                try
                {
                    DataTable dt = dd.buscar(objRecepcion.id, 0, 0, 0, 0, string.Empty, 0, 0,
                        DateTimePicker.MinimumDateTime, DateTimePicker.MinimumDateTime);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int estatusBd = Convert.ToInt32(dt.Rows[0]["id_estatus"]);
                        if (estatusBd != 6)
                            objRecepcion.id_estatus = estatusAnterior;
                    }
                    else
                        objRecepcion.id_estatus = estatusAnterior;
                }
                catch
                {
                    objRecepcion.id_estatus = estatusAnterior;
                }
            }

            bloquearControles();
        }

        private void bloquearControles()
        {
            if (objRecepcion.id_estatus == 6 || objRecepcion.id_estatus == 5)
            {
            lbAnulado.Text = objRecepcion.id_estatus == 5 ? string.Empty : "Anulado";
            btnEliminarAnalisis.Enabled = false;
            btnGuardar.Enabled = false;
            btnGuardarAnalisis.Enabled = false; 
            btnGuardarRomana.Enabled = false;
            btnAnular.Enabled = objRecepcion.id_estatus != 6;
            rbAnalisis.Enabled = false;
            rbAnalisisNo.Enabled = false;
            if (objRecepcion.id_estatus == 5 && txtObservaciones != null)
                txtObservaciones.Enabled = true;
            }
              else
            {
                lbAnulado.Text = string.Empty;
                btnEliminarAnalisis.Enabled = true;
                btnGuardar.Enabled = true;
                btnGuardarAnalisis.Enabled = true;
                btnGuardarRomana.Enabled = true;
                btnAnular.Enabled = true;
                AplicarPoliticaFechaDocumento();
            }

            ConfigurarAnalisisSinUso();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {

            txtDIProductor.AccessibleDescription = "validarImptimir";
            txtPesoGuia.AccessibleDescription = "validarImptimir";
            txtNroguia.AccessibleDescription = "validarImptimir";
            cbSilos.AccessibleDescription = "validarImptimir";
            if (!validateRegistrarOrEditar("validarImptimir"))
                return;
            //objRecepcion.nombre_producto = ((System.Data.DataRowView)(cbProducto.SelectedItem)).Row.ItemArray[2].ToString();
            int objetoAnterior = objRecepcion.id_estatus;

            objRecepcion.id_estatus = 5;

            if (objetoAnterior == 4)
                btnGuardar_Click(sender, e);
        if(objetoAnterior==6)
                objRecepcion.id_estatus = 6;

            objRecepcion.empresa = empresa;
            objRecepcion.direccion_empresa = direccionempresa;
            objRecepcion.usuario = nombre_Usuario;

            reporteForm rf = new reporteForm(objRecepcion.listaAnalisis);
            rf.listaAnalisis = objRecepcion.listaAnalisis;
            List<despacho> listaRecepcion = new List<despacho>();
            //objRecepcion.listaAnalisis = listaAnalisis;
            listaRecepcion.Add(objRecepcion);

            rf.rvReporte.ProcessingMode = ProcessingMode.Local;
            //rf.rvReporte.LocalReport.ReportPath = "//boletaRecepcion.rdlc";
            if(analisis)
            rf.rvReporte.LocalReport.ReportEmbeddedResource = "principal.nuevoDespacho.rdlc";
            else
                rf.rvReporte.LocalReport.ReportEmbeddedResource = "principal.nuevoDespacho.rdlc";
            ReportDataSource rds = new ReportDataSource("Boleta", listaRecepcion);
            //ReportDataSource rda = new ReportDataSource("analisis", listaAnalisis);
            rf.rvReporte.LocalReport.DataSources.Clear();
            rf.rvReporte.LocalReport.DataSources.Add(rds);
            //rf.rvReporte.LocalReport.DataSources.Add(rda);
            rf.rvReporte.LocalReport.Refresh();
            rf.ShowDialog();
            bloquearControles();
        }

        private void cbEstatus_SelectedValueChanged(object sender, EventArgs e)
        {
            if (Equals(cbEstatus.SelectedItem, null))
                return;
            objRecepcion.id_estatus = Convert.ToInt32(((System.Data.DataRowView)(cbEstatus.SelectedItem)).Row.ItemArray[0]);
            if (id_perfil == 1)
                btnGuardar.Enabled = true;
        }

        private void dgvRecepciones_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvRecepciones.SelectedRows.Count == 0) return;

            try
            {
                object ticketVal = dgvRecepciones.Columns.Contains("Nro_Ticket")
                    ? dgvRecepciones.SelectedRows[0].Cells["Nro_Ticket"].Value
                    : dgvRecepciones.SelectedRows[0].Cells[0].Value;
                int seleccionado = Convert.ToInt32(ticketVal);

                if (resultadodt == null || resultadodt.Rows.Count == 0)
                    return;

                DataRow[] rowseleccionado = resultadodt.Select("Nro_Ticket=" + seleccionado);
                if (rowseleccionado.Length == 0) return;

                llenarObjeto(rowseleccionado[0]);
                ActualizarPasosDespacho();
                IrAPasoCapturaDatos();
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "consultar el listado");
            }
        }

        private void cbSilos_SelectedValueChanged(object sender, EventArgs e)
        {
            if (Equals(cbSilos.SelectedItem, null))
                return;
            objRecepcion.id_silos = Convert.ToInt32(((System.Data.DataRowView)(cbSilos.SelectedItem)).Row.ItemArray[0]);
            AdvertirInventarioSiloSeleccionado();
        }

        private bool ValidarInventarioDespacho()
        {
            if (cbProducto.SelectedItem == null || string.IsNullOrWhiteSpace(cbProducto.Text))
                return true;

            if (cbSilos.SelectedItem == null || string.IsNullOrWhiteSpace(cbSilos.Text))
            {
                Base.msjAdvertencia("Debe seleccionar un silo.");
                return false;
            }

            return ValidarExistenciaInventario(cbProducto.Text, cbSilos.Text, ObtenerPesoNetoCapturado(), bloquearGuardado: true);
        }

        private void AdvertirInventarioSiloSeleccionado()
        {
            if (_omitirAdvertenciaInventario || _modoListado)
                return;
            if (cbProducto.SelectedItem == null || cbSilos.SelectedItem == null)
                return;
            if (string.IsNullOrWhiteSpace(cbProducto.Text) || string.IsNullOrWhiteSpace(cbSilos.Text))
                return;

            ValidarExistenciaInventario(cbProducto.Text, cbSilos.Text, ObtenerPesoNetoCapturado(), bloquearGuardado: false);
        }

        private decimal ObtenerPesoNetoCapturado()
        {
            decimal pesoBruto = !string.IsNullOrWhiteSpace(txtPesoSalida.Text) ? Convert.ToDecimal(txtPesoSalida.Text) : 0;
            decimal pesoTara = !string.IsNullOrWhiteSpace(txtPesoEntrada.Text) ? Convert.ToDecimal(txtPesoEntrada.Text) : 0;
            decimal pesoNeto = pesoBruto - pesoTara;
            return pesoNeto < 0 ? 0 : pesoNeto;
        }

        private bool ValidarExistenciaInventario(string nombreProducto, string nombreSilo, decimal pesoNetoDespacho, bool bloquearGuardado)
        {
            decimal pesoNetoInventario;
            decimal pesoAcondicionadoInventario;
            bool existe = _historicoData.TryObtenerInventario(nombreProducto, nombreSilo, out pesoNetoInventario, out pesoAcondicionadoInventario);

            if (!existe || pesoNetoInventario <= 0)
            {
                if (!_historicoData.ProductoTieneInventario(nombreProducto))
                {
                    Base.msjAdvertencia("El producto seleccionado no tiene inventario disponible. Registre recepciones completadas antes de crear un despacho.");
                }
                else
                {
                    Base.msjAdvertencia($"El silo \"{nombreSilo}\" no tiene inventario disponible para \"{nombreProducto}\". Seleccione otro silo con existencia.");
                }
                return !bloquearGuardado;
            }

            if (pesoNetoDespacho > 0 && pesoNetoDespacho > pesoNetoInventario)
            {
                Base.msjAdvertencia($"El peso neto del despacho ({pesoNetoDespacho:N2} Kg) supera el inventario disponible en el silo ({pesoNetoInventario:N2} Kg).");
                return !bloquearGuardado;
            }

            return true;
        }

        private void rtbFechaHasta_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {

            this.dtpFecha.ValueChanged -= new System.EventHandler(this.dtpFecha_ValueChanged);
            formatoFecha();
       
            this.dtpFecha.ValueChanged += new System.EventHandler(this.dtpFecha_ValueChanged);
        }

        private void dtpFechaHasta_ValueChanged(object sender, EventArgs e)
        {
            this.dtpFechaHasta.ValueChanged -= new System.EventHandler(this.dtpFechaHasta_ValueChanged);
            formatoFechaHasta();
         
            this.dtpFechaHasta.ValueChanged += new System.EventHandler(this.dtpFechaHasta_ValueChanged);
        }

        private void dtpHoraentrada_ValueChanged(object sender, EventArgs e)
        {
            this.dtpHoraentrada.ValueChanged -= new System.EventHandler(this.dtpHoraentrada_ValueChanged);
            formatoHoraentrada();
           
            this.dtpHoraentrada.ValueChanged += new System.EventHandler(this.dtpHoraentrada_ValueChanged);

        }

        private void dtpHoraSalida_ValueChanged(object sender, EventArgs e)
        {
            this.dtpHoraSalida.ValueChanged -= new System.EventHandler(this.dtpHoraSalida_ValueChanged);
            formatoHoraSalida();
            
            this.dtpHoraSalida.ValueChanged += new System.EventHandler(this.dtpHoraSalida_ValueChanged);
        }

        private void btnEditarSilos_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!validateRegistrarOrEditar("validareditar"))
                    return;
                object silos = cbSilos.SelectedItem;

                silosForm tf = new silosForm();
                tf.actualizar = true;
                tf.id = Convert.ToInt32(((System.Data.DataRowView)(cbSilos.SelectedItem)).Row.ItemArray[0]);
                tf.txtNombre.Text = cbSilos.Text;
                tf.btnEliminar.Visible = true;
                tf.ShowDialog();
                cargarSilos();
                cbSilos.SelectedValue = tf.id;
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }
        }



        private void rbResumen_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAnalisis.Checked)
            {

                dgvAnalisis.Enabled = true;
                btnGuardarAnalisis.Enabled = true;
            }
            else
            {
             
                if (analisis)
                {
                    DialogResult result = Base.msjPreguntaCancel("Se eliminará el análisis. Al eliminar este registro no podrá recuperarse. ¿Desea continuar con la eliminación?");
                    if (result == DialogResult.Yes)
                    {

                      

                        desdeBoton = true;
                        analisis = false;
                        btnEliminarAnalisis_Click(sender, e);


                    }
                    else
                    {
                        return;
                    }

                }
                        if (objRecepcion.id_estatus < 3)
                        {
                                objRecepcion.id_estatus = 3;
                        btnGuardar_Click(sender, e);
                        }


                        dgvAnalisis.Enabled = false;
                        btnGuardarAnalisis.Enabled = false;
                        lbAdvertencia.Text = string.Empty;
                
            }
        }

        private void rbtFechaHasta_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtFechaHasta.Checked)
            {

                this.rbtFechaHasta.CheckedChanged -= new System.EventHandler(this.rtbFechaHasta_CheckedChanged);
                rbtFechaHasta.Checked = false;
                this.rbtFechaHasta.CheckedChanged += new System.EventHandler(this.rtbFechaHasta_CheckedChanged);

                rbtFechaHasta.Text = !fechahasta ? "Ocultar" : "Mostrar";
                dtpFechaHasta.Visible = !fechahasta;
                if (fechahasta)
                {
                    dtpFechaHasta.CustomFormat = " ";

                }
                else
                {
                    formatoFechaHasta();
                }
                fechahasta = !fechahasta;
            }  
         
        }

        private void btnPesoEntrada_Click(object sender, EventArgs e)
        {
            try
            {
                if (!conexionSerial())
                    return;
                string trama;
                string detalle;
                decimal peso = RomanaSerialHelper.LeerPesoDecimal(spRomana, out trama, out detalle);
                txtPesoEntrada.Text = RomanaSerialHelper.FormatearPesoParaBd(peso);
                dtpHoraentrada.Value = DateTime.Now;
                spRomana.Close();
            }
            catch (Exception ex)
            {
                try { if (spRomana.IsOpen) spRomana.Close(); } catch { }
                Base.msjError(ex.Message);
            }
        }

        private void btnPesoSalida_Click(object sender, EventArgs e)
        {
            try
            {
                if (!conexionSerial())
                    return;
                string trama;
                string detalle;
                decimal peso = RomanaSerialHelper.LeerPesoDecimal(spRomana, out trama, out detalle);
                txtPesoSalida.Text = RomanaSerialHelper.FormatearPesoParaBd(peso);
                dtpHoraSalida.Value = DateTime.Now;
                spRomana.Close();
            }
            catch (Exception ex)
            {
                try { if (spRomana.IsOpen) spRomana.Close(); } catch { }
                Base.msjError(ex.Message);
            }
        }

        private bool conexionSerial()
        {
            string puerto = RomanaSerialHelper.PuertoConfigurado;

            bool existe = SerialPort.GetPortNames()
                .Any(p => string.Equals(p, puerto, StringComparison.OrdinalIgnoreCase));
            if (!existe)
            {
                if (Base.EsRomana() || (rbManual != null && !rbManual.Visible))
                {
                    UiDialog.Show(
                        "No hay báscula conectada en el puerto " + puerto + ".\n\nUse Configuración → Puerto / Monitor romana para detectar el COM.",
                        "Báscula no disponible",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    UiDialog.Show(
                        "No hay báscula conectada en el puerto " + puerto + ".\n\nUse modo Manual, o Configuración → Puerto / Monitor romana.",
                        "Báscula no disponible",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    if (rbManual != null)
                        rbManual.Checked = true;
                }
                return false;
            }

            if (spRomana.IsOpen)
                spRomana.Close();
            RomanaSerialHelper.ConfigurarPuerto(spRomana);
            spRomana.Open();
            return true;
        }

        private void txtConductor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                btnBuscarConductor.PerformClick();
            }
        }

        private void txtNombreConductor_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
               
                btnBuscarConductor.PerformClick();
            }
        }

        private void txtDIProductor_KeyDown(object sender, KeyEventArgs e)
        {
  
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
           
                btnBuscarProductor.PerformClick();
            }
           
        }

        private void DespachoForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                btnGuardar.PerformClick();
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (dgvRecepciones.Rows.Count > 0)
            {

                List<despacho> listaR = new List<despacho>();

                foreach (DataRow row in resultadodt.Rows)
                {
                    if (row.ItemArray[29].ToString() == "Culminado")
                        listaR.Add(llenarobjetoReporte(row));
                }

                reporteForm rf = new reporteForm(objRecepcion.listaAnalisis);
                rf.listaAnalisis = objRecepcion.listaAnalisis;
                List<recepcionBO> listaRecepcion = new List<recepcionBO>();
                //objRecepcion.listaAnalisis = listaAnalisis;


                rf.rvReporte.ProcessingMode = ProcessingMode.Local;


                ReportDataSource rds = new ReportDataSource("Boleta", listaR);
                rf.rvReporte.LocalReport.DataSources.Clear();
                rf.rvReporte.LocalReport.DataSources.Add(rds);
                //ReportDataSource rda = new ReportDataSource("analisis", listaAnalisis);
                rf.rvReporte.LocalReport.ReportEmbeddedResource = "principal.ResumenDespacho.rdlc";
                //rf.rvReporte.LocalReport.DataSources.Add(rda);
                rf.rvReporte.LocalReport.Refresh();
                rf.ShowDialog();


            }
            else
            {

                Base.msjAdvertencia("No existen elementos para realizar exportación.");
            }
        }
        despacho llenarobjetoReporte(DataRow fila)
        {
            try
            {
                despacho objRecepcionR = new despacho();

                objRecepcionR.empresa = empresa;
                objRecepcionR.direccion_empresa = direccionempresa;
                objRecepcionR.id = Convert.ToInt32(fila["Nro_Ticket"]);
                objRecepcionR.nro_placa = !Equals(fila["nro_placa"], DBNull.Value) ? fila["nro_placa"].ToString() : string.Empty;
                objRecepcionR.fecha = Convert.ToDateTime(fila["fecha"]);
                objRecepcionR.nro_guia = fila["nro_guia"].ToString();
                objRecepcionR.peso_guia = Convert.ToDecimal(fila["peso_guia"]);
                objRecepcionR.id_producto = Convert.ToInt32(fila["id_producto"]);

                objRecepcionR.id_destino = !Equals(fila["id_destino"], DBNull.Value) ? Convert.ToInt32(fila["id_destino"]) : 0;
                objRecepcionR.observaciones = fila["observaciones"].ToString();
                objRecepcionR.id_estatus = Convert.ToInt32(fila["id_estatus"]);
                objRecepcionR.peso_bruto = Math.Round(Convert.ToDecimal(fila["peso_bruto"]), 2);
                objRecepcionR.peso_tara = Math.Round(Convert.ToDecimal(fila["peso_tara"]), 2);
                objRecepcionR.peso_neto = Math.Round(Convert.ToDecimal(fila["peso_neto"]), 2);
                objRecepcionR.peso_acondicionado = Math.Round(Convert.ToDecimal(fila["peso_acondicionado"]), 2);
                objRecepcionR.id_silos = !Equals(fila["id_silos"], DBNull.Value) ? Convert.ToInt32(fila["id_silos"]) : 0;
                objRecepcionR.id_estatus = Convert.ToInt32(fila["id_estatus"]);

                objRecepcionR.hora_entrada = !Equals(fila["hora_entrada"], DBNull.Value) ? Convert.ToDateTime(fila["hora_entrada"]) : DateTimePicker.MinimumDateTime;
                objRecepcionR.hora_salida = !Equals(fila["hora_salida"], DBNull.Value) ? Convert.ToDateTime(fila["hora_salida"]) : DateTimePicker.MinimumDateTime;


                objRecepcionR.id_destino_natural = !Equals(fila["id_destino_natural"], DBNull.Value) ? Convert.ToInt32(fila["id_destino_natural"]) : 0;
                objRecepcionR.nombreconductor = fila["nombreconductor"].ToString();
                objRecepcionR.cedulaconductor = Convert.ToInt32(fila["cedulaconductor"]);
                objRecepcionR.nombre_producto = !Equals(fila["nombre_producto"], DBNull.Value) ? fila["nombre_producto"].ToString() : string.Empty;
                objRecepcionR.nombre_asociacion = !Equals(fila["nombre_asociacion"], DBNull.Value) ? fila["nombre_asociacion"].ToString() : string.Empty;
                objRecepcionR.nombre_trasnporte = !Equals(fila["nombre_transporte"], DBNull.Value) ? fila["nombre_transporte"].ToString() : string.Empty;
                objRecepcionR.id_conductor = Convert.ToInt32(fila["id_conductor"]);
                if (objRecepcionR.id_destino_natural > 0)
                {
                    objRecepcionR.letranatural = fila["letranatural"].ToString();
                    objRecepcionR.nombreproductornatural = fila["nombreclienternatural"].ToString();
                    objRecepcionR.cedulaproductor = Convert.ToInt32(fila["cedulacliente"]);


                }
                if (objRecepcionR.id_destino > 0)
                {

                    objRecepcionR.letrajuridica = fila["letrajuridica"].ToString();
                    objRecepcionR.nombreproductorjuridico = fila["clientejuridico"].ToString();
                    objRecepcionR.rif = fila["rif"].ToString();

                }
                objRecepcionR.resumen = Convert.ToBoolean(fila["resumen"]);
                 DataTable analisisDTR = dd.buscaranalisis(objRecepcionR.id);
                if (objRecepcionR.resumen && analisisDTR.Rows.Count > 0) {
                
                    
                    
                        if (objRecepcionR.id_producto == 3)
                        {
                            objRecepcionR.gdt = (Convert.ToDecimal(analisisDTR.Select("id_item=" + 3)[0].ItemArray[5]) + Convert.ToDecimal(analisisDTR.Select("id_item=" + 4)[0].ItemArray[5])).ToString();
                        }
                        else
                        {

                            objRecepcionR.gdt = analisisDTR.Select("id_item=" + 7).Count() > 0 ? analisisDTR.Select("id_item=" + 7)[0].ItemArray[5].ToString() : "0";

                        }
                        objRecepcionR.humedad = analisisDTR.Select("id_item=" + 1)[0].ItemArray[5].ToString();
                        objRecepcionR.impureza = analisisDTR.Select("id_item=" + 2)[0].ItemArray[5].ToString();
                    

                }
                else
                {
                    objRecepcionR.humedad = "S/A";
                    objRecepcionR.impureza = "S/A";
                    objRecepcionR.gdt = "S/A";
                }
                if (!Equals(cbSilos.SelectedItem, null))
                {
                    objRecepcionR.nombresilo = silosDT.Select("id=" + objRecepcionR.id_silos).Count() > 0 ? silosDT.Select("id=" + objRecepcionR.id_silos)[0].ItemArray[1].ToString() : "S/S";
                    //objRecepcionR.nombresilo = ((System.Data.DataRowView)(cbSilos.SelectedItem)).Row.ItemArray[1].ToString();
                }
                return objRecepcionR;



            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un llenando objeto recepcion. Error Contacte al Administrador del Sistema", ex);

            }
        }

    }
}
