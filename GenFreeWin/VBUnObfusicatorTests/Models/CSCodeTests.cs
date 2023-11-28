using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using VBUnObfusicatorTests.TestData;
using static VBUnObfusicator.Helper.TestHelper;

#pragma warning disable IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
namespace VBUnObfusicator.Models.Tests
#pragma warning restore IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
{
    [TestClass()]
    public class CCodeBlockTests : TestBase
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CSCode _testClass;

        public static IEnumerable<object[]> TestListMove => new object[][]
{
            new object[] { "0a", TestDataClass.TestDataList0(), (new int[] { }, 3, 3, 1), false, new[] { TestDataClass.testDataExp0 } },

            new object[] { "1a", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 5, 5, 1), false, new[] { TestDataClass.testDataExp1 } },
            new object[] { "1b", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, -1, 5, 1), false, new[] { TestDataClass.testDataExp1 } },
            new object[] { "1c", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 5, -1, 1), false, new[] { TestDataClass.testDataExp1 } },
            new object[] { "1d", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 131, 5, 1), false, new[] { TestDataClass.testDataExp1 } },
            new object[] { "1e", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 5, 131, 1), false, new[] { TestDataClass.testDataExp1 } },
            new object[] { "1f", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 125, 5, 6), false, new[] { TestDataClass.testDataExp1 } },
               new object[] { "1g0", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 6, 9, 3), true, new[] { TestDataClass.testDataExp1 } },
               new object[] { "1gd", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 6, 13, 3), true, new[] { TestDataClass.testDataMoveExp } },
               new object[] { "1gu", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 9, 6, 4), true, new[] { TestDataClass.testDataMoveExp } },

            new object[] { "2a", TestDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 5, 5, 1), false, new[] { TestDataClass.testDataExp2 } },
            new object[] { "2b", TestDataClass.TestDataList2(), (new[] { 7, 1, 3 }, -1, 5, 1), false, new[] { TestDataClass.testDataExp2 } },
            new object[] { "2c", TestDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 5, -1, 1), false, new[] { TestDataClass.testDataExp2 } },
            new object[] { "2d", TestDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 34, 5, 1), false, new[] { TestDataClass.testDataExp2 } },
            new object[] { "2e", TestDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 5, 34, 1), false, new[] { TestDataClass.testDataExp2 } },
            new object[] { "2f", TestDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 30, 5, 5), false, new[] { TestDataClass.testDataExp2 } },

         };

        public static IEnumerable<object[]> TestListDelete => new object[][]
 {
            new object[] { "0a", TestDataClass.TestDataList0(), (new int[] { }, 5, 9), false, new[] { TestDataClass.testDataExp0 } },

            new object[] { "1b", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, -1,  1), false, new[] { TestDataClass.testDataExp1 } },
            new object[] { "1d", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 131,  1), false, new[] { TestDataClass.testDataExp1 } },
            new object[] { "1f", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 125, 6), false, new[] { TestDataClass.testDataExp1 } },
               new object[] { "1gd", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 6, 3), true, new[] { TestDataClass.testDataDeleteExp } },
               new object[] { "1gu", TestDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 9, 4), true, new[] { TestDataClass.testDataDelete2Exp } },

            new object[] { "2b", TestDataClass.TestDataList2(), (new[] { 7, 1, 3 }, -1,  1), false, new[] { TestDataClass.testDataExp2 } },
            new object[] { "2d", TestDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 34,  1), false, new[] { TestDataClass.testDataExp2 } },
            new object[] { "2f", TestDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 30,  5), false, new[] { TestDataClass.testDataExp2 } },

          };
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        [TestInitialize]
        public void TestInitialize()
        {
            _testClass = new();
        }

        [TestMethod()]
        public void SetUpTest()
        {
            Assert.IsNotNull(_testClass);
        }

        [DataTestMethod()]
        [DynamicData(nameof(TestListMove))]
        public void MoveBlocksTest(string _, List<TokenData> actList, (int[], int, int, int) movedata, bool xSucc, string[] data)
        {
            var act = _testClass.Parse(actList);
            var act2 = act;
            foreach (var s in movedata.Item1)
                act2 = act2.SubBlocks[s];
            Assert.AreEqual(xSucc, act2.MoveSubBlocks(movedata.Item2, movedata.Item3, movedata.Item4));
            var sAct = act.ToString();
            if (sAct != data[0])
                DoLog(sAct);
            AssertAreEqual(data[0], sAct);
        }

        [DataTestMethod()]
        [DynamicData(nameof(TestListMove))]
        public void MoveBlocksTest2(string _, List<TokenData> actList, (int[], int, int, int) movedata, bool xSucc, string[] data)
        {
            var act = _testClass.Parse(actList);
            var act2 = act;
            foreach (var s in movedata.Item1)
                act2 = act2.SubBlocks[s];
            var c = movedata.Item3 is >= 0 && movedata.Item3 < act2.SubBlocks.Count ? act2.SubBlocks[movedata.Item3] : null;
            Assert.AreEqual(xSucc, act2.MoveSubBlocks(movedata.Item2, c!, movedata.Item4));
            var sAct = act.ToString();
            if (sAct != data[0])
                DoLog(sAct);
            AssertAreEqual(data[0], sAct);
        }

        [DataTestMethod()]
        [DynamicData(nameof(TestListDelete))]
        public void DeleteBlocksTest(string _, List<TokenData> actList, (int[], int, int) deldata, bool xSucc, string[] data)
        {
            var act = _testClass.Parse(actList);
            var act2 = act;
            foreach (var s in deldata.Item1)
                act2 = act2.SubBlocks[s];
            Assert.AreEqual(xSucc, act2.DeleteSubBlocks(deldata.Item2, deldata.Item3));
            var sAct = act.ToString();
            if (sAct != data[0])
                DoLog(sAct);
            AssertAreEqual(data[0], sAct);
        }

    }

    [TestClass()]
    public class CSCodeTests : TestBase
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CSCode _testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        public static IEnumerable<object[]> TestTokenizeList => new object[][]
        {
            new object[] { "0", new[] { TestDataClass.testData0 },
                    TestDataClass.TestDataList0() },
            new object[] { "1", new[] { TestDataClass.test1Data },
                    TestDataClass.TestDataList1()! },
            new object[] { "2", new[] { TestDataClass.test2Data },
                    TestDataClass.TestDataList2()! },
            new object[] { "3", new[] { TestDataClass.testData3 },
                    TestDataClass.TestDataList3() },
            new object[] { "4", new[] { TestDataClass.testData4 },
                    TestDataClass.TestDataList4() },
            new object[] { "5", new[] { TestDataClass.testData5 },
                    TestDataClass.TestDataList5() },
            new object[] { "6", new[] { TestDataClass.testData6 },
                    TestDataClass.TestDataList6() },
            new object[] { "7", new[] { TestDataClass.testData7 },
                    TestDataClass.TestDataList7() },
            new object[] { "8", new[] { TestDataClass.test8Data },
                    TestDataClass.TestDataList8() },
            new object[] { "9", new[] { TestDataClass.test9Data },
                    TestDataClass.TestDataList9()! },
            new object[] { "10", new[] { TestDataClass.test10Data },
                    TestDataClass.TestDataList10()! },
            new object[] { "11", new[] { TestDataClass.test11Data },
                    TestDataClass.TestDataList11()! },

        };

        public static IEnumerable<object[]> TestListParse => new object[][]
 {
            new object[] { "0", TestDataClass.TestDataList0(), new[] { TestDataClass.testDataExp0 } },
            new object[] { "1", TestDataClass.TestDataList1(), new[] { TestDataClass.testDataExp1 } },
            new object[] { "2", TestDataClass.TestDataList2(), new[] { TestDataClass.testDataExp2 } },
            new object[] { "3", TestDataClass.TestDataList3(), new[] { TestDataClass.testDataExp3 } },
            new object[] { "4", TestDataClass.TestDataList4(), new[] { TestDataClass.testDataExp4 } },
            new object[] { "5", TestDataClass.TestDataList5(), new[] { TestDataClass.testDataExp5 } },
            new object[] { "6", TestDataClass.TestDataList6(), new[] { TestDataClass.testDataExp6 } },
            new object[] { "7", TestDataClass.TestDataList7(), new[] { TestDataClass.testDataExp7 } },
       };
        public static IEnumerable<object[]> ReorderListData => new object[][]
{
            new object[] { "0", TestDataClass.TestDataList0(), new[] { TestDataClass.testDataExp0 } },
            new object[] { "1", TestDataClass.TestDataList1(), new[] { TestDataClass.testDataExpReorder1 } },
            new object[] { "2", TestDataClass.TestDataList2(), new[] { TestDataClass.testDataExpReorder2 } },
            new object[] { "3", TestDataClass.TestDataList3(), new[] { TestDataClass.testDataExpReorder3 } },
            new object[] { "4", TestDataClass.TestDataList4(), new[] { TestDataClass.testDataExpReorder4 } },
            new object[] { "7", TestDataClass.TestDataList7(), new[] { TestDataClass.testDataExp7 } },
            new object[] { "8", TestDataClass.TestDataList8(), new[] { TestDataClass.test8DataExp } },
            new object[] { "9", TestDataClass.TestDataList9(), new[] { TestDataClass.testDataExpReorder9 } },
};
        public static IEnumerable<object[]> RemoveLabelsData => new object[][]
{
            new object[] { "0", TestDataClass.TestDataList0(), new[] { TestDataClass.testDataExpRemoveL0 } },
            new object[] { "1", TestDataClass.TestDataList1(), new[] { TestDataClass.testDataExpRemoveL1 } },
            new object[] { "2", TestDataClass.TestDataList2(), new[] { TestDataClass.testDataExpRemoveL2 } },
            new object[] { "3", TestDataClass.TestDataList3(), new[] { TestDataClass.testDataExpRemoveL3 } },
            new object[] { "4", TestDataClass.TestDataList4(), new[] { TestDataClass.testDataExpRemoveL4 } },
            new object[] { "7", TestDataClass.TestDataList7(), new[] { TestDataClass.testDataExpRemoveL7 } },
            new object[] { "9", TestDataClass.TestDataList9(), new[] { TestDataClass.testDataExpRemoveL9 } },
};

        public static IEnumerable<object[]> TestListParse2 => new object[][]
 {
            new object[] { "0", TestDataClass.TestDataList0(), new[] { TestDataClass.cExpCode0 } },
            new object[] { "1", TestDataClass.TestDataList1(), new[] { TestDataClass.cExpCode1 } },
            new object[] { "2", TestDataClass.TestDataList2(), new[] { TestDataClass.cExpCode2 } },
            new object[] { "3", TestDataClass.TestDataList3(), new[] { TestDataClass.cExpCode3 } },
            new object[] { "4", TestDataClass.TestDataList4(), new[] { TestDataClass.cExpCode4 } },
            new object[] { "5", TestDataClass.TestDataList5(), new[] { TestDataClass.cExpCode5 } },
            new object[] { "6", TestDataClass.TestDataList6(), new[] { TestDataClass.cExpCode6 } },
            new object[] { "7", TestDataClass.TestDataList7(), new[] { TestDataClass.cExpCode7 } },
            new object[] { "8", TestDataClass.TestDataList8(), new[] { TestDataClass.cExpCode8 } },
            new object[] { "9", TestDataClass.TestDataList9(), new[] { TestDataClass.cExpCode9 } },
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
            new object[] {"1", new[] { TestDataClass.test1Data, TestDataClass.cExp1Log } },
            new object[] {"2", new[] { TestDataClass.test2Data, TestDataClass.cExp2Log } },
            new object[] {"8", new[] { TestDataClass.test8Data, TestDataClass.cExp8Log } },
            new object[] {"9", new[] { TestDataClass.test9Data, TestDataClass.cExp9Log } },
            new object[] {"10", new[] { TestDataClass.test10Data, TestDataClass.cExp10Log } },
            new object[] {"11", new[] { TestDataClass.test11Data, TestDataClass.cExp11Log } },
        };

        [DataTestMethod()]
        [DataRow("0", new[] { TestDataClass.testData0, TestDataClass.cExpLog0 }, DisplayName = "0")]
        //  [DataRow("2", new[] { TestDataClass.test2Data, TestDataClass.cExp2Log }, DisplayName = "2")]
        [DataRow("3", new[] { TestDataClass.testData3, TestDataClass.cExpLog3 }, DisplayName = "3")]
        [DataRow("4", new[] { TestDataClass.testData4, TestDataClass.cExpLog4 }, DisplayName = "4")]
        [DataRow("5", new[] { TestDataClass.testData5, TestDataClass.cExpLog5 }, DisplayName = "5")]
        [DataRow("6", new[] { TestDataClass.testData6, TestDataClass.cExpLog6 }, DisplayName = "6")]
        [DataRow("7", new[] { TestDataClass.testData7, TestDataClass.cExpLog7 }, DisplayName = "7")]
        [DynamicData(nameof(TokenizeTestData))]
        public void TokenizeTest(string _, string[] data)
        {
            _testClass.OriginalCode = data[0];
            _testClass.Tokenize((t) => DoLog($"T:{t.type},{t.Level},{t.Code}"));
            AssertAreEqual(data[1], DebugLog);
        }

        public static IEnumerable<object[]> ParseTestData => new object[][]
                {
            new object[] {"1",new[] { TestDataClass.test1Data, TestDataClass.testDataExp1 } },
            new object[] {"2",new[] { TestDataClass.test2Data, TestDataClass.testDataExp2 } },
            new object[] {"8",new[] { TestDataClass.test8Data, TestDataClass.test8DataExp } },
            new object[] {"9",new[] { TestDataClass.test9Data, TestDataClass.test9DataExp } },
            new object[] {"10",new[] { TestDataClass.test10Data, TestDataClass.test10DataExp } },
            new object[] {"11",new[] { TestDataClass.test11Data, TestDataClass.test11DataExp } }
                };


        [DataTestMethod()]
        [DataRow("0", new[] { TestDataClass.testData0, TestDataClass.testDataExp0 }, DisplayName = "0")]
        //  [DataRow("2", new[] { TestDataClass.test2Data, TestDataClass.testDataExp2 }, DisplayName = "2")]
        [DataRow("3", new[] { TestDataClass.testData3, TestDataClass.testDataExp3 }, DisplayName = "3")]
        [DataRow("4", new[] { TestDataClass.testData4, TestDataClass.testDataExp4 }, DisplayName = "4")]
        [DataRow("5", new[] { TestDataClass.testData5, TestDataClass.testDataExp5 }, DisplayName = "5")]
        [DataRow("6", new[] { TestDataClass.testData6, TestDataClass.testDataExp6 }, DisplayName = "6")]
        [DataRow("7", new[] { TestDataClass.testData7, TestDataClass.testDataExp7 }, DisplayName = "7")]
        [DynamicData(nameof(ParseTestData))]
        public void ParseTest(string name, string[] data)
        {
            _testClass.OriginalCode = data[0];
            var act = _testClass.Parse();
            var sAct = act.ToString();
            using var ms = new FileStream("Test" + name + "CodeBlock.json", FileMode.Create);
            new DataContractJsonSerializer(
                typeof(CSCode.CodeBlock),
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
            new object[] { "1", new[] { TestDataClass.test1Data, TestDataClass.cExpCode1 } },
            new object[] { "2", new[] { TestDataClass.test2Data, TestDataClass.cExpCode2 } },
            new object[] { "8", new[] { TestDataClass.test8Data, TestDataClass.cExpCode8 } },
            new object[] { "9", new[] { TestDataClass.test9Data, TestDataClass.cExpCode9 } }
                };

        [DataTestMethod()]
        [DataRow("0", new[] { TestDataClass.testData0, TestDataClass.cExpCode0 }, DisplayName = "0")]
        //[DataRow("1", new[] { TestDataClass.test1Data, TestDataClass.cExpCode1 }, DisplayName = "1")]
        //[DataRow("2", new[] { TestDataClass.test2Data, TestDataClass.cExpCode2 }, DisplayName = "2")]
        [DataRow("3", new[] { TestDataClass.testData3, TestDataClass.cExpCode3 }, DisplayName = "3")]
        [DataRow("4", new[] { TestDataClass.testData4, TestDataClass.cExpCode4 }, DisplayName = "4")]
        [DataRow("5", new[] { TestDataClass.testData5, TestDataClass.cExpCode5 }, DisplayName = "5")]
        [DataRow("6", new[] { TestDataClass.testData6, TestDataClass.cExpCode6 }, DisplayName = "6")]
        [DataRow("7", new[] { TestDataClass.testData7, TestDataClass.cExpCode7 }, DisplayName = "7")]
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
        [DataRow(new[] { TestDataClass.testData0, TestDataClass.cExpLog0 }, DisplayName = "0")]
        [DataRow(new[] { TestDataClass.testData5, TestDataClass.cExpLog0 }, DisplayName = "0")]
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