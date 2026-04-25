using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using BaseLib.Interfaces;
using BaseLib.Helper;

namespace GenFree.Data.Tests;

[TestClass()]
public class CNamesDataTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private CNamesData testClass;
    private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        testRS = Substitute.For<IRecordset>();
        testRS.NoMatch.Returns(true);
        (testRS.Fields[NameFields.PersNr] as IHasValue).Value.Returns(1, 2, 3);
        (testRS.Fields[NameFields.Kennz] as IHasValue).Value.Returns(2, 3, 4);
        (testRS.Fields[NameFields.Text] as IHasValue).Value.Returns(3, 4, 5);
        (testRS.Fields[NameFields.LfNr] as IHasValue).Value.Returns(4, 5, 6);
        (testRS.Fields[NameFields.Ruf] as IHasValue).Value.Returns(5, 6, 7);
        (testRS.Fields[NameFields.Spitz] as IHasValue).Value.Returns(6, 7, 8);
        testClass = new(testRS);
        testRS.ClearReceivedCalls();
    }

    [TestMethod()]
    public void CNamesDataTest()
    {
        Assert.IsNotNull(testClass);
        Assert.IsInstanceOfType(testClass, typeof(CNamesData));
        Assert.IsInstanceOfType(testClass, typeof(INamesData));
    }

    [TestMethod()]
    [DataRow((ENamesProp)1, (ETextKennz)3)]
    [DataRow((ENamesProp)2, 4)]
    [DataRow((ENamesProp)3, 5)]
    [DataRow((ENamesProp)4, true)]
    [DataRow((ENamesProp)5, true)]
    public void FillDataTest(ENamesProp eProp, object oExp)
    {
        testClass.FillData(testRS);
        Assert.AreEqual(oExp, testClass.GetPropValue(eProp));
    }

    [TestMethod()]
    [DataRow(ENamesProp.iPersNr, TypeCode.Int32)]
    [DataRow(ENamesProp.eTKennz, TypeCode.UInt32)]
    [DataRow(ENamesProp.iTextNr, TypeCode.Int32)]
    [DataRow(ENamesProp.iLfNr, TypeCode.Int32)]
    [DataRow(ENamesProp.bRuf, TypeCode.Boolean)]
    [DataRow(ENamesProp.bSpitz, TypeCode.Boolean)]
    public void GetPropTypeTest(ENamesProp pAct, TypeCode eExp)
    {
        Assert.AreEqual(eExp, Type.GetTypeCode(testClass.GetPropType(pAct)));
    }

    [TestMethod()]
    [DataRow((ENamesProp)(0 - 1), TypeCode.Int32)]
    [DataRow((ENamesProp)6, TypeCode.Int32)]
    [DataRow((ENamesProp)100, TypeCode.Int32)]
    public void GetPropTypeTest2(ENamesProp pAct, TypeCode eExp)
    {
        Assert.ThrowsExactly<NotImplementedException>(() => testClass.GetPropType(pAct));
    }

    [TestMethod()]
    [DataRow(ENamesProp.iPersNr, 1)]
    [DataRow(ENamesProp.eTKennz, (ETextKennz)2)]
    [DataRow(ENamesProp.iTextNr, 3)]
    [DataRow(ENamesProp.iLfNr, 4)]
    [DataRow(ENamesProp.bRuf, true)]
    [DataRow(ENamesProp.bSpitz, true)]
    public void GetPropValueTest(ENamesProp eExp, object oAct)
    {
        Assert.AreEqual(oAct, testClass.GetPropValue(eExp));
    }

    [TestMethod()]
    [DataRow((ENamesProp)(0 - 1), TypeCode.Int32)]
    [DataRow((ENamesProp)6, TypeCode.Int32)]
    [DataRow((ENamesProp)100, TypeCode.Int32)]
    public void GetPropValueTest2(ENamesProp eExp, object oAct)
    {
        Assert.ThrowsExactly<NotImplementedException>(() => testClass.GetPropValue(eExp));
    }

    [TestMethod()]
    [DataRow(ENamesProp.iPersNr, 1)]
    [DataRow(ENamesProp.iPersNr, 2)]
    [DataRow(ENamesProp.eTKennz, (ETextKennz)3)]
    [DataRow(ENamesProp.iTextNr, 4)]
    [DataRow(ENamesProp.iLfNr, 5)]
    [DataRow(ENamesProp.bRuf, true)]
    [DataRow(ENamesProp.bSpitz, true)]
    public void SetPropValueTest(ENamesProp eAct, object iVal)
    {
        testClass.SetPropValue(eAct, iVal);
        Assert.AreEqual(iVal, testClass.GetPropValue(eAct));
    }

    [TestMethod()]
    [DataRow((ENamesProp)(0 - 1), TypeCode.Int32)]
    [DataRow((ENamesProp)6, TypeCode.Int32)]
    [DataRow((ENamesProp)100, TypeCode.Int32)]
    public void SetPropValueTest1(ENamesProp eAct, object iVal)
    {
        Assert.ThrowsExactly<NotImplementedException>(() => testClass.SetPropValue(eAct, iVal));
    }


    [TestMethod()]
    [DataRow(ENamesProp.iPersNr, 1)]
    [DataRow(ENamesProp.iPersNr, 2)]
    [DataRow(ENamesProp.eTKennz, (ETextKennz)3)]
    [DataRow(ENamesProp.iTextNr, 4)]
    [DataRow(ENamesProp.iLfNr, 5)]
    [DataRow(ENamesProp.bRuf, false)]
    [DataRow(ENamesProp.bSpitz, false)]
    public void SetDBValueTest(ENamesProp eAct, object _)
    {
        testClass.SetDBValues(testRS, new[] { (Enum)eAct });
        _ = testRS.Received().Fields[eAct.ToString()];
    }

    [TestMethod()]
    [DataRow((ENamesProp)(0 - 1), TypeCode.Int32)]
    [DataRow((ENamesProp)6, TypeCode.Int32)]
    [DataRow((ENamesProp)100, TypeCode.Int32)]
    public void SetDBValueTest1(ENamesProp eAct, object _)
    {
        Assert.ThrowsExactly<NotImplementedException>(() => testClass.SetDBValues(testRS, new[] { (Enum)eAct }));
    }

    [TestMethod()]
    [DataRow(ENamesProp.iPersNr, 2)]
    [DataRow(ENamesProp.eTKennz, (ETextKennz)3)]
    [DataRow(ENamesProp.iTextNr, 4)]
    [DataRow(ENamesProp.iLfNr, 5)]
    [DataRow(ENamesProp.bRuf, false)]
    [DataRow(ENamesProp.bSpitz, false)]
    public void SetDBValueTest2(ENamesProp eAct, object oVal)
    {
        testClass.SetPropValue(eAct, oVal);
        testClass.SetDBValues(testRS, null);
        _ = testRS.Received().Fields[eAct.AsEnum<RepoFields>()];
    }

    [TestMethod()]
    [DataRow(false)]
    [DataRow(true)]
    public void DeleteTest(bool xAct)
    {
        testRS.NoMatch.Returns(xAct);
        testClass.Delete();
        Assert.AreEqual(NameIndex.NamKenn.AsFld(), testRS.Index);
        testRS.Received(xAct ? 0 : 1).Delete();
        testRS.Received(1).Seek("=", testClass.ID.Item1, testClass.ID.Item2, testClass.ID.Item3);
    }
}