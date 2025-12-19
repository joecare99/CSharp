using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.DriveBASIC.Data;
using TranspilerLib.DriveBASIC.Data.Interfaces;

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

    [TestMethod]
    [DataRow("   ", "")]
    [DataRow("  Both  ", "Both")]
    [DataRow("  Leading", "Leading")]
    [DataRow("", "")]
    [DataRow("< NoIdentifyer >", "< NoIdentifyer >")] // Leerzeichen innerhalb von <> werden entfernt, wenn sie am Rand stehen
    [DataRow("<Identifyer>", "<Identifyer>")]
    [DataRow("1 + 2 * 3", "1+2*3")]
    [DataRow("A + B", "A+B")]
    [DataRow("A  +  B", "A+B")]
    [DataRow("IF A > B THEN", "IF A > B THEN")]
    [DataRow("IF A < B THEN", "IF A < B THEN")]
    [DataRow("VAR < 10", "VAR < 10")]
    [DataRow("VAR > 10", "VAR > 10")]
    [DataRow("Label:", "Label:")]
    [DataRow("NoLabel :", "NoLabel :")]
    [DataRow("A := B", "A :=B")]
    [DataRow("A : = B", "A : =B")]
    [DataRow("A <Identifyer> B", "A <Identifyer> B")]
    [DataRow("A < NoIdentifyer > B", "A < NoIdentifyer > B")]
    [DataRow("String \"  Text  \"", "String\"Text\"")] // Anführungszeichen werden hier nicht gesondert behandelt, Logik entfernt Spaces basierend auf Nachbarn
    [DataRow("Trailing  ", "Trailing")]
    [DataRow("A . B", "A.B")]
    [DataRow("Func ( A , B )", "Func(A,B)")]
    [DataRow("A<B>C", "A<B>C")]
    [DataRow("A < B > C", "A < B > C")]
    [DataRow("Name : <Type>", "Name : <Type>")]
    public void MTSpaceTrimTests(string input, string expected)
    {
        // Act
        var result = DriveCompiler.MTSpaceTrim(input);

        // Assert
        Assert.AreEqual(expected, result, $"Input: '{input}'");
    }

    private const string RootPlaceholder = "<Root>";

    [TestMethod]
    [DataRow("NOP", "NOP", true, "", DisplayName = "Matches literal-only mask")]
    [DataRow("SUB Start", "SUB <Identifyer:Label>", true, "<Identifyer:Label>=Start", DisplayName = "Matches trailing placeholder")]
    [DataRow("Start: HELLO", "<Identifyer:Label>: <Text:Token>", true, "<Identifyer:Label>=Start|<Text:Token>=HELLO", DisplayName = "Matches placeholder surrounded by literals")]
    [DataRow("call 42", "CALL <Integer:Param1>", true, "<Integer:Param1>=42", DisplayName = "Matches while ignoring literal casing")]
    [DataRow("CALL TEXT", "CALL <Integer:Param1>", false, "", DisplayName = "Rejects when placeholder value violates expectations")]
    [DataRow("JMP Start", "SUB <Identifyer:Label>", false, "", DisplayName = "Rejects when literal prefix mismatches")]
    public void TryPlaceHolderMatching_EvaluatesPatternsCorrectly(string probe, string mask, bool expectedSuccess, string expectedPairsSpec)
    {
        var wildcardFill = CreateSeededWildcardFill();

        var actual = FCompiler.TryPlaceHolderMatching(probe, mask, wildcardFill,testCharset );

        Assert.AreEqual(expectedSuccess, actual);

        var expectedPairs = ParseExpectedPairs(expectedPairsSpec);
        var actualPairs = wildcardFill.Skip(1).ToList();

        Assert.AreEqual(expectedPairs.Count, actualPairs.Count);

        for (var index = 0; index < expectedPairs.Count; index++)
        {
            Assert.AreEqual(expectedPairs[index].Key, actualPairs[index].Key);
            Assert.AreEqual(expectedPairs[index].Value, actualPairs[index].Value);
        }
    }
    [TestMethod]
    [DataRow("NOP", "NOP", true, "", DisplayName = "Matches literal-only mask")]
    [DataRow("SUB Start", "SUB <Identifyer:Label>", true, "<Identifyer:Label>=Start", DisplayName = "Matches trailing placeholder")]
    [DataRow("Start: HELLO", "<Identifyer:Label>: <Text:Token>", true, "<Identifyer:Label>=Start|<Text:Token>=HELLO", DisplayName = "Matches placeholder surrounded by literals")]
    [DataRow("call 42", "CALL <Integer:Param1>", true, "<Integer:Param1>=42", DisplayName = "Matches while ignoring literal casing")]
    [DataRow("CALL TEXT", "CALL <Integer:Param1>", true, "<Integer:Param1>=TEXT", DisplayName = "Don't rejects when placeholder value violates expectations")]
    [DataRow("JMP Start", "SUB <Identifyer:Label>", false, "", DisplayName = "Rejects when literal prefix mismatches")]
    public void TryPlaceHolderMatching_EvaluatesPatternsCorrectly2(string probe, string mask, bool expectedSuccess, string expectedPairsSpec)
    {
        var wildcardFill = CreateSeededWildcardFill();

        var actual = FCompiler.TryPlaceHolderMatching(probe, mask, wildcardFill );

        Assert.AreEqual(expectedSuccess, actual);

        var expectedPairs = ParseExpectedPairs(expectedPairsSpec);
        var actualPairs = wildcardFill.Skip(1).ToList();

        Assert.AreEqual(expectedPairs.Count, actualPairs.Count);

        for (var index = 0; index < expectedPairs.Count; index++)
        {
            Assert.AreEqual(expectedPairs[index].Key, actualPairs[index].Key);
            Assert.AreEqual(expectedPairs[index].Value, actualPairs[index].Value);
        }
    }

    private bool testCharset(string arg1, string arg2) => !arg1.StartsWith("<Integer:") || int.TryParse(arg2, out _);

    private static List<KeyValuePair<string, string>> CreateSeededWildcardFill()
    {
        return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(RootPlaceholder, "0")
            };
    }

    private static List<KeyValuePair<string, string>> ParseExpectedPairs(string expectedPairsSpec)
    {
        var result = new List<KeyValuePair<string, string>>();
        if (string.IsNullOrWhiteSpace(expectedPairsSpec))
        {
            return result;
        }

        var segments = expectedPairsSpec.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var segment in segments)
        {
            var separatorIndex = segment.IndexOf('=');
            if (separatorIndex < 0)
            {
                continue;
            }

            var placeholder = segment.Substring(0, separatorIndex).Trim();
            var value = segment.Substring(separatorIndex + 1).Trim();
            result.Add(new KeyValuePair<string, string>(placeholder, value));
        }

        return result;
    }

    [TestMethod]
    [DataRow("Empty line", "", new[]
    {
        "<COMMAND>|rule|<TEXT:TOKEN>",
        "<TEXT:TOKEN>|nested|<TOKEN>",
        "<TOKEN>|rule|"
    })]
    [DataRow("Literal command without arguments", "NOP", new[]
    {
        "<COMMAND>|rule|<TEXT:TOKEN>",
        "<TEXT:TOKEN>|nested|<TOKEN>",
        "<TOKEN>|rule|NOP"
    })]
    [DataRow("Command with identifier parameter", "SUB Start", new[]
    {
        "<COMMAND>|rule|<TEXT:TOKEN>",
        "<TEXT:TOKEN>|nested|<TOKEN>",
        "<TOKEN>|rule|SUB <Identifyer:Label>",
        "<Identifyer:Label>|literal|Start"
    })]
    [DataRow("Label prefix followed by command", "Main: NOP", new[]
    {
        "<COMMAND>|rule|<Identifyer:Label>: <TEXT:TOKEN>",
        "<Identifyer:Label>|literal|Main",
        "<TEXT:TOKEN>|nested|<TOKEN>",
         "<TOKEN>|rule|NOP"
   })]
    public void ParseLine_Succeeds(string scenario, string line, string[] expectations)
    {
        var parseResult = AssertParseResult(FCompiler.ParseLine(ParseDefinitions.CCommand, line, out var errorCode));
        Debug.WriteLine(TokenParseTreeDbg(parseResult));

        Assert.AreEqual(0, errorCode, $"{scenario}: unexpected error code");
        AssertMatchesSpec(parseResult, expectations, scenario);
    }

    [TestMethod]
    [DataRow("Invalid integer placeholder", "FUNC TEXT", DisplayName = "Integer placeholder requires numeric value")]
    public void ParseLine_Fails(string scenario, string line)
    {
        var result = FCompiler.ParseLine(ParseDefinitions.CCommand, line, out var errorCode);

        Assert.IsNull(result, $"{scenario}: expected null result");
        Assert.AreNotEqual(0, errorCode, $"{scenario}: error code should be non-zero");
    }

    private static IReadOnlyList<KeyValuePair<string, object?>> AssertParseResult(object? candidate)
    {
        Assert.IsNotNull(candidate, "Parse result was expected but is null.");
        Assert.IsInstanceOfType(candidate, typeof(IList<KeyValuePair<string, object?>>));
        return (IReadOnlyList<KeyValuePair<string, object?>>)candidate;
    }

    private static void AssertMatchesSpec(IReadOnlyList<KeyValuePair<string, object?>> matches, string[] spec, string scenario)
    {
        foreach (var entry in spec)
        {
            var parts = entry.Split('|');
            Assert.IsTrue(parts.Length >= 3, $"{scenario}: invalid expectation entry '{entry}'");

            var placeholder = parts[0];
            var expectationType = parts[1];
            var expectationValue = parts[2];

            var match = GetMatch(matches, placeholder);
            switch (expectationType)
            {
                case "rule":
                    var ruleIndex = int.Parse(match.Value.ToString() ?? "0", CultureInfo.InvariantCulture);
                    var pattern = ResolveRulePattern(placeholder, ruleIndex);
                    Assert.AreEqual(expectationValue, pattern, $"{scenario}: placeholder {placeholder} rule mismatch");
                    break;
                case "literal":
                    Assert.AreEqual(expectationValue, match.Value, $"{scenario}: placeholder {placeholder} literal mismatch");
                    break;
                case "nested":
                    var nested = AssertParseResult(match.Value);
                    Assert.AreEqual(expectationValue, nested[0].Key, $"{scenario}: nested root mismatch for {placeholder}");
              //      Assert.AreEqual(parts[3], nested[0].Value.ToString());    
                    break;
                default:
                    Assert.Fail($"{scenario}: unknown expectation type '{expectationType}'");
                    break;
            }
        }
    }

    private static KeyValuePair<string, object?> GetMatch(IReadOnlyList<KeyValuePair<string, object?>> matches, string placeholder)
    {
        var match = matches.FirstOrDefault(pair => string.Equals(pair.Key, placeholder, StringComparison.OrdinalIgnoreCase));
        if (match.Key==null)
           match = matches.Select(pair2 => (pair2.Value is IReadOnlyList<KeyValuePair<string,object?>> kvp)?GetMatch(kvp,placeholder):default).FirstOrDefault(p=>p.Key!=null);
           
      //  Assert.AreNotEqual(default(KeyValuePair<string, object?>), match, $"Match for placeholder '{placeholder}' not found.");
        return match;
    }

    private static string ResolveRulePattern(string placeholder, int ruleIndex)
    {
        if (string.Equals(placeholder, ParseDefinitions.CToken, StringComparison.OrdinalIgnoreCase))
        {
            return ParseDefinitions.ParseDefBase[ruleIndex].text;
        }

        return ParseDefinitions.PlaceHolderDefBase[ruleIndex].text;
    }

    [TestMethod]
    [DataRow(new object[] { 
        new object[] { "<COMMAND>", 4 }, 
        new object[] { "<TEXT:TOKEN>", new object[] {
            new object[] { "<TOKEN>", 1 } } } }, EDriveToken.tt_Nop, (byte)1, 0, 0, 0d)]
    [DataRow("CALL 2", EDriveToken.tt_goto, (byte)1, 2, 0, 0d)]
    [DataRow("ON 1", EDriveToken.tte_goto2, (byte)0, 0, 0, 1d)]
    [DataRow(new object[] {
        new object[] {"<COMMAND>", 4 },
        new object[] {"<TEXT:TOKEN>", new object[] {
            new object[] {"<TOKEN>", 5 },
            new object[] {"<Identifyer:Label>", "Start" },
        } }, }, EDriveToken.tt_Nop, (byte)5, 0, 0, 1d)]
    public void BuildCommand_ProducesDriveCommandFromParseTree(object source, EDriveToken expectedToken, byte expectedSubToken, int expectedPar1, int expectedPar2, double expectedPar3)
    {
        IReadOnlyList<KeyValuePair<string, object?>>? parseResult =null;
        if (source is string sSource)
        {
            parseResult = FCompiler.ParseLine("<COMMAND>", sSource, out _);
            Debug.WriteLine(TokenParseTreeDbg(parseResult!));
        }
        else if (source is object[] arrSource)
            parseResult = TokenParseTree(arrSource);
        
        var tokenBuffer = new List<IDriveCommand>();
        const int pc = 0;
        InitializeCompilerState(tokenBuffer, pc);

        var resultName = InvokeBuildCommand(parseResult, tokenBuffer, 0, pc, out var errorText);

        Assert.AreEqual("BC_OK", resultName, $"{source}: unexpected build result");
        Assert.AreEqual(string.Empty, errorText, $"{source}: error text must stay empty");
        Assert.IsTrue(pc < tokenBuffer.Count, $"{source}: token buffer does not contain index {pc}");
        var command = tokenBuffer[pc];
        Assert.IsNotNull(command, $"{source}: token buffer entry must not be null");
        Assert.AreEqual(expectedToken, command.Token, $"{source}: token mismatch");
        Assert.AreEqual(expectedSubToken, command.SubToken, $"{source}: sub token mismatch");
        Assert.AreEqual(expectedPar1, command.Par1, $"{source}: Par1 mismatch");
        Assert.AreEqual(expectedPar2, command.Par2, $"{source}: Par2 mismatch");
        Assert.AreEqual(expectedPar3, command.Par3, $"{source}: Par3 mismatch");
    }

    [TestMethod]
    [DataRow(null, "BC_SyntaxError")]
    [DataRow("plain text payload", "BC_SyntaxError")]
    public void BuildCommand_ReturnsSyntaxErrorForInvalidPayload(object? invalidPayload, string expectedResult)
    {
        var tokenBuffer = new List<IDriveCommand>();
        InitializeCompilerState(tokenBuffer, 0);
        var placeholder = tokenBuffer[0];

        var resultName = InvokeBuildCommand(invalidPayload, tokenBuffer, 0, 0, out var errorText);

        Assert.AreEqual(expectedResult, resultName, "Invalid payload should yield syntax error");
        Assert.IsFalse(string.IsNullOrWhiteSpace(errorText), "Expected descriptive error text");
        Assert.AreSame(placeholder, tokenBuffer[0], "Invalid payload must not mutate the token buffer");
    }

    private static IReadOnlyList<KeyValuePair<string, object?>> TokenParseTree(object[] objects)
    {
        var result = new List<KeyValuePair<string, object?>>();
        foreach (var o in objects)
        {
            if (o is object[] arr && arr.Length >= 1)
            {
                if (arr[1] is object[] arr2)
                    result.Add(new KeyValuePair<string, object?>((string)arr[0], TokenParseTree(arr2)));
                else
                    result.Add(new KeyValuePair<string, object?>((string)arr[0], arr[1]));
            }
        }

        return result;
    }

    private static string TokenParseTreeDbg(IReadOnlyList<KeyValuePair<string,object?>> tree)
    {
        var sb = new StringBuilder();
        void Recurse(IReadOnlyList<KeyValuePair<string,object?>> nodes, int level)
        {
            var indent = new string(' ', level * 2);
            foreach (var node in nodes)
            {
                if (node.Value is IReadOnlyList<KeyValuePair<string, object?>> subnode)
                {
                    sb.AppendLine($"{indent} new object[] {{\"{node.Key}\", new object[] {{");
                    Recurse(subnode, level + 1);
                    sb.AppendLine($"{indent}}} }},");
                } 
                else
                {                    
                    var quotedValue = node.Value is string ? $"\"{node.Value}\"" : node.Value?.ToString() ?? "null";
                    sb.AppendLine($"{indent} new object[] {{\"{node.Key}\", {quotedValue} }},");
                }
            }
        }
        sb.AppendLine("new object[] {");
        Recurse(tree, 0);
        sb.AppendLine("}");
        return sb.ToString();
    }

    private void InitializeCompilerState(IList<IDriveCommand> tokenBuffer, int targetPc)
    {
        FCompiler.Log = new List<string>();
        FCompiler.Messages = new List<string>();
        FCompiler.Variables = new List<IVariable>();
        FCompiler.Labels = new List<ILabel>();
        FCompiler.TokenCode = tokenBuffer;
        FCompiler.SourceCode ??= Array.Empty<string>();

        while (tokenBuffer.Count <= targetPc)
        {
            tokenBuffer.Add(new DriveCommand(EDriveToken.tt_Nop));
        }
    }

    private string InvokeBuildCommand(object? parseTree, IList<IDriveCommand> tokenBuffer, int level, int pc, out string errorText)
    {
        var method = typeof(DriveCompiler).GetMethod("BuildCommand", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("BuildCommand method not found.");

        var arguments = new object?[] { parseTree, tokenBuffer, level, pc, string.Empty };
        var result = method.Invoke(FCompiler, arguments);
        errorText = arguments[4] as string ?? string.Empty;
        return result?.ToString() ?? string.Empty;
    }
}
