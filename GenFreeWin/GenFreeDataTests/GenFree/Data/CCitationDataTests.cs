using BaseLib.Interfaces;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace GenFree.Data.Tests;

[TestClass()]
public class CCitationDataTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private CCitationData testClass;
    private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        testRS = Substitute.For<IRecordset>();
        testRS.NoMatch.Returns(true);
        (testRS.Fields[SourceLinkFields.Art] as IHasValue).Value.Returns(1, 2, 3);
        (testRS.Fields[SourceLinkFields._1] as IHasValue).Value.Returns(2, 3, 4);
        (testRS.Fields[SourceLinkFields._2] as IHasValue).Value.Returns(3, 4, 5);
        (testRS.Fields[SourceLinkFields.LfNr] as IHasValue).Value.Returns(4, 5, 6);
        (testRS.Fields[SourceLinkFields._3] as IHasValue).Value.Returns(5, 6, 7);
        (testRS.Fields[SourceLinkFields._4] as IHasValue).Value.Returns("Field3");
        (testRS.Fields[SourceLinkFields.Aus] as IHasValue).Value.Returns("Aus");
        (testRS.Fields[SourceLinkFields.Orig] as IHasValue).Value.Returns("Orig");
        (testRS.Fields[SourceLinkFields.Kom] as IHasValue).Value.Returns("Kom");
        testClass = new(testRS);
        testRS.ClearReceivedCalls();
    }

    [TestMethod()]
    public void CCitationDataTest()
    {
        Assert.IsNotNull(testClass);
        Assert.IsInstanceOfType(testClass, typeof(CCitationData));
        Assert.IsInstanceOfType(testClass, typeof(ICitationData));
    }

    [TestMethod()]
    public void ClearTest()
    {
        testClass.Clear();
        Assert.AreEqual(0, testClass.iQuNr);
        Assert.AreEqual(EEventArt.eA_Unknown, testClass.eArt);
        Assert.AreEqual("", testClass.sSourceTitle);
        Assert.AreEqual("", testClass.sPage);
        Assert.AreEqual("", testClass.sEntry);
        Assert.AreEqual("", testClass.sOriginalText);
        Assert.AreEqual("", testClass.sComment);
        Assert.IsTrue(testClass.ChangedProps.Count == 0, "ChangedProps should be empty after Clear()");
    }

    [TestMethod()]
    [DataRow(1, EEventArt.eA_Birth, 100,true)]
    [DataRow(2, EEventArt.eA_Birth, 100,false)]
    [DataRow(3, EEventArt.eA_Birth, 100,true)]
    [DataRow(3, EEventArt.eA_Birth, 100,false)]
    public void CommitTest(int iPerFamNr, EEventArt eArt, int lfNR,bool xNMt)
    {
        testClass.iLinkType = (short)iPerFamNr;
        testRS.NoMatch.Returns(xNMt);
        testClass.Commit(iPerFamNr,eArt,(short)lfNR);
        testRS.Received(1).Update();
        testRS.ReceivedWithAnyArgs(1).Seek("=");
        testRS.Received(xNMt?0:1).Edit();
        testRS.Received(xNMt ? 1 : 0).AddNew();
        testRS.Received(xNMt ? 10 : 5).Fields[SourceLinkFields._1].Value = iPerFamNr;

    }

    [TestMethod()]
    public void GetPropTypeTest()
    { 
        Assert.AreEqual(TypeCode.String, Type.GetTypeCode(testClass.GetPropType(ESourceLinkProp.sSourceTitle)));
        Assert.AreEqual(TypeCode.Int32, Type.GetTypeCode(testClass.GetPropType(ESourceLinkProp.iPerFamNr)));
    }

    [TestMethod()]
    public void GetPropValueTest()
    {
        Assert.AreEqual(3, testClass.GetPropValue(ESourceLinkProp.iPerFamNr));
        Assert.AreEqual("Aus", testClass.GetPropValue(ESourceLinkProp.sPage));
        Assert.AreEqual("", testClass.GetPropValue(ESourceLinkProp.sSourceTitle));
    }

    [TestMethod()]
    public void SetPropValueTest()
    {
        testClass.SetPropValue(ESourceLinkProp.iPerFamNr, 10);
        Assert.AreEqual(10, testClass.iPerFamNr , "testClass.iPerFamNr = 10");
        Assert.AreEqual(2, testClass.ChangedProps.Count);
        testClass.SetPropValue(ESourceLinkProp.sSourceTitle, "New Title");
        Assert.AreEqual(2, testClass.ChangedProps.Count);
        Assert.AreEqual("New Title", testClass.sSourceTitle);
    }
}