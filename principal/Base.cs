using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using System.Windows.Forms;
namespace principal
{
   public static class Base
    {
       public static  string usuario
        {
            get;
            set;
        }
        public static int id_perfil
        {
            get;
            set;
        }
        public static int id_usuario
        {
            get;
            set;
        }
        public static bool administrador()
        {
            if (Base.id_perfil == 1)
                return true;
            else
                return false;


        }
        public static void  msjError(string msj)
        {
            MessageBox.Show(msj, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        public static void msjExito(string msj)
        {
            MessageBox.Show(msj, "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        public static void msjAdvertencia(string msj)
        {
            MessageBox.Show(msj, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }
        public static DialogResult msjPregunta(string msj)
        {
            DialogResult result = MessageBox.Show(msj, "Interrogante", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            return result;
          

        }
  
    }
}
