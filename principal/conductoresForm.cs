using System;
using System.Data;
using System.Drawing;
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
            UiTheme.ApplyAppIcon(this);
            AplicarLayoutConductorModerno();
            cargarTransporte();
        }

        private void AplicarLayoutConductorModerno()
        {
            // Reorganiza la ventana para que se vea igual de moderna que el resto.
            // No cambia la lógica de guardar/editar; solo el layout.
            if (txtCedula == null || txtNombre == null || cbTransporte == null) return;

            Text = actualizar ? "Editar conductor" : "Nuevo conductor";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = true;
            ClientSize = new Size(720, 330);
            BackColor = UiTheme.Surface;
            Font = UiTheme.FontRegular;

            UiTheme.StyleTextBox(txtCedula);
            UiTheme.StyleTextBox(txtNombre);
            cbTransporte.FlatStyle = FlatStyle.System;
            cbTransporte.BackColor = UiTheme.Card;
            cbTransporte.ForeColor = UiTheme.TextPrimary;
            cbTransporte.Font = UiTheme.FontRegular;
            cbTransporte.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTransporte.IntegralHeight = true;
            cbTransporte.MinimumSize = new Size(0, 32);

            var comboTransporte = UiTheme.WrapInputFrame(cbTransporte);

            // Título.
            if (label4 != null)
            {
                label4.Text = "Conductores";
                label4.AutoSize = false;
                label4.Dock = DockStyle.Fill;
                label4.Font = UiTheme.FontTitle;
                label4.ForeColor = UiTheme.Primary;
                label4.BackColor = Color.Transparent;
                label4.Margin = new Padding(0, 0, 0, 6);
                label4.TextAlign = ContentAlignment.MiddleLeft;
            }

            // Caja principal.
            var host = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(18, 14, 18, 12),
                BackColor = UiTheme.Surface
            };
            host.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            host.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            host.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));

            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Card,
                Padding = new Padding(16, 12, 16, 12)
            };

            // Campos.
            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = UiTheme.Card
            };
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));

            fields.Controls.Add(CrearFilaCampoDialogo(label1, "Cédula", txtCedula), 0, 0);
            fields.Controls.Add(CrearFilaCampoDialogo(label2, "Nombre", txtNombre), 0, 1);

            var transporteCell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = UiTheme.Card
            };
            transporteCell.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            transporteCell.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

            if (label3 != null)
            {
                label3.Text = "Transporte:";
                label3.AutoSize = false;
                label3.Dock = DockStyle.Fill;
                label3.TextAlign = ContentAlignment.MiddleLeft;
                label3.ForeColor = UiTheme.TextMuted;
                label3.Font = UiTheme.FontRegular;
                label3.BackColor = Color.Transparent;
            }

            // Combo + botones de transporte.
            var transporteRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = UiTheme.Card
            };
            transporteRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            transporteRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            transporteRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            transporteRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

            comboTransporte.Dock = DockStyle.Fill;
            comboTransporte.Margin = new Padding(0, 4, 8, 4);

            // Estilos compactos para los botones del lado.
            UiTheme.StyleCompactPrimaryButton(btnAgregarTransporte, "Agregar", 108, 32);
            UiTheme.StyleCompactSecondaryButton(button1, "Editar", 108, 32);

            btnAgregarTransporte.Margin = new Padding(0, 6, 8, 0);
            button1.Margin = new Padding(0, 6, 0, 0);

            transporteRow.Controls.Add(comboTransporte, 0, 0);
            transporteRow.Controls.Add(btnAgregarTransporte, 1, 0);
            transporteRow.Controls.Add(button1, 2, 0);

            if (label3 != null)
                transporteCell.Controls.Add(label3, 0, 0);
            transporteCell.Controls.Add(transporteRow, 0, 1);
            fields.Controls.Add(transporteCell, 0, 2);

            card.Controls.Add(fields);

            // Acciones.
            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 10, 0, 0),
                BackColor = UiTheme.Surface
            };

            btnEliminar.Visible = actualizar;
            btnEliminar.Enabled = actualizar;

            actions.Controls.Add(btnGuardar);
            actions.Controls.Add(btnCancelar);
            if (actualizar)
                actions.Controls.Add(btnEliminar);

            UiTheme.ApplyActionButtons(actions);

            // Aplicar.
            Controls.Clear();
            if (label4 != null)
                host.Controls.Add(label4, 0, 0);
            host.Controls.Add(card, 0, 1);
            host.Controls.Add(actions, 0, 2);
            Controls.Add(host);

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;
        }

        private static Control CrearFilaCampoDialogo(Label etiqueta, string texto, Control campo)
        {
            var cell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0, 0, 0, 6),
                BackColor = UiTheme.Card
            };
            cell.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            cell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            if (etiqueta != null)
            {
                etiqueta.Text = texto;
                etiqueta.AutoSize = false;
                etiqueta.Dock = DockStyle.Fill;
                etiqueta.TextAlign = ContentAlignment.MiddleLeft;
                etiqueta.ForeColor = UiTheme.TextMuted;
                etiqueta.Font = UiTheme.FontRegular;
                etiqueta.BackColor = Color.Transparent;
                cell.Controls.Add(etiqueta, 0, 0);
            }

            if (campo != null)
            {
                campo.Dock = DockStyle.Fill;
                cell.Controls.Add(campo, 0, 1);
            }

            return cell;
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

                cbTransporte.FlatStyle = FlatStyle.System;
                cbTransporte.DropDownStyle = ComboBoxStyle.DropDownList;
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
                    Base.msjExito("Registro guardado exitosamente.");
                }
                else
                {
                    if (id == 0)
                        Base.msjError("Ha ocurrido un error. Contacte al administrador del sistema.");
                    cd.actualizar(id, Convert.ToDecimal(txtCedula.Text), txtNombre.Text, idTransporte, false);
                }
                Close();
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "guardar el conductor");
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
                    Base.msjError("El registro no puede ser eliminado porque se encuentra asociado a trámites de recepción o despacho.");
                    return;
                }
                DialogResult result = Base.msjPreguntaCancel("Al eliminar este registro no podrá recuperarse. ¿Desea continuar con la eliminación?");
                if (result == DialogResult.Yes)
                {
                    cd.actualizar(id, Convert.ToDecimal(txtCedula.Text), txtNombre.Text, ObtenerIdTransporteSeleccionado(), true);
                    Base.msjExito("Registro eliminado exitosamente.");
                    eliminado = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
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
                Base.msjError(ex.Message);
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
                    Base.msjAdvertencia("Debe seleccionar un transporte.");
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
                Base.msjError(ex.Message);
            }
        }
    }
}
