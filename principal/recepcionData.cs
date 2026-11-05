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
    class recepcionData
    {
        static string connstring => AppConnection.ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstring);
        #region Buscar
        public DataTable buscar(int prmid, int prmid_producto, int prmid_condutor, int prmid_productor, int prmid_productor_juridico, string prmnro_placa, int prmid_silos, int prmid_estatus, DateTime prmfecha, DateTime prmfechahasta)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarcrecepciones\"", conn);
                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);
               
                     command.Parameters.AddWithValue(":prmid_producto", NpgsqlTypes.NpgsqlDbType.Integer, prmid_producto);
                     command.Parameters.AddWithValue(":prmid_condutor", NpgsqlTypes.NpgsqlDbType.Integer, prmid_condutor);
                    command.Parameters.AddWithValue(":prmid_productor", NpgsqlTypes.NpgsqlDbType.Integer, prmid_productor);
                     command.Parameters.AddWithValue(":prmid_productor_juridico", NpgsqlTypes.NpgsqlDbType.Integer, prmid_productor_juridico);
                    command.Parameters.AddWithValue(":prmnro_placa", NpgsqlTypes.NpgsqlDbType.Varchar, prmnro_placa);
                    command.Parameters.AddWithValue(":prmid_silos", NpgsqlTypes.NpgsqlDbType.Integer, prmid_silos);
                    command.Parameters.AddWithValue(":prmfecha", NpgsqlTypes.NpgsqlDbType.Date, prmfecha);
                    command.Parameters.AddWithValue(":prmid_estatus", NpgsqlTypes.NpgsqlDbType.Integer, prmid_estatus);
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
        #endregion

        #region Insertar
        public DataTable insertar(string prmnro_placa,int prmid_conductor,DateTime prmfecha,string prmnro_guia,decimal prmpeso_guia,int prmid_producto,int prmid_productor,
           string prmobservaciones, int prmid_estatus, decimal prmpeso_bruto, decimal prmpeso_tara, int prmid_silos, DateTime prmhora_entrada, DateTime prmhora_salida, int prmid_productor_juridico)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"spinsertarrecepcion\"", conn);
                
                    command.Parameters.AddWithValue(":prmnro_placa", NpgsqlTypes.NpgsqlDbType.Varchar, prmnro_placa);
                    command.Parameters.AddWithValue(":prmid_conductor", NpgsqlTypes.NpgsqlDbType.Integer, prmid_conductor);
                    command.Parameters.AddWithValue(":prmfecha", NpgsqlTypes.NpgsqlDbType.Date, prmfecha);
                    command.Parameters.AddWithValue(":prmnro_guia",NpgsqlTypes.NpgsqlDbType.Varchar, prmnro_guia);
                   command.Parameters.AddWithValue(":prmpeso_guia",NpgsqlTypes.NpgsqlDbType.Numeric, prmpeso_guia);
                   command.Parameters.AddWithValue(":prmid_producto",NpgsqlTypes.NpgsqlDbType.Integer, prmid_producto);
                   command.Parameters.AddWithValue(":prmid_productor", NpgsqlTypes.NpgsqlDbType.Integer,prmid_productor);
                   command.Parameters.AddWithValue(":prmobservaciones",NpgsqlTypes.NpgsqlDbType.Varchar, prmobservaciones);
                   command.Parameters.AddWithValue(":prmid_estatus",NpgsqlTypes.NpgsqlDbType.Integer, prmid_estatus);
                   command.Parameters.AddWithValue(":prmpeso_bruto",NpgsqlTypes.NpgsqlDbType.Numeric, prmpeso_bruto);
                   command.Parameters.AddWithValue(":prmpeso_tara", NpgsqlTypes.NpgsqlDbType.Numeric, prmpeso_tara);
                   command.Parameters.AddWithValue(":prmid_silos", NpgsqlTypes.NpgsqlDbType.Integer, prmid_silos);
                    command.Parameters.AddWithValue(":prmhora_entrada", NpgsqlTypes.NpgsqlDbType.Timestamp, prmhora_entrada);
                    command.Parameters.AddWithValue(":prmhora_salida", NpgsqlTypes.NpgsqlDbType.Timestamp, prmhora_salida);
                    command.Parameters.AddWithValue(":prmid_productor_juridico", NpgsqlTypes.NpgsqlDbType.Integer, prmid_productor_juridico);
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

        #region Actualizar
        public DataTable actualizar(int prmid,string prmnro_placa, int prmid_conductor, DateTime prmfecha, string prmnro_guia, decimal prmpeso_guia, int prmid_producto, int prmid_productor,
           string prmobservaciones, int prmid_estatus, decimal prmpeso_bruto, decimal prmpeso_tara, int prmid_silos, bool prmresumen, DateTime prmhora_entrada, DateTime prmhora_salida, int prmid_productor_juridico)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"spactualizarrecepcion\"", conn);
                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);
                    command.Parameters.AddWithValue(":prmnro_placa", NpgsqlTypes.NpgsqlDbType.Varchar, prmnro_placa);
                    command.Parameters.AddWithValue(":prmid_conductor", NpgsqlTypes.NpgsqlDbType.Integer, prmid_conductor);
                    command.Parameters.AddWithValue(":prmfecha", NpgsqlTypes.NpgsqlDbType.Date, prmfecha);
                    command.Parameters.AddWithValue(":prmnro_guia", NpgsqlTypes.NpgsqlDbType.Varchar, prmnro_guia);
                    command.Parameters.AddWithValue(":prmpeso_guia", NpgsqlTypes.NpgsqlDbType.Numeric, prmpeso_guia);
                    command.Parameters.AddWithValue(":prmid_producto", NpgsqlTypes.NpgsqlDbType.Integer, prmid_producto);
                    command.Parameters.AddWithValue(":prmid_productor", NpgsqlTypes.NpgsqlDbType.Integer, prmid_productor);
                    command.Parameters.AddWithValue(":prmobservaciones", NpgsqlTypes.NpgsqlDbType.Varchar, prmobservaciones);
                    command.Parameters.AddWithValue(":prmid_estatus", NpgsqlTypes.NpgsqlDbType.Integer, prmid_estatus);
                    command.Parameters.AddWithValue(":prmpeso_bruto", NpgsqlTypes.NpgsqlDbType.Numeric, prmpeso_bruto);
                    command.Parameters.AddWithValue(":prmpeso_tara", NpgsqlTypes.NpgsqlDbType.Numeric, prmpeso_tara);
                    command.Parameters.AddWithValue(":prmid_silos", NpgsqlTypes.NpgsqlDbType.Integer, prmid_silos);
                    command.Parameters.AddWithValue(":prmresumen", NpgsqlTypes.NpgsqlDbType.Boolean, prmresumen);
                    command.Parameters.AddWithValue(":prmhora_entrada", NpgsqlTypes.NpgsqlDbType.Timestamp, prmhora_entrada);
                    command.Parameters.AddWithValue(":prmhora_salida", NpgsqlTypes.NpgsqlDbType.Timestamp, prmhora_salida);
                    command.Parameters.AddWithValue(":prmid_productor_juridico", NpgsqlTypes.NpgsqlDbType.Integer, prmid_productor_juridico);
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

        #region Analisis
        #region Buscar Item analisi x id producto
        public DataTable buscarItemanalisxodproducto(int prmid, int prmid_recepcion)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscaritemanalisisxproducto\"", conn);
                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);
                    command.Parameters.AddWithValue(":prmid_recepcion", NpgsqlTypes.NpgsqlDbType.Integer, prmid_recepcion);
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

        public DataTable buscaranalisis(int prmid_recepcion)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscaranalisisrecepcion\"", conn);
                   
                    command.Parameters.AddWithValue(":prmid_recepcion", NpgsqlTypes.NpgsqlDbType.Integer, prmid_recepcion);
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


        #region Insertar Analisis
        public DataTable insertarAnalisis(int prmid_producto, int prmid_recepcion, decimal prmvalor_item, int prmid_item)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"spinsertaranalis_detalle\"", conn);

                    command.Parameters.AddWithValue(":prmid_producto", NpgsqlTypes.NpgsqlDbType.Integer, prmid_producto);
                    command.Parameters.AddWithValue(":prmid_recepcion", NpgsqlTypes.NpgsqlDbType.Integer, prmid_recepcion);
                    command.Parameters.AddWithValue(":prmvalor_item", NpgsqlTypes.NpgsqlDbType.Numeric, prmvalor_item);
                     command.Parameters.AddWithValue(":prmid_item", NpgsqlTypes.NpgsqlDbType.Integer, prmid_item);
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

        #region Actualizar Analisis
        public DataTable actualizarAnalisis(int prmid,int prmid_recepcion, decimal prmvalor_item, int prmid_item,bool prmeliminado)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"spactualizaranalis_detalle\"", conn);
                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);
                    command.Parameters.AddWithValue(":prmid_recepcion", NpgsqlTypes.NpgsqlDbType.Integer, prmid_recepcion);
                    command.Parameters.AddWithValue(":prmvalor_item", NpgsqlTypes.NpgsqlDbType.Numeric, prmvalor_item);
                    command.Parameters.AddWithValue(":prmid_item", NpgsqlTypes.NpgsqlDbType.Integer, prmid_item);
                    command.Parameters.AddWithValue(":prmeliminado", NpgsqlTypes.NpgsqlDbType.Boolean, prmeliminado);
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

        #region PAso
        public DataTable cargarPasos()
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarestatusrecepcion\"", conn);
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

        #region Modalidad de pago / tasa especial
        private static readonly object EsquemaIdTasaLock = new object();
        private static bool _esquemaIdTasaOk;

        private static void AsegurarColumnaIdTasa()
        {
            if (_esquemaIdTasaOk) return;
            lock (EsquemaIdTasaLock)
            {
                if (_esquemaIdTasaOk) return;
                using (var c = new NpgsqlConnection(connstring))
                {
                    c.Open();
                    using (var cmd = c.CreateCommand())
                    {
                        cmd.CommandText = @"
ALTER TABLE public.recepciones
    ADD COLUMN IF NOT EXISTS id_tasa integer NULL;

COMMENT ON COLUMN public.recepciones.id_tasa IS
    'Tasa especial (tasa_diaria.id) asociada a esta recepción; NULL = tasa diaria general';
";
                        cmd.ExecuteNonQuery();
                        try
                        {
                            cmd.CommandText = @"
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'fk_recepciones_tasa'
    ) THEN
        ALTER TABLE public.recepciones
            ADD CONSTRAINT fk_recepciones_tasa
            FOREIGN KEY (id_tasa) REFERENCES public.tasa_diaria(id);
    END IF;
END $$;";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            // Sin FK no impide asociar la tasa.
                        }
                    }
                }
                _esquemaIdTasaOk = true;
            }
        }

        public void GuardarIdTasa(int idRecepcion, int idTasa)
        {
            if (idRecepcion <= 0 || idTasa <= 0) return;
            AsegurarColumnaIdTasa();
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "UPDATE recepciones SET id_tasa = @id_tasa WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id_tasa", idTasa);
                    cmd.Parameters.AddWithValue("@id", idRecepcion);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                throw new Exception(ex.Message);
            }
        }

        public int ObtenerIdTasa(int idRecepcion)
        {
            if (idRecepcion <= 0) return 0;
            try
            {
                AsegurarColumnaIdTasa();
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "SELECT COALESCE(id_tasa, 0) FROM recepciones WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", idRecepcion);
                    object val = cmd.ExecuteScalar();
                    conn.Close();
                    if (val == null || val == DBNull.Value)
                        return 0;
                    return Convert.ToInt32(val);
                }
            }
            catch
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return 0;
            }
        }

        public Dictionary<int, int> ObtenerIdsTasa(IEnumerable<int> ids)
        {
            var mapa = new Dictionary<int, int>();
            if (ids == null) return mapa;
            int[] arreglo = ids.Where(x => x > 0).Distinct().ToArray();
            if (arreglo.Length == 0) return mapa;

            try
            {
                AsegurarColumnaIdTasa();
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "SELECT id, COALESCE(id_tasa, 0) AS id_tasa FROM recepciones WHERE id = ANY(@ids)",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@ids", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer, arreglo);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            int idTasa = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                            mapa[id] = idTasa;
                        }
                    }
                }
                conn.Close();
            }
            catch
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return mapa;
        }

        public void GuardarModalidadPago(int idRecepcion, string modalidadPago)
        {
            if (idRecepcion <= 0) return;
            string modalidad = string.IsNullOrWhiteSpace(modalidadPago) ? "BS/BCV" : modalidadPago.Trim();
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "UPDATE recepciones SET modalidad_pago = @m WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@m", modalidad);
                    cmd.Parameters.AddWithValue("@id", idRecepcion);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                throw new Exception(ex.Message);
            }
        }

        public string ObtenerModalidadPago(int idRecepcion)
        {
            if (idRecepcion <= 0) return "BS/BCV";
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "SELECT COALESCE(modalidad_pago, 'BS/BCV') FROM recepciones WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", idRecepcion);
                    object val = cmd.ExecuteScalar();
                    conn.Close();
                    if (val == null || val == DBNull.Value)
                        return "BS/BCV";
                    return Convert.ToString(val);
                }
            }
            catch
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return "BS/BCV";
            }
        }

        public Dictionary<int, string> ObtenerModalidadesPago(IEnumerable<int> ids)
        {
            var mapa = new Dictionary<int, string>();
            if (ids == null) return mapa;
            int[] arreglo = ids.Where(x => x > 0).Distinct().ToArray();
            if (arreglo.Length == 0) return mapa;

            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "SELECT id, COALESCE(modalidad_pago, 'BS/BCV') AS modalidad_pago FROM recepciones WHERE id = ANY(@ids)",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@ids", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer, arreglo);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string modalidad = reader.IsDBNull(1) ? "BS/BCV" : reader.GetString(1);
                            mapa[id] = string.IsNullOrWhiteSpace(modalidad) ? "BS/BCV" : modalidad.Trim();
                        }
                    }
                }
                conn.Close();
            }
            catch
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return mapa;
        }
        #endregion
    }
}
