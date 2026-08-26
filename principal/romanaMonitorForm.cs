using System;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;

namespace principal
{
    /// <summary>
    /// Herramienta para descubrir puerto COM y formato de trama de la romana.
    /// </summary>
    public class romanaMonitorForm : Form
    {
        private ComboBox _cbPuerto;
        private ComboBox _cbBaud;
        private TextBox _txtLog;
        private TextBox _txtComando;
        private TextBox _txtPesoDetectado;
        private Label _lblEstado;
        private Button _btnRefrescar;
        private Button _btnConectar;
        private Button _btnEnviar;
        private Button _btnTomar;
        private Button _btnGuardar;
        private Button _btnLimpiar;
        private Button _btnCerrar;
        private CheckBox _chkEscucha;
        private SerialPort _port;
        private readonly StringBuilder _buffer = new StringBuilder();

        public romanaMonitorForm()
        {
            ConstruirUi();
            FormClosing += romanaMonitorForm_FormClosing;
            Load += romanaMonitorForm_Load;
        }

        private void romanaMonitorForm_Load(object sender, EventArgs e)
        {
            UiTheme.ApplyForm(this);
            UiTheme.ApplyAppIcon(this);
            Text = "Monitor de romana (puerto serial)";
            RefrescarPuertos();
            _cbBaud.SelectedItem = RomanaSerialHelper.BaudConfigurado.ToString();
            if (_cbBaud.SelectedIndex < 0) _cbBaud.SelectedItem = "9600";
            _txtComando.Text = RomanaSerialHelper.ComandoPeso;
            Log("Consejo: conecte la báscula, elija el COM, Conectar, luego Tomar / Enviar.");
            Log("Copie la trama cruda y con eso ajustamos el parseo si hiciera falta.");
        }

