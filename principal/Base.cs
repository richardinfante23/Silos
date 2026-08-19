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
            UiDialog.Show(Form.ActiveForm, msj, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void msjExito(string msj)
        {
            UiDialog.Show(Form.ActiveForm, msj, "Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void msjAdvertencia(string msj)
        {
            UiDialog.Show(Form.ActiveForm, msj, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        public static DialogResult msjPregunta(string msj)
        {
            return UiDialog.Show(Form.ActiveForm, msj, "Interrogante", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public static DialogResult msjPreguntaCancel(string msj)
        {
            return UiDialog.Show(Form.ActiveForm, msj, "Advertencia", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        public static void msjInformacion(string msj)
        {
            UiDialog.Show(Form.ActiveForm, msj, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>Muestra un error de operación sin exponer detalles técnicos al usuario.</summary>
        public static void msjErrorOperacion(Exception ex, string operacion)
        {
            msjError(MensajeErrorOperacion(ex, operacion));
        }

        public static string MensajeErrorOperacion(Exception ex, string operacion)
        {
            string detalle = ObtenerMensajeRaiz(ex);
            if (string.IsNullOrWhiteSpace(detalle))
                return $"No se pudo {operacion}. Intente nuevamente o contacte al administrador del sistema.";

            string lower = detalle.ToLowerInvariant();

            if (lower.Contains("42883") || lower.Contains("no existe la función") || lower.Contains("no existe la funcion"))
                return $"No se pudo {operacion}. Existe un problema de configuración en la base de datos. Contacte al administrador del sistema.";

            if (lower.Contains("23502") || lower.Contains("not-null") || lower.Contains("not null"))
                return $"No se pudo {operacion}. Faltan datos obligatorios o el registro no se generó correctamente.";

            if (lower.Contains("23503") || lower.Contains("foreign key") || lower.Contains("llave foránea") || lower.Contains("llave foranea"))
                return $"No se pudo {operacion}. Alguno de los datos seleccionados no es válido o ya no existe.";

            if (lower.Contains("23505") || lower.Contains("duplicate key") || lower.Contains("llave duplicada"))
                return $"No se pudo {operacion}. Ya existe un registro con esos datos.";

            if (EsMensajeTecnico(detalle))
                return $"No se pudo {operacion}. Verifique los datos e intente nuevamente. Si el problema continúa, contacte al administrador del sistema.";

            return detalle;
        }

        private static string ObtenerMensajeRaiz(Exception ex)
        {
            if (ex == null) return string.Empty;
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex.Message ?? string.Empty;
        }

        private static bool EsMensajeTecnico(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje)) return true;

            string lower = mensaje.ToLowerInvariant();
            return lower.Contains("exception")
                || lower.Contains("npgsql")
                || lower.Contains("syntax error")
                || lower.Contains("error de sintaxis")
                || lower.Contains("stack")
                || lower.Contains("ha ocurrido un ")
                || lower.Contains("contacte al administrador del sistema")
                || lower.Contains("contacte al dersarrollador")
                || System.Text.RegularExpressions.Regex.IsMatch(mensaje, @"\b\d{5}\b:");
        }
  
    }
}
