using System;
using System.Data;
using System.Windows.Forms;

namespace principal
{
    public partial class conductoresForm : Form
    {
        conductoresData cd = new conductoresData();
        transportesData td = new transportesData();
        public string nombre;
        public int cedula;
        public bool actualizar = false;
        public int id = 0;
        public bool eliminado = false;
        private int? idTransporteSeleccionado = null;

        public conductoresForm()
        {
            InitializeComponent();
        }

        public conductoresForm(string prmcedula)
        {
            InitializeComponent();
            txtCedula.Text = prmcedula;
        }

        public conductoresForm(int prmid, string prmcedula, string prmnombre, int prm_idtransporte)
        {
            InitializeComponent();
            id = prmid;
            actualizar = true;
            cedula = Convert.ToInt32(prmcedula);
            txtNombre.Text = nombre = prmnombre;
            txtCedula.Text = prmcedula;
            idTransporteSeleccionado = prm_idtransporte;
            txtCedula.Enabled = false;
            btnEliminar.Enabled = true;
            btnEliminar.Visible = true;
        }

        private void conductoresForm_Load(object sender, EventArgs e)
        {
            UiTheme.ApplyForm(this);
            AcomodarBotonesCompaneros();
            cargarTransporte();
        }

        private void AcomodarBotonesCompaneros()
        {
            // Mantener Agregar/Editar a la derecha del combo, sin solaparlo.
            const int gap = 8;
            btnAgregarTransporte.Width = 110;
            btnAgregarTransporte.Height = 28;
            button1.Width = 100;
            button1.Height = 28;

            btnAgregarTransporte.Left = cbTransporte.Right + gap;
            btnAgregarTransporte.Top = cbTransporte.Top - 2;
            button1.Left = btnAgregarTransporte.Right + gap;
            button1.Top = btnAgregarTransporte.Top;

            int needed = button1.Right + 16;
            if (ClientSize.Width < needed)
                Width += needed - ClientSize.Width;
        }

        private void cargarTransporte()
        {
            try
            {
                DataTable dt = td.buscar();
                PrepararDisplayTransporte(dt);

                int? keep = idTransporteSeleccionado;
                if (!keep.HasValue && cbTransporte.SelectedValue != null && cbTransporte.SelectedValue != DBNull.Value)
                    keep = Convert.ToInt32(cbTransporte.SelectedValue);

                cbTransporte.BeginUpdate();
                cbTransporte.DataSource = null;
                cbTransporte.FormattingEnabled = true;
                cbTransporte.DropDownStyle = ComboBoxStyle.DropDownList;
                // Orden correcto en WinForms: miembros antes del DataSource.
                cbTransporte.DisplayMember = "display";
                cbTransporte.ValueMember = "id";
                cbTransporte.DataSource = dt;
                cbTransporte.EndUpdate();

                if (keep.HasValue)
                {
                    // SelectedValue falla a veces por tipo; buscar por id.
                    int index = -1;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(dt.Rows[i]["id"]) == keep.Value)
                        {
                            index = i;
                            break;
                        }
                    }
                    cbTransporte.SelectedIndex = index >= 0 ? index : (dt.Rows.Count > 0 ? 0 : -1);
                }
                else if (dt.Rows.Count > 0)
                {
                    cbTransporte.SelectedIndex = 0;
                }

