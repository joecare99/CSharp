using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Interfaces;
using BaseLib.Helper;
using BaseLib.Interfaces;
using System.Linq;

namespace GenFree.Models.Tests;

public enum TestIndex
{
    _MyKeyIndex,
    Idx1,
    Idx2
}

public enum TestIndexField
{
    ID,
    Description,
    Data
}

public interface ITestData : IHasID<int>, IHasIRecordset
{
    string Description { get; }
    int Data { get; }
}

[TestClass()]
public class CUsesIndexedRSetTests : CUsesIndexedRSet<int, TestIndex, TestIndexField, ITestData>
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    protected override TestIndex _keyIndex => TestIndex._MyKeyIndex;

    protected override IRecordset _db_Table => testRS;

    [TestInitialize]
    public void Init()
    {
        testRS = Substitute.For<IRecordset>();
        testRS.NoMatch.Returns(true);
        (testRS.Fields[TestIndexField.ID] as IHasValue).Value.Returns(1, 2, 3);
        (testRS.Fields[TestIndexField.Description] as IHasValue).Value.Returns("one", "two", "three");
        (testRS.Fields[TestIndexField.Data] as IHasValue).Value.Returns(2, 6, 4);
        testRS.EOF.Returns(false, false, true);
    }

    [TestMethod]
    public void SetUpTest()
    {
        Assert.IsNotNull(testRS);
        Assert.IsInstanceOfType(testRS, typeof(IRecordset));
        Assert.AreEqual(0, Count);
        Assert.AreEqual("", testRS.Index);
        Assert.AreEqual(true, testRS.NoMatch);
        Assert.AreEqual(0, testRS.RecordCount);
    }

    [TestMethod()]
    [DataRow("Null", 0, false)]
    [DataRow("1-0None", 1, true)]
    [DataRow("1-1Father", 1, true)]
    public void SeekTest(string sName, int iActPers, bool xExp)
    {
        testRS.NoMatch.Returns(iActPers is not (> 0 and < 3));
        Assert.AreEqual(xExp ? testRS : null, Seek(iActPers, out var xBreak));
        Assert.AreEqual(!xExp, xBreak);
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received().Seek("=", iActPers);
    }

    [TestMethod()]
    [DataRow("Null", 0, false)]
    [DataRow("1-0None", 1, true)]
    [DataRow("1-1Father", 1, true)]
    public void SeekTest1(string sName, int iActPers, bool xExp)
    {
        testRS.NoMatch.Returns(iActPers is not (> 0 and < 3));
        Assert.AreEqual(xExp ? testRS : null, Seek(iActPers));
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received().Seek("=", iActPers);
    }

    [TestMethod()]
    [DataRow("Null", 0, false)]
    [DataRow("1-0None", 1, true)]
    [DataRow("1-1Father", 1, true)]
    public void DeleteTest(string sName, int iActPers, bool xExp)
    {
        testRS.NoMatch.Returns(iActPers is not (> 0 and < 3));
        Assert.AreEqual(xExp, Delete(iActPers));
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received().Seek("=", iActPers);
        if (xExp)
        {
            testRS.Received().Delete();
        }
        else
        {
            testRS.DidNotReceive().Delete();
        }
    }

    [TestMethod()]
    [DataRow("Null", TestIndex.Idx1, 0, false)]
    [DataRow("1-0None", TestIndex.Idx1, 1, true)]
    [DataRow("1-1Father", TestIndex.Idx2, 1, true)]
    public void ExistTest(string sName, TestIndex eIx, int iActPers, bool xExp)
    {
        testRS.NoMatch.Returns(iActPers is not (> 0 and < 3));
        Assert.AreEqual(xExp, Exists(eIx, iActPers));
        Assert.AreEqual($"{eIx}", testRS.Index);
        testRS.Received().Seek("=", iActPers);
    }


    [TestMethod()]
    public void ForEachDoTest()
    {
        (testRS.Fields[TestIndexField.ID] as IHasValue).Value.Returns(1, 1, 1, 1, 3);
        testRS.EOF.Returns(false);
        testRS.NoMatch.Returns(false);
        var iCnt = 0;
        ForEachDo(TestIndex._MyKeyIndex, TestIndexField.ID, 1, ce =>
        {
            Assert.IsNotNull(ce);
            Assert.IsInstanceOfType(ce, typeof(ITestData));
            iCnt++;
            return true;
        });
        Assert.AreEqual(4, iCnt);
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received(0).MoveFirst();
        testRS.Received(4).MoveNext();
        testRS.Received(1).Seek("=", 1);
    }

    [TestMethod()]
    public void ForEachDo2Test()
    {
        (testRS.Fields[TestIndexField.ID] as IHasValue).Value.Returns(1, 1, 1, 1, 3);
        testRS.NoMatch.Returns(false);
        ForEachDo(TestIndex._MyKeyIndex, TestIndexField.ID, 1, null!);
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received(0).MoveFirst();
        testRS.Received(0).MoveNext();
        testRS.Received(1).Seek("=", 1);
    }

    [TestMethod()]
    public void ForEachDo3Test()
    {
        var iCnt = 0;
        (testRS.Fields[TestIndexField.ID] as IHasValue).Value.Returns(1, 1, 1, 1, 3);
        testRS.EOF.Returns(false);
        testRS.NoMatch.Returns(false, false, true);
        ForEachDo(TestIndex._MyKeyIndex, TestIndexField.ID, 1, rs =>
        {
            iCnt++;
            throw new AccessViolationException();
        });
        Assert.AreEqual(1, iCnt);
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received(0).MoveFirst();
        testRS.Received(1).MoveNext();
        testRS.Received(1).Seek("=", 1);
    }

    [TestMethod()]
    public void ReadAllTest()
    {
        (testRS.Fields[TestIndexField.ID] as IHasValue).Value.Returns(1, 1, 1, 1, 3);
        testRS.NoMatch.Returns(false);
        var iCnt = 0;
        foreach (var ce in ReadAll())
        {
            Assert.IsNotNull(ce);
            Assert.IsInstanceOfType(ce, typeof(ITestData));
            iCnt++;
        }
        ;
        Assert.AreEqual(2, iCnt);
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received(1).MoveFirst();
        testRS.Received(2).MoveNext();
        testRS.Received(0).Seek("=", 1);
    }

    [TestMethod()]
    public void ReadAllTest2()
    {
        (testRS.Fields[TestIndexField.ID] as IHasValue).Value.Returns(1, 1, 1, 1, 3);
        testRS.NoMatch.Returns(false);
        var iCnt = 0;
        foreach (var ce in ReadAll(TestIndex._MyKeyIndex))
        {
            Assert.IsNotNull(ce);
            Assert.IsInstanceOfType(ce, typeof(ITestData));
            iCnt++;
        }
        ;
        Assert.AreEqual(2, iCnt);
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received(1).MoveFirst();
        testRS.Received(2).MoveNext();
        testRS.Received(0).Seek("=", 1);
    }

    [TestMethod()]
    public void ReadAllTest3()
    {
        (testRS.Fields[TestIndexField.Data] as IHasValue).Value.Returns(1, 1, 1, 1, 3);
        testRS.NoMatch.Returns(false);
        var iCnt = 0;
        foreach (var ce in ReadAll(TestIndex.Idx2, 1))
        {
            Assert.IsNotNull(ce);
            Assert.IsInstanceOfType(ce, typeof(ITestData));
            iCnt++;
        }
        ;
        Assert.AreEqual(2, iCnt);
        Assert.AreEqual("Idx2", testRS.Index);
        testRS.Received(0).MoveFirst();
        testRS.Received(2).MoveNext();
        testRS.Received(1).Seek("=", 1);
    }

    [TestMethod()]
    public void ReadAllDataDBTest()
    {
        (testRS.Fields[TestIndexField.ID] as IHasValue).Value.Returns(1, 1, 1, 1, 3);
        testRS.NoMatch.Returns(false);
        var iCnt = 0;
        foreach (var ce in ReadAllDataDB(TestIndex._MyKeyIndex, r => r.Seek("=", 1), c => false))
        {
            Assert.IsNotNull(ce);
            Assert.IsInstanceOfType(ce, typeof(ITestData));
            iCnt++;
        }
        ;
        Assert.AreEqual(2, iCnt);
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received(0).MoveFirst();
        testRS.Received(2).MoveNext();
        testRS.Received(1).Seek("=", 1);
    }

    [TestMethod()]
    public void ReadAllDataDB2Test()
    {
        (testRS.Fields[TestIndexField.ID] as IHasValue).Value.Returns(1, 1, 1, 1, 3);
        testRS.NoMatch.Returns(false);
        var iCnt = 0;
        foreach (var ce in ReadAllDataDB(TestIndex._MyKeyIndex, r => r.Seek("=", 1), c => true))
        {
            iCnt++;
        }
        Assert.AreEqual(0, iCnt);
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received(0).MoveFirst();
        testRS.Received(0).MoveNext();
        testRS.Received(1).Seek("=", 1);
    }

    [TestMethod()]
    public void ReadAllDataDB3Test()
    {
        var iCnt = 0;
        (testRS.Fields[TestIndexField.ID] as IHasValue).Value.Returns(1, 1, 1, 1, 3);
        testRS.NoMatch.Returns(false, false, true);
        foreach (var ce in ReadAllDataDB(TestIndex._MyKeyIndex, r => r.Seek("=", 1), c => c.Data != 2))
        {
            Assert.IsNotNull(ce);
            Assert.IsInstanceOfType(ce, typeof(ITestData));
            iCnt++;
        }
        Assert.AreEqual(1, iCnt);
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received(0).MoveFirst();
        testRS.Received(1).MoveNext();
        testRS.Received(1).Seek("=", 1);
    }

    [TestMethod()]
    public void MaxIDTest()
    {
        Assert.AreEqual(1, MaxID);
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received(1).MoveLast();
    }

    [TestMethod()]
    public void CountTest()
    {
        testRS.RecordCount.Returns(99);
        Assert.AreEqual(99, Count);
        Assert.AreEqual("", testRS.Index);
    }

    [TestMethod()]
    [DataRow("Null", 0, false)]
    [DataRow("1-0None", 1, true)]
    [DataRow("1-1Father", 1, true)]
    public void ReadDataTest(string sName, int iActPers, bool xExp)
    {
        testRS.NoMatch.Returns(iActPers is not (> 0 and < 3));
        Assert.AreEqual(xExp, ReadData(iActPers, out var ce));
        if (xExp)
        {
            Assert.IsNotNull(ce);
            Assert.IsInstanceOfType(ce, typeof(ITestData));
        }
        else
        {
            Assert.IsNull(ce);
        }
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received().Seek("=", iActPers);
    }

    [TestMethod()]
    [DataRow("Null", 0, false)]
    [DataRow("1-0None", 1, true)]
    [DataRow("1-1Father", 1, true)]
    public void ReadData2Test(string sName, int iActPers, bool xExp)
    {
        testRS.NoMatch.Returns(iActPers is not (> 0 and < 3));
        Assert.AreEqual(xExp, ReadData(TestIndex._MyKeyIndex, iActPers, out var ce));
        if (xExp)
        {
            Assert.IsNotNull(ce);
            Assert.IsInstanceOfType(ce, typeof(ITestData));
        }
        else
        {
            Assert.IsNull(ce);
        }
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received().Seek("=", iActPers);
    }

    [TestMethod()]
    [DataRow("Null", 0, false)]
    [DataRow("1-0None", 1, true)]
    [DataRow("1-1Father", 1, true)]
    public void SetDataTest(string sName, int iActPers, bool xExp)
    {
        testRS.NoMatch.Returns(iActPers is not (> 0 and < 3));
        var ce = new CTestData(testRS);
        SetData(iActPers, ce);
        Assert.IsNotNull(ce);
        Assert.IsInstanceOfType(ce, typeof(ITestData));
        if (xExp)
        {
        }
        else
        {

        }
        Assert.AreEqual("_MyKeyIndex", testRS.Index);
        testRS.Received().Seek("=", iActPers);
    }

    protected override int GetID(IRecordset recordset)
    {
        return recordset.Fields[TestIndexField.ID].AsInt();
    }

    public override TestIndexField GetIndex1Field(TestIndex eIndex) => eIndex switch
    {
        TestIndex.Idx1 => TestIndexField.Description,
        TestIndex.Idx2 => TestIndexField.Data,
        _ => throw new NotImplementedException(),
    };

    protected override ITestData GetData(IRecordset rs, bool xNoInit = false) 
        => new CTestData(rs, xNoInit);

    private class CTestData : ITestData
    {
        private IRecordset rs;

        public CTestData(IRecordset rs, bool xNoInit = false)
        {
            this.rs = rs;
            if (!xNoInit) FillData(rs); else Description = string.Empty;
        }

        public string Description { get; set; }

        public int Data { get; set; }

        public int ID { get; set; }

        public void Delete()
        {
            rs.Delete();
        }

        public void FillData(IRecordset rs)
        {
            Description = rs.Fields[TestIndexField.Description].AsString();
            Data = rs.Fields[TestIndexField.Data].AsInt();
        }

        public void NewID()
        {
            rs.Index = nameof(TestIndexField.ID);
            rs.MoveLast();
            ReadID(rs);
            ID++;
        }

        public void ReadID(IRecordset dB_Table)
        {
            ID = rs.Fields[TestIndexField.ID].AsInt();
        }

        public void SetDBValues(IRecordset rs, Enum[]? asProps)
        {
            if (asProps == null || asProps.Length == 0 || asProps.Contains(TestIndexField.ID))
            {
                rs.Fields[TestIndexField.ID].Value = ID;
            }
            if (asProps == null || asProps.Length == 0 || asProps.Contains(TestIndexField.Description))
            {
                rs.Fields[TestIndexField.Description].Value = Description;
            }
            if (asProps == null || asProps.Length == 0 || asProps.Contains(TestIndexField.Data))
            {
                rs.Fields[TestIndexField.Data].Value = Data;
            }
        }
    }

    [TestMethod()]
    public void CreateNewTest()
    {
        // Arrange
      //  testRS.AddNew().Returns(_ => { });
        (testRS.Fields[TestIndexField.ID] as IHasValue).Value.Returns(0);
        (testRS.Fields[TestIndexField.Description] as IHasValue).Value.Returns(string.Empty);
        (testRS.Fields[TestIndexField.Data] as IHasValue).Value.Returns(0);

        // Act
        var newData = CreateNew();

        // Assert
        Assert.IsNotNull(newData);
        Assert.IsInstanceOfType(newData, typeof(ITestData));
        Assert.AreEqual(0, newData.ID);
        Assert.AreEqual(string.Empty, newData.Description);
        Assert.AreEqual(0, newData.Data);
    }

    [TestMethod()]
    [DataRow(1, "Test", 42)]
    [DataRow(2, "Beispiel", 99)]
    [DataRow(3, "", 0)]
    public void CommitTest(int id, string description, int data)
    {
        // Arrange
        var ce = new CTestData(testRS) { ID = id, Description = description, Data = data };
        testRS.NoMatch.Returns(id!=2);

        // Act
        Commit(ce);

        // Assert
        testRS.Received(id==2?1:0).Edit();
        testRS.Received(id==2?0:1).AddNew();
        testRS.Received(1).Update();
        Assert.AreEqual(2, ce.ID,"Id");
        Assert.AreEqual(description, ce.Description);
        Assert.AreEqual(data, ce.Data);
    }
}