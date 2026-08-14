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
    public class transportesData
    {

        static string connstring = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscaratransportes\"", conn);
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

        #region Insertar
        public bool insertar(string prmrif, string prmnombre)
        {
            bool guardo = false;
            try
            {

                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"spinsertartransporte\"", conn);
                    command.Parameters.AddWithValue(":prmrif", NpgsqlTypes.NpgsqlDbType.Varchar, prmrif);
                    command.Parameters.AddWithValue(":prmnombre", NpgsqlTypes.NpgsqlDbType.Varchar, prmnombre);


                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                    guardo = true;
                    t.Commit();
                    t.Dispose();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    guardo = false;
                    t.Dispose();
                    conn.Close();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return guardo;
        }
        #endregion

        #region Actualizar
        public bool actualizar(int prmid,string prmrif, string prmnombre ,bool prmeliminado)
        {

            bool guardo = false;
            try
            {

                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"spactualizartransportes\"", conn);
                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);
                    command.Parameters.AddWithValue(":prmrif", NpgsqlTypes.NpgsqlDbType.Varchar, prmrif);
                    command.Parameters.AddWithValue(":prmnombre", NpgsqlTypes.NpgsqlDbType.Varchar, prmnombre);
                    command.Parameters.AddWithValue(":prmeliminar", NpgsqlTypes.NpgsqlDbType.Boolean, prmeliminado);

                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();

                    guardo = true;
                    t.Commit();
                    t.Dispose();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    guardo = false;
                    t.Dispose();
                    conn.Close();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return guardo;
        }
        #endregion
    }
}
