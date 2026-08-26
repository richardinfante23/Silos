using System;
using System.Drawing;
using System.Windows.Forms;

namespace principal
{
    public class conexionForm : Form
    {
        private TextBox _txtServer;
        private TextBox _txtPort;
        private TextBox _txtDatabase;
        private TextBox _txtUser;
        private TextBox _txtPassword;
        private CheckBox _chkCifrar;
        private Label _lblEstado;
        private Button _btnProbar;
        private Button _btnGuardar;
        private Button _btnCifrar;
        private Button _btnDescifrar;
        private Button _btnCerrar;

        public conexionForm()
        {
            ConstruirUi();
            Load += conexionForm_Load;
        }

        private void conexionForm_Load(object sender, EventArgs e)
        {
            UiTheme.ApplyForm(this);
            UiTheme.ApplyAppIcon(this);
            Text = "Conexión a base de datos";
            CargarActual();
        }

        private void ConstruirUi()
        {
            SuspendLayout();
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(520, 390);
            BackColor = UiTheme.Surface;

            var titulo = new Label
            {
                Text = "Conexión PostgreSQL",
                Font = UiTheme.FontTitle,
                ForeColor = UiTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(20, 12)
            };

            var ayuda = new Label
            {
                Text = "Configure la conexión del cliente y guarde cifrada. El cifrado usa Windows (DPAPI) de esta PC.",
                ForeColor = UiTheme.TextMuted,
                Location = new Point(20, 42),
                Size = new Size(480, 28)
            };

            int y = 78;
            _txtServer = Campo(ref y, "Servidor / IP", 20, 280);
            // Puerto en la misma fila que servidor
            var lblPuerto = new Label
            {
                Text = "Puerto",
                Location = new Point(320, 78),
                AutoSize = true,
                ForeColor = UiTheme.TextMuted
            };
            _txtPort = new TextBox
            {
                Location = new Point(320, 96),
                Size = new Size(80, 26),
                Text = "5432"
            };
            UiTheme.StyleTextBox(_txtPort);
            Controls.Add(lblPuerto);
            Controls.Add(_txtPort);

            _txtDatabase = Campo(ref y, "Base de datos", 20, 380);
            _txtUser = Campo(ref y, "Usuario", 20, 180);
            int yPass = y - 44;
            var lblPass = new Label
            {
                Text = "Contraseña",
                Location = new Point(220, yPass),
                AutoSize = true,
                ForeColor = UiTheme.TextMuted
            };
            _txtPassword = new TextBox
            {
                Location = new Point(220, yPass + 18),
                Size = new Size(180, 26),
                UseSystemPasswordChar = true
            };
            UiTheme.StyleTextBox(_txtPassword);
            Controls.Add(lblPass);
            Controls.Add(_txtPassword);

            _chkCifrar = new CheckBox
            {
                Text = "Cifrar al guardar (recomendado en el cliente)",
                Checked = true,
                AutoSize = true,
                Location = new Point(20, y + 4),
                ForeColor = UiTheme.TextPrimary
            };
            y += 32;

            _lblEstado = new Label
            {
                Location = new Point(20, y),
                Size = new Size(480, 40),
                ForeColor = UiTheme.TextMuted
            };
            y += 44;

            _btnProbar = new Button { Text = "Probar", Location = new Point(20, y), Size = new Size(90, 32) };
            _btnGuardar = new Button { Text = "Guardar", Location = new Point(118, y), Size = new Size(90, 32) };
            _btnCifrar = new Button { Text = "Solo cifrar", Location = new Point(216, y), Size = new Size(90, 32) };
            _btnDescifrar = new Button { Text = "Descifrar", Location = new Point(314, y), Size = new Size(90, 32) };
            _btnCerrar = new Button { Text = "Cerrar", Location = new Point(412, y), Size = new Size(90, 32) };

            _btnProbar.Click += btnProbar_Click;
            _btnGuardar.Click += btnGuardar_Click;
            _btnCifrar.Click += btnCifrar_Click;
            _btnDescifrar.Click += btnDescifrar_Click;
            _btnCerrar.Click += (s, e) => Close();

            UiTheme.StyleCompactPrimaryButton(_btnProbar, "Probar", 90, 32);
            UiTheme.StyleCompactPrimaryButton(_btnGuardar, "Guardar", 90, 32);
            UiTheme.StyleCompactSecondaryButton(_btnCifrar, "Solo cifrar", 90, 32);
            UiTheme.StyleCompactSecondaryButton(_btnDescifrar, "Descifrar", 90, 32);
            UiTheme.StyleCompactSecondaryButton(_btnCerrar, "Cerrar", 90, 32);

            Controls.Add(titulo);
            Controls.Add(ayuda);
            Controls.Add(_chkCifrar);
            Controls.Add(_lblEstado);
            Controls.Add(_btnProbar);
            Controls.Add(_btnGuardar);
            Controls.Add(_btnCifrar);
            Controls.Add(_btnDescifrar);
            Controls.Add(_btnCerrar);
            ResumeLayout(true);
        }

