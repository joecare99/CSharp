using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Interfaces;
using GenFree.Helper;
using static BaseLib.Helper.TestHelper;


namespace GenFree.Data.Tests;

[TestClass()]
public class CEventDataTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private CEventData testClass;
    private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        testRS = Substitute.For<IRecordset>();
        testRS.NoMatch.Returns(true);
        testRS.Fields[nameof(EventFields.Art)].Value.Returns(1, 2, 3);
        testRS.Fields[nameof(EventFields.PerFamNr)].Value.Returns(2, 3, 4);
        testRS.Fields[nameof(EventFields.DatumV)].Value.Returns(3, 4, 5);
        testRS.Fields[nameof(EventFields.DatumV_S)].Value.Returns("DatumV_S");
        testRS.Fields[nameof(EventFields.DatumB)].Value.Returns(5, 6, 7);
        testRS.Fields[nameof(EventFields.DatumB_S)].Value.Returns("DatumB_S");
        testRS.Fields[nameof(EventFields.KBem)].Value.Returns(7,8,9);
        testRS.Fields[nameof(EventFields.Ort)].Value.Returns(8,9,10);
        testRS.Fields[nameof(EventFields.Ort_S)].Value.Returns("Ort_S");
        testRS.Fields[nameof(EventFields.Reg)].Value.Returns("Reg");
        testRS.Fields[nameof(EventFields.Platz)].Value.Returns(9,10,11);
        testRS.Fields[nameof(EventFields.VChr)].Value.Returns("VChr");
        testRS.Fields[nameof(EventFields.LfNr)].Value.Returns(10,11,12);
        testRS.Fields[nameof(EventFields.Bem1)].Value.Returns("Bem1");
        testRS.Fields[nameof(EventFields.Bem2)].Value.Returns("Bem2");
        testRS.Fields[nameof(EventFields.Bem3)].Value.Returns("Bem3");
        testRS.Fields[nameof(EventFields.Bem4)].Value.Returns("Bem4");
        testRS.Fields[nameof(EventFields.ArtText)].Value.Returns(11,12,13);
        testRS.Fields[nameof(EventFields.Zusatz)].Value.Returns("Zusatz");
        testRS.Fields[nameof(EventFields.priv)].Value.Returns(12,13,14);
        testRS.Fields[nameof(EventFields.DatumText)].Value.Returns(13,14,15);
        testRS.Fields[nameof(EventFields.Causal)].Value.Returns(14,15,16);
        testRS.Fields[nameof(EventFields.an)].Value.Returns(15,16,17);
        testRS.Fields[nameof(EventFields.tot)].Value.Returns("tot");
        testRS.Fields[nameof(EventFields.Hausnr)].Value.Returns(16,17,18);
        testRS.Fields[nameof(EventFields.GrabNr)].Value.Returns(17,18,19);
        testClass = new(testRS);
        CEventData.SetGetText(getTextFnc);
        testRS.ClearReceivedCalls();
    }

    private string getTextFnc(int arg)
    {
        return $"Text_{arg}";
    }

    [DataTestMethod()]
    [DataRow("Text_11", EventFields.ArtText)]
    [DataRow("Text_16", EventFields.Hausnr)]
    [DataRow("Text_7", EventFields.KBem)]
    [DataRow("Text_13", EventFields.DatumText)]
    [DataRow("Text_9", EventFields.Platz)]
    [DataRow("Text_14", EventFields.Causal)]
    [DataRow("Text_17", EventFields.GrabNr)]
    [DataRow("Text_15", EventFields.an)]
    public void SetGetTextTest(string sExp, EventFields iAct)
    {
        Assert.AreEqual(sExp, iAct switch
        {
            EventFields.ArtText => testClass.sArtText,
            EventFields.Hausnr => testClass.sHausNr,
            EventFields.KBem => testClass.sKBem,
            EventFields.DatumText => testClass.sDatumText,
            EventFields.Platz => testClass.sPlatz,
            EventFields.Causal => testClass.sCausal,
            EventFields.GrabNr => testClass.sGrabNr,
            EventFields.an => testClass.sAn,
        });
    }

    [TestMethod()]
    public void CEventDataTest()
    {
        Assert.IsNotNull(testClass);
        Assert.IsInstanceOfType(testClass, typeof(CEventData));
        Assert.IsInstanceOfType(testClass, typeof(IEventData));
    }

    [DataTestMethod()]
    [DataRow(2, (EEventArt)2)]
    [DataRow(3, 16)]
    [DataRow(4, 12)]
    [DataRow(5, 15)]
    [DataRow(6, 14)]
    public void FillDataTest(EEventProp eProp, object oExp)
    {
        testClass.FillData(testRS);
        Assert.AreEqual(oExp, testClass.GetPropValue(eProp));
    }

    [TestMethod()]
    public void IDTest()
    {
        Assert.AreEqual((EEventArt)1,testClass.ID.eArt);
        Assert.AreEqual(2,testClass.ID.iLink);
        Assert.AreEqual((short)10,testClass.ID.iLfNr);
    }

    [DataTestMethod()]
    [DataRow(true,false,true,null)]
    [DataRow(false,false,false,null)]
    [DataRow(false, true, true, new[] { "test" })]
    [DataRow(false,true,false, new[] { "sBem" })]
    [DataRow(false,true,true, new[] { "sBem" })]
    public void UpdateTest(bool xMatch,bool xSetRS,bool xEd, string[]? asS)
    {
        testClass.sBem[1] = "1";
        testClass.sBem[2] = "2";
        testClass.sBem[3] = "3";
        testClass.sBem[4] = "4";
        testRS.NoMatch.Returns(!xMatch);
        if (xSetRS) {
            testRS.Fields[nameof(EventFields.Art)].Value.Returns(1);
            testRS.Fields[nameof(EventFields.PerFamNr)].Value.Returns(2);
            testRS.Fields[nameof(EventFields.LfNr)].Value.Returns(10);
        }
        if (xEd) testRS.EditMode.Returns(1);
        testClass.Update(asS);
        if (xSetRS) {
            testRS.Received(0).Seek("=");
            testRS.Received(xEd ? 0 : 4).Edit();
            testRS.Received(xEd?1:0).Update();
        }else
        if (!xMatch)
        {
            testRS.Received(1).Seek("=", (EEventArt)1,2,10);
            testRS.Received(0).Edit();
            testRS.Received(0).Update();
        }
        else
        {
            testRS.Received(1).Seek("=", (EEventArt)1, 2, 10);
            testRS.Received(1).Edit();
            testRS.Received(1).Update();
        }
    }

    [DataTestMethod()]
    [DataRow(EEventProp.eArt, TypeCode.Int32)]
    [DataRow(EEventProp.iArtText, TypeCode.Int32)]
    [DataRow(EEventProp.iPerFamNr, TypeCode.Int32)]
    [DataRow(EEventProp.dDatumV, TypeCode.DateTime)]
    [DataRow(EEventProp.sDatumV_S, TypeCode.String)]
    [DataRow(EEventProp.dDatumB, TypeCode.DateTime)]
    [DataRow(EEventProp.sDatumB_S, TypeCode.String)]
    [DataRow(EEventProp.iKBem, TypeCode.Int32)]
    [DataRow(EEventProp.iOrt, TypeCode.Int32)]
    [DataRow(EEventProp.iDatumText, TypeCode.Int32)]
    [DataRow(EEventProp.sOrt_S, TypeCode.String)]
    [DataRow(EEventProp.sReg, TypeCode.String)]
    [DataRow(EEventProp.iPlatz, TypeCode.Int32)]
    [DataRow(EEventProp.sVChr, TypeCode.String)]
    [DataRow(EEventProp.iLfNr, TypeCode.Int32)]
    [DataRow(EEventProp.sBem, TypeCode.Object)]
    [DataRow(EEventProp.iCausal, TypeCode.Int32)]
    [DataRow(EEventProp.sZusatz, TypeCode.String)]
    [DataRow(EEventProp.iPrivacy, TypeCode.Int32)]
    [DataRow(EEventProp.iHausNr, TypeCode.Int32)]
    [DataRow(EEventProp.iGrabNr, TypeCode.Int32)]
    [DataRow(EEventProp.iAn, TypeCode.Int32)]
    [DataRow(EEventProp.xIsDead, TypeCode.Boolean)]
    public void GetPropTypeTest(EEventProp pAct, TypeCode eExp)
    {
        Assert.AreEqual(eExp, Type.GetTypeCode(testClass.GetPropType(pAct)));
    }

    [DataTestMethod()]
    [DataRow((EEventProp)(0 - 1), TypeCode.Int32)]
    [DataRow((EEventProp)23, TypeCode.Int32)]
    [DataRow((EEventProp)100, TypeCode.Int32)]
    public void GetPropTypeTest2(EEventProp pAct, TypeCode eExp)
    {
        Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropType(pAct));
    }

    [DataTestMethod()]
    [DataRow(EEventProp.eArt, (EEventArt)1)]
    [DataRow(EEventProp.iArtText, 11)]
    [DataRow(EEventProp.iPerFamNr, 2)]
    [DataRow(EEventProp.dDatumV, new int[] {1900,01,02 })]
    [DataRow(EEventProp.sDatumV_S, "DatumV_S")]
    [DataRow(EEventProp.dDatumB, new int[] { 1900, 01, 04 })]
    [DataRow(EEventProp.sDatumB_S, "DatumB_S")]
    [DataRow(EEventProp.iKBem, 7)]
    [DataRow(EEventProp.iOrt, 8)]
    [DataRow(EEventProp.iDatumText, 13)]
    [DataRow(EEventProp.sOrt_S, "Ort_S")]
    [DataRow(EEventProp.sReg, "Reg")]
    [DataRow(EEventProp.iPlatz, 9)]
    [DataRow(EEventProp.sVChr, "VChr")]
    [DataRow(EEventProp.iLfNr, 10)]
    [DataRow(EEventProp.sBem, new[] {"","Bem1","Bem2","Bem3","Bem4"})]
    [DataRow(EEventProp.iCausal, 14)]
    [DataRow(EEventProp.sZusatz, "Zusatz")]
    [DataRow(EEventProp.iPrivacy, 12)]
    [DataRow(EEventProp.iHausNr, 16)]
    [DataRow(EEventProp.iGrabNr, 17)]
    [DataRow(EEventProp.iAn, 15)]
    [DataRow(EEventProp.xIsDead, false)]
    public void GetPropValueTest(EEventProp eAct, object oExp)
    {
        if (oExp is int[] aiAct && aiAct.Length == 3 && aiAct[1] <= 12 && aiAct[2] <= 31)
            oExp = new DateTime(aiAct[0], aiAct[1], aiAct[2]);
        else if (oExp is string s && s.Length == 36 && Guid.TryParse(s, out var g))
            oExp = g;
        else if (oExp is object[] aO && aO.Length == 2 && aO[0] is int iX && aO[1] is string sD)
            oExp = new ListItem<int>(sD, iX);

        if (oExp is ListItem<int> l)
            Assert.AreEqual(l.ItemString, ((string[])testClass.GetPropValue(eAct))[l.ItemData]);
        else if (oExp is not string[] aS)
            Assert.AreEqual(oExp, testClass.GetPropValue(eAct));
        else
            AssertAreEqual(aS, (string[])testClass.GetPropValue(eAct));
    }

    [DataTestMethod()]
    [DataRow((EEventProp)(0 - 1), TypeCode.Int32)]
    [DataRow((EEventProp)23, TypeCode.Int32)]
    [DataRow((EEventProp)100, TypeCode.Int32)]
    public void GetPropValueTest2(EEventProp eExp, object oAct)
    {
        Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropValue(eExp));
    }

    [DataTestMethod()]
    [DataRow(EEventProp.eArt, (EEventArt)1)]
    [DataRow(EEventProp.eArt, (EEventArt)2)]
    [DataRow(EEventProp.iArtText, 12)]
    [DataRow(EEventProp.iPerFamNr, 3)]
    [DataRow(EEventProp.dDatumV, new int[] { 1900, 01, 01 })]
    [DataRow(EEventProp.sDatumV_S, "DatumV_S_")]
    [DataRow(EEventProp.dDatumB, new int[] { 1900, 01, 02 })]
    [DataRow(EEventProp.sDatumB_S, "DatumB_S_")]
    [DataRow(EEventProp.iKBem, 8)]
    [DataRow(EEventProp.iOrt, 9)]
    [DataRow(EEventProp.iDatumText, 14)]
    [DataRow(EEventProp.sOrt_S, "Ort_S_")]
    [DataRow(EEventProp.sReg, "Reg_")]
    [DataRow(EEventProp.iPlatz, 10)]
    [DataRow(EEventProp.sVChr, "VChr_")]
    [DataRow(EEventProp.iLfNr, 11)]
    [DataRow(EEventProp.sBem, new[] { "", "Bem1", "Bem2-", "Bem3", "Bem4" })]
    [DataRow(EEventProp.sBem, new object[] { 1, "Bem1-" })]
    [DataRow(EEventProp.iCausal, 15)]
    [DataRow(EEventProp.sZusatz, "Zusatz_")]
    [DataRow(EEventProp.iPrivacy, 13)]
    [DataRow(EEventProp.iHausNr, 17)]
    [DataRow(EEventProp.iGrabNr, 18)]
    [DataRow(EEventProp.iAn, 16)]
    [DataRow(EEventProp.xIsDead, true)]
    [DataRow(EEventProp.xIsDead, false)]

    public void SetPropValueTest(EEventProp eAct, object oExp)
    {
        if (oExp is int[] aiAct && aiAct.Length == 3 && aiAct[1] <= 12 && aiAct[2] <= 31)
            oExp = new DateTime(aiAct[0], aiAct[1], aiAct[2]);
        else if (oExp is string s && s.Length == 36 && Guid.TryParse(s, out var g))
            oExp = g;
        else if (oExp is object[] aO && aO.Length == 2 && aO[0] is int iX && aO[1] is string sD)
            oExp = new ListItem<int>(sD, iX);

        testClass.SetPropValue(eAct, oExp);
        if (oExp is ListItem<int> l)
            Assert.AreEqual(l.ItemString, ((string[])testClass.GetPropValue(eAct))[l.ItemData]);
        else if (oExp is not string[] aS)
            Assert.AreEqual(oExp, testClass.GetPropValue(eAct));
        else
            AssertAreEqual(aS, (string[])testClass.GetPropValue(eAct));
    }

    [DataTestMethod()]
    [DataRow((EEventProp)(0 - 1), TypeCode.Int32)]
    [DataRow((EEventProp)23, TypeCode.Int32)]
    [DataRow((EEventProp)100, TypeCode.Int32)]
    public void SetPropValueTest2(EEventProp eExp, object oAct)
    {
        Assert.ThrowsException<NotImplementedException>(() => testClass.SetPropValue(eExp,oAct));
    }

    [TestMethod()]
    public void SetPropValueTest3()
    {
        var eAct = EEventProp.xIsDead;
        Assert.AreEqual(false, testClass.GetPropValue(eAct));
        testClass.SetPropValue(eAct, true);
        Assert.AreEqual(true, testClass.GetPropValue(eAct));
        testClass.SetPropValue(eAct, false);
        Assert.AreEqual(false, testClass.GetPropValue(eAct));
    }

    [DataTestMethod()]
    [DataRow(EEventProp.eArt, (EEventArt)1)]
    [DataRow(EEventProp.eArt, (EEventArt)2)]
    [DataRow(EEventProp.iArtText, 12)]
    [DataRow(EEventProp.iPerFamNr, 3)]
    [DataRow(EEventProp.dDatumV, new int[] { 1900, 01, 01 })]
    [DataRow(EEventProp.sDatumV_S, "DatumV_S_")]
    [DataRow(EEventProp.dDatumB, new int[] { 1900, 01, 02 })]
    [DataRow(EEventProp.sDatumB_S, "DatumB_S_")]
    [DataRow(EEventProp.iKBem, 8)]
    [DataRow(EEventProp.iOrt, 9)]
    [DataRow(EEventProp.iDatumText, 14)]
    [DataRow(EEventProp.sOrt_S, "Ort_S_")]
    [DataRow(EEventProp.sReg, "Reg_")]
    [DataRow(EEventProp.iPlatz, 10)]
    [DataRow(EEventProp.sVChr, "VChr_")]
    [DataRow(EEventProp.iLfNr, 11)]
    [DataRow(EEventProp.sBem, new[] { "", "Bem1", "Bem2-", "Bem3", "Bem4" })]
    [DataRow(EEventProp.sBem, new object[] { 1, "Bem1-" })]
    [DataRow(EEventProp.iCausal, 15)]
    [DataRow(EEventProp.sZusatz, "Zusatz_")]
    [DataRow(EEventProp.iPrivacy, 13)]
    [DataRow(EEventProp.iHausNr, 17)]
    [DataRow(EEventProp.iGrabNr, 18)]
    [DataRow(EEventProp.iAn, 16)]
    [DataRow(EEventProp.xIsDead, true)]
    [DataRow(EEventProp.xIsDead, false)]
    public void SetDBValueTest(EEventProp eAct, object _)
    {
        testClass.SetDBValue(testRS, new[] { $"{eAct}" });
        _ = testRS.Received().Fields[eAct.ToString()];
    }
    [TestMethod()]
    [DataRow((EEventProp)(0 - 1), TypeCode.Int32)]
    [DataRow((EEventProp)23, TypeCode.Int32)]
    [DataRow((EEventProp)100, TypeCode.Int32)]
    public void SetDBValueTest1(EEventProp eAct, object _)
    {
        Assert.ThrowsException<NotImplementedException>(() => testClass.SetDBValue(testRS, new[] { $"{eAct}" }));
    }

    [DataTestMethod()]