        private void ConstruirUi()
        {
            SuspendLayout();
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(720, 520);
            BackColor = UiTheme.Surface;

            Controls.Add(new Label
            {
                Text = "Monitor serial de báscula / romana",
                Font = UiTheme.FontTitle,
                ForeColor = UiTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(16, 12)
            });

            Controls.Add(new Label { Text = "Puerto", Location = new Point(16, 52), AutoSize = true, ForeColor = UiTheme.TextMuted });
            _cbPuerto = new ComboBox { Location = new Point(16, 70), Size = new Size(120, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            UiTheme.StyleComboBox(_cbPuerto);
            Controls.Add(_cbPuerto);

            _btnRefrescar = new Button { Text = "Refrescar", Location = new Point(144, 68), Size = new Size(90, 30) };
            _btnRefrescar.Click += (s, e) => RefrescarPuertos();
            UiTheme.StyleCompactSecondaryButton(_btnRefrescar, "Refrescar", 90, 30);
            Controls.Add(_btnRefrescar);

            Controls.Add(new Label { Text = "Baudios", Location = new Point(250, 52), AutoSize = true, ForeColor = UiTheme.TextMuted });
            _cbBaud = new ComboBox
            {
                Location = new Point(250, 70),
                Size = new Size(100, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cbBaud.Items.AddRange(new object[] { "2400", "4800", "9600", "19200", "38400", "57600", "115200" });
            UiTheme.StyleComboBox(_cbBaud);
            Controls.Add(_cbBaud);

            _btnConectar = new Button { Text = "Conectar", Location = new Point(370, 68), Size = new Size(100, 30) };
            _btnConectar.Click += btnConectar_Click;
            UiTheme.StyleCompactPrimaryButton(_btnConectar, "Conectar", 100, 30);
            Controls.Add(_btnConectar);

            _btnGuardar = new Button { Text = "Usar en Sisges", Location = new Point(480, 68), Size = new Size(120, 30) };
            _btnGuardar.Click += btnGuardar_Click;
            UiTheme.StyleCompactSecondaryButton(_btnGuardar, "Usar en Sisges", 120, 30);
            Controls.Add(_btnGuardar);

            Controls.Add(new Label { Text = "Comando (Enter)", Location = new Point(16, 110), AutoSize = true, ForeColor = UiTheme.TextMuted });
            _txtComando = new TextBox { Location = new Point(16, 128), Size = new Size(80, 26) };
            UiTheme.StyleTextBox(_txtComando);
            Controls.Add(_txtComando);

            _btnEnviar = new Button { Text = "Enviar", Location = new Point(104, 126), Size = new Size(90, 30) };
            _btnEnviar.Click += btnEnviar_Click;
            UiTheme.StyleCompactSecondaryButton(_btnEnviar, "Enviar", 90, 30);
            Controls.Add(_btnEnviar);

            _btnTomar = new Button { Text = "Tomar (como recepción)", Location = new Point(204, 126), Size = new Size(170, 30) };
            _btnTomar.Click += btnTomar_Click;
            UiTheme.StyleCompactPrimaryButton(_btnTomar, "Tomar (como recepción)", 170, 30);
            Controls.Add(_btnTomar);

            _chkEscucha = new CheckBox
            {
                Text = "Escucha continua (DataReceived)",
                Location = new Point(390, 130),
                AutoSize = true,
                Checked = true
            };
            Controls.Add(_chkEscucha);

            Controls.Add(new Label { Text = "Peso detectado", Location = new Point(16, 168), AutoSize = true, ForeColor = UiTheme.TextMuted });
            _txtPesoDetectado = new TextBox
            {
                Location = new Point(16, 186),
                Size = new Size(200, 26),
                ReadOnly = true
            };
            UiTheme.StyleTextBox(_txtPesoDetectado);
            Controls.Add(_txtPesoDetectado);

            _lblEstado = new Label
            {
                Text = "Desconectado",
                Location = new Point(230, 190),
                AutoSize = true,
                ForeColor = UiTheme.TextMuted
            };
            Controls.Add(_lblEstado);

            Controls.Add(new Label
            {
                Text = "Log (trama cruda + hex). Deje peso en la báscula y pulse Tomar o vea si envía sola.",
                Location = new Point(16, 222),
                AutoSize = true,
                ForeColor = UiTheme.TextMuted
            });

            _txtLog = new TextBox
            {
                Location = new Point(16, 244),
                Size = new Size(688, 220),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new Font("Consolas", 9F)
            };
            Controls.Add(_txtLog);

            _btnLimpiar = new Button { Text = "Limpiar log", Location = new Point(16, 476), Size = new Size(100, 30) };
            _btnLimpiar.Click += (s, e) => { _txtLog.Clear(); };
            UiTheme.StyleCompactSecondaryButton(_btnLimpiar, "Limpiar log", 100, 30);
            Controls.Add(_btnLimpiar);

            _btnCerrar = new Button { Text = "Cerrar", Location = new Point(604, 476), Size = new Size(100, 30) };
            _btnCerrar.Click += (s, e) => Close();
            UiTheme.StyleCompactSecondaryButton(_btnCerrar, "Cerrar", 100, 30);
            Controls.Add(_btnCerrar);

            ResumeLayout(true);
        }

        private void RefrescarPuertos()
        {
            string actual = _cbPuerto.SelectedItem as string ?? RomanaSerialHelper.PuertoConfigurado;
            _cbPuerto.Items.Clear();
            string[] ports = RomanaSerialHelper.ListarPuertos();
            foreach (string p in ports)
                _cbPuerto.Items.Add(p);
            if (_cbPuerto.Items.Count == 0)
            {
                Log("No hay puertos COM. Conecte el convertidor USB-Serial / romana.");
                _lblEstado.Text = "Sin puertos";
                return;
            }
            int idx = _cbPuerto.Items.IndexOf(actual);
            _cbPuerto.SelectedIndex = idx >= 0 ? idx : 0;
            Log("Puertos: " + string.Join(", ", ports));
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            if (_port != null && _port.IsOpen)
            {
                CerrarPuerto();
                _btnConectar.Text = "Conectar";
                UiTheme.StyleCompactPrimaryButton(_btnConectar, "Conectar", 100, 30);
                _lblEstado.Text = "Desconectado";
                return;
            }

            if (_cbPuerto.SelectedItem == null)
            {
                Base.msjAdvertencia("Seleccione un puerto COM.");
                return;
            }

            try
            {
                int baud = 9600;
                int.TryParse(Convert.ToString(_cbBaud.SelectedItem), out baud);
                _port = new SerialPort
                {
                    PortName = _cbPuerto.SelectedItem.ToString(),
                    BaudRate = baud,
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None,
                    ReadTimeout = 5000,
                    WriteTimeout = 5000,
                    NewLine = "\r\n",
                    Encoding = Encoding.ASCII
                };
                if (_chkEscucha.Checked)
                    _port.DataReceived += port_DataReceived;
                _port.Open();
                _btnConectar.Text = "Desconectar";
                UiTheme.StyleCompactSecondaryButton(_btnConectar, "Desconectar", 100, 30);
                _lblEstado.Text = "Conectado " + _port.PortName + " @ " + baud;
                Log("Abierto " + _port.PortName + " baud=" + baud);
            }
            catch (Exception ex)
            {
                Base.msjError("No se pudo abrir el puerto: " + ex.Message);
                CerrarPuerto();
            }
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string chunk = _port.ReadExisting();
                if (string.IsNullOrEmpty(chunk)) return;
                BeginInvoke(new Action(() =>
                {
                    _buffer.Append(chunk);
                    Log("<< " + RomanaSerialHelper.ToHexPreview(chunk));
                    string detalle;
                    decimal peso;
                    if (RomanaSerialHelper.TryParsePeso(_buffer.ToString(), out peso, out detalle))
                    {
                        _txtPesoDetectado.Text = Math.Round(peso, 2).ToString();
                        Log("Parse: " + detalle);
                    }
                    if (chunk.IndexOf('\n') >= 0 || chunk.IndexOf('\r') >= 0)
                        _buffer.Clear();
                }));
            }
            catch { /* ignore partial reads */ }
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            if (_port == null || !_port.IsOpen)
            {
                Base.msjAdvertencia("Conecte el puerto primero.");
                return;
            }
            try
            {
                string cmd = _txtComando.Text ?? string.Empty;
                _port.WriteLine(cmd);
                Log(">> " + RomanaSerialHelper.ToHexPreview(cmd + "\r\n"));
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }
        }

        private void btnTomar_Click(object sender, EventArgs e)
        {
            if (_port == null || !_port.IsOpen)
            {
                Base.msjAdvertencia("Conecte el puerto primero.");
                return;
            }
            try
            {
                string trama;
                string detalle;
                string peso = RomanaSerialHelper.LeerPesoUnaVez(_port, out trama, out detalle);
                Log("Tomar trama: " + RomanaSerialHelper.ToHexPreview(trama));
                Log("Tomar parse: " + detalle);
                _txtPesoDetectado.Text = peso;
                Base.msjExito("Peso leído: " + peso + " Kg\n" + detalle);
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (_cbPuerto.SelectedItem == null)
            {
                Base.msjAdvertencia("Seleccione un puerto.");
                return;
            }
            try
            {
                int baud = 9600;
                int.TryParse(Convert.ToString(_cbBaud.SelectedItem), out baud);
                RomanaSerialHelper.GuardarPuertoEnConfig(_cbPuerto.SelectedItem.ToString(), baud, 8);
                Base.msjExito("Guardado en configuración:\nPuerto=" + _cbPuerto.SelectedItem + " Baud=" + baud +
                    "\nReinicie recepción/despacho si ya estaban abiertos.");
                Log("Config guardada PortName=" + _cbPuerto.SelectedItem + " BaudRate=" + baud);
            }
            catch (Exception ex)
            {
                Base.msjErrorOperacion(ex, "guardar puerto romana");
            }
        }

        private void Log(string line)
        {
            _txtLog.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + "  " + line + Environment.NewLine);
        }

        private void CerrarPuerto()
        {
            try
            {
                if (_port != null)
                {
                    _port.DataReceived -= port_DataReceived;
                    if (_port.IsOpen) _port.Close();
                    _port.Dispose();
                }
            }
            catch { }
            _port = null;
        }

        private void romanaMonitorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CerrarPuerto();
        }
    }
}
