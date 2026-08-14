using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;


namespace principal
{
    public partial class principalFom : Form
    {
        static string connstring = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        static string rutaPostgres = ConfigurationManager.AppSettings["rutaPostgres"];
        string _usuario;
        int _idusuario = 0;
        int _idPerfil = 0;
        bool supevisores = false;
        string strPG_dumpPath = string.Empty;
        string strInstallLocation = string.Empty;
        public principalFom()
        {
            InitializeComponent();
        }



        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void controlPrevioDespachoToolStripMenuItem_Click(object sender, EventArgs e)
        {



        }

        private void nuevoToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            DespachoForm pf = new DespachoForm();
            pf.nuevo = true;
            pf.id_perfil = _idPerfil;
            pf.id_usuario = _idusuario;
            pf.supervisor = supevisores;

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;
        }

        private void buscarToolStripMenuItem2_Click(object sender, EventArgs e)
        {


        }

        private void inventarioTotalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                Cursor = Cursors.WaitCursor;
                inventarioForm pf = new inventarioForm();

                pf.MdiParent = this;
                pf.Activate();
                //this.splitContainer1.SendToBack();
                pf.Show();
                Cursor = Cursors.Default;

            }
            catch (Exception ex)
            {

                throw new Exception("Ha ocurrido abriendo inventario. Error Contacte al Administrador del Sistema", ex);
            }

        }

        private void principalFom_Load(object sender, EventArgs e)
        {
            ApplyMainShellTheme();

            var lg = new loginForm();
            lg.ShowDialog();
            if (lg.Logeo)
            {
                _usuario = lg.usuario;
                _idusuario = lg.IdUsuario;
                _idPerfil = lg.id_perfil;
                lbUsuario.Text = "Usuario: " + _usuario;
                this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
            else
            {
                Application.Exit();
                return;
            }
            supevisores = (_idPerfil == 1 || _idPerfil == 5 || _idPerfil == 6);
            if (supevisores)
            {

                verToolStripMenuItem.Visible = true;
                btnInventario.Enabled = true;
                this.usuariosToolStripMenuItem1.Visible = true;
            }
            if (supevisores || _idPerfil == 4)
            {
                verToolStripMenuItem.Visible = true;
                nuevoToolStripMenuItem1.Visible = true;
                nuevoToolStripMenuItem2.Visible = true;
            }
            if (_idPerfil == 7)
            {
                verToolStripMenuItem.Visible = false;
                nuevoToolStripMenuItem1.Visible = false;
                nuevoToolStripMenuItem2.Visible = false;
                mantenimientoToolStripMenuItem.Visible = false;
                tsPrincipal.Visible = false;
                btnRecepcion.Enabled = false;
                btnDespacho.Enabled = false;
            }
        }

        private void ApplyMainShellTheme()
        {
            this.Text = UiTheme.AppName;
            UiTheme.ApplyForm(this);
            UiTheme.ApplyAppIcon(this);
            pizquierda.BackColor = UiTheme.Sidebar;
            plogo.BackColor = UiTheme.Primary;
            plogo.BackgroundImage = null;
            plogo.Controls.Clear();

            var brand = new PictureBox
            {
                Image = UiTheme.CreateBrandMark(56),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(56, 56),
                Location = new Point(52, 18),
                BackColor = Color.Transparent
            };
            var caption = new Label
            {
                Text = "Gestión de\nPesaje",
                Font = UiTheme.FontBrand,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Size = new Size(140, 40),
                Location = new Point(10, 78),
                BackColor = Color.Transparent
            };
            plogo.Controls.Add(brand);
            plogo.Controls.Add(caption);

            UiTheme.StyleNavButton(btnRecepcion);
            UiTheme.StyleNavButton(btnDespacho);
            UiTheme.StyleNavButton(btnInventario);
            UiTheme.StyleNavButton(btnsalir);
            btnsalir.ForeColor = Color.FromArgb(255, 210, 210);

            menuStrip1.BackColor = UiTheme.Primary;
            menuStrip1.ForeColor = Color.White;
            foreach (ToolStripItem item in menuStrip1.Items)
                item.ForeColor = Color.White;

            toolStrip1.BackColor = Color.FromArgb(232, 237, 242);
            lbUsuario.ForeColor = UiTheme.TextPrimary;

            this.MdiChildActivate -= principalFom_MdiChildActivate;
            this.MdiChildActivate += principalFom_MdiChildActivate;
        }

        private void principalFom_MdiChildActivate(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
                UiTheme.ApplyForm(this.ActiveMdiChild);
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                Cursor = Cursors.WaitCursor;
                usuariosForm pf = new usuariosForm();

                pf.MdiParent = this;
                pf.Activate();
                //this.splitContainer1.SendToBack();
                pf.Show();
                Cursor = Cursors.Default;

            }
            catch (Exception ex)
            {

                throw new Exception("Ha ocurrido abriendo inventario. Error Contacte al Administrador del Sistema", ex);
            }


        }

        private void btnRecepcion_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            recepcionForm pf = new recepcionForm();
            pf.nombre_Usuario = _usuario;
            pf.id_perfil = _idPerfil;
            pf.nuevo = false;
            pf.id_usuario = _idusuario;
            pf.supervisor = supevisores;
            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;

        }

        private void btnDespacho_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            DespachoForm pf = new DespachoForm();
            pf.nuevo = false;
            pf.id_perfil = _idPerfil;
            pf.id_usuario = _idusuario;
            pf.nombre_Usuario = _usuario;
            pf.supervisor = supevisores;

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;

        }

        private void btnInventario_Click(object sender, EventArgs e)
        {

            try
            {

                Cursor = Cursors.WaitCursor;
                inventarioForm pf = new inventarioForm();

                pf.MdiParent = this;
                pf.Activate();
                //this.splitContainer1.SendToBack();
                pf.Show();
                Cursor = Cursors.Default;

            }
            catch (Exception ex)
            {

                throw new Exception("Ha ocurrido abriendo inventario. Error Contacte al Administrador del Sistema", ex);
            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void nuevoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            recepcionForm pf = new recepcionForm();

            pf.id_perfil = _idPerfil;
            pf.id_usuario = _idusuario;
            pf.nombre_Usuario = _usuario;
            pf.supervisor = supevisores;
            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;

        }

        private void buscarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            recepcionForm pf = new recepcionForm();
            pf.nuevo = false;
            pf.id_perfil = _idPerfil;
            pf.nombre_Usuario = _usuario;
            pf.id_usuario = _idusuario;
            pf.supervisor = supevisores;
            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;
        }

        private void nuevoToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            DespachoForm pf = new DespachoForm();
            pf.nuevo = true;
            pf.id_perfil = _idPerfil;
            pf.id_usuario = _idusuario;

            pf.nombre_Usuario = _usuario;
            pf.supervisor = supevisores;

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;

        }

        private void buscarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            DespachoForm pf = new DespachoForm();
            pf.nuevo = false;
            pf.id_perfil = _idPerfil;
            pf.id_usuario = _idusuario;
            pf.supervisor = supevisores;
            pf.nombre_Usuario = _usuario;

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;
        }

        private void inventarioToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Cursor = Cursors.WaitCursor;
            inventarioForm pf = new inventarioForm();

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;
        }

        private void usuariosToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {

                Cursor = Cursors.WaitCursor;
                usuariosForm pf = new usuariosForm();

                pf.MdiParent = this;
                pf.Activate();
                //this.splitContainer1.SendToBack();
                pf.Show();
                Cursor = Cursors.Default;

            }
            catch (Exception ex)
            {

                throw new Exception("Ha ocurrido abriendo inventario. Error Contacte al Administrador del Sistema", ex);
            }
        }

        private void transporteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            transporteGridForm pf = new transporteGridForm();

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;
        }

        private void clientesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pruductorToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Cursor = Cursors.WaitCursor;
            productoresNaturalesForm pf = new productoresNaturalesForm();

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;


        }

        private void productosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            productosFrom pf = new productosFrom();

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;
        }

        private void juridicoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            productoresJuridicos pf = new productoresJuridicos();

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;
        }

        private void asociacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            asociacionGridForm pf = new asociacionGridForm();

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void asociacionesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            asociacionGridForm pf = new asociacionGridForm();

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnsalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void conductoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            conductoresGridForm pf = new conductoresGridForm(false);

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;
        }

        private void silosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            silosGridForm pf = new silosGridForm();

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;
        }

        private void clienteJuridicoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            productoresNaturalesForm pf = new productoresNaturalesForm();
            pf.productor = false;

            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;
        }

        private void clienteJuridicoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            productoresJuridicos pf = new productoresJuridicos();
            pf.productor = false;
            pf.MdiParent = this;
            pf.Activate();
            //this.splitContainer1.SendToBack();
            pf.Show();
            Cursor = Cursors.Default;
        }

        private void respaldoBaseDeDatosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            PostgreSqlDump(@"C:\Program Files\PostgreSQL\9.3\bin pg_dump.exe", @"C:\Users\Usuario\Desktop\Siges" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + ".backup",
                connstring.Split(';')[0].Split('=')[1]
                , "5432", connstring.Split(';')[1].Split('=')[1], connstring.Split(';')[2].Split('=')[1], connstring.Split(';')[3].Split('=')[1]);
            Cursor = Cursors.Default;
        }


        public void PostgreSqlDump(
        string pgDumpPath,
        string outFile,
        string host,
        string port,
        string database,
        string user,
        string password)
        {
            String dumpCommand = "\"" + pgDumpPath + "\"" + " -Fc" + " -h " + host + " -p " + port + " -d " + database + " -U " + user + " --format custom --blobs --verbose --file ";
            String passFileContent = "" + host + ":" + port + ":" + database + ":" + user + ":" + password + "";

            String batFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "Respaldos" + Guid.NewGuid().ToString() + ".bat");
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Respaldos"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\Respaldos");
            }

            // Try to create the directory.


            string nombreRespaldo = string.Empty;


            try
            {
                String batchContent = "";
                batchContent += "@echo off" + "\n";
                batchContent += Environment.NewLine;
                batchContent += @"for /f ""tokens=1-3 delims=/ "" %%i in (""%date%"") do (";
                batchContent += Environment.NewLine;
                batchContent += @"set day=%%i" + Environment.NewLine + " set month=%%j" + Environment.NewLine + "set year=%%k" + Environment.NewLine + " )";
                batchContent += Environment.NewLine;
                batchContent += @"set datestr=%day%_%month%_%year%";
                batchContent += Environment.NewLine;
                batchContent += @"set BACKUP_FILE=" + AppDomain.CurrentDomain.BaseDirectory + "Respaldos\\Sisges_%datestr%" + DateTime.Now.Hour + DateTime.Now.Minute + ".backup";
                batchContent += Environment.NewLine;
                batchContent += @"SET PGPASSWORD=" + password;
                batchContent += Environment.NewLine;
                batchContent += @"echo on";
                batchContent += Environment.NewLine;
                batchContent += @"cd " + "\"" + rutaPostgres + "\"";
                batchContent += Environment.NewLine;
                batchContent += @"pg_dump.exe --host localhost --port 5432 --username ""postgres"" --no-password  --format custom --blobs --verbose --file  %BACKUP_FILE% ""produccion_cliente""";
                //batchContent += Environment.NewLine;
                //batchContent += "pause";
                //batchContent += Environment.NewLine;
                //batchContent += "exit";
                nombreRespaldo = AppDomain.CurrentDomain.BaseDirectory + "Respaldos";
                File.WriteAllText(
                    batFilePath,
                    batchContent,
                    Encoding.ASCII);



                //if (File.Exists(outFile))
                //    File.Delete(outFile);

                ProcessStartInfo oInfo = new ProcessStartInfo(batFilePath);
                oInfo.UseShellExecute = false;
                oInfo.CreateNoWindow = true;

                using (Process proc = System.Diagnostics.Process.Start(oInfo))
                {
                    proc.WaitForExit();
                    proc.Close();
                }
            }
            catch (Exception e)
            {

                MessageBox.Show("Error " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                if (File.Exists(batFilePath))
                    File.Delete(batFilePath);
            }
            MessageBox.Show("Ha culimnado ejecución de Respaldo de Seguridad. Puede ser ubicado en la siguiente dirección" + "\n " + nombreRespaldo, "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void artiículosToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        private void nuevo_Click(object sender, EventArgs e)
        {
            try
            {
                abrirARticulos(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                abrirARticulos(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        void abrirARticulos(bool nuevo)
        {

            try
            {
                Cursor = Cursors.WaitCursor;
                ArticulosForm pf = new ArticulosForm(nuevo, _idusuario);

                pf.MdiParent = this;
                pf.Activate();
                //this.splitContainer1.SendToBack();
                pf.Show();
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void entradaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                abrirMovimientos(true, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }
        void abrirMovimientos(bool entrada, bool nuevo)
        {

            try
            {
                Cursor = Cursors.WaitCursor;
                MovimientoArticulosForm pf = new MovimientoArticulosForm(entrada, nuevo, _idusuario);

                pf.MdiParent = this;
                pf.Activate();
                //this.splitContainer1.SendToBack();
                pf.Show();
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void salidaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                abrirMovimientos(true, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void nuevoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                abrirMovimientos(false, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void buscarToolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            try
            {
                abrirMovimientos(false, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void reporteGeneralToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {

                Cursor = Cursors.WaitCursor;
                InventarioArticulos pf = new InventarioArticulos();

                pf.MdiParent = this;
                pf.Activate();
                //this.splitContainer1.SendToBack();
                pf.Show();
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void nuevoToolStripMenuItem3_Click_1(object sender, EventArgs e)
        {

            try
            {


                abrirEstantes(true);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        void abrirEstantes(bool nuevo)
        {

            try
            {
                Cursor = Cursors.WaitCursor;
                EstantesForm pf = new EstantesForm(nuevo, _idusuario);


                pf.MdiParent = this;
                pf.Activate();
                //this.splitContainer1.SendToBack();
                pf.Show();
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

    

        private void nuevoToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            try
            {


                abrirEstantes(true);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void buscarToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {


                abrirEstantes(false);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void reporteGeneralToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {

                Cursor = Cursors.WaitCursor;
                InventarioArticulos pf = new InventarioArticulos();

                pf.MdiParent = this;
                pf.Activate();
                //this.splitContainer1.SendToBack();
                pf.Show();
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
       
        }

        private void auditoriaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                Cursor = Cursors.WaitCursor;
                AuditoriaForm pf = new AuditoriaForm();

                pf.MdiParent = this;
                pf.Activate();
                //this.splitContainer1.SendToBack();
                pf.Show();
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Base.msjError(ex.Message);

            }
        }
    }
}
