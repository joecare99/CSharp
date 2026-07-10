using CommonDialogs.Models;
using System;
using System.Drawing;

namespace CommonDialogs.Helper;

/// <summary>
/// Provides conversion helpers between dialog abstraction models and <see cref="System.Drawing" /> types.
/// </summary>
public static class DialogModelConversionExtensions
{
    /// <summary>
    /// Converts a <see cref="Color" /> instance to a <see cref="DialogColor" /> abstraction.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The converted <see cref="DialogColor" /> value.</returns>
    public static DialogColor ToDialogColor(this Color color)
        => new((uint)color.ToArgb(), color.IsNamedColor ? color.Name : null);

    /// <summary>
    /// Converts a <see cref="DialogColor" /> abstraction to a <see cref="Color" /> instance.
    /// </summary>
    /// <param name="color">The source dialog color.</param>
    /// <returns>The converted <see cref="Color" /> value.</returns>
    public static Color ToDrawingColor(this DialogColor color)
    {
        if (!string.IsNullOrWhiteSpace(color.Name))
        {
            var namedColor = Color.FromName(color.Name);
            if (namedColor.ToArgb() == unchecked((int)color.Argb))
                return namedColor;
        }

        return Color.FromArgb(unchecked((int)color.Argb));
    }

    /// <summary>
    /// Converts a <see cref="Font" /> and optional text color to a <see cref="FontDialogSelection" /> abstraction.
    /// </summary>
    /// <param name="font">The source font. If <see langword="null" />, system defaults are used.</param>
    /// <param name="color">Optional source color for the selection. If omitted, black is used.</param>
    /// <returns>The converted <see cref="FontDialogSelection" /> value.</returns>
    public static FontDialogSelection ToDialogSelection(this Font? font, Color? color = null)
    {
        var sourceFont = font ?? SystemFonts.DefaultFont;
        var sourceColor = color ?? Color.Black;

        return new FontDialogSelection
        {
            FamilyName = sourceFont.FontFamily.Name,
            Size = sourceFont.Size,
            IsBold = sourceFont.Bold,
            IsItalic = sourceFont.Italic,
            IsUnderline = sourceFont.Underline,
            IsStrikethrough = sourceFont.Strikeout,
            Color = sourceColor.ToDialogColor()
        };
    }

    /// <summary>
    /// Converts a <see cref="FontDialogSelection" /> abstraction to a <see cref="Font" /> instance.
    /// </summary>
    /// <param name="selection">The source font dialog selection.</param>
    /// <param name="fallbackFont">Optional fallback font used for missing or invalid values.</param>
    /// <returns>The converted <see cref="Font" /> value.</returns>
    public static Font ToDrawingFont(this FontDialogSelection? selection, Font? fallbackFont = null)
    {
        var fallback = fallbackFont ?? SystemFonts.DefaultFont;
        if (selection is null)
            return (Font)fallback.Clone();

        var fontFamily = string.IsNullOrWhiteSpace(selection.FamilyName)
            ? fallback.FontFamily.Name
            : selection.FamilyName;
        var fontSize = selection.Size > 0d ? (float)selection.Size : fallback.Size;

        var style = FontStyle.Regular;
        if (selection.IsBold)
            style |= FontStyle.Bold;

        if (selection.IsItalic)
            style |= FontStyle.Italic;

        if (selection.IsUnderline)
            style |= FontStyle.Underline;

        if (selection.IsStrikethrough)
            style |= FontStyle.Strikeout;

        if (string.Equals(fontFamily, fallback.FontFamily.Name, StringComparison.OrdinalIgnoreCase)
            && Math.Abs(fontSize - fallback.Size) < 0.001f
            && style == fallback.Style)
        {
            return (Font)fallback.Clone();
        }

        try
        {
            return new Font(fontFamily, fontSize, style, fallback.Unit, fallback.GdiCharSet);
        }
        catch (ArgumentException)
        {
            return new Font(fallback.FontFamily, fontSize, style, fallback.Unit, fallback.GdiCharSet);
        }
    }
}