        private TextBox Campo(ref int y, string etiqueta, int x, int ancho)
        {
            var lbl = new Label
            {
                Text = etiqueta,
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = UiTheme.TextMuted
            };
            var txt = new TextBox
            {
                Location = new Point(x, y + 18),
                Size = new Size(ancho, 26)
            };
            UiTheme.StyleTextBox(txt);
            Controls.Add(lbl);
            Controls.Add(txt);
            y += 44;
            return txt;
        }

        private void CargarActual()
        {
            try
            {
                ConnectionSettings s = ConnectionConfigHelper.Load();
                _txtServer.Text = s.Server;
                _txtPort.Text = s.Port.ToString();
                _txtDatabase.Text = s.Database;
                _txtUser.Text = s.UserId;
                _txtPassword.Text = s.Password;
                ActualizarEstado();
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "cargar conexión");
            }
        }

        private void ActualizarEstado()
        {
            try
            {
                _lblEstado.Text = ConnectionConfigHelper.ResumenEstado();
            }
            catch
            {
                _lblEstado.Text = "No se pudo leer el estado del archivo de configuración.";
            }
        }

        private ConnectionSettings LeerFormulario()
        {
            int puerto;
            if (!int.TryParse((_txtPort.Text ?? string.Empty).Trim(), out puerto))
                throw new InvalidOperationException("El puerto debe ser un número.");

            return new ConnectionSettings
            {
                Server = (_txtServer.Text ?? string.Empty).Trim(),
                Port = puerto,
                Database = (_txtDatabase.Text ?? string.Empty).Trim(),
                UserId = (_txtUser.Text ?? string.Empty).Trim(),
                Password = _txtPassword.Text ?? string.Empty
            };
        }

        private void btnProbar_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                ConnectionConfigHelper.Test(LeerFormulario());
                Cursor = Cursors.Default;
                Base.msjExito("Conexión correcta.");
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                Base.msjError("No se pudo conectar: " + ex.Message);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectionSettings s = LeerFormulario();
                ConnectionConfigHelper.Test(s);
                ConnectionConfigHelper.Save(s, _chkCifrar.Checked);
                ActualizarEstado();
                Base.msjExito(_chkCifrar.Checked
                    ? "Conexión guardada y cifrada en este equipo.\nSi cambió de servidor, reinicie Sisges."
                    : "Conexión guardada (sin cifrar).\nSi cambió de servidor, reinicie Sisges.");
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "guardar conexión");
            }
        }

        private void btnCifrar_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectionConfigHelper.ProtectOnly();
                ActualizarEstado();
                Base.msjExito("Sección connectionStrings cifrada.");
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "cifrar conexión");
            }
        }

        private void btnDescifrar_Click(object sender, EventArgs e)
        {
            if (Base.msjPregunta("¿Descifrar el archivo de configuración?\nÚselo solo para edición técnica temporal.") != DialogResult.Yes)
                return;

            try
            {
                ConnectionConfigHelper.Unprotect();
                CargarActual();
                Base.msjAdvertencia("Descifrado. Vuelva a Guardar con cifrado al terminar.");
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "descifrar conexión");
            }
        }
    }
}
