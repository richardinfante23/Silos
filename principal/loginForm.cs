using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace principal
{
    public partial class loginForm : Form
    {
        public bool Logeo = false;
        usuarioData ud = new usuarioData();
        public int IdUsuario;
        public string usuario=string.Empty;
        public int id_perfil;
        public loginForm()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private bool ConexionWs()
        {
                

                try
                {

                  
                    DataTable logeoDT = ud.logeo(txtLogin.Text,Seguridad.Encriptar(txtClave.Text));
                    if (logeoDT.Rows.Count > 0)
                    {
                        foreach (DataRow row in logeoDT.Rows)
                        {
                            IdUsuario=Convert.ToInt16( row["id"]);
                            Base.id_usuario = Convert.ToInt16(row["id"]);
                            Base.id_perfil= id_perfil = Convert.ToInt16(row["id_perfil"]);
                            Base.usuario= usuario = row["nombre"].ToString();

                        }
                       

                        Logeo = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Datos Incorrectos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                       
                    }

                  

                    // productoDT.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un buscando Conductor. Error Contacte al Administrador del Sistema"+ex,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
               
            
            return Logeo;
        }

        

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            ConexionWs();
            Cursor = Cursors.Default;
        }

        private void loginForm_Load(object sender, EventArgs e)
        {
            this.Text = UiTheme.AppName;
            this.BackColor = UiTheme.Primary;
            UiTheme.ApplyAppIcon(this);
            panelCard.BackColor = UiTheme.Card;
            picBrand.Image = UiTheme.CreateBrandMark(64);
            UiTheme.StylePrimaryButton(btnLogin);
            UiTheme.StyleSecondaryButton(btnCancelar);
            UiTheme.StyleTextBox(txtLogin);
            UiTheme.StyleTextBox(txtClave);
        }

       

    }
}
