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
    public partial class InventarioArticulos : Form
    {
        historicoData hd = new historicoData();
        DataTable dt = new DataTable();
        List<detalle_articuloBO> lista = new List<detalle_articuloBO>();
        List<detalle_articuloBO> listaFiltrada = new List<detalle_articuloBO>();
        private readonly string empresa = ConfigurationManager.AppSettings["Empresa"];
        private readonly string direccionempresa = ConfigurationManager.AppSettings["direccionEmpresa"];
        public InventarioArticulos()
        {
            InitializeComponent();
            llenargrid();
            llenararticulos();
            llenarEstatntes();
        }


       void llenargrid()
        {


            try
            {
                dt = hd.buscarInventarioArticulos();
                dgvInventario.DataSource = new DataTable();
                lista= new List<detalle_articuloBO>();
                if (dt.Rows.Count > 0)
                {
                

                    foreach (DataRow fila in dt.Rows)
                    {

                        detalle_articuloBO deta = new detalle_articuloBO();

                        deta.codigo = Convert.ToInt16(fila["id"]);





                        deta.entrada = Convert.ToInt32(fila["Total Entrada"]);
                        deta.salida = Convert.ToInt32(fila["Total Salida"]);
                        deta.unidad = fila["Unidad"].ToString();
                        deta.descripcion = fila["descripcion"].ToString();
                        deta.cantidad = deta.entrada - deta.salida;
                        deta.estante = fila["Estante"].ToString();
                        deta.seccion = fila["Sección"].ToString();
                        deta.Precio = Convert.ToDecimal(fila["Precio"]);



                        //  deta.Precio =Math.Round( Convert.ToDecimal((totalSumaPrecios.cantidades * totalSumaPrecios.precios)/ totalSumaPrecios.cantidades),2);
                        deta.Total = deta.cantidad * deta.Precio;

                        lista.Add(deta);
                    }
                        int cantidadTotalArticulo = 0;
                        decimal costoPromedio =decimal.Zero;
                    int id_articulo = 0;
                    foreach (detalle_articuloBO fila in lista)
                    {
                      
                        if(fila.codigo!=id_articulo)
                        {
                          cantidadTotalArticulo = lista.Where(p => (p.entrada - p.salida) > 0 && p.codigo ==fila.codigo).GroupBy(r => new { r.codigo }).Select(g => g.Sum(s => s.cantidad)).ToList().FirstOrDefault();
                          decimal sumaCostos = lista.Where(p => (p.entrada - p.salida) > 0 && p.codigo == fila.codigo).GroupBy(r => new { r.codigo }).Select(g => g.Sum(s => s.Precio)).ToList().FirstOrDefault();
                          costoPromedio = Math.Round( sumaCostos / cantidadTotalArticulo,2);
                        }

                        fila.Precio = costoPromedio;
                     fila.Total = Math.Round( fila.cantidad * fila.Precio,2);

                        id_articulo = fila.codigo;
                    }
                    listaFiltrada = new List<detalle_articuloBO>();
                    listaFiltrada = lista;

                    dgvInventario.DataSource = listaFiltrada;
                    configuraremcabezadogrid();



                }
                else
                {

                    Base.msjAdvertencia("No se encontraron resultados.");

                }


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
                DataTable resultadodt = ad.buscar(0, string.Empty, 0,
                  DateTimePicker.MinimumDateTime);


                if (resultadodt.Rows.Count > 0)
                {
                    cbArticulos.DataSource = resultadodt;

                    cbArticulos.DisplayMember = "Descripción";
                    cbArticulos.ValueMember = "id";





                }
                else
                {

                    Base.msjAdvertencia("No se encontraron articulos.");

                }

                cbArticulos.SelectedValue = -1;

            }
            catch (Exception ex)
            {

                Base.msjError(ex.Message);
            }

        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                if (listaFiltrada.Count == 0)
                    return;
                reporteForm rf = new reporteForm(new List<analisis>());
                rf.inventario = true;
                rf.listaDetalle = listaFiltrada;
                MovimientoBO movimientoBo = new MovimientoBO();
                movimientoBo.empresa = empresa;
                movimientoBo.direccion_empresa = direccionempresa;
                List<MovimientoBO> listaMovimiento = new List<MovimientoBO>();
          
                listaMovimiento.Add(movimientoBo);

                rf.rvReporte.ProcessingMode = ProcessingMode.Local;



                   rf.rvReporte.LocalReport.ReportEmbeddedResource = "principal.inventarioArticulo.rdlc";
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

        private void cbArticulos_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            try
            {
                int id_articulo = 0;
                string estante = string.Empty;
                string seccion = string.Empty;
                if (!Equals(cbArticulos.SelectedItem, null))
                {
                    id_articulo = Convert.ToInt16(((System.Data.DataRowView)(cbArticulos.SelectedItem)).Row.ItemArray[0]);
                }
                if (!Equals(cbEstante.SelectedItem, null))
                {
                    estante = ((System.Data.DataRowView)(cbEstante.SelectedItem)).Row.ItemArray[1].ToString();
                }
                if (!Equals(cbSeccion.SelectedItem, null))
                {
                    seccion = ((System.Data.DataRowView)(cbSeccion.SelectedItem)).Row.ItemArray[1].ToString();
                }


                dgvInventario.DataSource = new DataTable();
                listaFiltrada = new List<detalle_articuloBO>();

                listaFiltrada = lista.Where(t => (id_articulo != 0 ? (t.codigo == id_articulo) : true) && (!string.IsNullOrWhiteSpace(seccion) ? (t.seccion == seccion) : true) && (!string.IsNullOrWhiteSpace(estante) ? (t.estante == estante) : true)).ToList();
                dgvInventario.DataSource = listaFiltrada;

                configuraremcabezadogrid();

            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }
        }

        private void configuraremcabezadogrid()
        {
            dgvInventario.Columns["codigo"].HeaderText = "Código";
            dgvInventario.Columns["descripcion"].HeaderText = "Artículo";
            dgvInventario.Columns["entrada"].HeaderText = "Total Entrada";
            dgvInventario.Columns["salida"].HeaderText = "Total Salida";
            dgvInventario.Columns["unidad"].HeaderText = "Unidad";
            dgvInventario.Columns["estante"].HeaderText = "Estante";
            dgvInventario.Columns["seccion"].HeaderText = "Sección";
            dgvInventario.Columns["cantidad"].HeaderText = "Total";
            dgvInventario.Columns["Total"].HeaderText = "Monto Total";
            dgvInventario.Columns["id_seccion"].Visible = false;
            dgvInventario.Columns["id_estante"].Visible = false;
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

                cbEstante.SelectedValue = -1;
            }
            catch (Exception ex)
            {

                Base.msjError(ex.Message);
            }

        }

        private void btnTodos_Click(object sender, EventArgs e)
        {
            try
            {
                llenargrid();


            }
            catch (Exception ex)
            {

                Base.msjError(ex.Message);
            }


        }

        void llenarSeccion(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                cbSeccion.DataSource = dt;
                cbSeccion.DisplayMember = "descripcion";
                cbSeccion.ValueMember = "id";
                cbSeccion.SelectedValue = -1;
            }


        }

        private void cbEstante_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SeccionArticuloData ed = new SeccionArticuloData();
            DataTable dt = ed.buscar(Convert.ToInt32(((ListControl)sender).SelectedValue));
            if (dt.Rows.Count == 0)
                return;

            llenarSeccion(dt);
        }
    }
    class totales
    {
        int id { get; set; }
        public decimal  cantidades { get; set; }
        public decimal precios { get; set; }


    }
}
