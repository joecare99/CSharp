using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptedSvgWpf.Dsl;
using ScriptedSvgWpf.Rendering;

namespace ScriptedSvgWpf.Tests;

[TestClass]
public sealed class SvgExporterTests
{
    [TestMethod]
    public void ExportsOptionalAttributesAndEscapesNullAndSpecialValues()
    {
        var document = new RenderDocument(12.5, 20, "<&\"");
        document.Commands.Add(new PolygonCommand(
            new[] { new ScriptedSvgWpf.Models.ScriptPoint(1, 2) },
            "<&\"",
            null));
        document.Commands.Add(new PathCommand("M 0,0", null, 2, null));

        var svg = new SvgExporter().Export(document);

        StringAssert.Contains(svg, "width=\"12.5\"");
        StringAssert.Contains(svg, "fill=\"&lt;&amp;&quot;\"");
        StringAssert.Contains(svg, "<polygon points=\"1,2\"");
        StringAssert.Contains(svg, "<path d=\"M 0,0\" fill=\"none\"");
        Assert.IsFalse(svg.Contains("stroke=", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ExportRejectsNullDocumentAndUnsupportedCommand()
    {
        var exporter = new SvgExporter();
        Assert.Throws<ArgumentNullException>(() => exporter.Export(null!));

        var document = new RenderDocument(10, 10, "white");
        document.Commands.Add(new UnknownRenderCommand());
        Assert.Throws<InvalidOperationException>(() => exporter.Export(document));
    }

    [TestMethod]
    public void RectangleCenterReflectsPositionAndSize()
    {
        var rectangle = new RectangleCommand(2, 4, 10, 6, "red");

        Assert.AreEqual(7, rectangle.Center.X);
        Assert.AreEqual(7, rectangle.Center.Y);
    }

    private sealed record UnknownRenderCommand : RenderCommand;

    [TestMethod]
    public void ExportsAllSupportedCommandsAsSvg()
    {
        const string source = """
canvas(200, 120, "white");
rect(10, 10, 20, 20, "black", 0.5, 12);
circle(70, 30, 10, "red");
line(0, 0, 100, 100, "blue", 2);
text(5, 110, "a < b", "black", 12);
polygon([Point(10, 60), Point(30, 60), Point(20, 80)], "green");
path("M 0,0 L 10,10", "purple", 1);
""";

        var document = new ScriptInterpreter().Execute(source);
        var svg = new SvgExporter().Export(document);

        StringAssert.Contains(svg, "<svg");
        StringAssert.Contains(svg, "<rect");
        StringAssert.Contains(svg, "rotate(-12)");
        StringAssert.Contains(svg, "<circle");
        StringAssert.Contains(svg, "<line");
        StringAssert.Contains(svg, "&lt;");
        StringAssert.Contains(svg, "<polygon");
        StringAssert.Contains(svg, "<path");
        Assert.AreEqual(6, document.Commands.Count);
    }
}
