using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace principal
{


    class AuditoriaData
    {
        static string connstring => AppConnection.ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstring);

        public DataTable buscarauditoria(int codigo,DateTime prmfecha,DateTime prmfechahasta)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarauditoria\"", conn);

                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, codigo);
                    command.Parameters.AddWithValue(":prmfecha", NpgsqlTypes.NpgsqlDbType.Date, prmfecha);
                    command.Parameters.AddWithValue(":prmfechahasta", NpgsqlTypes.NpgsqlDbType.Date, prmfechahasta);
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

        public DataTable buscarauditoriadetalle(int codigo,DateTime prmfecha, DateTime prmfechahasta)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarauditoriadetalles\"", conn);


                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, codigo);
                    command.Parameters.AddWithValue(":prmfecha", NpgsqlTypes.NpgsqlDbType.Date, prmfecha);
                    command.Parameters.AddWithValue(":prmfechahasta", NpgsqlTypes.NpgsqlDbType.Date, prmfechahasta);
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
