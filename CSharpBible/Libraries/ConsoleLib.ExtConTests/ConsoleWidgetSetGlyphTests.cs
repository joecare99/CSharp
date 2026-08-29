using ConsoleLib.Data;
using ConsoleLib.ExtCon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace ConsoleLibTests;

[TestClass]
public class ConsoleWidgetSetGlyphTests
{
    [TestMethod]
    [DataRow(GlyphStyle.PanelFill, '░')]
    [DataRow(GlyphStyle.ShadowFill, ' ')]
    [DataRow(GlyphStyle.Separator, '-')]
    [DataRow(GlyphStyle.ScrollBarVerticalDecrease, '\x18')]
    [DataRow(GlyphStyle.ScrollBarVerticalIncrease, '\x19')]
    [DataRow(GlyphStyle.ScrollBarHorizontalDecrease, '\x11')]
    [DataRow(GlyphStyle.ScrollBarHorizontalIncrease, '\x10')]
    [DataRow(GlyphStyle.ScrollBarThumb, '▓')]
    public void ResolveGlyph_Returns_Console_Compatible_Glyph(GlyphStyle glyphStyle, char expected)
    {
        MethodInfo? method = typeof(ConsoleWidgetSet).GetMethod("ResolveGlyph", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);

        char actual = (char)method.Invoke(null, new object[] { glyphStyle })!;

        Assert.AreEqual(expected, actual);
    }
}
