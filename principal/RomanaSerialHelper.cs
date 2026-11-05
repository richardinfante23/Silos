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
        // Número completo (evita partir 7880 en 788+0). Acepta miles EU/US y decimales.
        private static readonly Regex PesoRegex = new Regex(
            @"[+-]?(?:\d{1,3}(?:[.,]\d{3})+|\d+)(?:[.,]\d+)?",
            RegexOptions.Compiled);

        // Preferir el número pegado a la unidad (Brecknell 200E: "7880KG", "1.234,5 kg", etc.)
        private static readonly Regex PesoConUnidadRegex = new Regex(
            @"[+-]?(?:\d{1,3}(?:[.,]\d{3})+|\d+)(?:[.,]\d+)?\s*(?:kg|lb|g|t|tn|pcs)\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
        /// Pensado para romanas industriales (p. ej. Brecknell 200E: STX + espacios + 7880KG).
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

            // Quitar controles (STX=02, ETX, CR, LF, etc.) sin alterar el peso.
            string raw = NormalizarTrama(trama);
            if (string.IsNullOrWhiteSpace(raw))
            {
                detalle = "Trama sin datos útiles";
                return false;
            }

            // 1) Número junto a unidad: "7880KG", "7880 kg", "1,234.5 lb"
            Match mUnidad = PesoConUnidadRegex.Match(raw);
            if (mUnidad.Success)
            {
                string numTxt = PesoRegex.Match(mUnidad.Value).Value;
                if (TryParseNumero(numTxt, out peso) && peso > 0)
                {
                    detalle = "Modo unidad: \"" + mUnidad.Value.Trim() + "\"";
                    return true;
                }
            }

            // 2) Tras '=' o ':' (algunas romanas / software)
            foreach (char sep in new[] { '=', ':' })
            {
                int i = raw.IndexOf(sep);
                if (i < 0 || i >= raw.Length - 1) continue;
                Match m = PesoRegex.Match(raw.Substring(i + 1));
                if (m.Success && TryParseNumero(m.Value, out peso) && peso > 0)
                {
                    detalle = "Modo separador '" + sep + "': \"" + m.Value + "\"";
                    return true;
                }
            }

            // 3) El número con más dígitos enteros (peso de romana, no basura parcial)
            MatchCollection matches = PesoRegex.Matches(raw);
            decimal mejor = 0;
            string mejorTxt = null;
            int mejorDigitos = -1;
            foreach (Match m in matches)
            {
                decimal n;
                if (!TryParseNumero(m.Value, out n) || n <= 0) continue;
                int digitos = ContarDigitosEnteros(m.Value);
                // Preferir más dígitos; si empate, el valor mayor (típico bruto/tara)
                if (digitos > mejorDigitos || (digitos == mejorDigitos && n >= mejor))
                {
                    mejor = n;
                    mejorTxt = m.Value;
                    mejorDigitos = digitos;
                }
            }
            if (mejorTxt != null)
            {
                peso = mejor;
                detalle = "Modo número en trama: \"" + mejorTxt + "\"";
                return true;
            }

            detalle = "No se pudo interpretar peso en: \"" + ToHexPreview(trama) + "\"";
            return false;
        }

        private static string NormalizarTrama(string trama)
        {
            if (string.IsNullOrEmpty(trama)) return string.Empty;
            var sb = new StringBuilder(trama.Length);
            foreach (char c in trama)
            {
                if (c >= 32 && c <= 126) // imprimibles ASCII
                    sb.Append(c);
                else if (c == '\t')
                    sb.Append(' ');
                // STX/ETX/CR/LF y demás controles se descartan
            }
            return sb.ToString().Trim();
        }

        private static int ContarDigitosEnteros(string numero)
        {
            if (string.IsNullOrEmpty(numero)) return 0;
            int n = 0;
            foreach (char c in numero)
            {
                if (c == '.' || c == ',') break; // fin parte entera (aprox.)
                if (char.IsDigit(c)) n++;
            }
            return n;
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

        /// <summary>
        /// Solo dígitos y separador decimal (sin KG, espacios ni miles). Seguro para campos y BD numéricos.
        /// </summary>
        public static string FormatearPesoParaBd(decimal peso)
        {
            return Math.Round(peso, 2).ToString("0.##", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Lee peso de la báscula y lo deja listo para TextBox / BD (solo número, sin KG).
        /// </summary>
        public static string LeerPesoUnaVez(SerialPort port, out string tramaCruda, out string detalleParse)
        {
            return FormatearPesoParaBd(LeerPesoDecimal(port, out tramaCruda, out detalleParse));
        }

        /// <summary>Lee el peso como decimal (sin unidades). Usar en recepción y despacho.</summary>
        public static decimal LeerPesoDecimal(SerialPort port, out string tramaCruda, out string detalleParse)
        {
            tramaCruda = string.Empty;
            detalleParse = string.Empty;
            if (port == null || !port.IsOpen)
                throw new InvalidOperationException("Puerto serial cerrado.");

            // 1) Si la báscula envía sola, leer lo que ya hay / llegue en 1.5 s
            decimal pesoPasivo;
            if (TryLeerPasivo(port, 1500, out tramaCruda) && TryParsePeso(tramaCruda, out pesoPasivo, out detalleParse))
                return Math.Round(pesoPasivo, 2);

            // 2) Pedir peso con comando (R por defecto), probar terminadores
            string cmd = ComandoPeso;
            port.DiscardInBuffer();
            foreach (string nl in new[] { "\r\n", "\r", "\n", "" })
            {
                try
                {
                    if (string.IsNullOrEmpty(nl))
                        port.Write(cmd);
                    else
                    {
                        port.NewLine = nl;
                        port.WriteLine(cmd);
                    }

                    tramaCruda = LeerHastaTimeout(port, 2000);
                    if (string.IsNullOrWhiteSpace(tramaCruda))
                        continue;

                    decimal peso;
                    if (TryParsePeso(tramaCruda, out peso, out detalleParse))
                        return Math.Round(peso, 2);
                }
                catch (TimeoutException)
                {
                    // probar siguiente terminador
                }
            }

            throw new InvalidOperationException(
                "No llegó respuesta de la báscula.\n" +
                "Compruebe: puerto COM correcto (USB suele ser COM3+), baudios, cable,\n" +
                "y si la romana envía sola (use Solo leer / escucha continua) o otro comando.");
        }

        /// <summary>Lee bytes disponibles durante unos ms sin enviar nada.</summary>
        public static bool TryLeerPasivo(SerialPort port, int waitMs, out string trama)
        {
            trama = string.Empty;
            if (port == null || !port.IsOpen) return false;
            var sb = new StringBuilder();
            int waited = 0;
            int step = 100;
            while (waited < waitMs)
            {
                System.Threading.Thread.Sleep(step);
                waited += step;
                string chunk = port.ReadExisting();
                if (!string.IsNullOrEmpty(chunk))
                    sb.Append(chunk);
                if (sb.Length > 0 && (sb.ToString().IndexOf('\n') >= 0 || sb.ToString().IndexOf('\r') >= 0 || sb.Length > 40))
                    break;
            }
            trama = sb.ToString();
            return !string.IsNullOrWhiteSpace(trama);
        }

        private static string LeerHastaTimeout(SerialPort port, int timeoutMs)
        {
            int prev = port.ReadTimeout;
            try
            {
                port.ReadTimeout = Math.Max(200, timeoutMs);
                try
                {
                    return port.ReadLine();
                }
                catch (TimeoutException)
                {
                    string existing = port.ReadExisting();
                    if (!string.IsNullOrWhiteSpace(existing))
                        return existing;
                    throw;
                }
            }
            finally
            {
                port.ReadTimeout = prev;
            }
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
