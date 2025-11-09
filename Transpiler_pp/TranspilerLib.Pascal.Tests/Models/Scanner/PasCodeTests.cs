using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;
using TranspilerLib.Pascal.Models.Scanner;
using TranspilerLibTests.TestData;
using static TranspilerLib.Helper.TestHelper;

#pragma warning disable IDE0130
namespace TranspilerLib.Pascal.Models.Scanner.Tests;
#pragma warning restore IDE0130

[TestClass]
public class PasCodeTests : TranspilerLib.Models.Tests.TestBase
{
    private PasCode _testClass = null!;

    public static IEnumerable<object[]> TestTokenizeList
    {
        get
        {
            foreach (var dir in Directory.EnumerateDirectories(".\\Resources"))
            {
                if (!File.Exists(Path.Combine(dir, Path.GetFileName(dir) + "_Source.pas")))
                    continue;
                
                var value = new object[]
                {
                    Path.GetFileName(dir)!,
                    File.ReadAllText(Path.Combine(dir, Path.GetFileName(dir)+ "_Source.pas")) ,
                    File.Exists(Path.Combine(dir,Path.GetFileName(dir)+"_ExpectedTokens.json"))?File.ReadAllText(Path.Combine(dir,Path.GetFileName(dir)+"_ExpectedTokens.json")):""
                };
                yield return value;
            }
        }
    }

    public static IEnumerable<object[]> TestCodeBuilderList
    {
        get
        {
            foreach (var dir in Directory.EnumerateDirectories(".\\Resources"))
            {
                // require expected tokens to drive Parse() and optionally expected code blocks
                var tokensPath = Path.Combine(dir, Path.GetFileName(dir) + "_ExpectedTokens.json");
                if (!File.Exists(tokensPath))
                    continue;

                var codeBlocksPath = Path.Combine(dir, Path.GetFileName(dir) + "_ExpectedCodeBlocks.json");

                var value = new object[]
                {
                    Path.GetFileName(dir)!,
                    File.ReadAllText(tokensPath),
                    File.Exists(codeBlocksPath) ? File.ReadAllText(codeBlocksPath) : string.Empty
                };
                yield return value;
            }
        }
    } 


    [TestInitialize]
    public void Init() => _testClass = new PasCode();

    /*
        Erweiterung: JSON-basierte exakte Tokenprüfung
        ------------------------------------------------
        Ziel: Eine erwartete Sequenz inkl. Reihenfolge, Code und optional Typ präzise validieren.
        Vorgehen:
          - Record ExpectedToken(Code, Type?)
          - JsonSerializerOptions mit StringEnumConverter + CaseInsensitive
          - Neues DataTestMethod: Tokenize_Scenarios_Json(source, expectedJson)
          - Deserialisieren zu List<ExpectedToken>
          - Tokenize ausführen und komplette Übereinstimmung prüfen:
              * Anzahl gleich
              * Jedes Token[i].Code case-insensitive == Expected[i].Code
              * Falls Expected[i].Type != null -> Vergleich Token[i].type == Expected[i].Type
          - Fehlermeldungen klar mit Index / Soll / Ist
        Beispiel JSON:
          [ {"code":"begin","type":"Block"}, {"code":"end","type":"Block"}, {"code":";"} ]
        Hinweis: "type" kann entfallen falls nicht geprüft werden soll.
    */

