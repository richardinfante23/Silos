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
    class usuarioData
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarusuarios\"", conn);
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

        #region BuscarPerfil
        public DataTable buscarPerfil()
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarperfil\"", conn);
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
        public bool insertar(string prmlogin, string prmclave,string prmnombre,int prmid_perfil)
        {
            bool guardo = false;
            try
            {

                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"spinsertarusuarios\"", conn);

                    command.Parameters.AddWithValue(":prmlogin", NpgsqlTypes.NpgsqlDbType.Varchar, prmlogin);
                    command.Parameters.AddWithValue(":prmclave", NpgsqlTypes.NpgsqlDbType.Varchar, prmclave);
                    command.Parameters.AddWithValue(":prmnombre", NpgsqlTypes.NpgsqlDbType.Varchar, prmnombre);
                     command.Parameters.AddWithValue(":prmid_perfil", NpgsqlTypes.NpgsqlDbType.Integer, prmid_perfil);
    

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
        public bool actualizar(int prmid, string prmlogin, string prmclave,  string prmnombre, int prmid_perfil,bool prmeliminado)
        {

            bool guardo = false;
            try
            {

                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"spactualizarusuario\"", conn);
                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);
                    command.Parameters.AddWithValue(":prmlogin", NpgsqlTypes.NpgsqlDbType.Varchar, prmlogin);
                    command.Parameters.AddWithValue(":prmclave", NpgsqlTypes.NpgsqlDbType.Varchar, prmclave);
                    command.Parameters.AddWithValue(":prmnombre", NpgsqlTypes.NpgsqlDbType.Varchar, prmnombre);
                    command.Parameters.AddWithValue(":prmid_perfil", NpgsqlTypes.NpgsqlDbType.Integer, prmid_perfil);
                    command.Parameters.AddWithValue(":prmeliminado", NpgsqlTypes.NpgsqlDbType.Boolean, prmeliminado);

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
        public DataTable logeo(string prmlogin, string prmclave)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"logeo\"", conn);
                    command.Parameters.AddWithValue(":prmlogin", NpgsqlTypes.NpgsqlDbType.Varchar, prmlogin);
                    command.Parameters.AddWithValue(":prmclave", NpgsqlTypes.NpgsqlDbType.Varchar, prmclave);
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
