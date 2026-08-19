using Microsoft.Reporting.WinForms;
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
    public partial class reporteForm : Form
    {
        public List<analisis> listaAnalisis = new List<analisis>();
        public List<detalle_articuloBO> listaDetalle = new List<detalle_articuloBO>();
        public bool inventario = false;
        public reporteForm( List<analisis> prmlistaAnalisis)
        {
            InitializeComponent();
            listaAnalisis = prmlistaAnalisis;
        }
        

        private void reporteForm_Load(object sender, EventArgs e)
        {
            UiTheme.ApplyForm(this);
            this.rvReporte.LocalReport.SubreportProcessing +=new SubreportProcessingEventHandler(SubreportProcessingEventHandler);
            this.rvReporte.RefreshReport();
        }

        void SubreportProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            if (inventario)
            {
                e.DataSources.Add(new ReportDataSource("DataSet1", listaDetalle));
                return;
            }

            e.DataSources.Add(new ReportDataSource("boleta", listaAnalisis));
        }

    }
}
