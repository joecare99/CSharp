using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.DriveBASIC.Data;

#pragma warning disable IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
namespace TranspilerLib.DriveBASIC.Models.Tests;
#pragma warning restore IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.

[TestClass]
public class DriveCompilerTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private DriveCompiler FCompiler;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    [TestInitialize]
    public void TestInitialize()
    {
        FCompiler = new DriveCompiler();
    }

    [TestMethod]
    [TestCategory("DriveCompiler")]
    [DataRow(null, EDriveToken.tt_Nop, new object[] { })]
    [DataRow("NOP", EDriveToken.tt_Nop, new object[] { (byte)1 })]
    [DataRow("DEFINE 5,\"Dies ist ein Test\"", EDriveToken.tt_Nop, new object[] { (byte)3, 5u })]
    [DataRow("Goto 2", EDriveToken.tt_goto, new object[] { (byte)0, 2 })]
    [DataRow("Call 2", EDriveToken.tt_goto, new object[] { (byte)1, 2 })]
    [DataRow("On 1", EDriveToken.tte_goto2, new object[] { (byte)0, 1d })]
    [DataRow("End", EDriveToken.tt_end, new object[] { (byte)1 })]
    [DataRow("Stop", EDriveToken.tt_end, new object[] { (byte)2 })]
    [DataRow("Pause", EDriveToken.tt_end, new object[] { (byte)3 })]
    public void Compile1Nop(string? line, EDriveToken token, object[] oExp)
    {
        // Arrange
        var LMessage = new List<string>();
        FCompiler.SourceCode = [line];
        FCompiler.Log = LMessage;
        // Act
        bool LResult = FCompiler.Compile();
        // Assert

        CollectionAssert.AreEqual(new List<string>() { "Leerer TokenCode (OK)", "Leeres Label-Array (OK)", "Leeres Message-Array (OK)", "Kein (System-)Variablen-Array (OK)" }, LMessage, "Log-Entry");
        Assert.HasCount(0, FCompiler.Variables, "Var Count");
        Assert.HasCount(0, FCompiler.Labels, "Labels Count");
        Assert.HasCount(1, FCompiler.TokenCode, "Tokens Count");
        Assert.AreEqual(new DriveCommand(token, oExp), FCompiler.TokenCode[0], "Token[0]");
    }

    [TestMethod]
    [DataRow("DECLARE Testpunkt.", EDriveToken.tt_Nop, new object[] { (byte)2, 32769d }, "TESTPUNKT.", 32769)]
    [DataRow("DECLARE TestReal%", EDriveToken.tt_Nop, new object[] { (byte)2, 16385d }, "TESTREAL%", 16385)]
    [DataRow("DECLARE TestBool&", EDriveToken.tt_Nop, new object[] { (byte)2, 1d }, "TESTBOOL&", 1)]
    [DataRow("onc TestInt%", EDriveToken.tte_goto2, new object[] { (byte)96, 16385d }, "TESTINT%", 1)]
    public void Compile2SL_Declare(string line, EDriveToken token, object[] oExp, string sExp, int iExp)
    {
        // Arrange
        var LMessage = new List<string>();
        FCompiler.SourceCode = [line];
        FCompiler.Log = LMessage;
        // Act
        bool LResult = FCompiler.Compile();
        // Assert
        Assert.AreEqual(["Leerer TokenCode(OK)", "Leeres Label-Array (OK)", "Leeres Message-Array (OK)", "Kein (System-)Variablen-Array (OK)"], LMessage, "Log-Entry");
        Assert.HasCount(1, FCompiler.Variables, "Var Count");
        Assert.AreEqual(sExp, FCompiler.Variables[0].Name, "Var[0].Name");
        Assert.AreEqual(iExp, FCompiler.Variables[0].Index, "Var[0].Index");
        Assert.HasCount(1, FCompiler.TokenCode, "Tokens Count");
        Assert.AreEqual(new DriveCommand(token, oExp), FCompiler.TokenCode[0], "Token[0]");
    }

    [TestMethod]
    [DataRow("Let TestBool& := 1", EDriveToken.tte_let, new object[] { (byte)0, 1, 1d }, "TESTBOOL&", 1)]
    [DataRow("Let TestReal% := 1.1", EDriveToken.tte_let, new object[] { (byte)0, 16385, 1.1d }, "TESTPUNKT%", 16385)]
    [DataRow("Let TestPoint. := 1.1", EDriveToken.tte_let, new object[] { (byte)0, 32769, 1.1d }, "TESTPUNKT.", 32769)]
    [DataRow("Let TestPoint.z := 1", EDriveToken.tte_let, new object[] { (byte)0, 49160, 1d }, "TESTPUNKT.", 32769)]
    public void Compile3LetConst(string line, EDriveToken token, object[] oExp, string sExp, int iExp)
    {
        // Arrange
        var LMessage = new List<string>();
        FCompiler.SourceCode = [line];
        FCompiler.Log = LMessage;
        // Act
        bool LResult = FCompiler.Compile();
        // Assert
        Assert.AreEqual(["Leerer TokenCode(OK)", "Leeres Label-Array (OK)", "Leeres Message-Array (OK)", "Kein (System-)Variablen-Array (OK)"], LMessage, "Log-Entry");
        Assert.HasCount(1, FCompiler.Variables, "Var Count");
        Assert.AreEqual(sExp, FCompiler.Variables[0].Name, "Var[0].Name");
        Assert.AreEqual(iExp, FCompiler.Variables[0].Index, "Var[0].Index");
        Assert.HasCount(1, FCompiler.TokenCode, "Tokens Count");
        Assert.AreEqual(new DriveCommand(token, oExp), FCompiler.TokenCode[0], "Token[0]");
    }

    [TestMethod]
    [DataRow("Let TestBool& := Active&", EDriveToken.tte_let, new object[] { (byte)32, 2, 1d }, "TESTBOOL&", 1)]
    [DataRow("Let TestReal% := Active&", EDriveToken.tte_let, new object[] { (byte)32, 16385, 1d }, "TESTPUNKT%", 16385)]
    [DataRow("Let TestPoint. := Active&", EDriveToken.tte_let, new object[] { (byte)32, 32769, 1d }, "TESTPUNKT.", 32769)]
    [DataRow("Let TestPoint.z := Active&", EDriveToken.tte_let, new object[] { (byte)32, 49160, 1d }, "TESTPUNKT.", 32769)]
    public void Compile4LetVar(string line, EDriveToken token, object[] oExp, string sExp, int iExp)
    {
        // Arrange
        var LMessage = new List<string>();
        FCompiler.SourceCode = [line];
        FCompiler.Log = LMessage;
        // Act
        bool LResult = FCompiler.Compile();
        // Assert
        Assert.AreEqual(["Leerer TokenCode(OK)", "Leeres Label-Array (OK)", "Leeres Message-Array (OK)", "Kein (System-)Variablen-Array (OK)"], LMessage, "Log-Entry");
        Assert.HasCount(2, FCompiler.Variables, "Var Count");
        Assert.AreEqual("Active&", FCompiler.Variables[0].Name, "Var[0].Name");
        Assert.AreEqual(1, FCompiler.Variables[0].Index, "Var[0].Index");
        Assert.AreEqual(sExp, FCompiler.Variables[1].Name, "Var[1].Name");
        Assert.AreEqual(iExp, FCompiler.Variables[1].Index, "Var[1].Index");
        Assert.HasCount(1, FCompiler.TokenCode, "Tokens Count");
        Assert.AreEqual(new DriveCommand(token, oExp), FCompiler.TokenCode[0], "Token[0]");
    }

    [TestMethod]
    [DataRow("TestLbl: Goto TestLbl", EDriveToken.tt_goto, new object[] { (byte)2 }, "TESTLBL", 1)]
    [DataRow("TestLbl: Call TestLbl", EDriveToken.tt_goto, new object[] { (byte)3 }, "TESTLBL", 1)]
    public void Compile5GotoLbl(string line, EDriveToken token, object[] oExp, string sExp, int iExp)
    {
        // Arrange
        var LMessage = new List<string>();
        FCompiler.SourceCode = [line];
        FCompiler.Log = LMessage;
        // Act
        bool LResult = FCompiler.Compile();
        // Assert
        Assert.AreEqual(["Leerer TokenCode(OK)", "Leeres Label-Array (OK)", "Leeres Message-Array (OK)", "Kein (System-)Variablen-Array (OK)"], LMessage, "Log-Entry");
        Assert.HasCount(0, FCompiler.Variables, "Var Count");
        Assert.HasCount(1, FCompiler.Labels, "Label Count");
        Assert.AreEqual(sExp, FCompiler.Labels[0].Name, "Labels[0].Name");
        Assert.AreEqual(iExp, FCompiler.Labels[0].Index, "Labels[0].Index");
        Assert.HasCount(1, FCompiler.TokenCode, "Tokens Count");
        Assert.AreEqual(new DriveCommand(token, oExp), FCompiler.TokenCode[0], "Token[0]");
    }

    [TestMethod]
    [DataRow(new[] { "ON 1", "Goto 1", "Goto 2" },
        new[] { EDriveToken.tte_goto2, EDriveToken.tt_goto, EDriveToken.tt_goto },
        new object[]{
         new object[]{(byte)0,1d },
         new object[]{(byte)0,1 },
         new object[]{(byte)0,2 }
        })]
    [DataRow(new[] { "IF 1 THEN", "ENDIF" },
        new[] { EDriveToken.tte_if, EDriveToken.tt_end },
        new object[]{
         new object[]{(byte)0,1,1d },
         new object[]{(byte)4 },
        })]
    [DataRow(new[] { "IF 1 THEN", "ELSE", "ENDIF" },
        new[] { EDriveToken.tte_if,EDriveToken.tt_else, EDriveToken.tt_end },
        new object[]{
         new object[]{(byte)0,1,1d },
         new object[]{(byte)0,2 },
         new object[]{(byte)4 },
        })]
    [DataRow(new[] { "IF 1 THEN", "ELSIF 0 THEN", "ENDIF" },
        new[] { EDriveToken.tte_if,EDriveToken.tte_if, EDriveToken.tt_end },
        new object[]{
         new object[]{(byte)0,1,1d },
         new object[]{(byte)64,2 },
         new object[]{(byte)4 },
        })]
    [DataRow(new[] { "IF 1 THEN", "ELSIF 0 THEN", "ELSE", "ENDIF" },
        new[] { EDriveToken.tte_if, EDriveToken.tte_if, EDriveToken.tt_else, EDriveToken.tt_end },
        new object[]{
         new object[]{(byte)0,1,1d },
         new object[]{(byte)64,2 },
         new object[]{(byte)0,3 },
         new object[]{(byte)4 },
        })]
    [DataRow(new[] { "While 1", "NOP", "WEND" },
        new[] { EDriveToken.tte_while, EDriveToken.tt_Nop, EDriveToken.tt_end },
        new object[]{
         new object[]{(byte)0,2,1d },
         new object[]{(byte)1 },
         new object[]{(byte)6 },
        })]
    public void Compile6MLCommand(string[] lines, EDriveToken[] token, object[] oExp)
    {
        // Arrange
        List<string> LMessage = new List<string>();
        FCompiler.SourceCode = lines;
        FCompiler.Log = LMessage;
        // Act
        bool LResult = FCompiler.Compile();
        // Assert
        List<string> expectedMessages = ["Leerer TokenCode(OK)", "Leeres Label-Array (OK)", "Leeres Message-Array (OK)", "Kein (System-)Variablen-Array (OK)"];
        
        CollectionAssert.AreEqual(expectedMessages, LMessage, "Log-Entry");
        Assert.HasCount(0, FCompiler.Variables, "Var Count");
        Assert.HasCount(0, FCompiler.Labels, "Label Count");
        Assert.HasCount(token.Length, FCompiler.TokenCode, "Tokens Count");
        for (int i = 0; i < token.Length; i++)
            Assert.AreEqual(new DriveCommand(token[i], oExp[i] as object[]), FCompiler.TokenCode[i], $"Token[{i}]");
    }

    [TestMethod]
    [DataRow("1 + 2", 0, true, new[] { "(0, False, 1 + 2)" })]
    [DataRow("Wert2: <Wert2>", 0, true, new[] { "(0, False, Wert2: <Float:Param3>)", "(32, False, Wert2: <Identifyer:Param3>)" })]
    public void BuildExpressionNormalTests(string line,int iStart,bool xAct,string[]? lExp)
    {
        List<string> result = [];
        // Act
        FCompiler.BuildExpressionNormal(line, (o) => result.Add($"{o}"), 0, false);
        // Assert
        Assert.HasCount(lExp?.Length ?? 0, result, "Result Count");
        for (int i = 0; i < lExp.Length; i++)
            Assert.AreEqual(lExp[i], result[i], $"Result[{i}]");
    }

    [TestMethod]
    [DataRow("<Float:", "123.45", true)]
    [DataRow("<Float:", "123", true)]
    [DataRow("<Float:", "123.45.", false)]
    [DataRow("<Float:", "abc", false)]
    [DataRow("<Identifyer:", "MyVar", true)]
    [DataRow("<Identifyer:", "My_Var1", true)]
    [DataRow("<Identifyer:", "1MyVar", false)]
    [DataRow("<Identifyer:", "MyVar.", true)]
    [DataRow("<Identifyer:", "MyVar.x", true)]
    [DataRow("<UnknownPH>", "123", false)]
    public void TestPlaceHolderCharsetTests(string placeholder, string text, bool expected)
    {
        // Act
        var result = FCompiler.TestPlaceHolderCharset(placeholder, text);

        // Assert
        Assert.AreEqual(expected, result, $"Placeholder: {placeholder}, Text: {text}");
    }
}
