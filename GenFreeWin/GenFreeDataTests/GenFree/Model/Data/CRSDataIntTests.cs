using BaseLib.Helper;
using BaseLib.Interfaces;
using GenFree.Interfaces.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;

namespace GenFree.Models.Data.Tests;

[TestClass()]
public class CRSDataIntTests
{
    class TestClass : CRSDataInt<TestProp>
    {
        public TestClass(IRecordset db_Table) : base(db_Table)
        {
        }

        public string sDescription { get; private set; } = "Some Test";
        public int iData { get; private set; } = 321;

        protected override Enum _keyIndex => TestProp.ID;

        public override void FillData(IRecordset dB_Table)
        {
            ReadID(dB_Table);
            sDescription= dB_Table.Fields[TestProp.sDescription].AsString();
            iData=dB_Table.Fields[TestProp.iData].AsInt();
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

        public override void ReadID(IRecordset dB_Table)
        {
            _ID = dB_Table.Fields[TestProp.ID].AsInt();
        }

        public override void SetDBValues(IRecordset dB_Table, Enum[]? asProps)
        {
            asProps ??= _changedPropsList.Select(e => (Enum)e).ToArray();
            foreach (var sProp in asProps)
            {
                dB_Table.Fields[$"{sProp}"].Value = GetPropValue(sProp.AsEnum<TestProp>());
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
        testRS.NoMatch.Returns(true);
        (testRS.Fields[TestProp.ID] as IHasValue).Value.Returns(1, 2, 3);
        (testRS.Fields[TestProp.sDescription] as IHasValue).Value.Returns("Some Test", "Some_Test", "SomeTest_");
        (testRS.Fields[TestProp.iData] as IHasValue).Value.Returns(320, 321, 322);
        testClass = new(testRS);
        testRS.ClearReceivedCalls();
    }

    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testClass);
        Assert.IsInstanceOfType(testClass, typeof(CRSData<TestProp, int>));
    }

    [TestMethod()]
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

    [TestMethod()]
    public void NewIDTest()
    {
        testClass.NewID();
        Assert.AreEqual(3, testClass.ID);
        testRS.Received(1).Index="ID";
        testRS.Received(1).MoveLast();
        testRS.Received(1).Fields[TestProp.ID].Value = 1;
        testClass.SetDBValues(testRS, null);
        _=testRS.Received(2).Fields[TestProp.ID].Value;
    }
}