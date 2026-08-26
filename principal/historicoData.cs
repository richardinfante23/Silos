using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Npgsql;
using System.Configuration;
using System.Data;

namespace principal
{
    class historicoData
    {
        static string connstring => AppConnection.ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstring);
        #region Buscar
        public DataTable buscar()
        {

            DataTable redusltadoDT = new DataTable();
            try
            {
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter("fetch all in \"ref\"", conn); ;
                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarhistorico\"", conn);
                    #region parametro cursor
                    NpgsqlParameter prmCursor = new NpgsqlParameter(":ref", NpgsqlTypes.NpgsqlDbType.Refcursor);
                    prmCursor.Direction = ParameterDirection.InputOutput;
                    prmCursor.Value = "ref";
                    command.Parameters.Add(prmCursor);
                    #endregion
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                    NpgsqlCommandBuilder commandBuilder = new NpgsqlCommandBuilder(dataAdapter);
                    dataAdapter.Fill(redusltadoDT);
                    redusltadoDT.Locale = System.Globalization.CultureInfo.InvariantCulture;
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
            return redusltadoDT;
        }

        private static readonly CultureInfo CulturaEs = CultureInfo.GetCultureInfo("es-VE");

        private static string NormalizarNombre(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;
            return CulturaEs.TextInfo.ToTitleCase(valor.Trim().ToLowerInvariant());
        }

        public bool TryObtenerInventario(string nombreProducto, string nombreSilo, out decimal pesoNeto, out decimal pesoAcondicionado)
        {
            pesoNeto = 0;
            pesoAcondicionado = 0;

            string producto = NormalizarNombre(nombreProducto);
            string silo = NormalizarNombre(nombreSilo);
            if (string.IsNullOrEmpty(producto) || string.IsNullOrEmpty(silo))
                return false;

            DataTable dt = buscar();
            foreach (DataRow fila in dt.Rows)
            {
                if (NormalizarNombre(Convert.ToString(fila["producto"])) != producto)
                    continue;
                if (NormalizarNombre(Convert.ToString(fila["silos"])) != silo)
                    continue;

                pesoNeto = Convert.ToDecimal(fila["peso_neto"]);
                pesoAcondicionado = Convert.ToDecimal(fila["peso_acondicionado"]);
                return true;
            }

            return false;
        }

        public bool ProductoTieneInventario(string nombreProducto)
        {
            string producto = NormalizarNombre(nombreProducto);
            if (string.IsNullOrEmpty(producto))
                return false;

            DataTable dt = buscar();
            foreach (DataRow fila in dt.Rows)
            {
                if (NormalizarNombre(Convert.ToString(fila["producto"])) != producto)
                    continue;
                if (Convert.ToDecimal(fila["peso_neto"]) > 0)
                    return true;
            }

            return false;
        }
        #endregion


        public DataTable buscarInventarioArticulos()
        {

            DataTable redusltadoDT = new DataTable();
            try
            {
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter("fetch all in \"ref\"", conn); ;
                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"inventarioarticulos\"", conn);
                    #region parametro cursor
                    NpgsqlParameter prmCursor = new NpgsqlParameter(":ref", NpgsqlTypes.NpgsqlDbType.Refcursor);
                    prmCursor.Direction = ParameterDirection.InputOutput;
                    prmCursor.Value = "ref";
                    command.Parameters.Add(prmCursor);
                    #endregion
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                    NpgsqlCommandBuilder commandBuilder = new NpgsqlCommandBuilder(dataAdapter);
                    dataAdapter.Fill(redusltadoDT);
                    redusltadoDT.Locale = System.Globalization.CultureInfo.InvariantCulture;
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
            return redusltadoDT;
        }

    }
}
