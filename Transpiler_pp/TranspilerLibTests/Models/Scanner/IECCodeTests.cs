using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using TranspilerLib.Data;
using TranspilerLib.Models.Tests;
using TranspilerLibTests.TestData;
using static TranspilerLib.Helper.TestHelper;

namespace TranspilerLib.Models.Scanner.Tests;

[TestClass]
public class IECCodeTests : TestBase
{
    private IECCode _testClass;

    public static IEnumerable<object[]> TestTokenizeList =>
 [
        [ "0", new[] { IECTestDataClass.testData0 }, IECTestDataClass.TestDataList0() ],
        [ "1", new[] { IECTestDataClass.testData1 }, IECTestDataClass.TestDataList1() ],
        [ "2", new[] { IECTestDataClass.testData2 }, IECTestDataClass.TestDataList2() ],
  ];

    public static IEnumerable<object[]> TestListParse =>
[
            ["0", IECTestDataClass.TestDataList0(), new[] { IECTestDataClass.testDataExp0 }],
            ["1", IECTestDataClass.TestDataList1()!, new[] { IECTestDataClass.testDataExp1 }],
            ["2", IECTestDataClass.TestDataList2()!, new[] { IECTestDataClass.testDataExp2 }],
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
    public static IEnumerable<object[]> TokenizeTestData => new object[][]
{
           ["1", new[] { IECTestDataClass.testData1, IECTestDataClass.cExpLog1 }],
           ["2", new[] { IECTestDataClass.testData2, IECTestDataClass.cExpLog2 }],
};

    [DataTestMethod()]
    [DataRow("0", new[] { IECTestDataClass.testData0, IECTestDataClass.cExpLog0 }, DisplayName = "0")]
    [DynamicData(nameof(TokenizeTestData))]
    public void TokenizeTest(string _, string[] data)
    {
        _testClass.OriginalCode = data[0];
        _testClass.Tokenize((t) => DoLog($"T:{t.type},{t.Pos},{t.Level},{t.Code}"));
        AssertAreEqual(data[1], DebugLog);
    }

    [DataTestMethod()]
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

    [DataTestMethod()]
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

    [DataTestMethod()]
    [DynamicData(nameof(TestListParse))]
    public void ParseEnumTest(string _, List<TokenData> actList, string[] data)
    {
        var act = _testClass.Parse(actList);
        AssertAreEqual(data[0], act?.ToString().Replace("\"+", "\" +"));
    }


    [DataTestMethod()]
    [DynamicData(nameof(TestListParse2))]
    public void ParseEnum2Test(string _, List<TokenData> actList, string[] data)
    {
        var act = _testClass.Parse(actList);
        AssertAreEqual(data[0], act.ToCode());
    }
}
