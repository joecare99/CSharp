using GenFree.Model.Data;
using GenFree.Helper;
using GenFree.Interfaces.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;

namespace GenFree.Model.Data.Tests;

[TestClass()]
public class CRSDataTests
{
    class TestClass : CRSData<TestProp, int>
    {
        public int _ID = 1;

        public TestClass(IRecordset db_Table) : base(db_Table)
        {
        }

        public override int ID => _ID;

        public string sDescription { get; private set; } = "Some Test";
        public int iData { get; private set; } = 321;

        protected override Enum _keyIndex => TestProp.ID;

        public override void FillData(IRecordset dB_Table)
        {
            dB_Table.Fields[nameof(TestProp.ID)].Value = ID;
            dB_Table.Fields[nameof(TestProp.sDescription)].Value = sDescription;
            dB_Table.Fields[nameof(TestProp.iData)].Value = iData;
        }

        public override Type GetPropType(TestProp prop) => prop switch
        {
            TestProp.ID => typeof(int),
            TestProp.sDescription => typeof(string),
            TestProp.iData => typeof(int),
            _ => throw new NotImplementedException(),
        };

        public override object? GetPropValue(TestProp prop) => prop switch
        {
            TestProp.ID => ID,
            TestProp.sDescription => sDescription,
            TestProp.iData => iData,
            _ => null
        };

        public override void SetDBValue(IRecordset dB_Table, string[]? asProps)
        {
            asProps ??= _changedPropsList.Select(e => e.ToString()).ToArray();
            foreach (string sProp in asProps)
            {
                dB_Table.Fields[sProp].Value = GetPropValue(sProp.AsEnum<TestProp>());
            }
        }

        public override void SetPropValue(TestProp prop, object? value)
        {
            if (EqualsProp(prop, value)) return;
            AddChangedProp(prop);
            object _ = prop switch
            {
                TestProp.ID => _ID = (int?)value ?? 0,
                TestProp.sDescription => sDescription = (string?)value ?? "",
                TestProp.iData => iData = (int?)value ?? 0,
                _ => throw new NotImplementedException(),
            };
        }
    }

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private TestClass testClass;
    private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        testRS = Substitute.For<IRecordset>();
        testClass = new(testRS);
        testRS.NoMatch.Returns(true);
        testRS.Fields[nameof(TestProp.ID)].Value.Returns(1, 2, 3);
        testRS.Fields[nameof(TestProp.sDescription)].Value.Returns("Some Test", "Some_Test", "SomeTest_");
        testRS.Fields[nameof(TestProp.iData)].Value.Returns(320, 321, 322);
        testRS.ClearReceivedCalls();
    }

    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testClass);
        Assert.IsInstanceOfType(testClass, typeof(CRSData<TestProp, int>));
    }

    [DataTestMethod()]
    [DataRow(false)]
    [DataRow(true)]
    public void DeleteTest(bool xExp)
    {
        testRS.NoMatch.Returns(xExp);
        testClass.Delete();
        testRS.Received(xExp?0:1).Delete();
        Assert.AreEqual("ID",testRS.Index);
        testRS.Received(1).Seek("=", testClass.ID);
    }

    //[TestMethod()]
    //public void ClearChangedPropsTest()
    //{
    //    AddChangedPropTest();
    //    testClass.ClearChangedProps();
    //    Assert.AreEqual(0, testClass.ChangedProps.Count);
    //}

    //[DataTestMethod()]
    //[DataRow(TestProp.ID, 1)]
    //[DataRow(TestProp.sDescription, "Some Test")]
    //[DataRow(TestProp.iData, 321)]
    //public void GetPropValueTest(TestProp eAct, object? oExp)
    //{
    //    Assert.AreEqual(oExp, testClass.GetPropValue(eAct));
    //}

    //[DataTestMethod()]
    //[DataRow(TestProp.ID, 1, 0)]
    //[DataRow(TestProp.sDescription, "Some Test", 0)]
    //[DataRow(TestProp.iData, 321, 0)]
    //[DataRow(TestProp.ID, 2, 1)]
    //[DataRow(TestProp.sDescription, "Some_Test", 1)]
    //[DataRow(TestProp.iData, 322, 1)]
    //public void SetPropValueTest(TestProp eAct, object? oExp, int iExp)
    //{
    //    testClass.SetPropValue(eAct, oExp);
    //    Assert.AreEqual(iExp, testClass.ChangedProps.Count);
    //    Assert.AreEqual(oExp, testClass.GetPropValue(eAct));
    //}

    //[DataTestMethod()]
    //[DataRow((TestProp)(0 - 1), "Some_Test", 1)]
    //[DataRow((TestProp)3, 322, 1)]
    //public void SetPropValueTest1(TestProp eAct, object? oExp, int iExp)
    //{
    //    Assert.ThrowsException<NotImplementedException>(() => testClass.SetPropValue(eAct, oExp));
    //}
}