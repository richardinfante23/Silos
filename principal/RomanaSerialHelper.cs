using System;
using System.Configuration;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace principal
{
    /// <summary>
    /// Lectura y parseo del peso desde báscula serial.
    /// El monitor de romana sirve para ver el formato real en cada cliente.
    /// </summary>
    public static class RomanaSerialHelper
    {
        private static readonly Regex PesoRegex = new Regex(
            @"[+-]?\d{1,3}(?:[.,]\d{3})*(?:[.,]\d+)?|[+-]?\d+(?:[.,]\d+)?",
            RegexOptions.Compiled);

        public static string[] ListarPuertos()
        {
            return SerialPort.GetPortNames().OrderBy(p => p).ToArray();
        }

        public static string PuertoConfigurado
        {
            get
            {
                string p = ConfigurationManager.AppSettings["PortName"];
                return string.IsNullOrWhiteSpace(p) ? "COM1" : p.Trim();
            }
        }

        public static int BaudConfigurado
        {
            get
            {
                int baud;
                if (!int.TryParse(ConfigurationManager.AppSettings["BaudRate"], out baud) || baud <= 0)
                    return 9600;
                return baud;
            }
        }

        public static int DataBitsConfigurado
        {
            get
            {
                int bits;
                if (!int.TryParse(ConfigurationManager.AppSettings["DataBits"], out bits) || bits <= 0)
                    return 8;
                return bits;
            }
        }

        public static string ComandoPeso
        {
            get
            {
                string c = ConfigurationManager.AppSettings["RomanaCommand"];
                return string.IsNullOrWhiteSpace(c) ? "R" : c.Trim();
            }
        }

        public static void ConfigurarPuerto(SerialPort port)
        {
            if (port == null) throw new ArgumentNullException("port");
            port.PortName = PuertoConfigurado;
            port.BaudRate = BaudConfigurado;
            port.DataBits = DataBitsConfigurado;
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            port.Handshake = Handshake.None;
            port.ReadTimeout = 5000;
            port.WriteTimeout = 6000;
            port.NewLine = "\r\n";
            port.Encoding = Encoding.ASCII;
        }

        /// <summary>
        /// Extrae un peso numérico de la trama cruda.
        /// Prueba varios formatos comunes de romanas industriales.
        /// </summary>
        public static bool TryParsePeso(string trama, out decimal peso, out string detalle)
        {
            peso = 0;
            detalle = string.Empty;
            if (string.IsNullOrWhiteSpace(trama))
            {
                detalle = "Trama vacía";
                return false;
            }

            string raw = trama.Trim();

            // 1) Formato histórico Sisges: Substring(7, len-9)
            try
            {
                if (raw.Length > 9)
                {
                    string s = raw.Substring(7, raw.Length - 9).Trim();
                    if (TryParseNumero(s, out peso))
                    {
                        detalle = "Modo Substring(7,len-9): \"" + s + "\"";
                        return true;
                    }
                }
            }
            catch { /* seguir */ }

            // 2) Tras '=' o '+'
            foreach (char sep in new[] { '=', '+' })
            {
                int i = raw.IndexOf(sep);
                if (i < 0 || i >= raw.Length - 1) continue;
                string resto = raw.Substring(i + 1);
                Match m = PesoRegex.Match(resto);
                if (m.Success && TryParseNumero(m.Value, out peso))
                {
                    detalle = "Modo separador '" + sep + "': \"" + m.Value + "\"";
                    return true;
                }
            }

            // 3) Cualquier número en la trama (toma el de más dígitos / último razonable)
            MatchCollection matches = PesoRegex.Matches(raw);
            decimal mejor = 0;
            string mejorTxt = null;
            foreach (Match m in matches)
            {
                decimal n;
                if (!TryParseNumero(m.Value, out n)) continue;
                // Preferir valores típicos de romana (kg): > 0
                if (n <= 0) continue;
                if (mejorTxt == null || m.Value.Replace(",", "").Replace(".", "").Length >= mejorTxt.Replace(",", "").Replace(".", "").Length)
                {
                    mejor = n;
                    mejorTxt = m.Value;
                }
            }
            if (mejorTxt != null)
            {
                peso = mejor;
                detalle = "Modo regex (número en trama): \"" + mejorTxt + "\"";
                return true;
            }

            detalle = "No se pudo interpretar peso en: \"" + raw + "\"";
            return false;
        }

        private static bool TryParseNumero(string texto, out decimal valor)
        {
            valor = 0;
            if (string.IsNullOrWhiteSpace(texto)) return false;
            string t = texto.Trim().Replace(" ", "");

            // 1.234,56 (EU) vs 1,234.56 (US)
            if (t.Contains(",") && t.Contains("."))
            {
                if (t.LastIndexOf(',') > t.LastIndexOf('.'))
                    t = t.Replace(".", "").Replace(',', '.');
                else
                    t = t.Replace(",", "");
            }
            else if (t.Contains(","))
            {
                t = t.Replace(',', '.');
            }

            return decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out valor);
        }

        public static string LeerPesoUnaVez(SerialPort port, out string tramaCruda, out string detalleParse)
        {
            tramaCruda = string.Empty;
            detalleParse = string.Empty;
            if (port == null || !port.IsOpen)
                throw new InvalidOperationException("Puerto serial cerrado.");

            port.DiscardInBuffer();
            port.WriteLine(ComandoPeso);
            tramaCruda = port.ReadLine();

            decimal peso;
            if (!TryParsePeso(tramaCruda, out peso, out detalleParse))
                throw new InvalidOperationException(detalleParse + Environment.NewLine + "Trama: " + tramaCruda);

            return Math.Round(peso, 2).ToString(CultureInfo.CurrentCulture);
        }

        public static void GuardarPuertoEnConfig(string portName, int baudRate, int dataBits)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection app = config.AppSettings;
            SetApp(app, "PortName", portName);
            SetApp(app, "BaudRate", baudRate.ToString(CultureInfo.InvariantCulture));
            SetApp(app, "DataBits", dataBits.ToString(CultureInfo.InvariantCulture));
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private static void SetApp(AppSettingsSection app, string key, string value)
        {
            if (app.Settings[key] == null)
                app.Settings.Add(key, value);
            else
                app.Settings[key].Value = value;
        }

        public static string ToHexPreview(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            var sb = new StringBuilder();
            foreach (char c in text)
            {
                if (c == '\r') sb.Append("<CR>");
                else if (c == '\n') sb.Append("<LF>");
                else if (c < 32 || c > 126) sb.AppendFormat("<{0:X2}>", (int)c);
                else sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
