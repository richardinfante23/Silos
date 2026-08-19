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
    public partial class EstanteForm : Form
    {
        public EstanteForm( List<detalle_articuloBO> dt)
        {
            InitializeComponent();
            UiTheme.ApplyPickerDialog(this, label1, cbEstante);
            llenarEstante(dt);
            cbEstante.KeyPress += CbEstante_KeyPress;
        }

        private void CbEstante_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (int)Keys.Enter)
            {

                Close();
            }


        }

        void llenarEstante(List<detalle_articuloBO> dt)
        {
            if (dt.Count > 0)
            {
                cbEstante.DataSource = dt;

                cbEstante.DisplayMember = "Estante";
                cbEstante.ValueMember = "id_estante";

            }else
            {

                Base.msjError("Artículo sin Inventario");
            }
      


        }
    }
}
