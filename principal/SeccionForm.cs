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
    public partial class SeccionForm : Form
    {
       
        public SeccionForm(List<detalle_articuloBO> prmdt)
        {
            InitializeComponent();
            //cbseccion.SelectionChangeCommitted += Cbseccion_SelectedValueChanged;
            cbseccion.KeyPress += cbseccion_KeyPress;
            llenarSeccion(prmdt);
        }

   

        void llenarSeccion(List<detalle_articuloBO> dt)
        {
            if (dt.Count() > 0)
            {

                cbseccion.DataSource = dt;

                cbseccion.DisplayMember = "seccion";
                cbseccion.ValueMember = "id_seccion";





            }


        }

   
        private void cbseccion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(int)Keys.Enter)
            {

                Close();
            }
        }
    }
}
