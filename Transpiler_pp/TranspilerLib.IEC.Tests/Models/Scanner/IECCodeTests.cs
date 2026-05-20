using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;
using TranspilerLib.Data;
using TranspilerLib.IEC.TestData;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;
using TranspilerLib.Models.Tests;
using static TranspilerLib.Helper.TestHelper;

namespace TranspilerLib.IEC.Models.Scanner.Tests;

[TestClass]
public class IECCodeTests : TestBase
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private IECCode _testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    public static IEnumerable<object[]> TestTokenizeList =>
 [
        [ "0", new[] { IECTestDataClass.testData0 }, IECTestDataClass.TestDataList0() ],
        [ "1", new[] { IECTestDataClass.testData1 }, IECTestDataClass.TestDataList1() ],
        [ "2", new[] { IECTestDataClass.testData2 }, IECTestDataClass.TestDataList2() ],
  ];

    public static IEnumerable<object[]> TestListParse =>
[
            ["0", IECTestDataClass.TestDataList0(), new[] { Encoding.UTF8.GetBytes(IECTestDataClass.testDataExp0) }],
            ["1", IECTestDataClass.TestDataList1()!, new[] { Encoding.UTF8.GetBytes(IECTestDataClass.testDataExp1) }],
            ["2", IECTestDataClass.TestDataList2()!, new[] { Encoding.UTF8.GetBytes(IECTestDataClass.testDataExp2) }],
   ];
    public static IEnumerable<object[]> TestListParse2 =>
