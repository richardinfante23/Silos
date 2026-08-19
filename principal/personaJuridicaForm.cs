using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace principal
{
    public partial class personaJuridicaForm : Form
    {
        public int id = 0;
        public bool actualizar = false;
        ProductorData pd = new ProductorData();
        destinoData dd = new destinoData();
        public bool eliminado=false;
        public bool destino = false;
        public personaJuridicaForm()
        {
            InitializeComponent();
            cargarAsociaciones();
        }
        void cargarAsociaciones()
        {
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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {

                if (!validateRegistrarOrEditar("validar"))
                    return;
                if (destino)
                    destinos();
                else
                    productor();
                Close();
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }

        }

        private void destinos()
        {
            if (!actualizar)
            {
                dd.insertarPersonaJuridica(txtNombre.Text, Convert.ToDecimal(txtRif.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), false);
                DataTable productorDT = dd.buscarPersonaJuridica(cbLetra.SelectedItem.ToString(), Convert.ToDecimal(txtRif.Text));
                if (productorDT.Rows.Count > 0)
                {

                    foreach (DataRow fila in productorDT.Rows)
                    {
                        id = Convert.ToInt32(fila["id"]);
                        continue;
                    }
                }
                Base.msjExito("Registro guardado exitosamente.");


            }
            else
            {
                if (id == 0)
                {
                    Base.msjError("Ha ocurrido un error. Contacte al desarrollador.");
                    return;
                }
                dd.actualizarPersonaJuridica(id, txtNombre.Text, Convert.ToDecimal(txtRif.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), false);
                Base.msjExito("Registro guardado exitosamente.");
            }
        }

        private void productor()
        {
            if (!actualizar)
            {
                pd.insertarPersonaJuridica(txtNombre.Text, Convert.ToDecimal(txtRif.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), false);
                DataTable productorDT = pd.buscarPersonaJuridica(cbLetra.SelectedItem.ToString(), Convert.ToDecimal(txtRif.Text));
                if (productorDT.Rows.Count > 0)
                {

                    foreach (DataRow fila in productorDT.Rows)
                    {
                        id = Convert.ToInt32(fila["id"]);
                        continue;
                    }
                }
                Base.msjExito("Registro guardado exitosamente.");


            }
            else
            {
                if (id == 0)
                {
                    Base.msjError("Ha ocurrido un error. Contacte al desarrollador.");
                    return;
                }

                pd.actualizarPersonaJuridica(id, txtNombre.Text, Convert.ToDecimal(txtRif.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), false);
                Base.msjExito("Registro guardado exitosamente.");
            }
        }

        private void btnAgregarAsociacion_Click(object sender, EventArgs e)
        {
            try
            {
                asociacionForm tf = new asociacionForm();
                tf.ShowDialog(this);
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
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
                tf.ShowDialog(this);
                cargarAsociaciones();
                cbAsociaciones.SelectedValue = tf.id;
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (pd.bustramitesxidPorductorJuridico(id).Rows.Count > 0 || dd.bustramitesxidDestinoJuridico(id).Rows.Count>0)
                {

                    Base.msjError("Registro no puede ser eliminado ya que se encuentra asociado a trámites de recepción o despacho.");
                    return;
                }
                DialogResult result = Base.msjPregunta("Al eliminar el registro no podrá recuperarse. ¿Desea eliminarlo?");
                if (result == DialogResult.Yes)
                {
                    if (destino) {
                        dd.actualizarPersonaJuridica(id, txtNombre.Text, Convert.ToDecimal(txtRif.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), true);

                    }
                    else
                    {
                        pd.actualizarPersonaJuridica(id, txtNombre.Text, Convert.ToDecimal(txtRif.Text), cbLetra.SelectedItem.ToString(), Convert.ToInt32(((System.Data.DataRowView)(cbAsociaciones.SelectedItem)).Row.ItemArray[0]), true);

                    }
                  
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

        private void personaJuridicaForm_Load(object sender, EventArgs e)
        {
            if (actualizar)
                btnEliminar.Visible = true;
            if (destino)
            {
                this.Text = "Cliente Jurídico";
                this.lbNombre.Text = "Cliente Jurídico";
            }

            AplicarLayoutJuridico();

            // Al abrir desde "Nuevo" puede quedar `cbLetra` sin selección.
            if (cbLetra != null && cbLetra.SelectedItem == null && cbLetra.Items.Count > 0)
            {
                int indexV = cbLetra.Items.IndexOf("V");
                cbLetra.SelectedIndex = indexV >= 0 ? indexV : 0;
            }
        }

        private void AplicarLayoutJuridico()
        {
            UiTheme.ApplyAppIcon(this);
            BackColor = UiTheme.Surface;
            Font = UiTheme.FontRegular;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = true;
            ClientSize = new Size(700, 310);
            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            UiTheme.StyleTextBox(txtNombre);
            UiTheme.StyleTextBox(txtRif);
            UiTheme.StyleComboBox(cbLetra);
            UiTheme.StyleComboBox(cbAsociaciones);
            cbLetra.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAsociaciones.DropDownStyle = ComboBoxStyle.DropDownList;

            lbNombre.Text = destino ? "Cliente Jurídico" : "Persona Jurídica";
            lbNombre.AutoSize = false;
            lbNombre.Dock = DockStyle.Fill;
            lbNombre.Font = UiTheme.FontTitle;
            lbNombre.ForeColor = UiTheme.Primary;
            lbNombre.BackColor = Color.Transparent;
            lbNombre.TextAlign = ContentAlignment.MiddleLeft;

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

            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = UiTheme.Card
            };
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));
            fields.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            fields.Controls.Add(CrearFilaCampoDialogo(label7, "Nombre", txtNombre), 0, 0);

            var rifCell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0, 0, 0, 6),
                BackColor = UiTheme.Card
            };
            rifCell.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            rifCell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            label6.Text = "Letra:";
            label6.AutoSize = false;
            label6.Dock = DockStyle.Fill;
            label6.TextAlign = ContentAlignment.MiddleLeft;
            label6.ForeColor = UiTheme.TextMuted;
            label6.Font = UiTheme.FontRegular;
            label6.BackColor = Color.Transparent;

            label4.Text = "RIF:";
            label4.AutoSize = false;
            label4.Dock = DockStyle.Fill;
            label4.TextAlign = ContentAlignment.MiddleLeft;
            label4.ForeColor = UiTheme.TextMuted;
            label4.Font = UiTheme.FontRegular;
            label4.BackColor = Color.Transparent;

            var rifRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = UiTheme.Card
            };
            rifRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            rifRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            rifRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            rifRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rifRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            cbLetra.Dock = DockStyle.Fill;
            cbLetra.Margin = new Padding(0, 6, 12, 6);
            txtRif.Dock = DockStyle.Fill;
            txtRif.Margin = new Padding(0, 6, 0, 6);

            rifRow.Controls.Add(label6, 0, 0);
            rifRow.Controls.Add(cbLetra, 1, 0);
            rifRow.Controls.Add(label4, 2, 0);
            rifRow.Controls.Add(txtRif, 3, 0);
            rifCell.Controls.Add(new Panel(), 0, 0);
            rifCell.Controls.Add(rifRow, 0, 1);
            fields.Controls.Add(rifCell, 0, 1);

            var asociacionCell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0),
                BackColor = UiTheme.Card
            };
            asociacionCell.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            asociacionCell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            label5.Text = "Asociaciones:";
            label5.AutoSize = false;
            label5.Dock = DockStyle.Fill;
            label5.TextAlign = ContentAlignment.MiddleLeft;
            label5.ForeColor = UiTheme.TextMuted;
            label5.Font = UiTheme.FontRegular;
            label5.BackColor = Color.Transparent;

            var asociacionRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = UiTheme.Card
            };
            asociacionRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            asociacionRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            asociacionRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            asociacionRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            cbAsociaciones.Dock = DockStyle.Fill;
            cbAsociaciones.Margin = new Padding(0, 6, 8, 6);
            UiTheme.StyleCompactPrimaryButton(btnAgregarAsociacion, "Agregar", 108, 32);
            UiTheme.StyleCompactSecondaryButton(btnEditarAsociacion, "Editar", 108, 32);
            btnAgregarAsociacion.Margin = new Padding(0, 4, 8, 0);
            btnEditarAsociacion.Margin = new Padding(0, 4, 0, 0);

            asociacionRow.Controls.Add(cbAsociaciones, 0, 0);
            asociacionRow.Controls.Add(btnAgregarAsociacion, 1, 0);
            asociacionRow.Controls.Add(btnEditarAsociacion, 2, 0);
            asociacionCell.Controls.Add(label5, 0, 0);
            asociacionCell.Controls.Add(asociacionRow, 0, 1);
            fields.Controls.Add(asociacionCell, 0, 2);

            card.Controls.Add(fields);

            var acciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 10, 0, 0),
                BackColor = UiTheme.Surface
            };

            btnEliminar.Visible = actualizar;
            acciones.Controls.Add(btnGuardar);
            acciones.Controls.Add(btnCancelar);
            if (actualizar)
                acciones.Controls.Add(btnEliminar);
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

    }
}