                if (cbTransporte.SelectedValue != null && cbTransporte.SelectedValue != DBNull.Value)
                    idTransporteSeleccionado = Convert.ToInt32(cbTransporte.SelectedValue);
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando Transporte. Error Contacte al Administrador del Sistema", ex);
            }
        }

        private static void PrepararDisplayTransporte(DataTable dt)
        {
            if (dt == null) return;
            if (!dt.Columns.Contains("display"))
                dt.Columns.Add("display", typeof(string));

            bool hasRif = dt.Columns.Contains("rif");

            foreach (DataRow row in dt.Rows)
            {
                string nombre = Convert.ToString(row["nombre"]).Trim();
                string rif = hasRif ? Convert.ToString(row["rif"]).Trim() : string.Empty;
                string idTxt = Convert.ToString(row["id"]);

                bool nombreInutil =
                    string.IsNullOrWhiteSpace(nombre)
                    || nombre == "0"
                    || nombre == idTxt;

                if (nombreInutil)
                {
                    if (!string.IsNullOrWhiteSpace(rif) && !string.Equals(rif, "0", StringComparison.OrdinalIgnoreCase))
                        row["display"] = rif;
                    else
                        row["display"] = "Transporte #" + idTxt;
                }
                else if (!string.IsNullOrWhiteSpace(rif)
                    && !string.Equals(rif, "NO APLICA", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(rif, "0", StringComparison.OrdinalIgnoreCase))
                {
                    row["display"] = nombre + " — " + rif;
                }
                else
                {
                    row["display"] = nombre;
                }
            }
        }

        private int ObtenerIdTransporteSeleccionado()
        {
            if (cbTransporte.SelectedValue != null && cbTransporte.SelectedValue != DBNull.Value)
                return Convert.ToInt32(cbTransporte.SelectedValue);

            var row = cbTransporte.SelectedItem as DataRowView;
            if (row != null)
                return Convert.ToInt32(row["id"]);

            throw new Exception("Debe seleccionar un transporte.");
        }

        private bool validateRegistrarOrEditar(string etiqueta)
        {
            return ValidacionFormulario.validar(this.Controls, etiqueta);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!validateRegistrarOrEditar("validar"))
                    return;

                int idTransporte = ObtenerIdTransporteSeleccionado();

                if (!actualizar)
                {
                    cd.insertar(Convert.ToDecimal(txtCedula.Text), txtNombre.Text, idTransporte, false);
                    DataTable conductorDT = cd.buscar(Convert.ToDecimal(txtCedula.Text));
                    if (conductorDT.Rows.Count > 0)
                    {
                        foreach (DataRow fila in conductorDT.Rows)
                        {
                            id = Convert.ToInt32(fila["id"]);
                            break;
                        }
                    }
                    MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (id == 0)
                        MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador");
                    cd.actualizar(id, Convert.ToDecimal(txtCedula.Text), txtNombre.Text, idTransporte, false);
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void soloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
                e.Handled = false;
            else if (Char.IsControl(e.KeyChar))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void soloLetras_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
                e.Handled = true;
            else if (Char.IsControl(e.KeyChar))
                e.Handled = false;
            else
                e.Handled = false;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            txtNombre.Text = nombre = string.Empty;
            txtCedula.Text = string.Empty;
            id = 0;
            Close();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cd.buscartramitesxidconductor(id).Rows.Count > 0)
                {
                    MessageBox.Show("Registro no puede ser eliminado ya que se encuentra asociado a Tramites de recepcion o despacho", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                DialogResult result = MessageBox.Show("Recuerde al eliminar registro no podra ser recuperado, ¿Desea realmente eliminar registro?", "Advertencia", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    cd.actualizar(id, Convert.ToDecimal(txtCedula.Text), txtNombre.Text, ObtenerIdTransporteSeleccionado(), true);
                    MessageBox.Show("Registro Eliminado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    eliminado = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgregarTransporte_Click(object sender, EventArgs e)
        {
            try
            {
                using (transporteForm tf = new transporteForm())
                {
                    tf.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            cargarTransporte();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!validateRegistrarOrEditar("validareditar"))
                    return;

                if (cbTransporte.SelectedItem == null)
                {
                    MessageBox.Show("Debe seleccionar un transporte.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataRowView row = (DataRowView)cbTransporte.SelectedItem;
                using (transporteForm tf = new transporteForm(true))
                {
                    tf.actualizar = true;
                    tf.id = Convert.ToInt32(row["id"]);
                    tf.txtNombre.Text = Convert.ToString(row["nombre"]);
                    tf.txtRif.Text = row.Row.Table.Columns.Contains("rif")
                        ? Convert.ToString(row["rif"])
                        : string.Empty;
                    tf.btnEliminar.Visible = true;
                    tf.ShowDialog(this);
                    idTransporteSeleccionado = tf.id;
                }
                cargarTransporte();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