[
            ["0", IECTestDataClass.TestDataList0(), new[] { IECTestDataClass.cExpCode0 }],
            ["1", IECTestDataClass.TestDataList1(), new[] { IECTestDataClass.cExpCode1 }],
            ["2", IECTestDataClass.TestDataList2(), new[] { IECTestDataClass.cExpCode2 }],
];

    [TestInitialize]
    public void TestInitialize()
    {
        _testClass = new IECCode();
    }

    private static string NormalizeCodeSequence(string code)
    {
        var sb = new StringBuilder(code.Length);
        foreach (var ch in code)
        {
            if (ch != ' ' && ch != '\t' && ch != '\r' && ch != '\n')
            {
                sb.Append(ch);
            }
        }

        return sb.ToString();
    }

    private static ICodeBlock? FindDeclarationByLeftCode(ICodeBlock block, string leftCode)
    {
        if (block.Type == CodeBlockType.Declaration
            && block.SubBlocks.Count > 0
            && string.Equals(block.SubBlocks[0].Code, leftCode, StringComparison.Ordinal))
        {
            return block;
        }

        foreach (var subBlock in block.SubBlocks)
        {
            var found = FindDeclarationByLeftCode(subBlock, leftCode);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private static void AssertContainsInOrder(string actual, params string[] expectedParts)
    {
        var index = 0;
        foreach (var expectedPart in expectedParts)
        {
            var nextIndex = actual.IndexOf(expectedPart, index, StringComparison.Ordinal);
            Assert.IsTrue(nextIndex >= 0, $"Missing sequence part '{expectedPart}' in '{actual}'.");
            index = nextIndex + expectedPart.Length;
        }
    }
    public static IEnumerable<object[]> TokenizeTestData => new object[][]
{
           ["1", new[] { IECTestDataClass.testData1, IECTestDataClass.cExpLog1 }],
           ["2", new[] { IECTestDataClass.testData2, IECTestDataClass.cExpLog2 }],
};

    [TestMethod()]
    [DataRow("0", new[] { IECTestDataClass.testData0, IECTestDataClass.cExpLog0 }, DisplayName = "0")]
    [DynamicData(nameof(TokenizeTestData))]
    public void TokenizeTest(string _, string[] data)
    {
        _testClass.OriginalCode = data[0];
        _testClass.Tokenize((t) => DoLog($"T:{t.type},{t.Pos},{t.Level},{t.Code}"));
        AssertAreEqual(data[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("0", new[] { IECTestDataClass.testData0, IECTestDataClass.cExpLog0 }, DisplayName = "0")]
    [DynamicData(nameof(TokenizeTestData))]
    public void TokenizeTest2(string _, string[] data)
    {
        _testClass.OriginalCode = data[0];
        foreach (var item in _testClass.Tokenize())
        {
            DoLog($"T:{item.type},{item.Pos},{item.Level},{item.Code}");
        }
        AssertAreEqual(data[1], DebugLog);
    }

    [TestMethod()]
    [DynamicData(nameof(TestTokenizeList))]
    public void Tokenize2Test(string _, string[] data, List<TokenData> expList)
    {
        _testClass.OriginalCode = data[0];
        var list = new List<TokenData>();
        DoLog("new List<(string, ICSCode.CodeBlockType, int)>() {");
        _testClass.Tokenize((t) => list.Add(t));

        using var ms = new FileStream($"IECTestDataList{_}.json", FileMode.Create);
        new DataContractJsonSerializer(typeof(List<TokenData>)
            ).WriteObject(ms, list);
        ms.Close();
        if (expList.Count < 3)
            Assert.AreEqual("", DebugLog);
        for (var i = 0; i < Math.Min(expList.Count, list.Count); i++)
        {
            Assert.AreEqual(expList[i].type, list[i].type, $"Type({i})");
            Assert.AreEqual(expList[i].Pos, list[i].Pos, $"Pos({i})");
            Assert.AreEqual(expList[i].Code, list[i].Code, $"Code({i})");
            Assert.AreEqual(expList[i].Level, list[i].Level, $"Stack({i})");
        }
    }

    [TestMethod()]
    [DynamicData(nameof(TestListParse))]
    public void ParseEnumTest(string name, List<TokenData> actList, object data)
    {
        string sExp = "";
        if (data is string _sExp)
            sExp = _sExp;
        else if (data is byte[] _bExp)
            sExp = Encoding.UTF8.GetString(_bExp);
        
        var act = _testClass.Parse(actList);
        Assert.IsNotNull(act);
        var sAct = act.ToString();
        using var ms = new FileStream("Test" + name + "CodeBlock.json", FileMode.Create);
        new DataContractJsonSerializer(
            typeof(CodeBlock),
            new DataContractJsonSerializerSettings()
            {
                EmitTypeInformation = EmitTypeInformation.AsNeeded
            })
        .WriteObject(ms, act);
        ms.Close();
        if (sAct != sExp)
        {
            DoLog(sAct);
        }
        AssertAreEqual(sExp, sAct);
    }


    [TestMethod()]
    [DynamicData(nameof(TestListParse2))]
    public void ParseEnum2Test(string _, List<TokenData> actList, string[] data)
    {
        var act = _testClass.Parse(actList);
        var normalizedCode = NormalizeCodeSequence(act.ToCode());

        switch (_)
        {
            case "0":
                AssertContainsInOrder(
                    normalizedCode,
                    "TYPE",
                    "udtVector",
                    ":",
                    "UNION",
                    "v",
                    ":",
                    "udtVectorData",
                    ";",
                    "Koordinate",
                    ":",
                    "ARRAY[",
                    "0",
                    "..",
                    "1",
                    "]",
                    "OF",
                    "LREAL",
                    ";",
                    "END_UNION",
                    "END_TYPE");
                break;
            case "1":
                AssertContainsInOrder(
                    normalizedCode,
                    "//DiesisteinKommentarzumTypST_01",
                    "TYPE",
                    "ST_01",
                    ":",
                    "STRUCT",
                    "//DiesisteinKommentarzumElementa",
                    "a",
                    ":",
                    "INT",
                    ":=",
                    "INT#1",
                    ";",
                    "//Diesistein2.KommentarzumElementa",
                    "//DiesisteinKommentarzumElementb",
                    "b",
                    ":",
                    "LREAL",
                    ":=",
                    "LREAL#2.7e-2",
                    ";",
                    "//Diesistein2.KommentarzumElementb",
                    "END_STRUCT",
                    "END_TYPE");
                break;
            case "2":
                AssertContainsInOrder(
                    normalizedCode,
                    "FUNCTION",
                    "fbCalcWinkelvVector",
                    ":",
                    "LREAL",
                    ";",
                    "VAR_INPUT",
                    "Vec",
                    ":",
                    "udtVector",
                    ";",
                    "END_VAR",
                    "VAR_OUTPUT",
                    "Len",
                    ":",
                    "LREAL",
                    ";",
                    "END_VAR",
                    "VAR",
                    "ZwErg",
                    ":",
                    "LREAL",
                    ";",
                    "END_VAR",
                    "BEGIN",
                    "IF",
                    "THEN",
                    "ZwErg:=SEL(",
                     "ATAN(",
                     "len:=Vec.",
                     ":=v.",
                     ":=xCOS(",
                     "fbCalcWinkelvVector:=ZwErg",
                     "DINT_TO_REAL(",
                     "TRUNC(",
                    "ELSIF",
                     "THEN",
                    "fbCalcWinkelvVector:=SEL(",
                     "ATAN(-Vec.v.x/vec.v.y-)",
                     "len:=Vec.",
                     ":=v.",
                     ":=ySIN(",
                    "else",
                    "fbCalcWinkelvVector:=0.0",
                    "Len:=0.0",
                    "end_if",
                    ";",
                    "END_FUNCTION");
                break;
            default:
                Assert.AreEqual(NormalizeCodeSequence(data[0]), normalizedCode);
                break;
        }
    }
}
