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
    public partial class AuditoriaForm : Form
    {
        DataTable dt = new DataTable();
        DataTable dtd = new DataTable();
        AuditoriaData ad = new AuditoriaData();
        int codigo = 0;
        public AuditoriaForm()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtCodigo.Text))
                {

                    codigo = Convert.ToInt32(txtCodigo.Text);
                }

                gridEmcabezado();
                gridDetalles();
            }
            catch (Exception ex)
            {

                Base.msjError(ex.Message);
            }
        }

        private void gridEmcabezado()
        {
            try
            {
                dt = new DataTable();
                dt = ad.buscarauditoria(codigo,dtpDesde.Value, dtpHasta.Value);
                dgvAuditoria.DataSource = new DataTable();
                // lista = new List<detalle_articuloBO>();
                if (dt.Rows.Count > 0)
                {
                    dgvAuditoria.DataSource = dt;



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
        private void gridDetalles()
        {
            try
            {
                dtd = new DataTable();
                dtd = ad.buscarauditoriadetalle(codigo,dtpDesde.Value, dtpHasta.Value);
                dgvDetalles.DataSource = new DataTable();
                // lista = new List<detalle_articuloBO>();
                if (dtd.Rows.Count > 0)
                {
                    dgvDetalles.DataSource = dtd;



                }
                else
                {

                    Base.msjAdvertencia("No se encontraron detalles.");

                }


            }
            catch (Exception ex)
            {

                Base.msjError(ex.Message);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            dgvDetalles.DataSource = new DataTable();
            dgvAuditoria.DataSource = new DataTable();

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
    }
}
