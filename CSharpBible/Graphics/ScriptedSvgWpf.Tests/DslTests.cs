using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptedSvgWpf.Dsl;
using ScriptedSvgWpf.Models;
using ScriptedSvgWpf.Rendering;

namespace ScriptedSvgWpf.Tests;

[TestClass]
public sealed class DslTests
{
    [TestMethod]
    public void LexerHandlesCommentsEscapesNumbersAndOperators()
    {
        const string source = "// comment\n/* block */ let text = \"a\\n\\t\\\\\\\"\"; value += 1.5e-2; i++; j--; x *= 2; y /= 2; a != b && c || d <= e >= f;";

        var tokens = new ScriptLexer(source).Lex();

        Assert.AreEqual(TokenKind.Identifier, tokens[0].Kind);
        Assert.AreEqual("let", tokens[0].Lexeme);
        Assert.AreEqual("a\n\t\\\"", tokens[3].Lexeme);
        Assert.IsTrue(tokens.Any(token => token.Kind == TokenKind.Float && token.Lexeme == "1.5e-2"));
        CollectionAssert.AreEqual(
            new[] { TokenKind.PlusEqual, TokenKind.PlusPlus, TokenKind.MinusMinus, TokenKind.StarEqual, TokenKind.SlashEqual, TokenKind.BangEqual, TokenKind.AndAnd, TokenKind.OrOr, TokenKind.LessEqual, TokenKind.GreaterEqual },
            tokens.Where(token => token.Kind is TokenKind.PlusEqual or TokenKind.PlusPlus or TokenKind.MinusMinus or TokenKind.StarEqual or TokenKind.SlashEqual or TokenKind.BangEqual or TokenKind.AndAnd or TokenKind.OrOr or TokenKind.LessEqual or TokenKind.GreaterEqual).Select(token => token.Kind).ToArray());
    }

    [TestMethod]
    public void LexerRejectsUnexpectedCharactersAndInvalidStrings()
    {
        var characterError = Assert.Throws<ScriptSyntaxException>(() => new ScriptLexer("@\n").Lex());
        Assert.AreEqual(1, characterError.Line);
        Assert.AreEqual(1, characterError.Column);

        Assert.Throws<ScriptSyntaxException>(() => new ScriptLexer("\"unterminated").Lex());
        Assert.Throws<ScriptSyntaxException>(() => new ScriptLexer("\"bad\\q\"").Lex());
        Assert.Throws<ScriptSyntaxException>(() => new ScriptLexer("1e+").Lex());
    }

    [TestMethod]
    public void ParserSupportsLetElseBlocksAndCompoundAssignments()
    {
        const string source = "let int value = 1; if (false) { value += 10; } else { value *= 3; }";

        var document = new ScriptInterpreter().Execute(source);

        Assert.AreEqual(640, document.Width);
        Assert.AreEqual(480, document.Height);
    }

    [TestMethod]
    public void ScriptValueConvertsAndFormatsAllSupportedKinds()
    {
        var values = new IReadOnlyList<ScriptValue>[]
        {
            Array.Empty<ScriptValue>()
        };
        var point = ScriptValue.From(new ScriptedSvgWpf.Models.ScriptPoint(1.5, 2));
        var rectangle = ScriptValue.From(new ScriptedSvgWpf.Models.ScriptRect(1, 2, 3, 4));
        var array = ScriptValue.From(values[0]);

        Assert.AreEqual("null", ScriptValue.From(null).ToString());
        Assert.AreEqual("1", ScriptValue.From(1).ToString());
        Assert.AreEqual("1.5", ScriptValue.From(1.5).ToString());
        Assert.AreEqual("true", ScriptValue.From(true).ToString());
        Assert.AreEqual("text", ScriptValue.From("text").ToString());
        Assert.AreEqual("1.5,2", point.ToString());
        Assert.AreEqual("1,2,3,4", rectangle.ToString());
        Assert.AreEqual("[]", array.ToString());
        Assert.AreEqual(1.5, point.AsPoint().X);
        Assert.AreEqual(4, rectangle.AsRect().Height);
        Assert.AreEqual(0, array.AsArray().Count);
    }

