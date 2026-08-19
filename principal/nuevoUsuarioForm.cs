using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace principal
{
    public partial class nuevoUsuarioForm : Form
    {
        public bool nuevo = true;
        usuarioData ud = new usuarioData();
        public int id;
        public nuevoUsuarioForm()
        {
            InitializeComponent();
            cargarPerfil();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void nuevoUsuarioForm_Load(object sender, EventArgs e)
        {
            AplicarLayoutUsuario();
        }

        private void AplicarLayoutUsuario()
        {
            Text = nuevo ? "Nuevo usuario" : "Editar usuario";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = true;
            ClientSize = new Size(480, 400);
            BackColor = UiTheme.Surface;
            Font = UiTheme.FontRegular;
            UiTheme.ApplyAppIcon(this);
            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            foreach (Control c in new Control[] { txtLogin, txtClave, txtNombre })
                if (c is TextBox tb) UiTheme.StyleTextBox(tb);
            UiTheme.StyleComboBox(cbPerfil);
            cbPerfil.DropDownStyle = ComboBoxStyle.DropDownList;

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

            label4.Text = Text;
            label4.AutoSize = false;
            label4.Font = UiTheme.FontTitle;
            label4.ForeColor = UiTheme.Primary;
            label4.BackColor = UiTheme.Surface;
            label4.Dock = DockStyle.Fill;
            label4.TextAlign = ContentAlignment.MiddleLeft;

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
            fields.Controls.Add(CrearFilaCampo(label1, "Login", txtLogin), 0, 0);
            fields.Controls.Add(CrearFilaCampo(label2, "Clave", txtClave), 0, 1);
            fields.Controls.Add(CrearFilaCampo(label3, "Nombre", txtNombre), 0, 2);
            fields.Controls.Add(CrearFilaCampo(Perfil, "Perfil", cbPerfil), 0, 3);
            card.Controls.Add(fields);

            var acciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 10, 0, 0),
                BackColor = UiTheme.Surface
            };

            btnEliminar.Visible = !nuevo;
            foreach (var btn in new[] { btnGuardar, btnCancelar, btnEliminar })
            {
                if (!btn.Visible) continue;
                btn.Size = new Size(120, 38);
                btn.Margin = new Padding(0, 0, 10, 0);
                acciones.Controls.Add(btn);
            }
            UiTheme.ApplyActionButtons(acciones);

            Controls.Clear();
            host.Controls.Add(label4, 0, 0);
            host.Controls.Add(card, 0, 1);
            host.Controls.Add(acciones, 0, 2);
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
            etiqueta.Text = texto;
            etiqueta.AutoSize = false;
            etiqueta.Dock = DockStyle.Fill;
            etiqueta.TextAlign = ContentAlignment.MiddleLeft;
            etiqueta.ForeColor = UiTheme.TextMuted;
            etiqueta.Font = UiTheme.FontRegular;
            etiqueta.BackColor = UiTheme.Card;
            campo.Dock = DockStyle.Fill;
            cell.Controls.Add(etiqueta, 0, 0);
            cell.Controls.Add(campo, 0, 1);
            return cell;
        }

        void cargarPerfil()
        {
            try
            {
                DataTable perfilDT = ud.buscarPerfil();
                if (perfilDT.Rows.Count > 0)
                {
                    cbPerfil.DataSource = perfilDT;
                    cbPerfil.DisplayMember = "nombre";
                    cbPerfil.ValueMember = "id";
                }
                if (nuevo)
                    cbPerfil.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un error buscando perfiles. Contacte al Administrador del Sistema", ex);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtLogin.Text))
                {
                    Base.msjAdvertencia("Login obligatorio.");
                    return;
                }

                if (string.IsNullOrEmpty(txtClave.Text))
                {
                    Base.msjAdvertencia("Clave obligatoria.");
                    return;
                }

                if (string.IsNullOrEmpty(txtNombre.Text))
                {
                    Base.msjAdvertencia("Nombre obligatorio.");
                    return;
                }
                if (string.IsNullOrEmpty(cbPerfil.Text))
                {
                    Base.msjAdvertencia("Debe seleccionar un perfil.");
                    return;
                }

                int idPerfil = Convert.ToInt16(((DataRowView)cbPerfil.SelectedItem).Row.ItemArray[0]);
                if (nuevo)
                    ud.insertar(txtLogin.Text, Seguridad.Encriptar(txtClave.Text), txtNombre.Text, idPerfil);
                else
                    ud.actualizar(id, txtLogin.Text, Seguridad.Encriptar(txtClave.Text), txtNombre.Text, idPerfil, false);

                Base.msjExito(nuevo ? "Usuario creado exitosamente." : "Usuario actualizado exitosamente.");
                Close();
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (Base.msjPregunta("Al eliminar el registro no podrá recuperarse. ¿Desea eliminarlo?") != DialogResult.Yes)
                return;

            try
            {
                ud.actualizar(id, txtLogin.Text, Seguridad.Encriptar(txtClave.Text), txtNombre.Text,
                    Convert.ToInt16(((DataRowView)cbPerfil.SelectedItem).Row.ItemArray[0]), true);
                Base.msjExito("Usuario eliminado exitosamente.");
                Close();
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }
        }
    }
}
