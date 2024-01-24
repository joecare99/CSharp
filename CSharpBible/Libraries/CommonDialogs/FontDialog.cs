// ***********************************************************************
// Assembly         : CommonDialogs
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-11-2022
// ***********************************************************************
// <copyright file="FontDialog.cs" company="CommonDialogs">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommonDialogs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonDialogs
{
    /// <summary>
    /// Class FontDialog.
    /// Implements the <see cref="FontDialog" />
    /// </summary>
    /// <seealso cref="FontDialog" />
    public class FontDialog : System.Windows.Forms.FontDialog, IFontDialog
    {
        public System.Drawing.Font Font { get => base.Font;set => base.Font = value; }

        /// <summary>
        /// Führt ein Standarddialogfeld mit einem Standardbesitzer aus.
        /// </summary>
        /// <returns><see cref="F:System.Windows.Forms.DialogResult.OK" />, wenn der Benutzer im Dialogfeld auf OK klickt, andernfalls <see cref="F:System.Windows.Forms.DialogResult.Cancel" />.</returns>
        public new bool? ShowDialog()
        {
            return base.ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// Führt ein Standarddialogfeld mit dem angegebenen Besitzer aus.
        /// </summary>
        /// <param name="owner">Ein beliebiges Objekt, das <see cref="T:System.Windows.Forms.IWin32Window" /> implementiert, das das Fenster der obersten Ebene und damit den Besitzer des modalen Dialogfelds darstellt.</param>
        /// <returns><see cref="F:System.Windows.Forms.DialogResult.OK" />, wenn der Benutzer im Dialogfeld auf OK klickt, andernfalls <see cref="F:System.Windows.Forms.DialogResult.Cancel" />.</returns>
        public bool? ShowDialog(object owner)
        {
            return base.ShowDialog((IWin32Window)owner) == DialogResult.OK;
        }

    }
}
