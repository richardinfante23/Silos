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
    /// Puede apuntar al .config de Sisges Pro.exe (herramienta externa).
    /// </summary>
    public static class ConnectionConfigHelper
    {
        private const string SectionName = "connectionStrings";
        private const string SettingName = "ConnectionString";
        private const string Protector = "DataProtectionConfigurationProvider";
        public const string MainExeFileName = "Sisges Pro.exe";

        /// <summary>Si se asigna, se lee/escribe ese archivo en lugar del .config del exe actual.</summary>
        public static string TargetConfigFilePath { get; set; }

        public static string ConfigPath
        {
            get { return OpenConfig().FilePath; }
        }

        public static bool IsEncrypted
        {
            get
            {
                ConfigurationSection section = OpenConfig().GetSection(SectionName);
                return section != null && section.SectionInformation.IsProtected;
            }
        }

        /// <summary>
        /// Apunta al Sisges Pro.exe.config en la misma carpeta que este ejecutable.
        /// </summary>
        public static void UseSiblingSisgesConfig()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(dir, MainExeFileName + ".config");
            if (!File.Exists(path))
                path = Path.Combine(dir, MainExeFileName + ".Config");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    "No se encontró \"" + MainExeFileName + ".config\" en:\n" + dir +
                    "\nColoque esta herramienta en la misma carpeta que Sisges Pro.");
            }
            TargetConfigFilePath = path;
        }

        private static Configuration OpenConfig()
        {
            if (!string.IsNullOrWhiteSpace(TargetConfigFilePath))
            {
                var map = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = TargetConfigFilePath
                };
                return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            }
            return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        private static void AfterSave()
        {
            ConfigurationManager.RefreshSection(SectionName);
            AppConnection.Invalidate();
        }

        public static ConnectionSettings Load()
        {
            try
            {
                Configuration config = OpenConfig();
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

            Configuration config = OpenConfig();
            var section = config.GetSection(SectionName) as ConnectionStringsSection;
            if (section == null)
                throw new InvalidOperationException("No se encontró la sección connectionStrings en el archivo de configuración.");

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

            config.Save(ConfigurationSaveMode.Modified);
            AfterSave();
        }

        public static void Unprotect()
        {
            Configuration config = OpenConfig();
            ConfigurationSection section = config.GetSection(SectionName);
            if (section == null)
                return;

            if (section.SectionInformation.IsProtected)
            {
                section.SectionInformation.UnprotectSection();
                section.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Modified);
                AfterSave();
            }
        }

        public static void ProtectOnly()
        {
            Configuration config = OpenConfig();
            ConfigurationSection section = config.GetSection(SectionName);
            if (section == null)
                throw new InvalidOperationException("No se encontró connectionStrings.");

            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection(Protector);
                section.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Modified);
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
            return string.Format(
                CultureInfo.CurrentCulture,
                "Archivo: {0}{1}Cifrado: {2}",
                ConfigPath,
                Environment.NewLine,
                IsEncrypted ? "Sí (protegido por Windows)" : "No (texto legible)");
        }

        /// <summary>Ruta del ejecutable de configuración junto a Sisges.</summary>
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
