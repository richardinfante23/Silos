using System;

namespace principal
{
    /// <summary>
    /// Cadena de conexión centralizada desde SisgesConnection.config.
    /// Tras cambiar/cifrar, llamar Invalidate().
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
        }

        private static string LeerDesdeConfig()
        {
            ConnectionSettings settings = ConnectionConfigHelper.Load();
            if (settings == null
                || string.IsNullOrWhiteSpace(settings.Server)
                || string.IsNullOrWhiteSpace(settings.Database))
            {
                throw new InvalidOperationException(
                    "No hay ConnectionString configurada. Ejecute \"Sisges Configuracion.exe\".");
            }
            return settings.ToConnectionString();
        }
    }
}
