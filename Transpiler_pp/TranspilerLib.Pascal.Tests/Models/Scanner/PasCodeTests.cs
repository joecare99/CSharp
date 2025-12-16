using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Pascal.Helper;

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
                    Path.Combine(dir, Path.GetFileName(dir)+ "_Source.pas") ,
                    File.Exists(Path.Combine(dir,Path.GetFileName(dir)+"_ExpectedTokens.json"))?Path.Combine(dir,Path.GetFileName(dir)+"_ExpectedTokens.json"):""
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
                    tokensPath,
                    File.Exists(codeBlocksPath) ? codeBlocksPath : string.Empty
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

    private static List<ICodeBlock> DeserializeCodeBlocks(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return [];
        try
        {
            var list = JsonSerializer.Deserialize<List<PasCodeBlock>>(json, _jsonOptions);
            return list?.Select(o=>o as ICodeBlock).ToList() ?? [];
        }
        catch (Exception ex)
        {
            Assert.Fail($"Fehler beim Deserialisieren der Expected-Tokenliste: {ex.Message}\nJSON:\n{json}");
            return [];
        }
    }


    [TestMethod]
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
            Assert.HasCount(expectedCount.Value, tokens, $"Tokenanzahl stimmt nicht für Source: '{source}'.");

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
    [DataRow("begin end;", 
        "[{\"code\":\"begin\",\"type\":\"Block\",\"Level\":1},{\"code\":\"end\",\"type\":\"Block\",\"Level\":1},{\"code\":\";\"}]")]
    [DataRow("begin begin end; end;", 
        "[{\"code\":\"begin\",\"Level\":1},{\"code\":\"begin\",\"Level\":2},{\"code\":\"end\",\"Level\":2},{\"code\":\";\",\"Level\":1},{\"code\":\"end\",\"Level\":1},{\"code\":\";\"}]")]
    [DataRow("begin 'a''b'; end;", 
        "[{\"code\":\"begin\",\"Level\":1},{\"code\":\"'a''b'\",\"type\":\"String\",\"Level\":1},{\"code\":\";\",\"Level\":1},{\"code\":\"end\",\"Level\":1},{\"code\":\";\"}]")]
    [DataRow("begin //x\n(* abc *) {def} end;", 
        "[{\"code\":\"begin\",\"Level\":1},{\"code\":\"//x\",\"type\":\"LComment\",\"Level\":1},{\"code\":\"(* abc *)\",\"type\":\"FLComment\",\"Level\":1},{\"code\":\"{def}\",\"type\":\"Comment\",\"Level\":1},{\"code\":\"end\",\"Level\":1},{\"code\":\";\"}]")]
    [DataRow("var x: integer; begin end;", 
        "[{\"code\":\"var\"},{\"code\":\"x\"},{\"code\":\":\"},{\"code\":\"integer\"},{\"code\":\";\"},{\"code\":\"begin\",\"Level\":1},{\"code\":\"end\",\"Level\":1},{\"code\":\";\"}]")]
    [DataRow(" \t\r\n begin   end ;  ", 
        "[{\"code\":\"begin\",\"Level\":1},{\"code\":\"end\",\"Level\":1},{\"code\":\";\"}]")]
    [DataRow("var a : array[0..5] of byte;", 
        "[{\"Code\":\"var\",\"type\":\"operation\",\"Level\":0,\"Pos\":0},{\"Code\":\"a\",\"type\":\"variable\",\"Level\":0,\"Pos\":4},{\"Code\":\":\",\"type\":\"operation\",\"Level\":0,\"Pos\":5},{\"Code\":\"array\",\"type\":\"variable\",\"Level\":0,\"Pos\":8},{\"Code\":\"[\",\"type\":\"operation\",\"Level\":0,\"Pos\":13},{\"Code\":\"0\",\"type\":\"number\",\"Level\":0,\"Pos\":14},{\"Code\":\"..\",\"type\":\"operation\",\"Level\":0,\"Pos\":15},{\"Code\":\"5\",\"type\":\"number\",\"Level\":0,\"Pos\":17},{\"Code\":\"]\",\"type\":\"operation\",\"Level\":0,\"Pos\":18},{\"Code\":\"of\",\"type\":\"operation\",\"Level\":0,\"Pos\":20},{\"Code\":\"byte\",\"type\":\"variable\",\"Level\":0,\"Pos\":23},{\"Code\":\";\",\"type\":\"separator\",\"Level\":0,\"Pos\":27}]")]
    [DataRow("var a : 0..5;", 
        "[{\"Code\":\"var\",\"type\":\"operation\",\"Level\":0,\"Pos\":0},{\"Code\":\"a\",\"type\":\"variable\",\"Level\":0,\"Pos\":4},{\"Code\":\":\",\"type\":\"operation\",\"Level\":0,\"Pos\":5},{\"Code\":\"0\",\"type\":\"number\",\"Level\":0,\"Pos\":8},{\"Code\":\"..\",\"type\":\"operation\",\"Level\":0,\"Pos\":9},{\"Code\":\"5\",\"type\":\"number\",\"Level\":0,\"Pos\":11},{\"Code\":\";\",\"type\":\"separator\",\"Level\":0,\"Pos\":12}]")]
    [DataRow("a[2]:=6;",
        "[{\"Code\":\"a\",\"type\":\"variable\",\"Level\":0,\"Pos\":0},{\"Code\":\"[\",\"type\":\"operation\",\"Level\":0,\"Pos\":1},{\"Code\":\"2\",\"type\":\"number\",\"Level\":0,\"Pos\":2},{\"Code\":\"]\",\"type\":\"operation\",\"Level\":0,\"Pos\":3},{\"Code\":\":=\",\"type\":\"assignment\",\"Level\":0,\"Pos\":4},{\"Code\":\"6\",\"type\":\"number\",\"Level\":0,\"Pos\":6},{\"Code\":\";\",\"type\":\"separator\",\"Level\":0,\"Pos\":7}]")]
    [DataRow("a.b:=2;", 
        "[{\"Code\":\"a\",\"type\":\"variable\",\"Level\":0,\"Pos\":0},{\"Code\":\".\",\"type\":\"operation\",\"Level\":0,\"Pos\":1},{\"Code\":\"b\",\"type\":\"variable\",\"Level\":0,\"Pos\":2},{\"Code\":\":=\",\"type\":\"assignment\",\"Level\":0,\"Pos\":3},{\"Code\":\"2\",\"type\":\"number\",\"Level\":0,\"Pos\":5},{\"Code\":\";\",\"type\":\"separator\",\"Level\":0,\"Pos\":6}]")]
    [DataRow("", "[]")]
    public void Tokenize_Scenarios_Json(string source, string? expectedJson)
    {
        var expected = DeserializeTokens(expectedJson);
        _testClass.OriginalCode = source;
        var tokens = _testClass.Tokenize().ToList();
        var found = JsonSerializer.Serialize(tokens, _jsonOptions);
        Assert.HasCount(expected.Count, tokens,
            $"Tokenanzahl stimmt nicht. Erwartet: {expected.Count}, Ist: {tokens.Count} für Source: '{source}'. Gefundene Codes: {found}");

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
    public void Tokenize_Scenarios_Dyn(string name, string sourceFile, string? expectedJsonFile)
    {
        var source = File.ReadAllText(sourceFile);
        var expectedJson = expectedJsonFile != null ? File.ReadAllText(expectedJsonFile) : null;

        var expected = DeserializeTokens(expectedJson);
        _testClass.OriginalCode = source;
        var tokens = _testClass.Tokenize().ToList();

        try
        {
        Assert.HasCount(expected.Count, tokens,
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
        catch (AssertFailedException)
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
        Assert.Contains("begin",code.ToLower());
        Assert.Contains("end",code.ToLower());
    }

    [TestMethod]
    [DataRow("",
        "[]",
        "[{\"Name\":\"PascalRoot\",\"Code\":\"\",\"Type\":\"MainBlock\",\"SubBlocks\":[]}]")]
    [DataRow("a[2]:=6;",
        "[{\"Code\":\"a\",\"type\":\"variable\",\"Level\":0,\"Pos\":0},{\"Code\":\"[\",\"type\":\"operation\",\"Level\":0,\"Pos\":1},{\"Code\":\"2\",\"type\":\"number\",\"Level\":0,\"Pos\":2},{\"Code\":\"]\",\"type\":\"operation\",\"Level\":0,\"Pos\":3},{\"Code\":\":=\",\"type\":\"assignment\",\"Level\":0,\"Pos\":4},{\"Code\":\"6\",\"type\":\"number\",\"Level\":0,\"Pos\":6},{\"Code\":\";\",\"type\":\"separator\",\"Level\":0,\"Pos\":7}]",
        "[{\"Name\":\"PascalRoot\",\"Code\":\"\",\"Type\":\"MainBlock\",\"SubBlocks\":[{\"Name\":\"Assignment\",\"Code\":\":=\",\"Type\":\"Assignment\",\"SubBlocks\":[{\"Name\":\"Operation\",\"Code\":\"[\",\"Type\":\"Operation\",\"SubBlocks\":[{\"Name\":\"Variable\",\"Code\":\"a\",\"Type\":\"Variable\",\"SubBlocks\":[]},{\"Name\":\"Number\",\"Code\":\"2\",\"Type\":\"Number\",\"SubBlocks\":[]}]},{\"Name\":\"Number\",\"Code\":\"6\",\"Type\":\"Number\",\"SubBlocks\":[]}]}]}]")]
    [DataRow("a.b:=2;",
        "[{\"Code\":\"a\",\"type\":\"variable\",\"Level\":0,\"Pos\":0},{\"Code\":\".\",\"type\":\"operation\",\"Level\":0,\"Pos\":1},{\"Code\":\"b\",\"type\":\"variable\",\"Level\":0,\"Pos\":2},{\"Code\":\":=\",\"type\":\"assignment\",\"Level\":0,\"Pos\":3},{\"Code\":\"2\",\"type\":\"number\",\"Level\":0,\"Pos\":5},{\"Code\":\";\",\"type\":\"separator\",\"Level\":0,\"Pos\":6}]",
        "[{\"Name\":\"PascalRoot\",\"Code\":\"\",\"Type\":\"MainBlock\",\"SubBlocks\":[{\"Name\":\"Assignment\",\"Code\":\":=\",\"Type\":\"Assignment\",\"SubBlocks\":[{\"Name\":\"Operation\",\"Code\":\".\",\"Type\":\"Operation\",\"SubBlocks\":[{\"Name\":\"Variable\",\"Code\":\"a\",\"Type\":\"Variable\",\"SubBlocks\":[]},{\"Name\":\"Variable\",\"Code\":\"b\",\"Type\":\"Variable\",\"SubBlocks\":[]}]},{\"Name\":\"Number\",\"Code\":\"2\",\"Type\":\"Number\",\"SubBlocks\":[]}]}]}]")]
    [DataRow("begin end;",
        "[{\"code\":\"begin\",\"type\":\"Block\",\"Level\":1},{\"code\":\"end\",\"type\":\"Block\",\"Level\":1},{\"code\":\";\",\"type\":\"Operation\"}]",
        "[{\"Name\":\"PascalRoot\",\"Code\":\"\",\"Type\":\"MainBlock\",\"SubBlocks\":[{\"Name\":\"Body\",\"Code\":\"begin\",\"Type\":\"Block\",\"SubBlocks\":[]}]}]")]
    [DataRow("begin begin end; end;",
        "[{\"code\":\"begin\",\"Level\":1,\"type\":\"Block\"},{\"code\":\"begin\",\"Level\":2,\"type\":\"Block\"},{\"code\":\"end\",\"Level\":2,\"type\":\"Block\"},{\"code\":\";\",\"Level\":1,\"type\":\"Operation\"},{\"code\":\"end\",\"Level\":1,\"type\":\"Block\"},{\"code\":\";\",\"type\":\"Operation\"}]",
        "[{\"Name\":\"PascalRoot\",\"Code\":\"\",\"Type\":\"MainBlock\",\"SubBlocks\":[{\"Name\":\"Body\",\"Code\":\"begin\",\"Type\":\"Block\",\"SubBlocks\":[{\"Name\":\"Block\",\"Code\":\"begin\",\"Type\":\"Block\",\"SubBlocks\":[]}]}]}]")]
    [DataRow("begin 'a''b'; end;",
        "[{\"code\":\"begin\",\"Level\":1},{\"code\":\"'a''b'\",\"type\":\"String\",\"Level\":1},{\"code\":\";\",\"Level\":1},{\"code\":\"end\",\"Level\":1},{\"code\":\";\"}]",
        "[]")]
    [DataRow("begin //x\n(* abc *) {def} end;",
        "[{\"code\":\"begin\",\"Level\":1,\"type\":\"Block\"},{\"code\":\"//x\",\"type\":\"LComment\",\"Level\":1},{\"code\":\"(* abc *)\",\"type\":\"FLComment\",\"Level\":1},{\"code\":\"{def}\",\"type\":\"Comment\",\"Level\":1},{\"code\":\"end\",\"Level\":1,\"type\":\"Block\"},{\"code\":\";\",\"type\":\"Operation\"}]",
        "[{\"Name\":\"PascalRoot\",\"Code\":\"\",\"Type\":\"MainBlock\",\"SubBlocks\":[{\"Name\":\"Body\",\"Code\":\"begin\",\"Type\":\"Block\",\"SubBlocks\":[{\"Name\":\"LComment\",\"Code\":\"//x\",\"Type\":\"LComment\",\"SubBlocks\":[]},{\"Name\":\"FLComment\",\"Code\":\"(* abc *)\",\"Type\":\"FLComment\",\"SubBlocks\":[]},{\"Name\":\"Comment\",\"Code\":\"{def}\",\"Type\":\"Comment\",\"SubBlocks\":[]}]}]}]")]
    [DataRow(" \t\r\n begin   end ;  ",
        "[{\"code\":\"begin\",\"Level\":1,\"type\":\"Block\"},{\"code\":\"end\",\"Level\":1,\"type\":\"Block\"},{\"code\":\";\",\"type\":\"Operation\"}]",
        "[{\"Name\":\"PascalRoot\",\"Code\":\"\",\"Type\":\"MainBlock\",\"SubBlocks\":[{\"Name\":\"Body\",\"Code\":\"begin\",\"Type\":\"Block\",\"SubBlocks\":[]}]}]")]
    [DataRow("var a : array[0..5] of byte;",
        "[{\"Code\":\"var\",\"type\":\"operation\",\"Level\":0,\"Pos\":0},{\"Code\":\"a\",\"type\":\"variable\",\"Level\":0,\"Pos\":4},{\"Code\":\":\",\"type\":\"operation\",\"Level\":0,\"Pos\":5},{\"Code\":\"array\",\"type\":\"variable\",\"Level\":0,\"Pos\":8},{\"Code\":\"[\",\"type\":\"operation\",\"Level\":0,\"Pos\":13},{\"Code\":\"0\",\"type\":\"number\",\"Level\":0,\"Pos\":14},{\"Code\":\"..\",\"type\":\"operation\",\"Level\":0,\"Pos\":15},{\"Code\":\"5\",\"type\":\"number\",\"Level\":0,\"Pos\":17},{\"Code\":\"]\",\"type\":\"operation\",\"Level\":0,\"Pos\":18},{\"Code\":\"of\",\"type\":\"operation\",\"Level\":0,\"Pos\":20},{\"Code\":\"byte\",\"type\":\"variable\",\"Level\":0,\"Pos\":23},{\"Code\":\";\",\"type\":\"separator\",\"Level\":0,\"Pos\":27}]",
        "[]")]
    [DataRow("var a : 0..5;",
        "[{\"Code\":\"var\",\"type\":\"operation\",\"Level\":0,\"Pos\":0},{\"Code\":\"a\",\"type\":\"variable\",\"Level\":0,\"Pos\":4},{\"Code\":\":\",\"type\":\"operation\",\"Level\":0,\"Pos\":5},{\"Code\":\"0\",\"type\":\"number\",\"Level\":0,\"Pos\":8},{\"Code\":\"..\",\"type\":\"operation\",\"Level\":0,\"Pos\":9},{\"Code\":\"5\",\"type\":\"number\",\"Level\":0,\"Pos\":11},{\"Code\":\";\",\"type\":\"separator\",\"Level\":0,\"Pos\":12}]",
        "[{\"Name\":\"PascalRoot\",\"Code\":\"\",\"Type\":\"MainBlock\",\"SubBlocks\":[{\"Name\":\"Declaration\",\"Code\":\"var\",\"Type\":\"Declaration\",\"SubBlocks\":[{\"Name\":\"Operation\",\"Code\":\":\",\"Type\":\"Operation\",\"SubBlocks\":[{\"Name\":\"Variable\",\"Code\":\"a\",\"Type\":\"Variable\",\"SubBlocks\":[]},{\"Name\":\"Range\",\"Code\":\"..\",\"Type\":\"Operation\",\"SubBlocks\":[{\"Name\":\"Number\",\"Code\":\"0\",\"Type\":\"Number\",\"SubBlocks\":[]}]},{\"Name\":\"Number\",\"Code\":\"5\",\"Type\":\"Number\",\"SubBlocks\":[]}]}]}]}]")]
    [DataRow("var x: integer; begin end;",
        "[{\"Code\":\"var\",\"type\":\"operation\",\"Level\":0,\"Pos\":0},{\"Code\":\"x\",\"type\":\"variable\",\"Level\":0,\"Pos\":4},{\"Code\":\":\",\"type\":\"operation\",\"Level\":0,\"Pos\":5},{\"Code\":\"integer\",\"type\":\"variable\",\"Level\":0,\"Pos\":7},{\"Code\":\";\",\"type\":\"separator\",\"Level\":0,\"Pos\":14},{\"Code\":\"begin\",\"type\":\"block\",\"Level\":1,\"Pos\":16},{\"Code\":\"end\",\"type\":\"block\",\"Level\":1,\"Pos\":22},{\"Code\":\";\",\"type\":\"separator\",\"Level\":0,\"Pos\":25}]",
        "[{\"Name\":\"PascalRoot\",\"Code\":\"\",\"Type\":\"MainBlock\",\"SubBlocks\":[{\"Name\":\"Declaration\",\"Code\":\"var\",\"Type\":\"Declaration\",\"SubBlocks\":[{\"Name\":\"Operation\",\"Code\":\":\",\"Type\":\"Operation\",\"SubBlocks\":[{\"Name\":\"Variable\",\"Code\":\"x\",\"Type\":\"Variable\",\"SubBlocks\":[]},{\"Name\":\"Type\",\"Code\":\"integer\",\"Type\":\"Variable\",\"SubBlocks\":[]}]}]},{\"Name\":\"Body\",\"Code\":\"begin\",\"Type\":\"Block\",\"SubBlocks\":[]}]}]")]
    public void Parse_ShortJson(string pascode, string tokenj, string codeblockj)
    {
        var tokens = DeserializeTokens(tokenj);
        var codeBlocks = DeserializeCodeBlocks(codeblockj);

        var result = _testClass.Parse(tokens);
        var found = JsonSerializer.Serialize<IList<ICodeBlock>>([result], _jsonOptions);

        Assert.IsNotNull(result);
        try
        {
            CheckSubBlocks(codeBlocks, [result], "ShortJsonTest");
        }
        catch (AssertFailedException)
        {
            Debug.WriteLine($"Expected CodeBlocks: {found}");
            throw;
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestCodeBuilderList))]
    public void Parse_CodeBlockList(string name, string tokensFile, string codeBlocksFile)
    {
        var tokens = string.IsNullOrWhiteSpace(tokensFile) ? null : File.ReadAllText(tokensFile);
        var codeBlocks = string.IsNullOrWhiteSpace(codeBlocksFile) ? null : File.ReadAllText(codeBlocksFile);

        var tokenlist = DeserializeTokens(tokens);
        var codeBlockList = DeserializeCodeBlocks(codeBlocks);

        var result = _testClass.Parse(tokenlist);
        Assert.IsNotNull(result);
        try
        {
            if (codeBlockList.Count == 1 && codeBlockList[0].Type == CodeBlockType.MainBlock)
            {
                var expectedBlock = codeBlockList[0];
                var actualBlock = result as PasCodeBlock;
                Assert.IsNotNull(actualBlock, $"Result ist null oder nicht vom Typ PasCodeBlock für Testcase: '{name}'.");
                Assert.AreEqual(expectedBlock.Name, actualBlock!.Name,
                    $"CodeBlock-Name stimmt nicht. Erwartet: '{expectedBlock.Name}', Ist: '{actualBlock.Name}' für Testcase: '{name}'.");
                Assert.AreEqual(expectedBlock.Type, actualBlock.Type,
                    $"CodeBlock-Type stimmt nicht. Erwartet: '{expectedBlock.Type}', Ist: '{actualBlock.Type}' für Testcase: '{name}'.");
                Assert.AreEqual(expectedBlock.Code, actualBlock.Code,
                    $"CodeBlock-Code stimmt nicht. Erwartet: '{expectedBlock.Code}', Ist: '{actualBlock.Code}' für Testcase: '{name}'.");

                // Recursively check sub-blocks?
                // The original test only checked top-level.
                // But if we want to be thorough, we should check children.
                // But let's stick to what the test was trying to do (top level check).
                // Wait, the original test iterated codeBlockList and compared with result.SubBlocks.
                // If codeBlockList contained children, it would check children.
                // Since codeBlockList contains ROOT, it checked ROOT against CHILD.
                
                // If I fix it to check ROOT against ROOT, I should also check children if possible.
                // But the JSON contains subBlocks!
                // So I should probably implement a recursive check or at least check the first level of subBlocks.
                
                CheckSubBlocks(expectedBlock.SubBlocks, actualBlock.SubBlocks, name);
            }
            else
            {
                Assert.HasCount(codeBlockList.Count, result.SubBlocks,
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
                    
                    CheckSubBlocks(expectedBlock.SubBlocks, actualBlock.SubBlocks, name);
                }
            }
        }
        catch (AssertFailedException)
        {
            if (File.Exists(Path.Combine(".","Resources", name, name + "_actual_codeblocks.json")))
                File.Delete(Path.Combine(".","Resources", name, name + "_actual_codeblocks.json"));
            File.WriteAllText(Path.Combine(".","Resources", name, name + "_actual_codeblocks.json"), JsonSerializer.Serialize<IList<ICodeBlock>>([result], _jsonOptions));
            throw;
        }
    }

    private void CheckSubBlocks(IList<ICodeBlock> expected, IList<ICodeBlock> actual, string name)
    {
        if (expected.Count != actual.Count)
        {
            Console.WriteLine($"SubBlock count mismatch for {name}. Expected: {expected.Count}, Actual: {actual.Count}");
            Console.WriteLine("Actual blocks:");
            foreach(var b in actual) Console.WriteLine($" - {b.Type} {b.Code}");
        }
        Assert.HasCount(expected.Count, actual, $"SubBlock count mismatch for {name}");
        for(int i=0; i<expected.Count; i++)
        {
            var exp = expected[i] as PasCodeBlock;
            var act = actual[i] as PasCodeBlock;
            Assert.IsNotNull(exp, "Expected block is not PasCodeBlock");
            Assert.IsNotNull(act, "Actual block is not PasCodeBlock");
            Assert.AreEqual(exp.Name, act.Name, $"Name mismatch at index {i} in {name}");
            Assert.AreEqual(exp.Type, act.Type, $"Type mismatch at index {i} in {name}");
            Assert.AreEqual(exp.Code, act.Code, $"Code mismatch at index {i} in {name}");
            CheckSubBlocks(exp.SubBlocks, act.SubBlocks, name);
        }
    }

}
