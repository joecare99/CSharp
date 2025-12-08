using BaseLib.Interfaces;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CRepoDataTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CRepoData testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testRS.NoMatch.Returns(true);
            (testRS.Fields[RepoFields.Nr] as IHasValue).Value.Returns(1, 2, 3);
            (testRS.Fields[RepoFields.Ort] as IHasValue).Value.Returns(2, 3, 4);
            (testRS.Fields[RepoFields.PLZ] as IHasValue).Value.Returns(3, 4, 5);
            (testRS.Fields[RepoFields.Fon] as IHasValue).Value.Returns(4, 5, 6);
            (testRS.Fields[RepoFields.Mail] as IHasValue).Value.Returns(5, 6, 7);
            (testRS.Fields[RepoFields.Http] as IHasValue).Value.Returns(6, 7, 8);
            (testRS.Fields[RepoFields.Bem] as IHasValue).Value.Returns(7, 8, 9);
            (testRS.Fields[RepoFields.Suchname] as IHasValue).Value.Returns(8,9,10);
            (testRS.Fields[RepoFields.Name] as IHasValue).Value.Returns(9,10,11);
            (testRS.Fields[RepoFields.Strasse] as IHasValue).Value.Returns(10, 11, 12);
            testClass = new(testRS);
            testRS.ClearReceivedCalls();
        }


        [TestMethod()]
        public void CRepoDataTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(CRepoData));
            Assert.IsInstanceOfType(testClass, typeof(IRepoData));
        }

        [TestMethod()]
        public void ReadIDTest()
        {
            testClass.ReadID(testRS);
            Assert.AreEqual(2, testClass.ID);
            testRS.Received(1).Fields[RepoFields.Nr].Value = 0;
        }

        [TestMethod()]
        [DataRow(ERepoProp.ID, 2)]
        [DataRow(ERepoProp.sOrt, "3")]
        [DataRow(ERepoProp.sPLZ, "4")]
        [DataRow(ERepoProp.sFon, "5")]
        [DataRow(ERepoProp.sMail, "6")]
        [DataRow(ERepoProp.sHttp, "7")]
        [DataRow(ERepoProp.sBem, "8")]
        [DataRow(ERepoProp.sSuchname, "9")]
        [DataRow(ERepoProp.sName, "10")]
        [DataRow(ERepoProp.sStrasse, "11")]

        public void FillDataTest(ERepoProp eProp, object oExp)
        {
            testClass.FillData(testRS);
            Assert.AreEqual(oExp, testClass.GetPropValue(eProp));
        }

        [TestMethod()]
        [DataRow(ERepoProp.ID, TypeCode.Int32)]
        [DataRow(ERepoProp.sOrt, TypeCode.String)]
        [DataRow(ERepoProp.sPLZ, TypeCode.String)]
        [DataRow(ERepoProp.sFon, TypeCode.String)]
        [DataRow(ERepoProp.sMail, TypeCode.String)]
        [DataRow(ERepoProp.sHttp, TypeCode.String)]
        [DataRow(ERepoProp.sBem, TypeCode.String)]
        [DataRow(ERepoProp.sSuchname, TypeCode.String)]
        [DataRow(ERepoProp.sName, TypeCode.String)]
        [DataRow(ERepoProp.sStrasse, TypeCode.String)]
        public void GetPropTypeTest(ERepoProp pAct, TypeCode eExp)
        {
            Assert.AreEqual(eExp, Type.GetTypeCode(testClass.GetPropType(pAct)));
        }

        [TestMethod()]
        [DataRow((ERepoProp)(0 - 1), TypeCode.Int32)]
        [DataRow((ERepoProp)10, TypeCode.String)]
        [DataRow((ERepoProp)100, TypeCode.DateTime)]
        public void GetPropTypeTest2(ERepoProp pAct, TypeCode eExp)
        {
            Assert.ThrowsExactly<NotImplementedException>(() => testClass.GetPropType(pAct));
        }

        [TestMethod()]
        [DataRow(ERepoProp.ID, 1)]
        [DataRow(ERepoProp.sOrt, "2")]
        [DataRow(ERepoProp.sPLZ, "3")]
        [DataRow(ERepoProp.sFon, "4")]
        [DataRow(ERepoProp.sMail, "5")]
        [DataRow(ERepoProp.sHttp, "6")]
        [DataRow(ERepoProp.sBem, "7")]
        [DataRow(ERepoProp.sSuchname, "8")]
        [DataRow(ERepoProp.sName, "9")]
        [DataRow(ERepoProp.sStrasse, "10")]
        public void GetPropValueTest(ERepoProp eProp, object oExp)
        {
            Assert.AreEqual(oExp, testClass.GetPropValue(eProp));
        }

        [TestMethod()]
        [DataRow((ERepoProp)(0 - 1), TypeCode.Int32)]
        [DataRow((ERepoProp)10, TypeCode.String)]
        [DataRow((ERepoProp)100, TypeCode.DateTime)]
        public void GetPropValueTest2(ERepoProp eProp, object oExp)
        {
            Assert.ThrowsExactly<NotImplementedException>(() => testClass.GetPropValue(eProp));
        }

        [TestMethod()]
        [DataRow(ERepoProp.ID, 1)]
        [DataRow(ERepoProp.ID, 2)]
        [DataRow(ERepoProp.sOrt, "3")]
        [DataRow(ERepoProp.sPLZ, "4")]
        [DataRow(ERepoProp.sFon, "5")]
        [DataRow(ERepoProp.sMail, "6")]
        [DataRow(ERepoProp.sHttp, "7")]
        [DataRow(ERepoProp.sBem, "8")]
        [DataRow(ERepoProp.sSuchname, "9")]
        [DataRow(ERepoProp.sName, "10")]
        [DataRow(ERepoProp.sStrasse, "11")]
        public void SetPropValueTest(ERepoProp eAct, object iVal)
        {
            testClass.SetPropValue(eAct, iVal);
            Assert.AreEqual(iVal, testClass.GetPropValue(eAct));
        }

        [TestMethod()]
        [DataRow((ERepoProp)(0 - 1), TypeCode.Int32)]
        [DataRow((ERepoProp)10, TypeCode.String)]
        [DataRow((ERepoProp)100, TypeCode.DateTime)]
        public void SetPropValueTest1(ERepoProp eAct, object iVal)
        {
            Assert.ThrowsExactly<NotImplementedException>(() => testClass.SetPropValue(eAct, iVal));
        }


        [TestMethod()]
        [DataRow(ERepoProp.ID, 1)]
        [DataRow(ERepoProp.ID, 2)]
        [DataRow(ERepoProp.sOrt, "3")]
        [DataRow(ERepoProp.sPLZ, "4")]
        [DataRow(ERepoProp.sFon, "5")]
        [DataRow(ERepoProp.sMail, "6")]
        [DataRow(ERepoProp.sHttp, "7")]
        [DataRow(ERepoProp.sBem, "8")]
        [DataRow(ERepoProp.sSuchname, "9")]
        [DataRow(ERepoProp.sName, "10")]
        [DataRow(ERepoProp.sStrasse, "11")]

        public void SetDBValueTest(ERepoProp eAct, object _)
        {
            testClass.SetDBValues(testRS, new[] { (Enum)eAct });
            _ = testRS.Received(1).Fields[eAct.ToString()];
        }

        [TestMethod()]
        [DataRow((ERepoProp)(0 - 1), TypeCode.Int32)]
        [DataRow((ERepoProp)10, TypeCode.String)]
        [DataRow((ERepoProp)100, TypeCode.DateTime)]
        public void SetDBValueTest1(ERepoProp eAct, object _)
        {
            Assert.ThrowsExactly<NotImplementedException>(() => testClass.SetDBValues(testRS, new[] { (Enum)eAct }));
        }

        [TestMethod()]
        [DataRow(ERepoProp.ID, 2)]
        [DataRow(ERepoProp.sOrt, "3")]
        [DataRow(ERepoProp.sPLZ, "4")]
        [DataRow(ERepoProp.sFon, "5")]
        [DataRow(ERepoProp.sMail, "6")]
        [DataRow(ERepoProp.sHttp, "7")]
        [DataRow(ERepoProp.sBem, "8")]
        [DataRow(ERepoProp.sSuchname, "9")]
        [DataRow(ERepoProp.sName, "10")]
        [DataRow(ERepoProp.sStrasse, "11")]
        public void SetDBValueTest2(ERepoProp eAct, object oVal)
        {
            testClass.SetPropValue(eAct, oVal);
            testClass.SetDBValues(testRS, null);
            _ = testRS.Received().Fields[eAct];
        }


    }
}