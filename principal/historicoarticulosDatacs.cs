using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Configuration;
using System.Data;
using System.Windows.Forms;

namespace principal
{

  
    class historicoarticulosDatacs
    {

        static string connstring = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstring);

        public DataTable insertar(int prmid_tipo_mov,int prmid_cli_prov,string prmobservacion,int prmid_usuario,int prmid_conductor,DateTime prmfecha)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"spinsertarhistorico\"", conn);

                    command.Parameters.AddWithValue(":prmid_tipo_mov", NpgsqlTypes.NpgsqlDbType.Integer, prmid_tipo_mov);
                    command.Parameters.AddWithValue(":prmid_cli_prov", NpgsqlTypes.NpgsqlDbType.Integer, prmid_cli_prov);
                    command.Parameters.AddWithValue(":prmobservacion", NpgsqlTypes.NpgsqlDbType.Text, prmobservacion);
                    command.Parameters.AddWithValue(":prmid_usuario", NpgsqlTypes.NpgsqlDbType.Integer, prmid_usuario);
                    command.Parameters.AddWithValue(":prmid_conductor", NpgsqlTypes.NpgsqlDbType.Integer, prmid_conductor);
                    command.Parameters.AddWithValue(":prmfecha", NpgsqlTypes.NpgsqlDbType.Date, prmfecha);
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

        public DataTable actualizar(int prmid,int prmid_tipo_mov, int prmid_cli_prov, string prmobservacion, int prmid_usuario, int prmid_conductor, DateTime prmfecha,bool prmeliminado)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"spactualizarhistorico\"", conn);
                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);
                    command.Parameters.AddWithValue(":prmid_tipo_mov", NpgsqlTypes.NpgsqlDbType.Integer, prmid_tipo_mov);
                    command.Parameters.AddWithValue(":prmid_cli_prov", NpgsqlTypes.NpgsqlDbType.Integer, prmid_cli_prov);
                    command.Parameters.AddWithValue(":prmobservacion", NpgsqlTypes.NpgsqlDbType.Text, prmobservacion);
                    command.Parameters.AddWithValue(":prmid_usuario", NpgsqlTypes.NpgsqlDbType.Integer, prmid_usuario);
                    command.Parameters.AddWithValue(":prmid_conductor", NpgsqlTypes.NpgsqlDbType.Integer, prmid_conductor);
                    command.Parameters.AddWithValue(":prmeliminado", NpgsqlTypes.NpgsqlDbType.Boolean, prmeliminado);
                    command.Parameters.AddWithValue(":prmfecha", NpgsqlTypes.NpgsqlDbType.Date, prmfecha);

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



        public DataTable insertardetalle(DataGridView dgvDetalle,int id_historico,int prmidusuario)
        {
            DataTable redusltadoDT = new DataTable();
            try
            {


        
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter("fetch all in \"ref\"", conn);
                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {
                    int prmcantidad=0;
                    decimal prmiprecio = decimal.Zero ;
                    int pmrid_articulo=0;
                    int pmrid_seccion=0;
                    int prmid_historico_articulo= id_historico;
                    foreach(DataGridViewRow fila in dgvDetalle.Rows)
                    {


                        bool nuevo = true;
                        int id = 0;
                        bool eliminado = false;
                        prmcantidad = Convert.ToInt32(fila.Cells["txtCantidad"].Value);
                        pmrid_articulo = Convert.ToInt32(fila.Cells["cbArticulo"].Value);
                        prmiprecio = Convert.ToDecimal(fila.Cells["txtPRecio"].Value);
                        pmrid_seccion = Convert.ToInt32(fila.Cells["id_seccion"].Value);

                        NpgsqlCommand command = new NpgsqlCommand("\"spinsertarhistoricodetalle\"", conn);
                        if (Convert.ToBoolean( fila.Cells["filanueva"].Value))
                        {

                      


                        

                        }else
                        {
                            id = Convert.ToInt32(fila.Cells["id"].Value);
                             eliminado = Convert.ToBoolean(fila.Cells["eliminado"].Value);
                            nuevo = false;
                             command = new NpgsqlCommand("\"spactualizarhistoricodetalle\"", conn);
                            command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, id);

                        }
                        command.Parameters.AddWithValue(":prmcantidad", NpgsqlTypes.NpgsqlDbType.Integer, prmcantidad);
                        command.Parameters.AddWithValue(":prmiprecio", NpgsqlTypes.NpgsqlDbType.Numeric, prmiprecio);
                        command.Parameters.AddWithValue(":pmrid_articulo", NpgsqlTypes.NpgsqlDbType.Integer, pmrid_articulo);
                        command.Parameters.AddWithValue(":pmrid_seccion", NpgsqlTypes.NpgsqlDbType.Integer, pmrid_seccion);
                        command.Parameters.AddWithValue(":prmid_historico_articulo", NpgsqlTypes.NpgsqlDbType.Integer, prmid_historico_articulo);
                        if(!nuevo)
                            command.Parameters.AddWithValue(":prmeliminado", NpgsqlTypes.NpgsqlDbType.Boolean, eliminado);
                        command.Parameters.AddWithValue(":prmidusuario", NpgsqlTypes.NpgsqlDbType.Integer, prmidusuario);
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();
                    }

                 



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
            redusltadoDT = buscardetalle(id_historico);
            return redusltadoDT;
        }

        public DataTable actualzardetalle(int prmid,int prmcantidad, decimal prmiprecio, int pmrid_articulo, int pmrid_seccion, int prmid_historico_articulo,bool prmeliminado,int prmidusuario)
        {
            DataTable redusltadoDT = new DataTable();
            try
            {
               // NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter("fetch all in \"ref\"", conn);
                conn.Open();
                NpgsqlTransaction t = conn.BeginTransaction();
                try
                {


                    // Specify command StoredProcedure
                    NpgsqlCommand command = new NpgsqlCommand("\"spactualizarhistoricodetalle\"", conn);

                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);
                    command.Parameters.AddWithValue(":prmcantidad", NpgsqlTypes.NpgsqlDbType.Integer, prmcantidad);
                    command.Parameters.AddWithValue(":prmiprecio", NpgsqlTypes.NpgsqlDbType.Numeric, prmiprecio);
                    command.Parameters.AddWithValue(":pmrid_articulo", NpgsqlTypes.NpgsqlDbType.Integer, pmrid_articulo);
                    command.Parameters.AddWithValue(":pmrid_seccion", NpgsqlTypes.NpgsqlDbType.Integer, pmrid_seccion);
                    command.Parameters.AddWithValue(":prmid_historico_articulo", NpgsqlTypes.NpgsqlDbType.Integer, prmid_historico_articulo);
                    command.Parameters.AddWithValue(":prmeliminado", NpgsqlTypes.NpgsqlDbType.Boolean, prmeliminado);
                    command.Parameters.AddWithValue(":prmidusuario", NpgsqlTypes.NpgsqlDbType.Integer, prmidusuario);

                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();







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

        #region Buscar Padre
        public DataTable buscar(int prmid,int prmid_conductor, int prmid_tipo_mov,int prmid_cli_prov,DateTime prmfecha,DateTime prmfecha_hasta)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarmovimientos\"", conn);
                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid);
                    command.Parameters.AddWithValue(":prmid_conductor", NpgsqlTypes.NpgsqlDbType.Integer, prmid_conductor);
                    command.Parameters.AddWithValue(":prmid_tipo_mov", NpgsqlTypes.NpgsqlDbType.Integer, prmid_tipo_mov);
                    command.Parameters.AddWithValue(":prmid_cli_prov", NpgsqlTypes.NpgsqlDbType.Integer, prmid_cli_prov);
                    command.Parameters.AddWithValue(":prmfecha", NpgsqlTypes.NpgsqlDbType.Date, prmfecha);
                    command.Parameters.AddWithValue(":prmfecha_hasta", NpgsqlTypes.NpgsqlDbType.Date, prmfecha_hasta);

                  
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

        #region Buscar detalle
        public DataTable buscardetalle(int prmid_historico)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscardetallehistorico\"", conn);
                    command.Parameters.AddWithValue(":prmid", NpgsqlTypes.NpgsqlDbType.Integer, prmid_historico);
               

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

        public DataTable buscarxarticulo(int prmidarticulo,int prmid_tipo_mov)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarmovimientosxarticulo\"", conn);
                    command.Parameters.AddWithValue(":prmidarticulo", NpgsqlTypes.NpgsqlDbType.Integer, prmidarticulo);
                    command.Parameters.AddWithValue(":prmid_tipo_mov", NpgsqlTypes.NpgsqlDbType.Integer, prmid_tipo_mov);


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

        public DataTable buscarpreciosxarticulos(int prmid_articulos)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarpreciosxarticulos\"", conn);
                    command.Parameters.AddWithValue(":prmid_articulos", NpgsqlTypes.NpgsqlDbType.Integer, prmid_articulos);
                   


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

        public DataTable existenciaarticulo(int prmid_articulo, int prmid_seccion)
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
                    NpgsqlCommand command = new NpgsqlCommand("\"buscarexistencia\"", conn);
                    command.Parameters.AddWithValue(":prmid_articulo", NpgsqlTypes.NpgsqlDbType.Integer, prmid_articulo);
                    command.Parameters.AddWithValue(":prmid_seccion", NpgsqlTypes.NpgsqlDbType.Integer, prmid_seccion);


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
