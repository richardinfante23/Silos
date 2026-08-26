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
    class Tercero_ArticulosDatacs
    {
        static string connstring => AppConnection.ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstring);


        #region Buscar
        public DataTable buscar(int prmidprm_tipo_tercero)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarterceroarticulos\"", conn);
                    command.Parameters.AddWithValue(":prm_tipo_tercero", NpgsqlTypes.NpgsqlDbType.Integer, prmidprm_tipo_tercero);
                  

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

        public DataTable insertar(string prmnombre, string prmcedula,string prmrif,string prmdireccion,string prmtelefono,int prmid_tipo_terceros)
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
                     NpgsqlCommand command = new NpgsqlCommand("\"spinsertartercerosarticulos\"", conn);

                    command.Parameters.AddWithValue(":prmnombre", NpgsqlTypes.NpgsqlDbType.Text, prmnombre);
                    command.Parameters.AddWithValue(":prmcedula", NpgsqlTypes.NpgsqlDbType.Text, prmcedula);
                    command.Parameters.AddWithValue(":prmrif", NpgsqlTypes.NpgsqlDbType.Text, prmrif);
                    command.Parameters.AddWithValue(":prmdireccion", NpgsqlTypes.NpgsqlDbType.Text, prmdireccion);
                    command.Parameters.AddWithValue(":prmtelefono", NpgsqlTypes.NpgsqlDbType.Text, prmtelefono);
                    command.Parameters.AddWithValue(":prmid_tipo_terceros", NpgsqlTypes.NpgsqlDbType.Integer, prmid_tipo_terceros);

                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();

                    dataAdapter.Dispose();


                    t.Commit();
                    t.Dispose();
                    conn.Close();
                }
                catch (Exception ex)
                {
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

        public DataTable actualizar(int prmid,string prmnombre, string prmcedula, string prmrif, string prmdireccion, string prmtelefono, int prmid_tipo_terceros,bool prmeliminado)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"spactualizartercerosarticulos\"", conn);

                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);
                    command.Parameters.AddWithValue(":prmnombre", NpgsqlTypes.NpgsqlDbType.Text, prmnombre);
                    command.Parameters.AddWithValue(":prmcedula", NpgsqlTypes.NpgsqlDbType.Text, prmcedula);
                    command.Parameters.AddWithValue(":prmrif", NpgsqlTypes.NpgsqlDbType.Text, prmrif);
                    command.Parameters.AddWithValue(":prmdireccion", NpgsqlTypes.NpgsqlDbType.Text, prmdireccion);
                    command.Parameters.AddWithValue(":prmtelefono", NpgsqlTypes.NpgsqlDbType.Text, prmtelefono);
                    command.Parameters.AddWithValue(":prmid_tipo_terceros", NpgsqlTypes.NpgsqlDbType.Integer, prmid_tipo_terceros);
                    command.Parameters.AddWithValue(":prmeliminado", NpgsqlTypes.NpgsqlDbType.Boolean, prmeliminado);

                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();

                    dataAdapter.Dispose();


                    t.Commit();
                    t.Dispose();
                    conn.Close();
                }
                catch (Exception ex)
                {
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


        #region Buscar articulo spor unidad
        public DataTable inventarioxtercero(int prmid_tercero)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"inventarioxtercero\"", conn);
                    command.Parameters.AddWithValue(":prmid_tercero", NpgsqlTypes.NpgsqlDbType.Integer, prmid_tercero);
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
