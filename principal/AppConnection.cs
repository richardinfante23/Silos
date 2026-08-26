using System;
using System.Configuration;

namespace principal
{
    /// <summary>
    /// Cadena de conexión centralizada. Tras cambiar/cifrar config, llamar Invalidate().
    /// </summary>
    public static class AppConnection
    {
        private static readonly object Sync = new object();
        private static string _cached;

        public static string ConnectionString
        {
            get
            {
                lock (Sync)
                {
                    if (_cached == null)
                        _cached = LeerDesdeConfig();
                    return _cached;
                }
            }
        }

        public static void Invalidate()
        {
            lock (Sync)
            {
                _cached = null;
            }
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        private static string LeerDesdeConfig()
        {
            ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["ConnectionString"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
                throw new InvalidOperationException(
                    "No hay ConnectionString configurada. Use Configuración → Conexión a base de datos.");
            return cs.ConnectionString.Trim();
        }
    }
}
