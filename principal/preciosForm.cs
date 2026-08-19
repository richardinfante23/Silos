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
    public partial class preciosForm : Form
    {
        public preciosForm(List<detalle_articuloBO> lista)
        {
            InitializeComponent();
            UiTheme.ApplyPickerDialog(this, Precios, cbPrecios);
            llenarPrecios(lista);
            cbPrecios.KeyPress += cbseccion_KeyPress;
        }

        void llenarPrecios(List<detalle_articuloBO> dt)
        {
            if (dt.Count > 0)
            {
                cbPrecios.DataSource = dt;

                cbPrecios.DisplayMember = "Precio";
                cbPrecios.ValueMember = "Precio";





            }


        }
        private void cbseccion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (int)Keys.Enter)
            {

                Close();
            }
        }
    }
}
