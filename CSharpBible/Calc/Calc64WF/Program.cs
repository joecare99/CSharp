// ***********************************************************************
// Assembly         : Calc64WF
// Author           : Mir
// Created          : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-28-2022
// ***********************************************************************
// <copyright file="Program.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Forms;
using Calc64WF.Visual;

namespace Calc64WF
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
            Application.Run(new FrmCalc64Main());
        }
    }
}