    private sealed record ExpectedToken(string Code, CodeBlockType? Type);

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true),
            new ICodeBlockPasCodeBlockConverter()
        }
    };

    private static List<TokenData> DeserializeTokens(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new();
        try
        {
            var list = JsonSerializer.Deserialize<List<TokenData>>(json, _jsonOptions);
            return list ?? new();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Fehler beim Deserialisieren der Expected-Tokenliste: {ex.Message}\nJSON:\n{json}");
            return new();
        }
    }

    private static List<PasCodeBlock> DeserializeCodeBlocks(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new();
        try
        {
            var list = JsonSerializer.Deserialize<List<PasCodeBlock>>(json, _jsonOptions);
            return list ?? new();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Fehler beim Deserialisieren der Expected-Tokenliste: {ex.Message}\nJSON:\n{json}");
            return new();
        }
    }


    [DataTestMethod]
    [DataRow("", null, null, 0)]
    [DataRow("begin end;", new[] { "begin", "end",";" }, null, 3)]
    [DataRow("begin begin end; end;", new[] { "begin", "begin", "end",";", "end",";" }, null, null)]
    [DataRow("begin 'a''b'; end;", null, new[] { CodeBlockType.String }, null)]
    [DataRow("begin //x\n(* abc *) {def} end;", null, new[] { CodeBlockType.LComment, CodeBlockType.Comment }, null)]
    [DataRow("var x: integer; begin end;", new[] { "var","x",":","integer",";", "begin", "end",";" }, null, null)]
    [DataRow(" \t\r\n begin   end ;  ", new[] { "begin", "end", ";" }, null, null)]
    public void Tokenize_Scenarios(string source, string[]? expectedCodes, CodeBlockType[]? expectedTypes, int? expectedCount)
    {
        _testClass.OriginalCode = source;
        var tokens = new List<TokenData>();
        _testClass.Tokenize(t => tokens.Add(t));

        if (expectedCount.HasValue)
            Assert.AreEqual(expectedCount.Value, tokens.Count, $"Tokenanzahl stimmt nicht für Source: '{source}'.");

        if (expectedCodes != null)
        {
            foreach (var ec in expectedCodes.Zip(tokens))
            {
                Assert.AreEqual(ec.First, ec.Second.Code.ToLower(),
                    $"Erwartetes Token-Code '{ec.First}' nicht gefunden but '{ec.Second.Code}' in Source: '{source}'. Gefundene Codes: {string.Join(", ", tokens.Select(t => t.Code))}");
            }
        }

        if (expectedTypes != null)
        {
            foreach (var et in expectedTypes)
            {
                Assert.IsTrue(tokens.Any(t => t.type == et),
                    $"Erwarteter Token-Typ '{et}' nicht gefunden in Source: '{source}'. Gefundene Typen: {string.Join(", ", tokens.Select(t => t.type))}");
            }
        }
    }

    [TestMethod]
    [DataRow("begin end;", "[{\"code\":\"begin\",\"type\":\"Block\",\"Level\":1},{\"code\":\"end\",\"type\":\"Block\",\"Level\":1},{\"code\":\";\"}]")]
    [DataRow("begin begin end; end;", "[{\"code\":\"begin\",\"Level\":1},{\"code\":\"begin\",\"Level\":2},{\"code\":\"end\",\"Level\":2},{\"code\":\";\",\"Level\":1},{\"code\":\"end\",\"Level\":1},{\"code\":\";\"}]")]
    [DataRow("begin 'a''b'; end;", "[{\"code\":\"begin\",\"Level\":1},{\"code\":\"'a''b'\",\"type\":\"String\",\"Level\":1},{\"code\":\";\",\"Level\":1},{\"code\":\"end\",\"Level\":1},{\"code\":\";\"}]")]
    [DataRow("begin //x\n(* abc *) {def} end;", "[{\"code\":\"begin\",\"Level\":1},{\"code\":\"//x\",\"type\":\"LComment\",\"Level\":1},{\"code\":\"(* abc *)\",\"type\":\"Comment\",\"Level\":1},{\"code\":\"{def}\",\"type\":\"Comment\",\"Level\":1},{\"code\":\"end\",\"Level\":1},{\"code\":\";\"}]")]
    [DataRow("var x: integer; begin end;", "[{\"code\":\"var\"},{\"code\":\"x\"},{\"code\":\":\"},{\"code\":\"integer\"},{\"code\":\";\"},{\"code\":\"begin\",\"Level\":1},{\"code\":\"end\",\"Level\":1},{\"code\":\";\"}]")]
    [DataRow(" \t\r\n begin   end ;  ", "[{\"code\":\"begin\",\"Level\":1},{\"code\":\"end\",\"Level\":1},{\"code\":\";\"}]")]
    [DataRow("", "[]")]
    public void Tokenize_Scenarios_Json(string source, string? expectedJson)
    {
        var expected = DeserializeTokens(expectedJson);
        _testClass.OriginalCode = source;
        var tokens = _testClass.Tokenize().ToList();

        Assert.AreEqual(expected.Count, tokens.Count,
            $"Tokenanzahl stimmt nicht. Erwartet: {expected.Count}, Ist: {tokens.Count} für Source: '{source}'. Gefundene Codes: {string.Join(", ", tokens.Select(t => t.Code))}");

        for (int i = 0; i < expected.Count; i++)
        {
            var exp = expected[i];
            var tok = tokens[i];

            Assert.AreEqual(exp.Code.ToLowerInvariant(), tok.Code.ToLowerInvariant(),
                $"Code-Mismatch an Index {i}. Erwartet: '{exp.Code}' Ist: '{tok.Code}' Source: '{source}'.");

            if (exp.type != CodeBlockType.Unknown)
            {
                Assert.AreEqual(exp.type, tok.type,
                    $"Typ-Mismatch an Index {i}. Erwartet: '{exp.type}' Ist: '{tok.type}' für Token-Code '{tok.Code}' Source: '{source}'.");
            }
            if (exp.Level != -1)
            {
                Assert.AreEqual(exp.Level, tok.Level,
                    $"Typ-Mismatch an Index {i}. Erwartet: '{exp.Level}' Ist: '{tok.type}' für Token-Code '{tok.Code}' Source: '{source}'.");
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestTokenizeList))]
    public void Tokenize_Scenarios_Dyn(string name, string source, string? expectedJson)
    {
        var expected = DeserializeTokens(expectedJson);
        _testClass.OriginalCode = source;
        var tokens = _testClass.Tokenize().ToList();

        try
        {
        Assert.AreEqual(expected.Count, tokens.Count,
            $"Tokenanzahl stimmt nicht. Erwartet: {expected.Count}, Ist: {tokens.Count} für Source: '{source}'. Gefundene Codes: {string.Join(", ", tokens.Select(t => t.Code))}");
            for (int i = 0; i < expected.Count; i++)
            {
                var exp = expected[i];
                var tok = tokens[i];

                Assert.AreEqual(exp.Code.ToLowerInvariant(), tok.Code.ToLowerInvariant(),
                    $"Code-Mismatch an Index {i}. Erwartet: '{exp.Code}' Ist: '{tok.Code}' Source: '{source}'.");

                if (exp.type != CodeBlockType.Unknown)
                {
                    Assert.AreEqual(exp.type, tok.type,
                        $"Typ-Mismatch an Index {i}. Erwartet: '{exp.type}' Ist: '{tok.type}' für Token-Code '{tok.Code}' Source: '{source}'.");
                }
                if (exp.Level != -1)
                {
                    Assert.AreEqual(exp.Level, tok.Level,
                        $"Typ-Mismatch an Index {i}. Erwartet: '{exp.Level}' Ist: '{tok.type}' für Token-Code '{tok.Code}' Source: '{source}'.");
                }
            }
        }
        catch (AssertFailedException ex)
        {
            if (File.Exists(Path.Combine(".","Resources", name, name + "_actual.json")))
                File.Delete(Path.Combine(".","Resources", name, name + "_actual.json"));
            File.WriteAllText(Path.Combine(".","Resources", name, name + "_actual.json"), JsonSerializer.Serialize(tokens, _jsonOptions));
            throw;
        }
    }

    [TestMethod]
    public void Parse_SimpleBeginEnd()
    {
        _testClass.OriginalCode = "begin end;";
        var root = _testClass.Parse();
        var code = root.ToCode();
        Assert.IsTrue(code.ToLower().Contains("begin"));
        Assert.IsTrue(code.ToLower().Contains("end"));
    }

    [TestMethod]
    [DynamicData(nameof(TestCodeBuilderList))]
    public void Parse_CodeBlockList(string name, string tokens, string codeBlocks)
    {
        var tokenlist = DeserializeTokens(tokens);
        var codeBlockList = DeserializeCodeBlocks(codeBlocks);

        var result = _testClass.Parse(tokenlist);
        Assert.IsNotNull(result);
        try
        {
            Assert.AreEqual(codeBlockList.Count, result.SubBlocks.Count,
                $"CodeBlock-Anzahl stimmt nicht. Erwartet: {codeBlockList.Count}, Ist: {result.SubBlocks.Count} für Testcase: '{name}'.");
            for (int i = 0; i < codeBlockList.Count; i++)
            {
                var expectedBlock = codeBlockList[i];
                var actualBlock = result.SubBlocks[i] as PasCodeBlock;
                Assert.IsNotNull(actualBlock, $"CodeBlock an Index {i} ist null oder nicht vom Typ PasCodeBlock für Testcase: '{name}'.");
                Assert.AreEqual(expectedBlock.Name, actualBlock!.Name,
                    $"CodeBlock-Name stimmt nicht an Index {i}. Erwartet: '{expectedBlock.Name}', Ist: '{actualBlock.Name}' für Testcase: '{name}'.");
                Assert.AreEqual(expectedBlock.Type, actualBlock.Type,
                    $"CodeBlock-Type stimmt nicht an Index {i}. Erwartet: '{expectedBlock.Type}', Ist: '{actualBlock.Type}' für Testcase: '{name}'.");
                Assert.AreEqual(expectedBlock.Code, actualBlock.Code,
                    $"CodeBlock-Code stimmt nicht an Index {i}. Erwartet: '{expectedBlock.Code}', Ist: '{actualBlock.Code}' für Testcase: '{name}'.");
            }
        }
        catch (AssertFailedException ex)
        {
            if (File.Exists(Path.Combine(".","Resources", name, name + "_actual_codeblocks.json")))
                File.Delete(Path.Combine(".","Resources", name, name + "_actual_codeblocks.json"));
            File.WriteAllText(Path.Combine(".","Resources", name, name + "_actual_codeblocks.json"), JsonSerializer.Serialize(result, _jsonOptions));
            throw;
        }
    }

}
