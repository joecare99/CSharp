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
using CommonDialogs.Models;
using System.Windows.Forms;
using System.ComponentModel;
using DrawingFontStyle = System.Drawing.FontStyle;

namespace CommonDialogs;

/// <summary>
/// Class FontDialog.
/// Implements the <see cref="FontDialog" />
/// </summary>
/// <seealso cref="FontDialog" />
public class FontDialog : System.Windows.Forms.FontDialog, IFontDialog
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public new FontDialogSelection Font
    {
        get
        {
            var font = base.Font;
            return new FontDialogSelection
            {
                FamilyName = font?.FontFamily?.Name,
                Size = font?.Size ?? 0d,
                IsBold = font?.Bold ?? false,
                IsItalic = font?.Italic ?? false,
                IsUnderline = font?.Underline ?? false,
                IsStrikethrough = font?.Strikeout ?? false,
                Color = new((uint)base.Color.ToArgb(), base.Color.IsNamedColor ? base.Color.Name : null)
            };
        }
        set
        {
            var selection = value ?? new FontDialogSelection();
            var fontFamily = string.IsNullOrWhiteSpace(selection.FamilyName)
                ? base.Font?.FontFamily?.Name ?? System.Drawing.SystemFonts.DefaultFont.FontFamily.Name
                : selection.FamilyName;
            var fontSize = selection.Size > 0d ? (float)selection.Size : System.Drawing.SystemFonts.DefaultFont.Size;

            var style = DrawingFontStyle.Regular;
            if (selection.IsBold)
                style |= DrawingFontStyle.Bold;

            if (selection.IsItalic)
                style |= DrawingFontStyle.Italic;

            if (selection.IsUnderline)
                style |= DrawingFontStyle.Underline;

            if (selection.IsStrikethrough)
                style |= DrawingFontStyle.Strikeout;

            base.Font = new System.Drawing.Font(fontFamily, fontSize, style);
            base.Color = System.Drawing.Color.FromArgb(unchecked((int)selection.Color.Argb));
        }
    }

    /// <summary>
    /// Führt ein Standarddialogfeld mit einem Standardbesitzer aus.
    /// </summary>
    /// <returns><see cref="F:System.Windows.Forms.DialogResult.OK" />, wenn der Benutzer im Dialogfeld auf OK klickt, andernfalls <see cref="F:System.Windows.Forms.DialogResult.Cancel" />.</returns>
    public new bool? ShowDialog() => base.ShowDialog() == DialogResult.OK;

    /// <summary>
    /// Führt ein Standarddialogfeld mit dem angegebenen Besitzer aus.
    /// </summary>
    /// <param name="owner">Ein beliebiges Objekt, das <see cref="T:System.Windows.Forms.IWin32Window" /> implementiert, das das Fenster der obersten Ebene und damit den Besitzer des modalen Dialogfelds darstellt.</param>
    /// <returns><see cref="F:System.Windows.Forms.DialogResult.OK" />, wenn der Benutzer im Dialogfeld auf OK klickt, andernfalls <see cref="F:System.Windows.Forms.DialogResult.Cancel" />.</returns>
    public bool? ShowDialog(object owner) => base.ShowDialog((IWin32Window)owner) == DialogResult.OK;

}
