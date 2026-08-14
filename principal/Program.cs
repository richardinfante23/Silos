using System;
using System.Windows.Forms;

namespace principal
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            UiTheme.InstallGlobalFormIcon();
            Application.Run(new principalFom());
        }
    }
}
