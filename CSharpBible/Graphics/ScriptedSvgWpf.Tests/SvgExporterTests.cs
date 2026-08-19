using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptedSvgWpf.Dsl;
using ScriptedSvgWpf.Rendering;

namespace ScriptedSvgWpf.Tests;

[TestClass]
public sealed class SvgExporterTests
{
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
