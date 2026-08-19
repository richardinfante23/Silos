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
    public partial class transporteForm : Form
    {
        transportesData td = new transportesData();
        conductoresData cd = new conductoresData();
        public int id = 0;
        public bool actualizar = false;
        public transporteForm()
        {
            InitializeComponent();
            Load += (s, e) => UiTheme.ApplyForm(this);
        }

        public transporteForm(bool prmactualizar)
        {
            InitializeComponent();

            actualizar = prmactualizar;
            Load += (s, e) => UiTheme.ApplyForm(this);
        }

        public transporteForm(int prmid, string prmrif, string prmnombre)
        {
            InitializeComponent();
            id = prmid;
            txtRif.Text = prmrif ?? string.Empty;
            txtNombre.Text = prmnombre ?? string.Empty;
            actualizar = true;
            btnEliminar.Visible = true;
            Text = "Editar transporte";
            Load += (s, e) => UiTheme.ApplyForm(this);
        }
        private bool validateRegistrarOrEditar(string etiqueta)
        {
            return ValidacionFormulario.validar(this.Controls,etiqueta);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {

                if (!validateRegistrarOrEditar("validar"))
                    return;
                if (!actualizar)
                {
                    td.insertar(txtRif.Text, txtNombre.Text);
                   
                    Base.msjExito("Registro guardado exitosamente.");


                }
                else
                {
                    if (id == 0)
                    {
                        Base.msjError("Ha ocurrido un error. Contacte al administrador del sistema.");
                    }
                    td.actualizar(id, txtRif.Text, txtNombre.Text, false);
                    Base.msjExito("Registro guardado exitosamente.");
                }
                Close();
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "guardar el transporte");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
              try
            {
                if (cd.buscarconductorxidtransporte(id).Rows.Count > 0)
                {

                    Base.msjError("El registro no puede ser eliminado porque se encuentra asociado a conductores.");
                    return;
                }
                DialogResult result = Base.msjPreguntaCancel("Al eliminar este registro no podrá recuperarse. ¿Desea continuar con la eliminación?");

                if (result == DialogResult.Yes)
                {
                   
                    td.actualizar(id, txtRif.Text, txtNombre.Text, true);
                    Base.msjExito("Registro eliminado exitosamente.");


                    Close(); 
                }
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }

        
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
