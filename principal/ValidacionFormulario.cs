
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace principal
{
    internal class ValidacionFormulario
    {
        public static bool is_valid = true;

        public static bool validar(Control.ControlCollection Controles,string etiqueta)
        {
            ValidacionFormulario.is_valid = true;
            ValidacionFormulario.validarControles(Controles,etiqueta);
            return ValidacionFormulario.is_valid;
        }

        private static void validarControles(Control.ControlCollection Controles,string etiqueta)
        {
            if (Controles == null)
                return;
            foreach (Control controle in (ArrangedElementCollection)Controles)
            {
                if (controle.AccessibleDescription == etiqueta && controle.Enabled)
                {
                    if (string.IsNullOrWhiteSpace(controle.Text) )
                    {
                        ValidacionFormulario.is_valid = false;
                        controle.BackColor = Color.FromArgb((int)byte.MaxValue, 192, 192);
                    }
                    else
                        controle.BackColor = Color.White;
                }
                if (controle.Enabled)
                    ValidacionFormulario.validarControles(controle.Controls,etiqueta);
            }
        }
    }
}

