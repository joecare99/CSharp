using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using NSubstitute;
using Microsoft.VisualBasic;
using GenFree.Interfaces;
using GenFree.Interfaces.Model;
using GenFree.Interfaces.DB;
using static BaseLib.Helper.TestHelper;

namespace GenFree.Data.Tests;

[TestClass()]
public class CLinkTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private CLink testClass;
    private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        testRS = Substitute.For<IRecordset>();
        testClass = new CLink(() => testRS);
        testRS.NoMatch.Returns(true);
        testRS.Fields[nameof(ILinkData.LinkFields.Kennz)].Value.Returns(1, 2, 3, 8, 7);
        testRS.Fields[nameof(ILinkData.LinkFields.FamNr)].Value.Returns(1, 3, 5);
        testRS.Fields[nameof(ILinkData.LinkFields.PerNr)].Value.Returns(2, 6, 4, 9);
    }
    [TestMethod()]
    public void CLinkTest()
    {
        Assert.IsNotNull(testClass);
        Assert.IsInstanceOfType(testClass, typeof(ILink));
        Assert.IsInstanceOfType(testClass, typeof(CLink));
    }

    [DataTestMethod()]
    [DataRow("Null", 0, new[] { ELinkKennz.lkNone }, false)]
    [DataRow("1-0None", 1, new[] { ELinkKennz.lkNone }, false)]
    [DataRow("1-1Father", 1, new[] { ELinkKennz.lkFather }, true)]
    public void ExistFamTest(string sName, int iActFam, ELinkKennz[] eLinkKennzs, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3));
        Assert.AreEqual(xExp, testClass.ExistFam(iActFam, eLinkKennzs));
        Assert.AreEqual(nameof(LinkIndex.FamNr), testRS.Index);
        testRS.Received().Seek("=", iActFam);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, ELinkKennz.lkNone, true)]
    [DataRow("1-1Father", 1, ELinkKennz.lkFather, true)]
    public void ExistETest(string sName, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActPers is not (> 0 and < 3));
        Assert.AreEqual(xExp, testClass.ExistE(iActPers, eLinkKennz));
        Assert.AreEqual(nameof(LinkIndex.ElSu), testRS.Index);
        testRS.Received().Seek("=", iActPers, eLinkKennz);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, ELinkKennz.lkNone, true)]
    [DataRow("1-1Father", 1, ELinkKennz.lkFather, true)]
    public void ExistFTest(string sName, int iActFam, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3));
        Assert.AreEqual(xExp, testClass.ExistF(iActFam, eLinkKennz));
        Assert.AreEqual(nameof(LinkIndex.FamSu), testRS.Index);
        testRS.Received().Seek("=", iActFam, eLinkKennz);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, ELinkKennz.lkNone, true)]
    [DataRow("1-1Father", 1, ELinkKennz.lkFather, true)]
    public void ExistPTest(string sName, int iActFam, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3));
        Assert.AreEqual(xExp, testClass.ExistP(iActFam));
        Assert.AreEqual(nameof(LinkIndex.Per), testRS.Index);
        testRS.Received().Seek("=", iActFam);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, -1, -1)]
    [DataRow("1-0None", 1, 2, ELinkKennz.lkNone, -2, 0)]
    [DataRow("1-2Father", 1, 1, ELinkKennz.lkFather, 0, 1)]
    [DataRow("1-2Father", 1, 1, ELinkKennz.lkMother, -2, -2)]
    [DataRow("1-2Father", 1, 1, ELinkKennz.lkFather, -3, -3)]
    public void AppendFamilyParentTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, int iActr, int iExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam);
        Assert.AreEqual(iExp, testClass.AppendFamilyParent(iActFam, iActPers, eLinkKennz, (p, x, k) => iActr));
        Assert.AreEqual(nameof(LinkIndex.FamSu), testRS.Index);
        testRS.Received().Seek("=", iActFam, eLinkKennz);
        if (iExp == 1)
        {
            testRS.Received().AddNew();
            testRS.Received().Update();
        }
    }

    [DataTestMethod()]
    [DataRow("1-2Father", 1, 1, ELinkKennz.lkFather, -3, 1)]
    public void AppendFamilyParentTest2(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, int iActr, int iExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam);
        Assert.AreEqual(iExp, testClass.AppendFamilyParent(iActFam, iActPers, eLinkKennz, null));
        Assert.AreEqual(nameof(LinkIndex.FamSu), testRS.Index);
        testRS.Received().Seek("=", iActFam, eLinkKennz);
        if (iExp == 1)
        {
            testRS.Received().AddNew();
            testRS.Received().Update();
        }
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-1Father", 1, 2, ELinkKennz.lkFather, true)]
    public void DeleteTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, true);
        Assert.AreEqual(xExp, testClass.Delete(iActFam, iActPers, eLinkKennz));
        Assert.AreEqual(nameof(LinkIndex.FamPruef), testRS.Index);
        testRS.Received().Seek("=", iActFam, iActPers, eLinkKennz);
        if (xExp)
            testRS.Received().Delete();
        else
            testRS.DidNotReceive().Delete();
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-1Father", 1, 2, ELinkKennz.lkFather, true)]
    public void Delete2Test(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, true);
        Assert.AreEqual(xExp, testClass.Delete((iActFam, iActPers, eLinkKennz)));
        Assert.AreEqual(nameof(LinkIndex.FamPruef), testRS.Index);
        testRS.Received().Seek("=", iActFam, iActPers, eLinkKennz);
        if (xExp)
            testRS.Received().Delete();
        else
            testRS.DidNotReceive().Delete();
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkFather, true)]
    public void DeleteAllETest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, true);
        testRS.EOF.Returns(false, false, true);
        Assert.AreEqual(xExp, testClass.DeleteAllE(iActPers, eLinkKennz));
        Assert.AreEqual(nameof(LinkIndex.ElSu), testRS.Index);
        testRS.Received().Seek("=", iActPers, eLinkKennz);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkFather, true)]
    public void DeleteAllFTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, true);
        testRS.EOF.Returns(false, false, true);
        Assert.AreEqual(xExp, testClass.DeleteAllF(iActFam, eLinkKennz));
        Assert.AreEqual(nameof(LinkIndex.FamSu), testRS.Index);
        testRS.Received().Seek("=", iActFam, eLinkKennz);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkFather, true)]
    public void DeleteFamWhereTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, false, true);
        testRS.Fields[nameof(ILinkData.LinkFields.FamNr)].Value.Returns(1, 1, 1, 1, iActFam + 1);
        testClass.DeleteFamWhere(iActFam, (l) => l.eKennz == ELinkKennz.lkMother);//);
        Assert.AreEqual(nameof(LinkIndex.FamNr), testRS.Index);
        testRS.Received().Seek("=", iActFam);
        if (xExp)
        {
            testRS.Received().Delete();
        }
        else
        {
            testRS.DidNotReceive().Delete();
        }
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, 0, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, 1, false)]
    [DataRow("1-2Father-1", 1, 2, ELinkKennz.lkFather, -1, true)]
    [DataRow("1-2Father 2", 1, 2, ELinkKennz.lkFather, 2, true)]
    public void DeleteQTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, int iRes, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, false, true);
        testRS.Fields[nameof(ILinkData.LinkFields.FamNr)].Value.Returns(1, 1, 1, 1, iActFam + 1);
        Assert.AreEqual(xExp, testClass.DeleteQ(iActFam, iActPers, eLinkKennz, -1, (f, p) => iRes));//);
        Assert.AreEqual(nameof(LinkIndex.FamPruef), testRS.Index);
        testRS.Received().Seek("=", iActFam, iActPers, eLinkKennz);
        if (xExp && (iRes == -1))
            testRS.Received().Delete();
        else
            testRS.DidNotReceive().Delete();
    }


    [TestMethod()]
    public void MaxIDTest()
    {
        Assert.AreEqual((1, 2, ELinkKennz.lkFather), testClass.MaxID);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, true)]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkFather, false)]
    public void DeleteInvalidPersonTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 == iActFam, false, true);
        testRS.Fields[nameof(ILinkData.LinkFields.PerNr)].Value.Returns(0, 1, 1, 1, iActFam + 1);
        testClass.DeleteInvalidPerson();//);
        Assert.AreEqual(nameof(LinkIndex.Per), testRS.Index);
        testRS.Received().Seek("=", 0);
        if (xExp)
        {
            testRS.Received().Delete();
            testRS.Received().MoveNext();
        }
        else
        {
            testRS.DidNotReceive().Delete();
            testRS.DidNotReceive().MoveNext();
        }
    }


    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, 0, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, 1, false)]
    [DataRow("1-2Father-1", 1, 2, ELinkKennz.lkFather, -1, true)]
    [DataRow("1-2Father 2", 1, 2, ELinkKennz.lkFather, 2, true)]
    public void SetVerknQTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, int iRes, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, false, true);
        testRS.Fields[nameof(ILinkData.LinkFields.FamNr)].Value.Returns(1, 1, 1, 1, iActFam + 1);
        Assert.AreEqual(xExp, testClass.SetVerknQ(iActFam, iActPers, eLinkKennz, -1, (f, p) => iRes));//);
        Assert.AreEqual(nameof(LinkIndex.FamPruef), testRS.Index);
        testRS.Received().Seek("=", iActFam, iActPers, eLinkKennz);
        if (xExp && (iRes == -1))
            testRS.Received().Delete();
        else
            testRS.DidNotReceive().Delete();
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, 0, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, 1, false)]
    [DataRow("1-2Father-1", 1, 2, ELinkKennz.lkFather, -1, true)]
    [DataRow("1-2Father 2", 1, 2, ELinkKennz.lkFather, 2, true)]
    public void SetEQTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, int iRes, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, false, true);
        testRS.Fields[nameof(ILinkData.LinkFields.FamNr)].Value.Returns(1, 1, 1, 1, iActFam + 1);
        Assert.AreEqual(xExp, testClass.SetEQ(iActFam, iActPers, eLinkKennz, -1, (f, p) => iRes));//);
        Assert.AreEqual(nameof(LinkIndex.ElSu), testRS.Index);
        testRS.Received().Seek("=", iActPers, eLinkKennz);
        if (xExp && (iRes == -1))
            testRS.Received().Delete();
        else
            testRS.DidNotReceive().Delete();
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, true)]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkFather, true)]
    public void AppendTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, true);
        //Assert.AreEqual(xExp, 
        testClass.Append(iActFam, iActPers, eLinkKennz);//);
        Assert.AreEqual(nameof(LinkIndex.FamPruef), testRS.Index);
        testRS.Received().Seek("=", iActFam, iActPers, eLinkKennz);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, true)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, true)]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkFather, false)]
    public void AppendETest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, true);
        Assert.AreEqual(xExp, testClass.AppendE(iActFam, iActPers, eLinkKennz));
        Assert.AreEqual(nameof(LinkIndex.ElSu), testRS.Index);
        testRS.Received().Seek("=", iActPers, eLinkKennz);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkFather, true)]
    public void ExistTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, true);
        Assert.AreEqual(xExp, testClass.Exist(iActFam, iActPers, eLinkKennz));
        Assert.AreEqual(nameof(LinkIndex.FamPruef), testRS.Index);
        testRS.Received().Seek("=", iActFam, iActPers, eLinkKennz);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkFather, true)]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkMother, true)]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkChild, true)]
    public void ReadFamilyTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, false, false, false, false, true);
        testRS.Fields[nameof(ILinkData.LinkFields.FamNr)].Value.Returns(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, iActFam + 1);
        var iCnt = 0;
        Action<ELinkKennz, int>? _testAct = eLinkKennz switch
        {
            ELinkKennz.lkFather => testAct,
            ELinkKennz.lkMother => (e, i) => { iCnt++; },
            _ => null
        };
        IFamilyPersons testFamily = Substitute.For<IFamilyPersons>();
        //Assert.AreEqual(xExp, 
        testClass.ReadFamily(iActFam, testFamily, _testAct);//);
        Assert.AreEqual(nameof(LinkIndex.FamNr), testRS.Index);
        testRS.Received().Seek("=", iActFam);
        if (!xExp)
        {
            testRS.Received(0).MoveNext();
            testFamily.Kinder.Received().Add((0, ""));
            Assert.AreEqual(0, testFamily.Mann);
            Assert.AreEqual(0, testFamily.Frau);
            Assert.AreEqual(0, iCnt);
        }
        else
        {
            testFamily.Kinder.Received().Add((0, ""));
            testRS.Received(5).MoveNext();
            Assert.AreEqual(2, testFamily.Mann);
            Assert.AreEqual(6, testFamily.Frau);
            testFamily.Kinder.Received().Add((4, ""));
            testFamily.Kinder.Received().Add((9, "A"));
            if (eLinkKennz== ELinkKennz.lkMother)
                Assert.AreEqual(1, iCnt);
            else
                Assert.AreEqual(0, iCnt);
        }

    }

    private void testAct(ELinkKennz kennz, int arg2)
    {
        throw new NotImplementedException();
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, MsgBoxResult.Ok)]
    [DataRow("1-0None", 1, 2, ELinkKennz.lkNone, MsgBoxResult.Cancel)]
    [DataRow("1-2Father", 2, 5, ELinkKennz.lkFather, MsgBoxResult.Ok)]
    public void DeleteChildrenTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, MsgBoxResult eExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, true);
        testRS.Fields[nameof(ILinkData.LinkFields.PerNr)].Value.Returns(iActPers, 6, 4);
        Assert.AreEqual(eExp, testClass.DeleteChildren(iActFam, eLinkKennz, iActPers, MsgBoxResult.Ok, (i) => i % 2 == 0 ? MsgBoxResult.Cancel : MsgBoxResult.Ok));
        Assert.AreEqual(nameof(LinkIndex.FamPruef), testRS.Index);
        testRS.Received().Seek("=", iActFam, iActPers, eLinkKennz);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, false)]
    [DataRow("0-1None", 0, 1, ELinkKennz.lkNone, false)]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkFather, true)]
    public void GetPersonFamTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, true);
        Assert.AreEqual(xExp, testClass.GetPersonFam(iActPers, eLinkKennz, out int iFam));
        Assert.AreEqual(iActFam, iFam);
        Assert.AreEqual(nameof(LinkIndex.ElSu), testRS.Index);
        testRS.Received().Seek("=", iActPers, eLinkKennz);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, new int[0])]
    [DataRow("0-1None", 0, 1, ELinkKennz.lkNone, new int[0])]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkFather, new[] { 1 })]
    [DataRow("2-2Father", 2, 2, ELinkKennz.lkFather, new int[0])]
    public void GetPersonFamsTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, int[] aiExp)
    {
        CLinkData.SetLinkTblGetter(() => testRS);
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, true);
        AssertAreEqual(aiExp.ToList(), testClass.GetPersonFams(iActPers, eLinkKennz));
        //   Assert.AreEqual(iActFam, iFam);
        Assert.AreEqual(nameof(LinkIndex.ElSu), testRS.Index);
        if (eLinkKennz == ELinkKennz.lkNone)
            testRS.Received().Seek("=", iActPers);
        else
            testRS.Received().Seek("=", iActPers, eLinkKennz);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkFather, true)]
    public void GetFamPersonTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, true);
        Assert.AreEqual(xExp, testClass.GetFamPerson(iActFam, eLinkKennz, out int iPers));
        Assert.AreEqual(iActPers, iPers);
        Assert.AreEqual(nameof(LinkIndex.FamSu), testRS.Index);
        testRS.Received().Seek("=", iActFam, eLinkKennz);
    }

    [DataTestMethod()]
    [DataRow("Null", 0, 0, ELinkKennz.lkNone, false)]
    [DataRow("1-0None", 1, 0, ELinkKennz.lkNone, true)]
    [DataRow("1-2Father", 1, 2, ELinkKennz.lkFather, true)]
    public void DeleteFamTest(string sName, int iActFam, int iActPers, ELinkKennz eLinkKennz, bool xExp)
    {
        testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iActPers / 2 != iActFam, true);
        //Assert.AreEqual(xExp, 
        testClass.DeleteFam(iActFam, eLinkKennz);//);
                                                 //Assert.AreEqual(iActPers, iPers);
        Assert.AreEqual(nameof(LinkIndex.FamSu), testRS.Index);
        testRS.Received().Seek("=", iActFam, eLinkKennz);
    }
}
