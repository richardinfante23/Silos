using System;
using System.Configuration;
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
        public decimal precio_tonelada_usd { get; set; }
        public decimal tasa_dolar_bs { get; set; }
    }

    class tasaData
    {
        static string connstring = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstring);

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

        public DataTable buscar()
        {
            return EjecutarCursor("buscartasadiaria", null);
        }

        public DataTable buscarPorId(int id)
        {
            return EjecutarCursor("buscartasadiariaxid", cmd =>
            {
                cmd.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, id);
            });
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
            return EjecutarCursor("buscartasadiariaxfechaproducto", cmd =>
            {
                cmd.Parameters.AddWithValue(":prmfecha", NpgsqlTypes.NpgsqlDbType.Date, fecha.Date);
                cmd.Parameters.AddWithValue(":prmid_producto", NpgsqlTypes.NpgsqlDbType.Integer, idProducto);
            });
        }

        public bool ExisteTasa(DateTime fecha, int idProducto)
        {
            DataTable dt = buscarPorFechaProducto(fecha, idProducto);
            return dt != null && dt.Rows.Count > 0;
        }

        public DataTable insertar(DateTime fecha, int idProducto, decimal precioToneladaUsd, decimal tasaDolarBs)
        {
            return EjecutarCursor("spinsertartasadiaria", cmd =>
            {
                cmd.Parameters.AddWithValue(":prmfecha", NpgsqlTypes.NpgsqlDbType.Date, fecha.Date);
                cmd.Parameters.AddWithValue(":prmid_producto", NpgsqlTypes.NpgsqlDbType.Integer, idProducto);
                cmd.Parameters.AddWithValue(":prmprecio_tonelada_usd", NpgsqlTypes.NpgsqlDbType.Numeric, precioToneladaUsd);
                cmd.Parameters.AddWithValue(":prmtasa_dolar_bs", NpgsqlTypes.NpgsqlDbType.Numeric, tasaDolarBs);
            });
        }

        public DataTable actualizar(int id, int idProducto, decimal precioToneladaUsd, decimal tasaDolarBs)
        {
            return EjecutarCursor("spactualizartasadiaria", cmd =>
            {
                cmd.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue(":prmid_producto", NpgsqlTypes.NpgsqlDbType.Integer, idProducto);
                cmd.Parameters.AddWithValue(":prmprecio_tonelada_usd", NpgsqlTypes.NpgsqlDbType.Numeric, precioToneladaUsd);
                cmd.Parameters.AddWithValue(":prmtasa_dolar_bs", NpgsqlTypes.NpgsqlDbType.Numeric, tasaDolarBs);
            });
        }
    }
}
