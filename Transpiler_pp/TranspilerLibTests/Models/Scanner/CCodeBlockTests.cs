using System.Collections.Generic;
using TranspilerLib.Data;
using TranspilerLib.Models.Tests;
using TranspilerLibTests.TestData;
using static TranspilerLib.Helper.TestHelper;

#pragma warning disable IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
namespace TranspilerLib.Models.Scanner.Tests
#pragma warning restore IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
{
    [TestClass()]
    public class CCodeBlockTests : TestBase
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CSCode _testClass;

        public static IEnumerable<object[]> TestListMove => new object[][]
{
            ["0a", TestCSDataClass.TestDataList0(), (new int[] { }, 3, 3, 1), false, new[] { TestCSDataClass.testDataExp0 }],

            ["1a", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 5, 5, 1), false, new[] { TestCSDataClass.testDataExp1 }],
            ["1b", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, -1, 5, 1), false, new[] { TestCSDataClass.testDataExp1 }],
            ["1c", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 5, -1, 1), false, new[] { TestCSDataClass.testDataExp1 }],
            ["1d", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 131, 5, 1), false, new[] { TestCSDataClass.testDataExp1 }],
            ["1e", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 5, 131, 1), false, new[] { TestCSDataClass.testDataExp1 }],
            ["1f", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 125, 5, 6), false, new[] { TestCSDataClass.testDataExp1 }],
               ["1g0", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 6, 9, 3), true, new[] { TestCSDataClass.testDataExp1 }],
               ["1gd", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 6, 13, 3), true, new[] { TestCSDataClass.testDataMoveExp }],
               ["1gu", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 9, 6, 4), true, new[] { TestCSDataClass.testDataMoveExp }],

            ["2a", TestCSDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 5, 5, 1), false, new[] { TestCSDataClass.testDataExp2 }],
            ["2b", TestCSDataClass.TestDataList2(), (new[] { 7, 1, 3 }, -1, 5, 1), false, new[] { TestCSDataClass.testDataExp2 }],
            ["2c", TestCSDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 5, -1, 1), false, new[] { TestCSDataClass.testDataExp2 }],
            ["2d", TestCSDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 34, 5, 1), false, new[] { TestCSDataClass.testDataExp2 }],
            ["2e", TestCSDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 5, 34, 1), false, new[] { TestCSDataClass.testDataExp2 }],
            ["2f", TestCSDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 30, 5, 5), false, new[] { TestCSDataClass.testDataExp2 }],

         };

        public static IEnumerable<object[]> TestListDelete => new object[][]
 {
            ["0a", TestCSDataClass.TestDataList0(), (new int[] { }, 5, 9), false, new[] { TestCSDataClass.testDataExp0 }],

            ["1b", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, -1,  1), false, new[] { TestCSDataClass.testDataExp1 }],
            ["1d", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 131,  1), false, new[] { TestCSDataClass.testDataExp1 }],
            ["1f", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 125, 6), false, new[] { TestCSDataClass.testDataExp1 }],
               ["1gd", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 6, 3), true, new[] { TestCSDataClass.testDataDeleteExp }],
               ["1gu", TestCSDataClass.TestDataList1(), (new[] { 8, 1, 5 }, 9, 4), true, new[] { TestCSDataClass.testDataDelete2Exp }],

            ["2b", TestCSDataClass.TestDataList2(), (new[] { 7, 1, 3 }, -1,  1), false, new[] { TestCSDataClass.testDataExp2 }],
            ["2d", TestCSDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 34,  1), false, new[] { TestCSDataClass.testDataExp2 }],
            ["2f", TestCSDataClass.TestDataList2(), (new[] { 7, 1, 3 }, 30,  5), false, new[] { TestCSDataClass.testDataExp2 }],

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

        [DataTestMethod()]
        public void GetItemIndex()
        {
            var act = _testClass.Parse(TestCSDataClass.TestDataList1() as List<TokenData>);
            var act2 = act.SubBlocks[7];
            ((CodeBlock)act2).SourcesIndex = new() { new() { 1 } };
            Assert.AreEqual(true, act2.Sources[0].TryGetTarget(out var act3));
            Assert.AreEqual(act, act3);
        }

        [DataTestMethod()]
        public void GetItemByIndexIndexTest()
        {
            var act = _testClass.Parse(TestCSDataClass.TestDataList1() as List<TokenData>);
            var act2 = act.SubBlocks[7];
            ((CodeBlock)act2).DestinationIndex = new() { 1 };
            Assert.AreEqual(true, act2.Destination.TryGetTarget(out var act3));
            Assert.AreEqual(act, act3);
        }

    }
}