//    [DataRow(EEventProp.eArt, (EEventArt)1)]
    [DataRow(EEventProp.eArt, (EEventArt)2)]
    [DataRow(EEventProp.iArtText, 12)]
    [DataRow(EEventProp.iPerFamNr, 3)]
    [DataRow(EEventProp.dDatumV, new int[] { 1900, 01, 01 })]
    [DataRow(EEventProp.sDatumV_S, "DatumV_S_")]
    [DataRow(EEventProp.dDatumB, new int[] { 1900, 01, 02 })]
    [DataRow(EEventProp.sDatumB_S, "DatumB_S_")]
    [DataRow(EEventProp.iKBem, 8)]
    [DataRow(EEventProp.iOrt, 9)]
    [DataRow(EEventProp.iDatumText, 14)]
    [DataRow(EEventProp.sOrt_S, "Ort_S_")]
    [DataRow(EEventProp.sReg, "Reg_")]
    [DataRow(EEventProp.iPlatz, 10)]
    [DataRow(EEventProp.sVChr, "VChr_")]
    [DataRow(EEventProp.iLfNr, 11)]
    [DataRow(EEventProp.sBem, new[] { "", "Bem1", "Bem2-", "Bem3", "Bem4" })]
    [DataRow(EEventProp.sBem, new object[] { 1, "Bem1-" })]
    [DataRow(EEventProp.iCausal, 15)]
    [DataRow(EEventProp.sZusatz, "Zusatz_")]
    [DataRow(EEventProp.iPrivacy, 13)]
    [DataRow(EEventProp.iHausNr, 17)]
    [DataRow(EEventProp.iGrabNr, 18)]
    [DataRow(EEventProp.iAn, 16)]
    [DataRow(EEventProp.xIsDead, true)]
//    [DataRow(EEventProp.xIsDead, false)]
    public void SetDBValueTest2(EEventProp eAct, object oExp)
    {
        SetPropValueTest(eAct, oExp);
        testClass.SetDBValue(testRS, null);
        _ = testRS.Received().Fields[eAct.ToString()];
    }
}