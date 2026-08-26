using System;
using System.Drawing;
using System.Windows.Forms;
using principal;

namespace SisgesConfiguracion
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                ConnectionConfigHelper.UseSiblingSisgesConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sisges Configuración", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new ConfigConexionForm());
        }
    }

    public class ConfigConexionForm : Form
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

        public ConfigConexionForm()
        {
            ConstruirUi();
            Load += (s, e) => CargarActual();
        }

        private void ConstruirUi()
        {
            Text = "Sisges — Configuración de conexión";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(520, 390);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9F);

            Controls.Add(new Label
            {
                Text = "Conexión PostgreSQL",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 42, 66),
                AutoSize = true,
                Location = new Point(20, 12)
            });
            Controls.Add(new Label
            {
                Text = "Herramienta externa (no requiere login). Guarda en Sisges Pro.exe.config",
                ForeColor = Color.FromArgb(95, 108, 122),
                Location = new Point(20, 42),
                Size = new Size(480, 28)
            });

            int y = 78;
            _txtServer = Campo(ref y, "Servidor / IP", 20, 280);
            Controls.Add(new Label { Text = "Puerto", Location = new Point(320, 78), AutoSize = true, ForeColor = Color.FromArgb(95, 108, 122) });
            _txtPort = new TextBox { Location = new Point(320, 96), Size = new Size(80, 26), Text = "5432" };
            Controls.Add(_txtPort);

            _txtDatabase = Campo(ref y, "Base de datos", 20, 380);
            _txtUser = Campo(ref y, "Usuario", 20, 180);
            int yPass = y - 44;
            Controls.Add(new Label { Text = "Contraseña", Location = new Point(220, yPass), AutoSize = true, ForeColor = Color.FromArgb(95, 108, 122) });
            _txtPassword = new TextBox
            {
                Location = new Point(220, yPass + 18),
                Size = new Size(180, 26),
                UseSystemPasswordChar = true
            };
            Controls.Add(_txtPassword);

            _chkCifrar = new CheckBox
            {
                Text = "Cifrar al guardar (recomendado en el cliente)",
                Checked = true,
                AutoSize = true,
                Location = new Point(20, y + 4)
            };
            y += 32;
            Controls.Add(_chkCifrar);

            _lblEstado = new Label
            {
                Location = new Point(20, y),
                Size = new Size(480, 40),
                ForeColor = Color.FromArgb(95, 108, 122)
            };
            y += 44;
            Controls.Add(_lblEstado);

            _btnProbar = Boton("Probar", 20, y);
            _btnGuardar = Boton("Guardar", 118, y);
            _btnCifrar = Boton("Solo cifrar", 216, y);
            _btnDescifrar = Boton("Descifrar", 314, y);
            _btnCerrar = Boton("Cerrar", 412, y);

            _btnProbar.Click += (s, e) =>
            {
                try
                {
                    ConnectionConfigHelper.Test(LeerFormulario());
                    MessageBox.Show(this, "Conexión correcta.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "No se pudo conectar:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            _btnGuardar.Click += (s, e) =>
            {
                try
                {
                    var cfg = LeerFormulario();
                    ConnectionConfigHelper.Test(cfg);
                    ConnectionConfigHelper.Save(cfg, _chkCifrar.Checked);
                    ActualizarEstado();
                    MessageBox.Show(this,
                        _chkCifrar.Checked
                            ? "Guardado y cifrado.\nYa puede abrir Sisges Pro e iniciar sesión."
                            : "Guardado (sin cifrar).\nYa puede abrir Sisges Pro e iniciar sesión.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            _btnCifrar.Click += (s, e) =>
            {
                try
                {
                    ConnectionConfigHelper.ProtectOnly();
                    ActualizarEstado();
                    MessageBox.Show(this, "Sección cifrada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            _btnDescifrar.Click += (s, e) =>
            {
                if (MessageBox.Show(this, "¿Descifrar el archivo?\nSolo para edición técnica temporal.", "Confirmar",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                try
                {
                    ConnectionConfigHelper.Unprotect();
                    CargarActual();
                    MessageBox.Show(this, "Descifrado. Vuelva a Guardar con cifrado al terminar.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            _btnCerrar.Click += (s, e) => Close();
        }

        private Button Boton(string texto, int x, int y)
        {
            var b = new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(90, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(18, 42, 66),
                ForeColor = Color.White
            };
            b.FlatAppearance.BorderSize = 0;
            Controls.Add(b);
            return b;
        }

        private TextBox Campo(ref int y, string etiqueta, int x, int ancho)
        {
            Controls.Add(new Label
            {
                Text = etiqueta,
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = Color.FromArgb(95, 108, 122)
            });
            var txt = new TextBox { Location = new Point(x, y + 18), Size = new Size(ancho, 26) };
            Controls.Add(txt);
            y += 44;
            return txt;
        }

        private void CargarActual()
        {
            ConnectionSettings s = ConnectionConfigHelper.Load();
            _txtServer.Text = s.Server;
            _txtPort.Text = s.Port.ToString();
            _txtDatabase.Text = s.Database;
            _txtUser.Text = s.UserId;
            _txtPassword.Text = s.Password;
            ActualizarEstado();
        }

        private void ActualizarEstado()
        {
            try { _lblEstado.Text = ConnectionConfigHelper.ResumenEstado(); }
            catch { _lblEstado.Text = "No se pudo leer el estado."; }
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
    }
}
