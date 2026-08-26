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
   public  class conductoresData
    {

        static string connstring => AppConnection.ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstring);
        #region Buscar
        public DataTable buscar(decimal prmCedula)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarconductores\"", conn);
                    command.Parameters.AddWithValue(":prmcedula", NpgsqlTypes.NpgsqlDbType.Numeric, prmCedula);
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

        public DataTable buscarGeneral()
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarconductoresgeneral\"", conn);
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
        public bool insertar(decimal prmCedula, string prmnombre, int prmid_transporte, bool prmeliminado)
        {
            bool guardo = false;
            try
            {
               
                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"spinsertarcondcutores\"", conn);
                    command.Parameters.AddWithValue(":prmcedula", NpgsqlTypes.NpgsqlDbType.Integer, prmCedula);
                    command.Parameters.AddWithValue(":prmnombre", NpgsqlTypes.NpgsqlDbType.Varchar, prmnombre);
                    command.Parameters.AddWithValue(":prmid_transporte", NpgsqlTypes.NpgsqlDbType.Integer, prmid_transporte);
                   
             
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
        public bool actualizar(int prmid,decimal prmCedula, string prmnombre, int prmid_transporte, bool prmeliminado)
        {

            bool guardo = false;
            try
            {
              
                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"spactualizconductores\"", conn);
                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);
                    command.Parameters.AddWithValue(":prmcedula", NpgsqlTypes.NpgsqlDbType.Integer, prmCedula);
                    command.Parameters.AddWithValue(":prmnombre", NpgsqlTypes.NpgsqlDbType.Varchar, prmnombre);
                    command.Parameters.AddWithValue(":prmid_transporte", NpgsqlTypes.NpgsqlDbType.Integer, prmid_transporte);
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

        #region BuscarXconductorid
        public DataTable buscarconductorxidtransporte(int prmid_transporte)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarconductoresxidtransporte\"", conn);
                    #region parametro cursor
                    NpgsqlParameter prmCursor = new NpgsqlParameter(":ref", NpgsqlTypes.NpgsqlDbType.Refcursor);
                    command.Parameters.AddWithValue(":prmid_transporte", NpgsqlTypes.NpgsqlDbType.Integer, prmid_transporte);

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
       

           #region BuscarproductorXid
        public DataTable buscartramitesxidconductor(int prmid_conductor)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscartramitesxidconductor\"", conn);
                    #region parametro cursor
                    NpgsqlParameter prmCursor = new NpgsqlParameter(":ref", NpgsqlTypes.NpgsqlDbType.Refcursor);
                    command.Parameters.AddWithValue(":prmid_conductor", NpgsqlTypes.NpgsqlDbType.Integer, prmid_conductor);

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
