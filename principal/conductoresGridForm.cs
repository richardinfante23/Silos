using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace principal
{
    public partial class conductoresGridForm : Form
    {
        private readonly string letrasNaturales = ConfigurationManager.AppSettings["letrasNaturales"];
        conductoresData cd = new conductoresData();
        public string cedula = string.Empty;
        bool desdeotraventana = false;
        DataTable dt = new DataTable();
        List<conductor> lista = new List<conductor>();
        private GridHeaderFilter headerFilter;

        int id_conductor = 0;

        string nombre = string.Empty;
        int idTransporte = 0;
        public conductoresGridForm(bool prmdesdeotraventana)
        {
            desdeotraventana = prmdesdeotraventana;
            InitializeComponent();
        }

        private void conductoresGridForm_Load(object sender, EventArgs e)
        {
            UiTheme.ApplyForm(this);
            UiTheme.StylePrimaryButton(btnNuevo);
            UiTheme.StyleSecondaryButton(btnCancelar);
            Icon = UiTheme.GetAppIcon();
            Text = "Conductores";
            CatalogGridHelper.EnsureHeaderFilter(dgvProdcutores, ref headerFilter);
            cargarConductor();

            if (desdeotraventana)
            {
                txtNombre.AutoCompleteCustomSource = CargarDatos();
                txtNombre.AutoCompleteMode = AutoCompleteMode.Suggest;
                txtNombre.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            else
            {
                label.Visible = false;
                txtNombre.Visible = false;
            }
        }

        private AutoCompleteStringCollection CargarDatos()
        {
            AutoCompleteStringCollection datos = new AutoCompleteStringCollection();

            foreach (DataRow fila in dt.Rows)
            {
                conductor ObjConductor = new conductor();
                ObjConductor.cedula = fila["cedula"].ToString();
                ObjConductor.nombre = fila["nombre"].ToString();
                ObjConductor.idTransporte = Convert.ToInt32(fila["id_transporte"]);
                ObjConductor.id_conductor = Convert.ToInt32(fila["id"]);

                lista.Add(ObjConductor);
                datos.Add(fila["nombre"].ToString());
                
            }
          
          
     

            return datos;
        }
        void cargarConductor()
        {
            try
            {
                dt = cd.buscarGeneral();
                EnriquecerColumnaTransporte(dt);
                if (headerFilter == null)
                    headerFilter = new GridHeaderFilter(dgvProdcutores);
                headerFilter.Bind(dt);
                CatalogGridHelper.HideColumn(dgvProdcutores, "id");
                CatalogGridHelper.HideColumn(dgvProdcutores, "eliminado");
                CatalogGridHelper.HideColumn(dgvProdcutores, "id_transporte");
                CatalogGridHelper.SetHeader(dgvProdcutores, "cedula", "Cédula");
                CatalogGridHelper.SetHeader(dgvProdcutores, "nombre", "Nombre");
                if (dgvProdcutores.Columns.Contains("Transporte"))
                    CatalogGridHelper.SetHeader(dgvProdcutores, "Transporte", "Transporte");
                headerFilter.SyncHeaders();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando productor. Error Contacte al Administrador del Sistema", ex);
            }
        }

        /// <summary>
        /// El SP trae t.nombre como Transporte; si el nombre es "0"/vacío, muestra RIF o Transporte #id.
        /// </summary>
        private static void EnriquecerColumnaTransporte(DataTable table)
        {
            if (table == null) return;

            string colTransporte = table.Columns.Contains("Transporte")
                ? "Transporte"
                : (table.Columns.Contains("transporte") ? "transporte" : null);

            bool hasIdTransporte = table.Columns.Contains("id_transporte");
            if (colTransporte == null && !hasIdTransporte) return;

            // Mapa id -> etiqueta legible desde catálogo de transportes.
            var etiquetas = new Dictionary<int, string>();
            try
            {
                DataTable transportes = new transportesData().buscar();
                bool hasRif = transportes.Columns.Contains("rif");
                foreach (DataRow t in transportes.Rows)
                {
                    int tid = Convert.ToInt32(t["id"]);
                    string nom = Convert.ToString(t["nombre"]).Trim();
                    string rif = hasRif ? Convert.ToString(t["rif"]).Trim() : string.Empty;
                    etiquetas[tid] = FormatearEtiquetaTransporte(nom, rif, tid);
                }
            }
            catch
            {
                // Si falla el catálogo, se corrige solo con lo que venga en la fila.
            }

            if (colTransporte == null)
            {
                colTransporte = "Transporte";
                table.Columns.Add(colTransporte, typeof(string));
            }

            foreach (DataRow row in table.Rows)
            {
                int tid = 0;
                if (hasIdTransporte && row["id_transporte"] != DBNull.Value)
                    tid = Convert.ToInt32(row["id_transporte"]);

                string actual = Convert.ToString(row[colTransporte]).Trim();
                string etiqueta;
                if (tid > 0 && etiquetas.TryGetValue(tid, out etiqueta))
                    row[colTransporte] = etiqueta;
                else if (string.IsNullOrWhiteSpace(actual) || actual == "0")
                    row[colTransporte] = tid > 0 ? ("Transporte #" + tid) : "(Sin transporte)";
            }
        }

        private static string FormatearEtiquetaTransporte(string nombre, string rif, int id)
        {
            nombre = (nombre ?? string.Empty).Trim();
            rif = (rif ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(nombre) || nombre == "0")
            {
                if (!string.IsNullOrWhiteSpace(rif) &&
                    !string.Equals(rif, "NO APLICA", StringComparison.OrdinalIgnoreCase))
                    return rif;
                if (!string.IsNullOrWhiteSpace(rif))
                    return rif + " (#" + id + ")";
                return "Transporte #" + id;
            }

            if (!string.IsNullOrWhiteSpace(rif) &&
                !string.Equals(rif, "NO APLICA", StringComparison.OrdinalIgnoreCase))
                return nombre + " — " + rif;

            return nombre;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {


            try
            {
                conductoresForm cf = new conductoresForm();
              
                cf.actualizar = false;
                
                cf.ShowDialog();

                cargarConductor();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando productor. Error Contacte al Administrador del Sistema" + ex.Message);

            }

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dgvProdcutores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
             


                foreach (DataGridViewRow Seleccionado in dgvProdcutores.SelectedRows)
                {
                    id_conductor =Convert.ToInt32( Seleccionado.Cells["id"].Value);
                    nombre = Seleccionado.Cells["nombre"].Value.ToString();
                    cedula = Seleccionado.Cells["cedula"].Value.ToString();
                    idTransporte = Convert.ToInt32( Seleccionado.Cells["id_transporte"].Value);
                    
                    

                }
                if(desdeotraventana)
                {

                    Close();
                    return;
                }
                conductoresForm cf = new conductoresForm(id_conductor, cedula, nombre, idTransporte);
                cf.actualizar = true;
                cf.ShowDialog();

                cargarConductor();
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando conductor. Error Contacte al Administrador del Sistema" + ex.Message);

            }
        }

        private void txtNoambre_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (!desdeotraventana)
                {
                    return;
                }
                    if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrWhiteSpace(txtNombre.Text))
                    {

                        Base.msjAdvertencia("Debe llenar campo Nombre");
                        return;

                    }
                    List<conductor> listaUnica = new List<conductor>();
                    listaUnica = lista.Where(r => r.nombre.ToUpper() == txtNombre.Text).ToList();

                    if(listaUnica.Count>0)
                    {
                            if (listaUnica.Count() == 1)
                            {
                                id_conductor = listaUnica[0].id_conductor;
                                nombre = listaUnica[0].nombre;
                                 cedula = listaUnica[0].cedula;
                  
                                idTransporte = listaUnica[0].idTransporte; ;

                                Close();


                            }
                            else
                            {
                                Base.msjError("Se encontraron mas de un registro con el mismo nombre. Por favor seleccionarlos del Grid.");

                            }
                    }





                }
            }

            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
        }
    }
    class conductor
    {
        public int  id_conductor{get;set;}
        public int idTransporte { get; set; }

        public string nombre { get; set; }
        public string cedula { get; set; }

    }
}
