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
    public partial class productorNatural : Form
    {
        public int id = 0;
       public bool actualizar = false;
       public bool eliminado = false;
       public bool destino = false;
       ProductorData pd = new ProductorData();
       destinoData dd = new destinoData();
        
        public productorNatural()
        {
            InitializeComponent();
            cargarAsociaciones();
            AplicarLayoutProductorNatural();
        }

        private void AplicarLayoutProductorNatural()
        {
            Text = destino ? "Cliente Natural" : "Productor Natural";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = true;
            ClientSize = new Size(560, 360);
            BackColor = UiTheme.Surface;
            Font = UiTheme.FontRegular;
            UiTheme.ApplyAppIcon(this);

            foreach (Control c in new Control[] { txtNombre, txtCedula })
                if (c is TextBox tb) UiTheme.StyleTextBox(tb);
            UiTheme.StyleComboBox(cbLetra);
            UiTheme.StyleComboBox(cbAsociaciones);

            UiTheme.StyleCompactPrimaryButton(btnAgregarAsociacion, "Agregar", 108, 32);
            UiTheme.StyleCompactSecondaryButton(btnEditarAsociacion, "Editar", 100, 32);
            btnEditarAsociacion.ForeColor = Color.White;
            btnEditarAsociacion.BackColor = UiTheme.Primary;
            btnEditarAsociacion.FlatAppearance.MouseOverBackColor = UiTheme.PrimaryDark;

            var host = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(18, 14, 18, 12),
                BackColor = UiTheme.Surface
            };
            host.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            host.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            host.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));

            lbNombre.Text = destino ? "Cliente Natural" : "Productor Natural";
            lbNombre.AutoSize = true;
            lbNombre.Font = UiTheme.FontTitle;
            lbNombre.ForeColor = UiTheme.Primary;
            lbNombre.BackColor = UiTheme.Surface;
            lbNombre.Dock = DockStyle.Fill;
            lbNombre.TextAlign = ContentAlignment.MiddleLeft;

            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Card,
                Padding = new Padding(16, 12, 16, 12)
            };

            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = UiTheme.Card
            };
            fields.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            fields.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            fields.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            fields.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));

            fields.Controls.Add(CrearFilaCampoDialogo(label1, "Nombre", txtNombre), 0, 0);

            var letraCedula = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = UiTheme.Card
            };
            letraCedula.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            letraCedula.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            letraCedula.Controls.Add(CrearFilaCampoDialogo(label2, "Letra", cbLetra), 0, 0);
            letraCedula.Controls.Add(CrearFilaCampoDialogo(s, "Cédula", txtCedula), 1, 0);
            fields.Controls.Add(letraCedula, 0, 1);

            var asociacionCampo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0)
            };
            asociacionCampo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            asociacionCampo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 116F));
            asociacionCampo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108F));
            cbAsociaciones.Dock = DockStyle.Fill;
            btnAgregarAsociacion.Anchor = AnchorStyles.Left;
            btnEditarAsociacion.Anchor = AnchorStyles.Left;
            btnAgregarAsociacion.Margin = new Padding(8, 2, 4, 2);
            btnEditarAsociacion.Margin = new Padding(4, 2, 0, 2);
            asociacionCampo.Controls.Add(cbAsociaciones, 0, 0);
            asociacionCampo.Controls.Add(btnAgregarAsociacion, 1, 0);
            asociacionCampo.Controls.Add(btnEditarAsociacion, 2, 0);
            fields.Controls.Add(CrearFilaCampoDialogo(label3, "Asociación", asociacionCampo), 0, 2);

            card.Controls.Add(fields);

            var acciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 10, 0, 0),
                BackColor = UiTheme.Surface
            };
            foreach (var btn in new[] { btnGuardar, btnCancelar, btnEliminar })
            {
                btn.Size = new Size(120, 38);
                btn.Margin = new Padding(0, 0, 10, 0);
                acciones.Controls.Add(btn);
            }
            UiTheme.ApplyActionButtons(acciones);

            Controls.Clear();
            host.Controls.Add(lbNombre, 0, 0);
            host.Controls.Add(card, 0, 1);
            host.Controls.Add(acciones, 0, 2);
            Controls.Add(host);
        }

        private static Control CrearFilaCampoDialogo(Label etiqueta, string texto, Control campo)
        {
            var cell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0, 0, 8, 6),
                BackColor = UiTheme.Card
            };
            cell.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            cell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            etiqueta.Text = texto;
            etiqueta.AutoSize = false;
            etiqueta.Dock = DockStyle.Fill;
            etiqueta.TextAlign = ContentAlignment.MiddleLeft;
            etiqueta.ForeColor = UiTheme.TextMuted;
            etiqueta.Font = UiTheme.FontRegular;
            campo.Dock = DockStyle.Fill;
            cell.Controls.Add(etiqueta, 0, 0);
            cell.Controls.Add(campo, 0, 1);
            return cell;
        }
        void cargarAsociaciones() {
            try
            {
                asiciasionesData ad = new asiciasionesData();
                DataTable productoDT = ad.buscar();
                if (productoDT.Rows.Count > 0)
                {
                    cbAsociaciones.DataSource = productoDT;

                    cbAsociaciones.DisplayMember = "nombre";
                    cbAsociaciones.ValueMember = "id";
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un buscando Conductor. Error Contacte al Administrador del Sistema", ex);

            }

        
        
        }
        private bool validateRegistrarOrEditar(string etiqueta)
        {
            return ValidacionFormulario.validar(this.Controls, etiqueta);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            id = 0;
            
            Close();
        }

        private void productorNatural_Load(object sender, EventArgs e)
        {
            if (destino)
            {
                Text = "Cliente Natural";
                lbNombre.Text = "Cliente Natural";
            }
            btnEliminar.Visible = actualizar;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {

                if (!validateRegistrarOrEditar("validar"))
                    return;
                if (destino) { destinos(); } else { productor(); }
               
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void productor()
        {
            if (!actualizar)
            {
                pd.insertar(txtNombre.Text, Convert.ToDecimal(txtCedula.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), false);
                DataTable productorDT = pd.buscar(cbLetra.SelectedItem.ToString(), Convert.ToDecimal(txtCedula.Text));
                if (productorDT.Rows.Count > 0)
                {

                    foreach (DataRow fila in productorDT.Rows)
                    {
                        id = Convert.ToInt32(fila["id"]);
                        continue;
                    }
                }
                MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            else
            {
                if (id == 0)
                {
                    MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador");
                }
                pd.actualizar(id, txtNombre.Text, Convert.ToDecimal(txtCedula.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), false);
                //MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void destinos()
        {
            if (!actualizar)
            {
                dd.insertar(txtNombre.Text, Convert.ToDecimal(txtCedula.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), false);
                DataTable productorDT = dd.buscar(cbLetra.SelectedItem.ToString(), Convert.ToDecimal(txtCedula.Text));
                if (productorDT.Rows.Count > 0)
                {

                    foreach (DataRow fila in productorDT.Rows)
                    {
                        id = Convert.ToInt32(fila["id"]);
                        continue;
                    }
                }
                MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            else
            {
                if (id == 0)
                {
                    MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador");
                }
                dd.actualizar(id, txtNombre.Text, Convert.ToDecimal(txtCedula.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), false);
                MessageBox.Show("Registro guardado exitosamente", "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAgregarAsociacion_Click(object sender, EventArgs e)
        {
            try
            {
                asociacionForm tf = new asociacionForm();
                tf.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            cargarAsociaciones();
        }

        private void btnEditarAsociacion_Click(object sender, EventArgs e)
        {
            try
            {
                if (!validateRegistrarOrEditar("validareditar"))
                    return;
                object transporte = cbAsociaciones.SelectedItem;
                asociacionForm tf = new asociacionForm();
                tf.actualizar = true;
                tf.btnEliminar.Visible = true;
                tf.txtRif.Text = Equals(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[2], null) ? string.Empty : ((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[2].ToString();
                tf.id = Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]);
                tf.txtNombre.Text = cbAsociaciones.Text;
                tf.ShowDialog();
                cargarAsociaciones();
                cbAsociaciones.SelectedValue = tf.id;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un Error. Contacte al dersarrollador", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (pd.bustramitesxidPorductor(id).Rows.Count > 0 || dd.bustramitesxidPorductor(id).Rows.Count > 0)
                {

                    MessageBox.Show("Registro no puede ser eliminado ya que se encuentra asociado a Tramites de recepcion o despacho", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                DialogResult result = MessageBox.Show("Recuerde al eliminar registro no podra ser recuperado, ¿Desea realmente eliminar registro?", "Advertencia", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    if (destino) { eliminarDestino(); } else { eliminarProductor(); }
                    

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

        private void eliminarProductor()
        {
            pd.actualizar(id, txtNombre.Text, Convert.ToDecimal(txtCedula.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), true);

           
        }
        private void eliminarDestino()
        {
            dd.actualizar(id, txtNombre.Text, Convert.ToDecimal(txtCedula.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), true);


        }

     

     

    

      
    }
}
