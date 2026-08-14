using CommonDialogs.Helper;
using CommonDialogs.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace MVVM_20_Sysdialogs.Helper.Tests;

[TestClass]
public class DialogModelConversionExtensionsTests
{
    [TestMethod]
    public void ToDialogColor_MapsArgbAndName()
    {
        var result = Color.Red.ToDialogColor();

        Assert.AreEqual((uint)Color.Red.ToArgb(), result.Argb);
        Assert.AreEqual(Color.Red.Name, result.Name);
    }

    [TestMethod]
    public void ToDrawingColor_MapsArgb()
    {
        DialogColor value = new(0xFF102030, "Custom");

        var result = value.ToDrawingColor();

        Assert.AreEqual(0xFF, result.A);
        Assert.AreEqual(0x10, result.R);
        Assert.AreEqual(0x20, result.G);
        Assert.AreEqual(0x30, result.B);
    }

    [TestMethod]
    public void ToDialogSelection_MapsFontAndColor()
    {
        using var font = new Font(FontFamily.GenericSansSerif, 11f, FontStyle.Bold | FontStyle.Italic);

        var result = font.ToDialogSelection(Color.Lime);

        Assert.AreEqual(font.Name, result.FamilyName);
        Assert.AreEqual(font.Size, result.Size, 0.001d);
        Assert.IsTrue(result.IsBold);
        Assert.IsTrue(result.IsItalic);
        Assert.AreEqual((uint)Color.Lime.ToArgb(), result.Color.Argb);
    }

    [TestMethod]
    public void ToDrawingFont_MapsSelectionAndUsesValues()
    {
        var selection = new FontDialogSelection
        {
            FamilyName = FontFamily.GenericSansSerif.Name,
            Size = 13,
            IsBold = true,
            IsItalic = true,
            IsUnderline = true,
            IsStrikethrough = true
        };

        using var result = selection.ToDrawingFont();

        Assert.AreEqual(selection.FamilyName, result.FontFamily.Name);
        Assert.AreEqual(13f, result.Size, 0.001f);
        Assert.IsTrue(result.Bold);
        Assert.IsTrue(result.Italic);
        Assert.IsTrue(result.Underline);
        Assert.IsTrue(result.Strikeout);
    }
}
