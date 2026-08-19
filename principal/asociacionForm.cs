using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace principal
{
    public partial class asociacionForm : Form
    {
        asiciasionesData td = new asiciasionesData();
        ProductorData pd = new ProductorData();

       public int id = 0;
       public bool actualizar = false;
        public asociacionForm()
        {
            InitializeComponent();
        }

        public asociacionForm(int prmid, string prmrif, string prmnombre)
        {
            InitializeComponent();
            id = prmid;
            txtRif.Text = prmrif ?? string.Empty;
            txtNombre.Text = prmnombre ?? string.Empty;
            actualizar = true;
            btnEliminar.Visible = true;
            Text = "Editar asociación";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
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
                if (!actualizar)
                {
                    td.insertar(txtRif.Text, txtNombre.Text);

                    Base.msjExito("Registro guardado exitosamente.");


                }
                else
                {
                    if (id == 0)
                    {
                        Base.msjError("Ha ocurrido un error. Contacte al desarrollador.");
                    }
                    td.actualizar(id, txtRif.Text, txtNombre.Text, false);
                    Base.msjExito("Registro guardado exitosamente.");
                }
                Close();
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
                DialogResult result = Base.msjPregunta("Al eliminar el registro no podrá recuperarse. ¿Desea eliminarlo?");
                if (result == DialogResult.Yes)
                {
                    if (pd.buscarproductorxAsociacion(id).Rows.Count>0)
                    {

                        Base.msjError("Registro no puede ser eliminado ya que se encuentra asociado a productores.");
                        return;
                    }
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AplicarLayoutAsociacion();
        }

        private void AplicarLayoutAsociacion()
        {
            UiTheme.ApplyAppIcon(this);
            BackColor = UiTheme.Surface;
            Font = UiTheme.FontRegular;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = true;
            ClientSize = new Size(460, 250);
            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            UiTheme.StyleTextBox(txtNombre);
            UiTheme.StyleTextBox(txtRif);

            label3.Text = actualizar ? "Editar Asociación" : "Nueva Asociación";
            label3.AutoSize = false;
            label3.Dock = DockStyle.Fill;
            label3.Font = UiTheme.FontTitle;
            label3.ForeColor = UiTheme.Primary;
            label3.BackColor = Color.Transparent;
            label3.TextAlign = ContentAlignment.MiddleLeft;

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
                RowCount = 2,
                BackColor = UiTheme.Card
            };
            fields.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            fields.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            fields.Controls.Add(CrearFilaCampo(label2, "Nombre", txtNombre), 0, 0);
            fields.Controls.Add(CrearFilaCampo(label1, "RIF", txtRif), 0, 1);
            card.Controls.Add(fields);

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 10, 0, 0),
                BackColor = UiTheme.Surface
            };
            actions.Controls.Add(btnGuardar);
            actions.Controls.Add(btnCancelar);
            if (btnEliminar.Visible)
                actions.Controls.Add(btnEliminar);
            UiTheme.ApplyActionButtons(actions);

            Controls.Clear();
            host.Controls.Add(label3, 0, 0);
            host.Controls.Add(card, 0, 1);
            host.Controls.Add(actions, 0, 2);
            Controls.Add(host);
        }

        private static Control CrearFilaCampo(Label etiqueta, string texto, Control campo)
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
