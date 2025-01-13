using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using TranspilerLib.Data;
using TranspilerLib.Models.Tests;
using TranspilerLibTests.TestData;
using static TranspilerLib.Helper.TestHelper;

#pragma warning disable IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
namespace TranspilerLib.Models.Scanner.Tests
#pragma warning restore IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
{

    [TestClass()]
    public class CSCodeTests : TestBase
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CSCode _testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        public static IEnumerable<object[]> TestTokenizeList => new object[][]
        {
            [ "0", new[] { TestCSDataClass.testData0 },
                    TestCSDataClass.TestDataList0() ],
            [ "1", new[] { TestCSDataClass.test1Data },
                    TestCSDataClass.TestDataList1()! ],
            [ "2", new[] { TestCSDataClass.test2Data },
                    TestCSDataClass.TestDataList2()! ],
            [ "3", new[] { TestCSDataClass.testData3 },
                    TestCSDataClass.TestDataList3() ],
            [ "4", new[] { TestCSDataClass.testData4 },
                    TestCSDataClass.TestDataList4() ],
            [ "5", new[] { TestCSDataClass.testData5 },
                    TestCSDataClass.TestDataList5() ],
            [ "6", new[] { TestCSDataClass.testData6 },
                    TestCSDataClass.TestDataList6() ],
            [ "7", new[] { TestCSDataClass.testData7 },
                    TestCSDataClass.TestDataList7() ],
            [ "8", new[] { TestCSDataClass.test8Data },
                    TestCSDataClass.TestDataList8()! ],
            [ "9", new[] { TestCSDataClass.test9Data },
                    TestCSDataClass.TestDataList9()! ],
            [ "10", new[] { TestCSDataClass.test10Data },
                    TestCSDataClass.TestDataList10()! ],
            [ "11", new[] { TestCSDataClass.test11Data },
                    TestCSDataClass.TestDataList11()! ],
            [ "12", new[] { TestCSDataClass.test12Data },
                    TestCSDataClass.TestDataList12()! ],
            [ "13", new[] { TestCSDataClass.test13Data },
                    TestCSDataClass.TestDataList13()! ],

        };

        public static IEnumerable<object[]> TestListParse => new object[][]
 {
            ["0", TestCSDataClass.TestDataList0(), new[] { TestCSDataClass.testDataExp0 }],
            ["1", TestCSDataClass.TestDataList1()!, new[] { TestCSDataClass.testDataExp1 }],
            ["2", TestCSDataClass.TestDataList2()!, new[] { TestCSDataClass.testDataExp2 }],
            ["3", TestCSDataClass.TestDataList3(), new[] { TestCSDataClass.testDataExp3 }],
            ["4", TestCSDataClass.TestDataList4(), new[] { TestCSDataClass.testDataExp4 }],
            ["5", TestCSDataClass.TestDataList5(), new[] { TestCSDataClass.testDataExp5 }],
            ["6", TestCSDataClass.TestDataList6(), new[] { TestCSDataClass.testDataExp6 }],
            ["7", TestCSDataClass.TestDataList7(), new[] { TestCSDataClass.testDataExp7 }],
       };
        public static IEnumerable<object[]> ReorderListData => new object[][]
{
            ["0", TestCSDataClass.TestDataList0(), new[] { TestCSDataClass.testDataExp0 }],
            ["1", TestCSDataClass.TestDataList1(), new[] { TestCSDataClass.testDataExpReorder1 }],
            ["2", TestCSDataClass.TestDataList2(), new[] { TestCSDataClass.testDataExpReorder2 }],
            ["3", TestCSDataClass.TestDataList3(), new[] { TestCSDataClass.testDataExpReorder3 }],
            ["4", TestCSDataClass.TestDataList4(), new[] { TestCSDataClass.testDataExpReorder4 }],
            ["7", TestCSDataClass.TestDataList7(), new[] { TestCSDataClass.testDataExp7 }],
            ["8", TestCSDataClass.TestDataList8(), new[] { TestCSDataClass.test8DataExp }],
            ["9", TestCSDataClass.TestDataList9(), new[] { TestCSDataClass.testDataExpReorder9 }],
};
        public static IEnumerable<object[]> RemoveLabelsData => new object[][]
{
            ["0", TestCSDataClass.TestDataList0(), new[] { TestCSDataClass.testDataExpRemoveL0 }],
            ["1", TestCSDataClass.TestDataList1(), new[] { TestCSDataClass.testDataExpRemoveL1 }],
            ["2", TestCSDataClass.TestDataList2(), new[] { TestCSDataClass.testDataExpRemoveL2 }],
            ["3", TestCSDataClass.TestDataList3(), new[] { TestCSDataClass.testDataExpRemoveL3 }],
            ["4", TestCSDataClass.TestDataList4(), new[] { TestCSDataClass.testDataExpRemoveL4 }],
            ["7", TestCSDataClass.TestDataList7(), new[] { TestCSDataClass.testDataExpRemoveL7 }],
            ["9", TestCSDataClass.TestDataList9(), new[] { TestCSDataClass.testDataExpRemoveL9 }],
            ["12", TestCSDataClass.TestDataList12(), new[] { TestCSDataClass.testDataExpRemoveL12 }],
            ["13", TestCSDataClass.TestDataList13(), new[] { TestCSDataClass.testDataExpRemoveL13 }],
};

        public static IEnumerable<object[]> TestListParse2 => new object[][]
 {
            ["0", TestCSDataClass.TestDataList0(), new[] { TestCSDataClass.cExpCode0 }],
            ["1", TestCSDataClass.TestDataList1(), new[] { TestCSDataClass.cExpCode1 }],
            ["2", TestCSDataClass.TestDataList2(), new[] { TestCSDataClass.cExpCode2 }],
            ["3", TestCSDataClass.TestDataList3(), new[] { TestCSDataClass.cExpCode3 }],
            ["4", TestCSDataClass.TestDataList4(), new[] { TestCSDataClass.cExpCode4 }],
            ["5", TestCSDataClass.TestDataList5(), new[] { TestCSDataClass.cExpCode5 }],
            ["6", TestCSDataClass.TestDataList6(), new[] { TestCSDataClass.cExpCode6 }],
            ["7", TestCSDataClass.TestDataList7(), new[] { TestCSDataClass.cExpCode7 }],
            ["8", TestCSDataClass.TestDataList8(), new[] { TestCSDataClass.cExpCode8 }],
            ["9", TestCSDataClass.TestDataList9(), new[] { TestCSDataClass.cExpCode9 }],
 };

        [TestInitialize]
        public void TestInitialize()
        {
            _testClass = new();
        }

        [TestMethod()]
        public void SetUpTest()
        {
            Assert.IsNotNull(_testClass);
            /*
            using var ms = new FileStream(nameof(TestDataClass.Test1DataList)+".json",FileMode.OpenOrCreate);
            ms.SetLength(0);
            ms.Position = 0;
            var l = TestDataClass.Test1DataList();
            // Serialize l as a JSON array.
            DataContractJsonSerializer ser = new(typeof(List<(string, ICSCode.CodeBlockType, int)>));
            ser.WriteObject(ms, l);
            ms.Close();
            */
        }

        public static IEnumerable<object[]> TokenizeTestData => new object[][]
        {
            ["1", new[] { TestCSDataClass.test1Data, TestCSDataClass.cExp1Log }],
            ["2", new[] { TestCSDataClass.test2Data, TestCSDataClass.cExp2Log }],
            ["8", new[] { TestCSDataClass.test8Data, TestCSDataClass.cExp8Log }],
            ["9", new[] { TestCSDataClass.test9Data, TestCSDataClass.cExp9Log }],
            ["10", new[] { TestCSDataClass.test10Data, TestCSDataClass.cExp10Log }],
            ["11", new[] { TestCSDataClass.test11Data, TestCSDataClass.cExp11Log }],
            ["12", new[] { TestCSDataClass.test12Data, TestCSDataClass.cExp12Log }],
            ["13", new[] { TestCSDataClass.test13Data, TestCSDataClass.cExp13Log }],
        };

        [DataTestMethod()]
        [DataRow("0", new[] { TestCSDataClass.testData0, TestCSDataClass.cExpLog0 }, DisplayName = "0")]
        //  [DataRow("2", new[] { TestDataClass.test2Data, TestDataClass.cExp2Log }, DisplayName = "2")]
        [DataRow("3", new[] { TestCSDataClass.testData3, TestCSDataClass.cExpLog3 }, DisplayName = "3")]
        [DataRow("4", new[] { TestCSDataClass.testData4, TestCSDataClass.cExpLog4 }, DisplayName = "4")]
        [DataRow("5", new[] { TestCSDataClass.testData5, TestCSDataClass.cExpLog5 }, DisplayName = "5")]
        [DataRow("6", new[] { TestCSDataClass.testData6, TestCSDataClass.cExpLog6 }, DisplayName = "6")]
        [DataRow("7", new[] { TestCSDataClass.testData7, TestCSDataClass.cExpLog7 }, DisplayName = "7")]
        [DynamicData(nameof(TokenizeTestData))]
        public void TokenizeTest(string _, string[] data)
        {
            _testClass.OriginalCode = data[0];
            _testClass.Tokenize((t) => DoLog($"T:{t.type},{t.Level},{t.Code}"));
            AssertAreEqual(data[1], DebugLog);
        }

        public static IEnumerable<object[]> ParseTestData => new object[][]
                {
            ["1",new[] { TestCSDataClass.test1Data, TestCSDataClass.testDataExp1 }],
            ["2",new[] { TestCSDataClass.test2Data, TestCSDataClass.testDataExp2 }],
            ["8",new[] { TestCSDataClass.test8Data, TestCSDataClass.test8DataExp }],
            ["9",new[] { TestCSDataClass.test9Data, TestCSDataClass.test9DataExp }],
            ["10",new[] { TestCSDataClass.test10Data, TestCSDataClass.test10DataExp }],
            ["11",new[] { TestCSDataClass.test11Data, TestCSDataClass.test11DataExp }],
            ["12",new[] { TestCSDataClass.test12Data, TestCSDataClass.test12DataExp }],
            ["13",new[] { TestCSDataClass.test13Data, TestCSDataClass.test13DataExp }],
                };


        [DataTestMethod()]
        [DataRow("0", new[] { TestCSDataClass.testData0, TestCSDataClass.testDataExp0 }, DisplayName = "0")]
        //  [DataRow("2", new[] { TestDataClass.test2Data, TestDataClass.testDataExp2 }, DisplayName = "2")]
        [DataRow("3", new[] { TestCSDataClass.testData3, TestCSDataClass.testDataExp3 }, DisplayName = "3")]
        [DataRow("4", new[] { TestCSDataClass.testData4, TestCSDataClass.testDataExp4 }, DisplayName = "4")]
        [DataRow("5", new[] { TestCSDataClass.testData5, TestCSDataClass.testDataExp5 }, DisplayName = "5")]
        [DataRow("6", new[] { TestCSDataClass.testData6, TestCSDataClass.testDataExp6 }, DisplayName = "6")]
        [DataRow("7", new[] { TestCSDataClass.testData7, TestCSDataClass.testDataExp7 }, DisplayName = "7")]
        [DynamicData(nameof(ParseTestData))]
        public void ParseTest(string name, string[] data)
        {
            _testClass.OriginalCode = data[0];
            var act = _testClass.Parse();
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
            if (sAct != data[1])
            {
                DoLog(sAct);
            }
            AssertAreEqual(data[1], sAct);
        }
        public static IEnumerable<object[]> Parse2TestData => new object[][]
                {
            ["1", new[] { TestCSDataClass.test1Data, TestCSDataClass.cExpCode1 }],
            ["2", new[] { TestCSDataClass.test2Data, TestCSDataClass.cExpCode2 }],
            ["8", new[] { TestCSDataClass.test8Data, TestCSDataClass.cExpCode8 }],
            ["9", new[] { TestCSDataClass.test9Data, TestCSDataClass.cExpCode9 }],
            ["10", new[] { TestCSDataClass.test10Data, TestCSDataClass.cExpCode10 }],
            ["11", new[] { TestCSDataClass.test11Data, TestCSDataClass.cExpCode11 }],
            ["12", new[] { TestCSDataClass.test12Data, TestCSDataClass.cExpCode12 }],
            ["13", new[] { TestCSDataClass.test13Data, TestCSDataClass.cExpCode13 }],
                };

        [DataTestMethod()]
        [DataRow("0", new[] { TestCSDataClass.testData0, TestCSDataClass.cExpCode0 }, DisplayName = "0")]
        //[DataRow("1", new[] { TestDataClass.test1Data, TestDataClass.cExpCode1 }, DisplayName = "1")]
        //[DataRow("2", new[] { TestDataClass.test2Data, TestDataClass.cExpCode2 }, DisplayName = "2")]
        [DataRow("3", new[] { TestCSDataClass.testData3, TestCSDataClass.cExpCode3 }, DisplayName = "3")]
        [DataRow("4", new[] { TestCSDataClass.testData4, TestCSDataClass.cExpCode4 }, DisplayName = "4")]
        [DataRow("5", new[] { TestCSDataClass.testData5, TestCSDataClass.cExpCode5 }, DisplayName = "5")]
        [DataRow("6", new[] { TestCSDataClass.testData6, TestCSDataClass.cExpCode6 }, DisplayName = "6")]
        [DataRow("7", new[] { TestCSDataClass.testData7, TestCSDataClass.cExpCode7 }, DisplayName = "7")]
        [DynamicData(nameof(Parse2TestData))]
        public void Parse2Test(string _, string[] data)
        {
            _testClass.OriginalCode = data[0];
            var act = _testClass.Parse();
            var sAct = act.ToCode();
            if (sAct != data[1])
                DoLog(sAct);
            AssertAreEqual(data[1], sAct);
        }

        [DataTestMethod()]
        [DataRow([TestCSDataClass.testData0, TestCSDataClass.cExpLog0], DisplayName = "0")]
        [DataRow([TestCSDataClass.testData5, TestCSDataClass.cExpLog0], DisplayName = "0")]
        public void Tokenize0Test(string[] data)
        {
            _testClass.OriginalCode = data[0];
            _testClass.Tokenize(null);
            AssertAreEqual("", DebugLog);
        }


        [DataTestMethod()]
        [DynamicData(nameof(TestTokenizeList))]
        public void Tokenize2Test(string _, string[] data, List<TokenData> expList)
        {
            _testClass.OriginalCode = data[0];
            var list = new List<TokenData>();
            DoLog("new List<(string, ICSCode.CodeBlockType, int)>() {");
            _testClass.Tokenize((t) => list.Add(t));

            using var ms = new FileStream($"Test{_}DataList.json", FileMode.Create);
            new DataContractJsonSerializer(typeof(List<TokenData>)
                ).WriteObject(ms, list);
            ms.Close();
            if (expList.Count < 3)
                Assert.AreEqual("", DebugLog);
            for (var i = 0; i < Math.Min(expList.Count, list.Count); i++)
            {
                Assert.AreEqual(expList[i].type, list[i].type, $"Type({i})");
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

        [DataTestMethod()]
        [DynamicData(nameof(ReorderListData))]
        public void ReorderLabelsTest(string _, List<TokenData> actList, string[] data)
        {
            var act = _testClass.Parse(actList);
            _testClass.ReorderLabels(act);
            var sAct = act.ToString();
            if (sAct != data[0])
                DoLog(sAct);
            //   else
            //       DoLog(_testClass.ToCode(act,4));
            AssertAreEqual(data[0], sAct);
        }

        [DataTestMethod()]
        [DynamicData(nameof(RemoveLabelsData))]
        public void RemoveLabelsTest(string _, List<TokenData> actList, string[] data)
        {
            var act = _testClass.Parse(actList);
            _testClass.ReorderLabels(act);
            _testClass.RemoveSingleSourceLabels1(act);
            var sAct = act.ToString();
            if (sAct != data[0])
                DoLog(sAct);
            else
                DoLog(_testClass.ToCode(act, 4));
            AssertAreEqual(data[0], sAct);
        }


    }
}