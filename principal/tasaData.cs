using System;
using System.Data;
using Npgsql;

namespace principal
{
    public class tasaDiariaBO
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public int id_producto { get; set; }
        public string producto { get; set; }
        /// <summary>Precio tonelada $ a BCV.</summary>
        public decimal precio_tonelada_usd { get; set; }
        /// <summary>Precio tonelada en $ efectivo.</summary>
        public decimal precio_efectivo_usd { get; set; }
        /// <summary>Precio tonelada $ para modalidad 50/50 (mitad efectivo).</summary>
        public decimal precio_50_50_usd { get; set; }
        public decimal tasa_dolar_bs { get; set; }
    }

    class tasaData
    {
        static string connstring => AppConnection.ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstring);

        private static readonly object EsquemaLock = new object();
        private static bool _esquemaEspecialOk;

        private DataTable EjecutarCursor(string nombreSp, Action<NpgsqlCommand> agregarParametros)
        {
            DataTable resultado = new DataTable();
            try
            {
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter("fetch all in \"ref\"", conn);
                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    NpgsqlCommand command = new NpgsqlCommand("\"" + nombreSp + "\"", conn);
                    if (agregarParametros != null)
                        agregarParametros(command);

                    NpgsqlParameter prmCursor = new NpgsqlParameter(":ref", NpgsqlTypes.NpgsqlDbType.Refcursor);
                    prmCursor.Direction = ParameterDirection.InputOutput;
                    prmCursor.Value = "ref";
                    command.Parameters.Add(prmCursor);

                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                    dataAdapter.Fill(resultado);
                    resultado.Locale = System.Globalization.CultureInfo.InvariantCulture;
                    dataAdapter.Dispose();
                    t.Commit();
                    t.Dispose();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    dataAdapter.Dispose();
                    t.Dispose();
                    conn.Close();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return resultado;
        }

        private static void EjecutarSqlIgnorandoError(NpgsqlConnection c, string sql)
        {
            try
            {
                using (var cmd = c.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // No bloquear recepción/consulta por fallos de DDL secundarios.
            }
        }

        /// <summary>
        /// Esquema de tasa especial. Nunca lanza: la validación de recepción no debe fallar por el índice.
        /// </summary>
        private static void AsegurarEsquemaTasaEspecial()
        {
            if (_esquemaEspecialOk) return;
            lock (EsquemaLock)
            {
                if (_esquemaEspecialOk) return;
                try
                {
                    using (var c = new NpgsqlConnection(connstring))
                    {
                        c.Open();

                        EjecutarSqlIgnorandoError(c, @"
ALTER TABLE public.recepciones
    ADD COLUMN IF NOT EXISTS id_tasa integer NULL;");

                        EjecutarSqlIgnorandoError(c, @"
ALTER TABLE public.tasa_diaria DROP CONSTRAINT IF EXISTS fk_tasa_diaria_recepcion;
DROP INDEX IF EXISTS ix_tasa_diaria_id_recepcion;
ALTER TABLE public.tasa_diaria DROP COLUMN IF EXISTS id_recepcion;");

                        EjecutarSqlIgnorandoError(c, @"
ALTER TABLE public.tasa_diaria
    ADD COLUMN IF NOT EXISTS es_especial boolean NOT NULL DEFAULT false;");

                        // Tasas ya ligadas a una recepción = especiales
                        EjecutarSqlIgnorandoError(c, @"
UPDATE public.tasa_diaria t
SET es_especial = true
WHERE EXISTS (
    SELECT 1 FROM public.recepciones r
    WHERE r.id_tasa = t.id
);");

                        // Duplicados generales: deja la de menor id
                        EjecutarSqlIgnorandoError(c, @"
UPDATE public.tasa_diaria t
SET es_especial = true
FROM (
    SELECT fecha, id_producto, MIN(id) AS id_general
    FROM public.tasa_diaria
    WHERE es_especial = false
    GROUP BY fecha, id_producto
    HAVING COUNT(*) > 1
) d
WHERE t.fecha = d.fecha
  AND t.id_producto = d.id_producto
  AND t.es_especial = false
  AND t.id <> d.id_general;");

                        EjecutarSqlIgnorandoError(c, @"
ALTER TABLE public.tasa_diaria DROP CONSTRAINT IF EXISTS uq_tasa_diaria_fecha_producto;
DROP INDEX IF EXISTS uq_tasa_diaria_fecha_producto;
DROP INDEX IF EXISTS ux_tasa_diaria_fecha_producto_general;");

                        // Si aún hay duplicados, este CREATE puede fallar: se ignora.
                        EjecutarSqlIgnorandoError(c, @"
CREATE UNIQUE INDEX IF NOT EXISTS ux_tasa_diaria_fecha_producto_general
    ON public.tasa_diaria (fecha, id_producto)
    WHERE NOT es_especial;");
                    }
                }
                catch
                {
                    // Seguir: las consultas usan SQL directo.
                }
                _esquemaEspecialOk = true;
            }
        }

        public DataTable buscar()
        {
            try
            {
                AsegurarEsquemaTasaEspecial();
                return ConsultarSql(@"
SELECT
    t.id,
    t.fecha,
    t.id_producto,
    COALESCE(p.nombre, '') AS producto,
    t.precio_tonelada_usd,
    t.precio_efectivo_usd,
    t.precio_50_50_usd,
    t.tasa_dolar_bs,
    CASE WHEN t.es_especial THEN 'Especial' ELSE 'General' END AS tipo,
    t.es_especial,
    t.creado_en,
    t.actualizado_en
FROM tasa_diaria t
LEFT JOIN productos p ON p.id = t.id_producto
ORDER BY t.fecha DESC, t.es_especial ASC, p.nombre ASC, t.id DESC;");
            }
            catch
            {
                return EjecutarCursor("buscartasadiaria", null);
            }
        }

        public DataTable buscarPorId(int id)
        {
            try
            {
                AsegurarEsquemaTasaEspecial();
                return ConsultarSql(@"
SELECT
    t.id,
    t.fecha,
    t.id_producto,
    COALESCE(p.nombre, '') AS producto,
    t.precio_tonelada_usd,
    t.precio_efectivo_usd,
    t.precio_50_50_usd,
    t.tasa_dolar_bs,
    COALESCE(t.es_especial, false) AS es_especial,
    t.creado_en,
    t.actualizado_en
FROM tasa_diaria t
LEFT JOIN productos p ON p.id = t.id_producto
WHERE t.id = @id;",
                    cmd => cmd.Parameters.AddWithValue("id", id));
            }
            catch
            {
                return EjecutarCursor("buscartasadiariaxid", cmd =>
                {
                    cmd.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, id);
                });
            }
        }

        public bool EsTasaEspecial(int idTasa)
        {
            if (idTasa <= 0) return false;
            try
            {
                DataTable dt = buscarPorId(idTasa);
                if (dt == null || dt.Rows.Count == 0) return false;
                if (!dt.Columns.Contains("es_especial") || dt.Rows[0]["es_especial"] == DBNull.Value)
                    return false;
                return Convert.ToBoolean(dt.Rows[0]["es_especial"]);
            }
            catch
            {
                return false;
            }
        }

        public DataTable buscarPorFecha(DateTime fecha)
        {
            return EjecutarCursor("buscartasadiariaxfecha", cmd =>
            {
                cmd.Parameters.AddWithValue(":prmfecha", NpgsqlTypes.NpgsqlDbType.Date, fecha.Date);
            });
        }

        public DataTable buscarPorFechaProducto(DateTime fecha, int idProducto)
        {
            AsegurarEsquemaTasaEspecial();

            // Preferir GENERAL; si no hay, usar cualquier tasa de esa fecha/producto (no bloquear recepción).
            try
            {
                DataTable general = ConsultarSql(@"
SELECT
    t.id,
    t.fecha,
    t.id_producto,
    COALESCE(p.nombre, '') AS producto,
    t.precio_tonelada_usd,
    t.precio_efectivo_usd,
    t.precio_50_50_usd,
    t.tasa_dolar_bs,
    t.creado_en,
    t.actualizado_en
FROM tasa_diaria t
LEFT JOIN productos p ON p.id = t.id_producto
WHERE t.id = (
    SELECT MIN(t2.id)
    FROM tasa_diaria t2
    WHERE t2.fecha = @fecha
      AND t2.id_producto = @id_producto
      AND COALESCE(t2.es_especial, false) = false
);",
                    cmd =>
                    {
                        cmd.Parameters.AddWithValue("fecha", fecha.Date);
                        cmd.Parameters.AddWithValue("id_producto", idProducto);
                    });
                if (general != null && general.Rows.Count > 0)
                    return general;

                return ConsultarSql(@"
SELECT
    t.id,
    t.fecha,
    t.id_producto,
    COALESCE(p.nombre, '') AS producto,
    t.precio_tonelada_usd,
    t.precio_efectivo_usd,
    t.precio_50_50_usd,
    t.tasa_dolar_bs,
    t.creado_en,
    t.actualizado_en
FROM tasa_diaria t
LEFT JOIN productos p ON p.id = t.id_producto
WHERE t.id = (
    SELECT MIN(t2.id)
    FROM tasa_diaria t2
    WHERE t2.fecha = @fecha
      AND t2.id_producto = @id_producto
);",
                    cmd =>
                    {
                        cmd.Parameters.AddWithValue("fecha", fecha.Date);
                        cmd.Parameters.AddWithValue("id_producto", idProducto);
                    });
            }
            catch
            {
                return EjecutarCursor("buscartasadiariaxfechaproducto", cmd =>
                {
                    cmd.Parameters.AddWithValue(":prmfecha", NpgsqlTypes.NpgsqlDbType.Date, fecha.Date);
                    cmd.Parameters.AddWithValue(":prmid_producto", NpgsqlTypes.NpgsqlDbType.Integer, idProducto);
                });
            }
        }

        public bool ExisteTasa(DateTime fecha, int idProducto)
        {
            try
            {
                DataTable dt = buscarPorFechaProducto(fecha, idProducto);
                return dt != null && dt.Rows.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public DataTable insertar(
            DateTime fecha,
            int idProducto,
            decimal precioToneladaUsd,
            decimal tasaDolarBs,
            decimal precioEfectivoUsd,
            decimal precio5050Usd)
        {
            return EjecutarCursor("spinsertartasadiaria", cmd =>
            {
                cmd.Parameters.AddWithValue(":prmfecha", NpgsqlTypes.NpgsqlDbType.Date, fecha.Date);
                cmd.Parameters.AddWithValue(":prmid_producto", NpgsqlTypes.NpgsqlDbType.Integer, idProducto);
                cmd.Parameters.AddWithValue(":prmprecio_tonelada_usd", NpgsqlTypes.NpgsqlDbType.Numeric, precioToneladaUsd);
                cmd.Parameters.AddWithValue(":prmtasa_dolar_bs", NpgsqlTypes.NpgsqlDbType.Numeric, tasaDolarBs);
                cmd.Parameters.AddWithValue(":prmprecio_efectivo_usd", NpgsqlTypes.NpgsqlDbType.Numeric, precioEfectivoUsd);
                cmd.Parameters.AddWithValue(":prmprecio_50_50_usd", NpgsqlTypes.NpgsqlDbType.Numeric, precio5050Usd);
            });
        }

        /// <summary>
        /// Inserta tasa especial nueva (es_especial=true). La recepción se asocia con recepciones.id_tasa.
        /// </summary>
        public DataTable insertarEspecial(
            DateTime fecha,
            int idProducto,
            int idRecepcion,
            decimal precioToneladaUsd,
            decimal tasaDolarBs,
            decimal precioEfectivoUsd,
            decimal precio5050Usd)
        {
            if (idRecepcion <= 0)
                throw new Exception("La tasa especial requiere una recepción válida.");

            AsegurarEsquemaTasaEspecial();

            return ConsultarSql(@"
INSERT INTO tasa_diaria (
    fecha, id_producto, precio_tonelada_usd, tasa_dolar_bs,
    precio_efectivo_usd, precio_50_50_usd, es_especial
)
VALUES (
    @fecha, @id_producto, @precio_tonelada_usd, @tasa_dolar_bs,
    @precio_efectivo_usd, @precio_50_50_usd, true
)
RETURNING
    id,
    fecha,
    id_producto,
    precio_tonelada_usd,
    precio_efectivo_usd,
    precio_50_50_usd,
    tasa_dolar_bs,
    creado_en,
    actualizado_en;",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("fecha", fecha.Date);
                    cmd.Parameters.AddWithValue("id_producto", idProducto);
                    cmd.Parameters.AddWithValue("precio_tonelada_usd", precioToneladaUsd);
                    cmd.Parameters.AddWithValue("tasa_dolar_bs", tasaDolarBs);
                    cmd.Parameters.AddWithValue("precio_efectivo_usd", precioEfectivoUsd);
                    cmd.Parameters.AddWithValue("precio_50_50_usd", precio5050Usd);
                });
        }

        public DataTable actualizar(
            int id,
            int idProducto,
            decimal precioToneladaUsd,
            decimal tasaDolarBs,
            decimal precioEfectivoUsd,
            decimal precio5050Usd)
        {
            return EjecutarCursor("spactualizartasadiaria", cmd =>
            {
                cmd.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue(":prmid_producto", NpgsqlTypes.NpgsqlDbType.Integer, idProducto);
                cmd.Parameters.AddWithValue(":prmprecio_tonelada_usd", NpgsqlTypes.NpgsqlDbType.Numeric, precioToneladaUsd);
                cmd.Parameters.AddWithValue(":prmtasa_dolar_bs", NpgsqlTypes.NpgsqlDbType.Numeric, tasaDolarBs);
                cmd.Parameters.AddWithValue(":prmprecio_efectivo_usd", NpgsqlTypes.NpgsqlDbType.Numeric, precioEfectivoUsd);
                cmd.Parameters.AddWithValue(":prmprecio_50_50_usd", NpgsqlTypes.NpgsqlDbType.Numeric, precio5050Usd);
            });
        }

        private DataTable ConsultarSql(string sql, Action<NpgsqlCommand> agregarParametros = null)
        {
            var resultado = new DataTable();
            using (var c = new NpgsqlConnection(connstring))
            {
                c.Open();
                using (var cmd = c.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (agregarParametros != null)
                        agregarParametros(cmd);
                    using (var da = new NpgsqlDataAdapter(cmd))
                    {
                        da.Fill(resultado);
                    }
                }
            }
            resultado.Locale = System.Globalization.CultureInfo.InvariantCulture;
            return resultado;
        }
    }
}
