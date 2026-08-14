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
    class ProductorData
    {
        static string connstring = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstring);

        #region PersonaNatural
        #region Buscar
        public DataTable buscar(string prmletra,decimal prmcedula)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarproductornatural\"", conn);
                    #region parametro cursor
                    NpgsqlParameter prmCursor = new NpgsqlParameter(":ref", NpgsqlTypes.NpgsqlDbType.Refcursor);
                    command.Parameters.AddWithValue(":prmletra", NpgsqlTypes.NpgsqlDbType.Varchar, prmletra);
                    command.Parameters.AddWithValue(":prmcedula", NpgsqlTypes.NpgsqlDbType.Numeric, prmcedula);
                    
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

        #region buscar todos
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
                            NpgsqlCommand command = new NpgsqlCommand("\"buscarproductornatural\"", conn);
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
        public bool insertar(string prmnombre,decimal prmCedula,string prmletra ,int prmid_asociacion, bool prmeliminado)
        {
            bool guardo = false;
            try
            {

                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"spinsertarproductornatural\"", conn);
                    command.Parameters.AddWithValue(":prmnombre", NpgsqlTypes.NpgsqlDbType.Varchar, prmnombre);
                    command.Parameters.AddWithValue(":prmcedula", NpgsqlTypes.NpgsqlDbType.Numeric, prmCedula);
                    command.Parameters.AddWithValue(":prmletra", NpgsqlTypes.NpgsqlDbType.Varchar, prmletra); 
                    command.Parameters.AddWithValue(":prmid_asociacion", NpgsqlTypes.NpgsqlDbType.Integer, prmid_asociacion);
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
        public bool actualizar(int prmid, string prmnombre,decimal prmCedula,string prmletra ,int prmid_asociacion, bool prmeliminado)
        {

            bool guardo = false;
            try
            {

                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"spactualizarproductornatural\"", conn);
                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);
                    command.Parameters.AddWithValue(":prmnombre", NpgsqlTypes.NpgsqlDbType.Varchar, prmnombre);
                    command.Parameters.AddWithValue(":prmcedula", NpgsqlTypes.NpgsqlDbType.Numeric, prmCedula);
                    command.Parameters.AddWithValue(":prmletra", NpgsqlTypes.NpgsqlDbType.Varchar, prmletra);
                    command.Parameters.AddWithValue(":prmid_asociacion", NpgsqlTypes.NpgsqlDbType.Integer, prmid_asociacion);
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
        #endregion


       #region Buscarxid
        public DataTable buscarproductorxAsociacion(int prmid_asociacion)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarproductornaturalxidasociacion\"", conn);
                    #region parametro cursor
                    NpgsqlParameter prmCursor = new NpgsqlParameter(":ref", NpgsqlTypes.NpgsqlDbType.Refcursor);
                    command.Parameters.AddWithValue(":prmid_asociacion", NpgsqlTypes.NpgsqlDbType.Integer, prmid_asociacion);
                    
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

       #region Buscar Productoc X id
        public DataTable bustramitesxidPorductor(int prmid_productor)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscartramitesxidproductor\"", conn);
                    #region parametro cursor
                    NpgsqlParameter prmCursor = new NpgsqlParameter(":ref", NpgsqlTypes.NpgsqlDbType.Refcursor);
                    command.Parameters.AddWithValue(":prmid_productor", NpgsqlTypes.NpgsqlDbType.Integer, prmid_productor);
                    
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

        

        #region Persona Juridica
        #region Buscar
        public DataTable buscarPersonaJuridica(string prmletra, decimal prmrif)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarproductorjuridico\"", conn);
                    #region parametro cursor
                    NpgsqlParameter prmCursor = new NpgsqlParameter(":ref", NpgsqlTypes.NpgsqlDbType.Refcursor);
                    command.Parameters.AddWithValue(":prmletra", NpgsqlTypes.NpgsqlDbType.Varchar, prmletra);
                    command.Parameters.AddWithValue(":prmrif", NpgsqlTypes.NpgsqlDbType.Numeric, prmrif);

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
        public bool insertarPersonaJuridica(string prmnombre, decimal prmrif, string prmletra, int prmid_asociacion, bool prmeliminado)
        {
            bool guardo = false;
            try
            {

                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"spinsertarproductorjuridico\"", conn);
                    command.Parameters.AddWithValue(":prmnombre", NpgsqlTypes.NpgsqlDbType.Varchar, prmnombre);
                    command.Parameters.AddWithValue(":prmrif", NpgsqlTypes.NpgsqlDbType.Numeric, prmrif);
                    command.Parameters.AddWithValue(":prmletra", NpgsqlTypes.NpgsqlDbType.Varchar, prmletra);
                    command.Parameters.AddWithValue(":prmid_asociacion", NpgsqlTypes.NpgsqlDbType.Integer, prmid_asociacion);
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
        public bool actualizarPersonaJuridica(int prmid, string prmnombre, decimal prmrif, string prmletra, int prmid_asociacion, bool prmeliminado)
        {

            bool guardo = false;
            try
            {

                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"spactualizarproductojuridico\"", conn);
                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);
                    command.Parameters.AddWithValue(":prmnombre", NpgsqlTypes.NpgsqlDbType.Varchar, prmnombre);
                    command.Parameters.AddWithValue(":prmrif", NpgsqlTypes.NpgsqlDbType.Bigint, prmrif);
                    command.Parameters.AddWithValue(":prmletra", NpgsqlTypes.NpgsqlDbType.Varchar, prmletra);
                    command.Parameters.AddWithValue(":prmid_asociacion", NpgsqlTypes.NpgsqlDbType.Integer, prmid_asociacion);
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

        #region buscar todos
        public DataTable buscarJuridicoGeneral()
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarproductorjuridicogeneral\"", conn);
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

        #region Buscar Productoc X id
        public DataTable bustramitesxidPorductorJuridico(int prmid_productor)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscartramitesxidproductorjuridico\"", conn);
                    #region parametro cursor
                    NpgsqlParameter prmCursor = new NpgsqlParameter(":ref", NpgsqlTypes.NpgsqlDbType.Refcursor);
                    command.Parameters.AddWithValue(":prmid_productor", NpgsqlTypes.NpgsqlDbType.Integer, prmid_productor);

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
        #endregion


    }
}
