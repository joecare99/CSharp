using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace GenFree.Data.Tests;

public enum TestProp
{
    ID,
    sDescription,
    iData
}

[TestClass()]
public class CDataTests
{
    class TestClass : CData<TestProp>
    {
        public int ID { get; private set; } = 1;
        public string sDescription { get; private set; } = "Some Test";
        public int iData { get; private set; } = 321;
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

        public override void SetPropValue(TestProp prop, object? value)
        {
            if (EqualsProp(prop,value)) return;
            AddChangedProp(prop);
            object _ = prop switch
            {
                TestProp.ID => ID = (int?)value ?? 0,
                TestProp.sDescription => sDescription = (string?)value ?? "",
                TestProp.iData => iData = (int?)value ?? 0,
                _ => throw new NotImplementedException(),
            };
        }
    }

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private TestClass testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        testClass = new();
    }

    [TestMethod()]
    public void AddChangedPropTest()
    {
        testClass.AddChangedProp(TestProp.ID);
        testClass.AddChangedProp(TestProp.iData);
        testClass.AddChangedProp(TestProp.iData);
        Assert.AreEqual(2, testClass.ChangedProps.Count);
    }

    [TestMethod()]
    public void ClearChangedPropsTest()
    {
        AddChangedPropTest();
        testClass.ClearChangedProps();
        Assert.AreEqual(0, testClass.ChangedProps.Count);
    }

    [DataTestMethod()]
    [DataRow(TestProp.ID, 1)]
    [DataRow(TestProp.sDescription, "Some Test")]
    [DataRow(TestProp.iData, 321)]
    public void GetPropValueTest(TestProp eAct, object? oExp)
    {
        Assert.AreEqual(oExp, testClass.GetPropValue(eAct));
    }

    [DataTestMethod()]
    [DataRow(TestProp.ID, 1, 0)]
    [DataRow(TestProp.sDescription, "Some Test", 0)]
    [DataRow(TestProp.iData, 321, 0)]
    [DataRow(TestProp.ID, 2, 1)]
    [DataRow(TestProp.sDescription, "Some_Test", 1)]
    [DataRow(TestProp.iData, 322, 1)]
    public void SetPropValueTest(TestProp eAct, object? oExp, int iExp)
    {
        testClass.SetPropValue(eAct, oExp);
        Assert.AreEqual(iExp, testClass.ChangedProps.Count);
        Assert.AreEqual(oExp,testClass.GetPropValue(eAct));            
    }

    [DataTestMethod()]
    [DataRow((TestProp)(0-1), "Some_Test", 1)]
    [DataRow((TestProp)3, 322, 1)]
    public void SetPropValueTest1(TestProp eAct, object? oExp, int iExp)
    {
        Assert.ThrowsException<NotImplementedException>(()=>testClass.SetPropValue(eAct, oExp));
    }
    }