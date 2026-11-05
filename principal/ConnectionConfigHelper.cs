using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using Npgsql;

namespace principal
{
    public class ConnectionSettings
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }

        public ConnectionSettings()
        {
            Server = "localhost";
            Database = string.Empty;
            UserId = string.Empty;
            Password = string.Empty;
            Port = 5432;
        }

        public string ToConnectionString()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = (Server ?? string.Empty).Trim(),
                Database = (Database ?? string.Empty).Trim(),
                Username = (UserId ?? string.Empty).Trim(),
                Password = Password ?? string.Empty,
                Port = Port > 0 ? Port : 5432
            };
            return builder.ConnectionString;
        }

        public static ConnectionSettings FromConnectionString(string connectionString)
        {
            var settings = new ConnectionSettings();
            if (string.IsNullOrWhiteSpace(connectionString))
                return settings;

            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            settings.Server = builder.Host ?? "localhost";
            settings.Database = builder.Database ?? string.Empty;
            settings.UserId = builder.Username ?? string.Empty;
            settings.Password = builder.Password ?? string.Empty;
            settings.Port = builder.Port > 0 ? builder.Port : 5432;
            return settings;
        }
    }

    /// <summary>
    /// Lectura/escritura y cifrado DPAPI de connectionStrings.
    /// Usa SisgesConnection.config (configSource) para no reescribir el .exe.config principal.
    /// </summary>
    public static class ConnectionConfigHelper
    {
        private const string SectionName = "connectionStrings";
        private const string SettingName = "ConnectionString";
        private const string Protector = "DataProtectionConfigurationProvider";
        public const string MainExeFileName = "Sisges Pro.exe";
        public const string ConnectionFileName = "SisgesConnection.config";

        /// <summary>Si se asigna, carpeta donde está Sisges Pro (para herramienta externa).</summary>
        public static string TargetInstallDirectory { get; set; }

        public static string ConfigPath
        {
            get { return ResolveConnectionConfigFilePath(); }
        }

        public static bool IsEncrypted
        {
            get
            {
                try
                {
                    ConfigurationSection section = OpenConnectionConfig().GetSection(SectionName);
                    return section != null && section.SectionInformation.IsProtected;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static void UseSiblingSisgesConfig()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string exeConfig = Path.Combine(dir, MainExeFileName + ".config");
            if (!File.Exists(exeConfig))
                exeConfig = Path.Combine(dir, MainExeFileName + ".Config");
            if (!File.Exists(exeConfig))
            {
                throw new FileNotFoundException(
                    "No se encontró \"" + MainExeFileName + ".config\" en:\n" + dir +
                    "\nColoque esta herramienta en la misma carpeta que Sisges Pro.");
            }
            TargetInstallDirectory = dir;
        }

        /// <summary>
        /// Abre el archivo de conexión (SisgesConnection.config), no el .exe.config completo.
        /// </summary>
        private static Configuration OpenConnectionConfig()
        {
            string connFile = ResolveConnectionConfigFilePath();
            EnsureConnectionConfigFileExists(connFile);

            var map = new ExeConfigurationFileMap
            {
                ExeConfigFilename = connFile
            };
            return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }

        private static string ResolveConnectionConfigFilePath()
        {
            string baseDir = !string.IsNullOrWhiteSpace(TargetInstallDirectory)
                ? TargetInstallDirectory
                : AppDomain.CurrentDomain.BaseDirectory;

            return Path.Combine(baseDir, ConnectionFileName);
        }

        private static void EnsureConnectionConfigFileExists(string connFile)
        {
            string dir = Path.GetDirectoryName(connFile);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(connFile))
            {
                File.WriteAllText(connFile,
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                    "<configuration>\r\n" +
                    "  <connectionStrings>\r\n" +
                    "    <add name=\"ConnectionString\" connectionString=\"Server=localhost;Port=5432;Database=;User ID=postgres;Password=\"/>\r\n" +
                    "  </connectionStrings>\r\n" +
                    "</configuration>\r\n");
                return;
            }

            // Migrar fragmento antiguo (solo <connectionStrings>) al formato con <configuration>
            string text = File.ReadAllText(connFile);
            if (text.IndexOf("<configuration", StringComparison.OrdinalIgnoreCase) < 0
                && text.IndexOf("<connectionStrings", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string migrated =
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                    "<configuration>\r\n" +
                    text.Trim().Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "").Trim() +
                    "\r\n</configuration>\r\n";
                File.WriteAllText(connFile, migrated);
            }
        }

        private static void AfterSave()
        {
            AppConnection.Invalidate();
        }

        public static ConnectionSettings Load()
        {
            try
            {
                Configuration config = OpenConnectionConfig();
                var section = config.GetSection(SectionName) as ConnectionStringsSection;
                if (section == null)
                    return new ConnectionSettings();

                ConnectionStringSettings item = section.ConnectionStrings[SettingName];
                if (item == null || string.IsNullOrWhiteSpace(item.ConnectionString))
                    return new ConnectionSettings();

                return ConnectionSettings.FromConnectionString(item.ConnectionString);
            }
            catch
            {
                return new ConnectionSettings();
            }
        }

        public static void Save(ConnectionSettings settings, bool encrypt)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            if (string.IsNullOrWhiteSpace(settings.Server))
                throw new InvalidOperationException("Indique el servidor.");
            if (string.IsNullOrWhiteSpace(settings.Database))
                throw new InvalidOperationException("Indique la base de datos.");
            if (string.IsNullOrWhiteSpace(settings.UserId))
                throw new InvalidOperationException("Indique el usuario.");
            if (settings.Port <= 0 || settings.Port > 65535)
                throw new InvalidOperationException("El puerto no es válido.");

            Configuration config = OpenConnectionConfig();
            var section = config.GetSection(SectionName) as ConnectionStringsSection;
            if (section == null)
                throw new InvalidOperationException("No se encontró connectionStrings en " + ConnectionFileName);

            if (section.SectionInformation.IsProtected)
                section.SectionInformation.UnprotectSection();

            string cs = settings.ToConnectionString();
            if (section.ConnectionStrings[SettingName] != null)
                section.ConnectionStrings[SettingName].ConnectionString = cs;
            else
                section.ConnectionStrings.Add(new ConnectionStringSettings(SettingName, cs));

            if (encrypt)
            {
                section.SectionInformation.ProtectSection(Protector);
                section.SectionInformation.ForceSave = true;
            }

            config.Save(ConfigurationSaveMode.Full);
            AfterSave();
        }

        public static void Unprotect()
        {
            Configuration config = OpenConnectionConfig();
            ConfigurationSection section = config.GetSection(SectionName);
            if (section == null)
                return;

            if (section.SectionInformation.IsProtected)
            {
                section.SectionInformation.UnprotectSection();
                section.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Full);
                AfterSave();
            }
        }

        public static void ProtectOnly()
        {
            Configuration config = OpenConnectionConfig();
            ConfigurationSection section = config.GetSection(SectionName);
            if (section == null)
                throw new InvalidOperationException("No se encontró connectionStrings.");

            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection(Protector);
                section.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Full);
                AfterSave();
            }
        }

        public static void Test(ConnectionSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            using (var conn = new NpgsqlConnection(settings.ToConnectionString()))
            {
                conn.Open();
            }
        }

        public static string ResumenEstado()
        {
            string exeConfig = Path.Combine(
                !string.IsNullOrWhiteSpace(TargetInstallDirectory)
                    ? TargetInstallDirectory
                    : AppDomain.CurrentDomain.BaseDirectory,
                MainExeFileName + ".config");

            return string.Format(
                CultureInfo.CurrentCulture,
                "Conexión: {0}{1}Exe.config: {2}{3}Cifrado: {4}",
                ConfigPath,
                Environment.NewLine,
                exeConfig,
                Environment.NewLine,
                IsEncrypted ? "Sí (protegido por Windows)" : "No (texto legible)");
        }

        public static string ConfigToolExePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sisges Configuracion.exe");
            }
        }

        public static bool TryLaunchConfigTool()
        {
            string path = ConfigToolExePath;
            if (!File.Exists(path))
                return false;
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = path,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                UseShellExecute = true
            });
            return true;
        }
    }
}
