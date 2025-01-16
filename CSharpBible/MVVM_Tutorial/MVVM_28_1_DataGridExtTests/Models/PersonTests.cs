using BaseLib.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MVVM_28_1_DataGridExt.Models.Tests;

[TestClass]
public class PersonTests
{
    Person testItem;

    [TestInitialize]
    public void Init()
    {
        testItem = new Person();
    }

    static IEnumerable<object[]> PersonPropertyTestData { get
        { foreach (var p in typeof(Person).GetProperties())
                switch (p.PropertyType.TC()) {
                    case TypeCode.String:                           
                        yield return new object[] { p.Name, "Null", null, null };
                        yield return new object[] { p.Name, "Empty", "", "" };
                        yield return new object[] { p.Name, "Peter", "Peter", "Peter" };
                        yield return new object[] { p.Name, "Müller", "Müller", "Müller" };
                        break;
                    case TypeCode.Int32:
                        yield return new object[] { p.Name, "0", 0, 0 };
                        yield return new object[] { p.Name, "1", 1, 1 };
                        yield return new object[] { p.Name, "-1", -1, -1 };
                        yield return new object[] { p.Name, "MaxInt", int.MaxValue, int.MaxValue };
                        yield return new object[] { p.Name, "MinInt", int.MinValue, int.MinValue };
                        break;
                    case TypeCode.Object when p.PropertyType == typeof(DateTime?):
                        yield return new object[] { p.Name, "Null", null, null };
                        yield return new object[] { p.Name, "0", (DateTime?)new DateTime(1980,1,1), (DateTime?)new DateTime(1980, 1, 1) };
                        yield return new object[] { p.Name, "1", (DateTime?)new DateTime(2001, 1, 1), new DateTime(2001, 1, 1) };
                        yield return new object[] { p.Name, "Today", (DateTime?)DateTime.Today, DateTime.Today };
                        yield return new object[] { p.Name, "MaxDate", (DateTime?)DateTime.MaxValue, DateTime.MaxValue };
                        yield return new object[] { p.Name, "MinDate", (DateTime?)DateTime.MinValue, DateTime.MinValue };
                        break;
                    default:
                        yield return new object[] { p.Name, "Null", null, null };
                        break;
                }
        } }

    [DataTestMethod]
    [DynamicData(nameof(PersonPropertyTestData))]
    public void TestProperties(string sProp,string sName,object oVal,object oExp)
    {
        if (oVal is DateTime? )
            testItem.SetProp(sProp, oVal as DateTime?);
        else
            testItem.SetProp(sProp, oVal);
        Assert.AreEqual(oExp,testItem.GetProp(sProp));
    }
}
