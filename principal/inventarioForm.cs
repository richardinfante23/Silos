using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace principal
{
    public partial class inventarioForm : Form
    {
        DataTable dt = new DataTable();
        List<inventario> listaInventario = new List<inventario>();
        List<inventario> listaFiltrada = new List<inventario>();
        private readonly string empresa = ConfigurationManager.AppSettings["Empresa"];
        private readonly string direccionempresa = ConfigurationManager.AppSettings["direccionEmpresa"];
        private static readonly TextInfo textoEs = CultureInfo.GetCultureInfo("es-VE").TextInfo;
        public inventarioForm()
        {
            InitializeComponent();
            Load += InventarioForm_Load;
            llenarGrid();
            cargarSilos();
            cargarProducto();
        }

        private void InventarioForm_Load(object sender, EventArgs e)
        {
            UiTheme.ApplyForm(this);
            AlinearBotonesSuperiores();
        }

        private void AlinearBotonesSuperiores()
        {
            const int gap = 14;
            const int height = 34;

            if (btnFiltrar == null || btnTodos == null || btnImprimir == null)
                return;

            UiTheme.StyleCompactPrimaryButton(btnFiltrar, "Filtrar", 100, height);
            UiTheme.StyleCompactSecondaryButton(btnTodos, "Todos", 100, height);
            btnImprimir.Width = Math.Max(btnImprimir.Width, 118);
            btnImprimir.Height = height;

            int top = cbSilos != null
                ? cbSilos.Top + Math.Max(0, (cbSilos.Height - height) / 2)
                : Math.Max(8, dgvInventario.Top - height - 8);

            int x = cbSilos != null ? cbSilos.Right + gap : btnFiltrar.Left;
            btnFiltrar.SetBounds(x, top, 100, height);
            btnTodos.SetBounds(btnFiltrar.Right + gap, top, 100, height);
            btnImprimir.SetBounds(btnTodos.Right + gap, top, btnImprimir.Width, height);

            btnFiltrar.Anchor = AnchorStyles.Top;
            btnTodos.Anchor = AnchorStyles.Top;
            btnImprimir.Anchor = AnchorStyles.Top;
        }
        private void llenarGrid()
        {
            try {
                historicoData hd = new historicoData();
                dgvInventario.DataSource = new DataTable();
                dt = hd.buscar();
                listaFiltrada = new List<inventario>();
                listaInventario = new List<inventario>();


                foreach (DataRow fila in dt.Rows)
                {
                    inventario inv = new inventario();

                    inv.nombre_producto = FormatoTitulo(fila["producto"].ToString());
                    inv.nombresilo = FormatoTitulo(fila["silos"].ToString());
                    inv.peso_neto = Convert.ToDecimal(fila["peso_neto"]);
                    inv.peso_acondicionado = Convert.ToDecimal(fila["peso_acondicionado"]);
                    inv.empresa = empresa;
                    inv.direccion_empresa = direccionempresa;
                    listaInventario.Add(inv);
                }
                dgvInventario.DataSource = listaInventario;
                listaFiltrada = listaInventario;
                ConfigurarColumnasInventario();
                lbTotales.Text = string.Empty;
            }
            catch (Exception ex) {


                throw new Exception("Ha ocurrido un llenando grid. Error Contacte al Administrador del Sistema", ex);
            }

        }
        #region Silos
        void cargarSilos()
        {
            try
            {
                silosData cd = new silosData();
                DataTable productoDT = cd.buscar();
                if (productoDT.Rows.Count > 0)
                {
                    cbSilos.DataSource = productoDT;

                    cbSilos.DisplayMember = "nombre";
                    cbSilos.ValueMember = "id";

                }
                // productoDT.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando Conductor. Error Contacte al Administrador del Sistema", ex);

            }


        }
        #endregion
        private void dgvInventario_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
                    cbProductos.DataSource = productoDT;

                    cbProductos.DisplayMember = "nombre";
                    cbProductos.ValueMember = "id";

                }
                // productoDT.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando cargando producto. Error Contacte al Administrador del Sistema", ex);

            }
            cbProductos.Text = string.Empty;

        }
        #endregion

        private void btnTodos_Click(object sender, EventArgs e)
        {
            try
            {

          
                    llenarGrid();
              

             }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }

    }

    private void cbProductos_SelectionChangeCommitted(object sender, EventArgs e)
        {

            try
            {
                if (Equals(cbProductos.SelectedItem,null))
                {
                    return;
                }
                string nombre = ((System.Data.DataRowView)(cbProductos.SelectedItem)).Row.ItemArray[1].ToString();
               // var resltado=dt.Select("producto='"+nombre+"'");
            
               
                dgvInventario.DataSource = new DataTable();
                listaFiltrada=new List<inventario>();
                listaFiltrada = listaInventario.Where(t => t.nombre_producto == FormatoTitulo(nombre)).ToList();
                dgvInventario.DataSource = listaFiltrada;
                ConfigurarColumnasInventario();
                sumarTotales();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un error seleccionando productos. Error Contacte al Administrador del Sistema", ex);

            }
        }

        void sumarTotales()
        {
            decimal sumaNeto = decimal.Zero;
            decimal sumaAcondicionado = decimal.Zero;
            foreach (DataGridViewRow fila in dgvInventario.Rows)
            {

                sumaNeto += Convert.ToDecimal(fila.Cells["peso_neto"].Value);
                sumaAcondicionado += Convert.ToDecimal(fila.Cells["peso_acondicionado"].Value);

            }

            lbTotales.Text = "Total Neto: "+sumaNeto+" Total Acondicionado: "+sumaAcondicionado;
    }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (dgvInventario.Rows.Count > 0)
            {

              

            

                reporteForm rf = new reporteForm(new List<analisis>());
                rf.listaAnalisis = new List<analisis>();
                
        


                rf.rvReporte.ProcessingMode = ProcessingMode.Local;


                ReportDataSource rds = new ReportDataSource("Boleta", listaFiltrada);
                rf.rvReporte.LocalReport.DataSources.Clear();
                rf.rvReporte.LocalReport.DataSources.Add(rds);
                
                rf.rvReporte.LocalReport.ReportEmbeddedResource = "principal.Inventario.rdlc";
              
                rf.rvReporte.LocalReport.Refresh();
                rf.ShowDialog();


            }
            else
            {

                Base.msjAdvertencia("No existen elementos para realizar exportación.");
            }
        }

        private static string FormatoTitulo(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;
            return textoEs.ToTitleCase(valor.Trim().ToLowerInvariant());
        }

        private void ConfigurarColumnasInventario()
        {
            if (dgvInventario.Columns.Count == 0)
                return;

            if (dgvInventario.Columns.Contains("nombre_producto"))
                dgvInventario.Columns["nombre_producto"].HeaderText = "Producto";
            if (dgvInventario.Columns.Contains("nombresilo"))
                dgvInventario.Columns["nombresilo"].HeaderText = "Silo";
            if (dgvInventario.Columns.Contains("peso_neto"))
                dgvInventario.Columns["peso_neto"].HeaderText = "Peso Neto (Kg)";
            if (dgvInventario.Columns.Contains("peso_acondicionado"))
                dgvInventario.Columns["peso_acondicionado"].HeaderText = "Peso Acondicionado (Kg)";
            if (dgvInventario.Columns.Contains("empresa"))
                dgvInventario.Columns["empresa"].Visible = false;
            if (dgvInventario.Columns.Contains("direccion_empresa"))
                dgvInventario.Columns["direccion_empresa"].Visible = false;

            dgvInventario.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            try
            {

                string nombre = string.Empty;
                string silos = string.Empty;
                if (!Equals(cbProductos.SelectedItem, null))
                {
                    nombre = ((DataRowView)(cbProductos.SelectedItem)).Row.ItemArray[1].ToString();
                }
                if (!Equals(cbSilos.SelectedItem, null))
                {
                    silos = ((DataRowView)(cbSilos.SelectedItem)).Row.ItemArray[1].ToString();
                }



                dgvInventario.DataSource = new DataTable();
                listaFiltrada = new List<inventario>();
                listaFiltrada = listaInventario.Where(t => (!string.IsNullOrWhiteSpace(nombre) ? t.nombre_producto == FormatoTitulo(nombre) : true) && (!string.IsNullOrWhiteSpace(silos) ? t.nombresilo == FormatoTitulo(silos) : true)).ToList();
                dgvInventario.DataSource = listaFiltrada;
                ConfigurarColumnasInventario();
                sumarTotales();

            }
            catch(Exception ex)
            {

                Base.msjError(ex.Message);
            }
        }
    }

  
}

class inventario
{

    public string nombre_producto { get; set; }
   public string nombresilo { get; set; }
   public decimal peso_neto { get; set; }
    public decimal peso_acondicionado { get; set; }
    public string empresa { get; set; }
    public string direccion_empresa { get; set; }
}