    [TestMethod]
    public void ScriptValueRejectsInvalidConversionsAndTypes()
    {
        Assert.Throws<ScriptRuntimeException>(() => ScriptValue.From("x").AsInt());
        Assert.Throws<ScriptRuntimeException>(() => ScriptValue.From("x").AsNumber());
        Assert.Throws<ScriptRuntimeException>(() => ScriptValue.From(1).AsBool());
        Assert.Throws<ScriptRuntimeException>(() => ScriptValue.From(1).AsString());
        Assert.Throws<ScriptRuntimeException>(() => ScriptValue.From(1).AsPoint());
        Assert.Throws<ScriptRuntimeException>(() => ScriptValue.From(1).AsRect());
        Assert.Throws<ScriptRuntimeException>(() => ScriptValue.From(1).AsArray());
        Assert.Throws<ScriptRuntimeException>(() => ScriptValue.From(new object()));
        Assert.Throws<OverflowException>(() => new ScriptValue(ScriptValueKind.Float, (double)int.MaxValue + 1).AsInt());
    }

    [TestMethod]
    public void ErrorFormatterHandlesRuntimeWrappedAndGenericErrors()
    {
        var runtime = ScriptErrorFormatter.Format("source", new ScriptRuntimeException("bad value"));
        StringAssert.StartsWith(runtime, "Runtime error:");

        var wrapped = ScriptErrorFormatter.Format("source", new Exception("wrapper", new ScriptSyntaxException("bad syntax", 1, 2)));
        StringAssert.Contains(wrapped, "Syntax error at line 1, column 2");

        var generic = ScriptErrorFormatter.Format("source", new Exception());
        StringAssert.Contains(generic, "Script error (Exception)");
        Assert.Throws<ArgumentNullException>(() => ScriptErrorFormatter.Format(null!, new Exception()));
        Assert.Throws<ArgumentNullException>(() => ScriptErrorFormatter.Format("source", null!));
    }

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

    [TestMethod]
    public void EnforcesLoopAndDrawCommandLimits()
    {
        const string loopSource = """
int i = 0;
while (i < 3) {
    i++;
}
""";

        var loopException = Assert.Throws<ScriptRuntimeException>(() =>
            new ScriptInterpreter().Execute(loopSource, new InterpreterOptions { MaxLoopIterations = 2 }));

        StringAssert.Contains(loopException.Message, "Loop limit of 2");

        const string drawSource = """
rect(0, 0, 1, 1, "black");
rect(1, 0, 1, 1, "black");
""";

        var drawException = Assert.Throws<ScriptRuntimeException>(() =>
            new ScriptInterpreter().Execute(drawSource, new InterpreterOptions { MaxDrawCommands = 1 }));

        StringAssert.Contains(drawException.Message, "Draw command limit of 1");
    }

    [TestMethod]
    public void RejectsDivisionByZeroAndInvalidMembersOrFunctions()
    {
        Assert.Throws<ScriptRuntimeException>(() => new ScriptInterpreter().Execute("float value = 1 / 0;"));
        Assert.Throws<ScriptRuntimeException>(() => new ScriptInterpreter().Execute("Point point = Point(1, 2); float value = point.Z;"));
        Assert.Throws<ScriptRuntimeException>(() => new ScriptInterpreter().Execute("unknown(1);"));
    }

    [TestMethod]
    public void KeepsBlockAndForVariablesInsideTheirScopes()
    {
        var blockException = Assert.Throws<ScriptRuntimeException>(() =>
            new ScriptInterpreter().Execute("{ int local = 1; } int outside = local;"));
        StringAssert.Contains(blockException.Message, "Unknown variable 'local'");

        var forException = Assert.Throws<ScriptRuntimeException>(() =>
            new ScriptInterpreter().Execute("for (int index = 0; index < 1; index++) { } int outside = index;"));
        StringAssert.Contains(forException.Message, "Unknown variable 'index'");
    }

    [TestMethod]
    public void ShortCircuitBooleanOperatorsDoNotEvaluateUnneededOperands()
    {
        var document = new ScriptInterpreter().Execute("bool left = false && (1 / 0 > 0); bool right = true || (1 / 0 > 0);");

        Assert.AreEqual(0, document.Commands.Count);
    }
}
