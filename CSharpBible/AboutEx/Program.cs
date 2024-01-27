using System;
using System.Windows.Forms;
using CSharpBible.AboutEx.Visual;

namespace CSharpBible.AboutEx
{
    /// <summary>
    /// Class Program.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmAboutExMain());
        }
    }
}
