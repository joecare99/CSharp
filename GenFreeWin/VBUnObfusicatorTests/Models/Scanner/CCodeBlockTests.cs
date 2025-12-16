using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using VBUnObfusicator.Data;
using VBUnObfusicator.Models.Tests;
using VBUnObfusicatorTests.TestData;
using static VBUnObfusicator.Helper.TestHelper;

#pragma warning disable IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
namespace VBUnObfusicator.Models.Scanner.Tests;
#pragma warning restore IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.

[TestClass()]
public class CCodeBlockTests : TestBase
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private VBUnObfusicator.Models.Scanner.CSCode _testClass;

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

    [TestMethod()]
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

    [TestMethod()]
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

    [TestMethod()]
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

    [TestMethod()]
    public void GetItemIndex()
    {
        var act = _testClass.Parse(TestDataClass.TestDataList1() as List<TokenData>);
        var act2 = act.SubBlocks[7];
        ((CodeBlock)act2).SourcesIndex = new() { new() { 1 } };
        Assert.IsTrue(act2.Sources[0].TryGetTarget(out var act3));
        Assert.AreEqual(act, act3);
    }

    [TestMethod()]
    public void GetItemByIndexIndexTest()
    {
        var act = _testClass.Parse(TestDataClass.TestDataList1() as List<TokenData>);
        var act2 = act.SubBlocks[7];
        ((CodeBlock)act2).DestinationIndex = new() { 1 };
        Assert.IsTrue(act2.Destination.TryGetTarget(out var act3));
        Assert.AreEqual(act, act3);
    }

}
