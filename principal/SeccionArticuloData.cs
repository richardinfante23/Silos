using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Configuration;
using System.Data;

namespace principal
{
  

    class SeccionArticuloData
    {

        static string connstring => AppConnection.ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstring);

        #region Buscar
        public DataTable buscar(int prmid)
        {

            DataTable redusltadoDT = new DataTable();
            try
            {
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter("fetch all in \"ref\"", conn);
                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarsecciones\"", conn);
                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);

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
        #endregion


        #region Buscar
        public DataTable buscarxarticulo(int prmid_articulo, int prmid_estante)
        {

            DataTable redusltadoDT = new DataTable();
            try
            {
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter("fetch all in \"ref\"", conn);
                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarseccionxarticulo\"", conn);
                    command.Parameters.AddWithValue(":prmid_articulo", NpgsqlTypes.NpgsqlDbType.Integer, prmid_articulo);
                    command.Parameters.AddWithValue(":prmid_estante", NpgsqlTypes.NpgsqlDbType.Integer, prmid_estante);

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
        #endregion
    }
}
