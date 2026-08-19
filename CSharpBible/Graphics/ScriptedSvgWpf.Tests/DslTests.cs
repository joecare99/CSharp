using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptedSvgWpf.Dsl;
using ScriptedSvgWpf.Rendering;

namespace ScriptedSvgWpf.Tests;

[TestClass]
public sealed class DslTests
{
    [TestMethod]
    public void ParsesAndExecutesTypedExpressionsAndMath()
    {
        const string source = """
canvas(100, 80, "white");
int baseValue = 3;
float height = sqrt(16) + baseValue;
bool isLarge = height > 5 && true;
string label = "value";
if (isLarge) {
    text(5, height, label + "!", "black", 12);
}
""";

        var document = new ScriptInterpreter().Execute(source);

        Assert.AreEqual(100, document.Width);
        Assert.AreEqual(80, document.Height);
        var text = document.Commands.OfType<TextCommand>().Single();
        Assert.AreEqual("value!", text.Text);
        Assert.AreEqual(7, text.Y, 0.0001);
    }

    [TestMethod]
    public void SupportsPointAndRectValues()
    {
        const string source = """
canvas(100, 100, "white");
Point origin = Point(10, 20);
Rect bounds = Rect(origin.X, origin.Y, 30, 40);
rect(bounds.X, bounds.Y, bounds.Width, bounds.Height, "blue");
""";

        var document = new ScriptInterpreter().Execute(source);

        var rectangle = (RectangleCommand)document.Commands.Single();
        Assert.AreEqual(10, rectangle.X);
        Assert.AreEqual(40, rectangle.Height);
    }

    [TestMethod]
    public void EnforcesExecutionBounds()
    {
        const string source = """
int i = 0;
while (true) {
    i = i + 1;
}
""";

        var exception = Assert.Throws<ScriptRuntimeException>(() =>
            new ScriptInterpreter().Execute(source, new InterpreterOptions { MaxSteps = 100, MaxLoopIterations = 20 }));

        StringAssert.Contains(exception.Message, "limit");
    }

    [TestMethod]
    public void SupportsCSharpStyleIncrementInForLoops()
    {
        const string source = """
canvas(100, 100, "white");
for (int i = 0; i < 3; i++) {
    rect(i * 10, 0, 5, 5, "black");
}
""";

        var document = new ScriptInterpreter().Execute(source);

        Assert.AreEqual(3, document.Commands.Count);
    }

    [TestMethod]
    public void SupportsAtanAndAtan2MathFunctions()
    {
        const string source = """
canvas(100, 100, "white");
rect(0, 0, 10, 10, "black", atan(1), atan2(1, -1));
""";

        var document = new ScriptInterpreter().Execute(source);
        var rectangle = (RectangleCommand)document.Commands.Single();

        Assert.AreEqual(Math.PI / 4, rectangle.Scale, 0.0001);
        Assert.AreEqual(3 * Math.PI / 4, rectangle.Rotation, 0.0001);
    }

    [TestMethod]
    public void FormatsSyntaxErrorsWithSourceLocation()
    {
        const string source = "canvas(100, 100, \"white\")\n";

        var exception = Assert.Throws<ScriptSyntaxException>(() =>
            new ScriptInterpreter().Execute(source));

        var formatted = ScriptErrorFormatter.Format(source, exception);

        StringAssert.Contains(formatted, "line 1");
        StringAssert.Contains(formatted, "^");
        StringAssert.Contains(formatted, "Expected ';'");
    }

    [TestMethod]
    public void KeepsVariableTypesOnAssignment()
    {
        const string source = """
int count = 1;
count = 1.5;
""";

        var exception = Assert.Throws<ScriptRuntimeException>(() =>
            new ScriptInterpreter().Execute(source));

        StringAssert.Contains(exception.Message, "Cannot assign");
    }
}
