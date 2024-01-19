using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Helper;
using BaseLib.Helper;
using GenFree.Interfaces;
using static BaseLib.Helper.TestHelper;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CFamilyPersonsTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CFamilyPersons testClass;
        private IRecordset testRS;
        private Guid _guid;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testRS.NoMatch.Returns(true);
            testRS.Fields[nameof(FamilyFields.AnlDatum)].Value.Returns(new DateTime(1980, 12, 31));
            testRS.Fields[nameof(FamilyFields.EditDat)].Value.Returns(new DateTime(2024, 01, 01));
            testRS.Fields[nameof(FamilyFields.Prüfen)].Value.Returns("Pruefen");
            testRS.Fields[nameof(FamilyFields.Bem1)].Value.Returns("Bem1");
            testRS.Fields[nameof(FamilyFields.FamNr)].Value.Returns(3, 4, 5);
            testRS.Fields[nameof(FamilyFields.Aeb)].Value.Returns(4, 5, 6);
            testRS.Fields[nameof(FamilyFields.Name)].Value.Returns(5, 6, 7);
            testRS.Fields[nameof(FamilyFields.Bem2)].Value.Returns("Bem2");
            testRS.Fields[nameof(FamilyFields.Bem3)].Value.Returns("Bem3");
            testRS.Fields[nameof(FamilyFields.Eltern)].Value.Returns(6, 7, 8);
            testRS.Fields[nameof(FamilyFields.Fuid)].Value.Returns(_guid = new Guid("0123456789ABCDEF0123456789ABCDEF"));
            testRS.Fields[nameof(FamilyFields.Prae)].Value.Returns(7, 8, 9);
            testRS.Fields[nameof(FamilyFields.Suf)].Value.Returns(8, 9, 10);
            testRS.Fields[nameof(FamilyFields.ggv)].Value.Returns(9, 10, 11);
            testClass = new(testRS);
            CFamilyPersons.SetGetText(getTextFnc);
            testRS.ClearReceivedCalls();
        }
        private string getTextFnc(int arg)
        {
            return $"Text_{arg}";
        }

        [TestMethod()]
        public void SetTableTest()
        {
            var testTable = Substitute.For<IRecordset>();
            CFamilyPersons.SetTableGtr(() => testRS);
        }

        [TestMethod()]
        public void CFamilyPersonsTest()
        {
            var testClass = new CFamilyPersons();
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(IFamilyData));
            Assert.AreEqual(0, testClass.ID);
            Assert.AreEqual(0, testClass.Mann);
            Assert.AreEqual(0, testClass.Frau);
            Assert.AreEqual(0, testClass.Kinder.Count);
        }

        [TestMethod()]
        public void CFamilyPersonsTest1()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(IFamilyData));
        }

        [DataTestMethod()]
        [DataRow(3,6)]
        [DataRow(4, 8)]
        public void FillDataTest(EFamilyProp eProp, object oExp)
        {
            testClass.FillData(testRS);
            Assert.AreEqual(oExp, testClass.GetPropValue(eProp));
        }

        [TestMethod()]
        public void ClearTest()
        {
            testClass.Mann = 1;
            testClass.Frau = 2;
            testClass.Clear();
            Assert.AreEqual(0, testClass.Mann);
            Assert.AreEqual(0, testClass.Frau);
            Assert.AreEqual(0, testClass.Kinder.Count);
        }

        [DataTestMethod()]
        [DataRow(false)]
        [DataRow(true)]
        public void CheckSetAnlDatumTest(bool xAct)
        {
            if (xAct) testClass.SetPropValue(EFamilyProp.dAnlDatum, DateTime.MinValue);
            testClass.CheckSetAnlDatum(testRS);
            testRS.Received(xAct?1:0).Edit();
            testRS.Received(xAct?1:0).Update();
        }

        [DataTestMethod()]
        [DataRow(EFamilyProp.ID, 3)]
        [DataRow(EFamilyProp.ID, 4)]
        [DataRow(EFamilyProp.iName, 6)]
        [DataRow(EFamilyProp.sBem, new[] { "", "Bem1-", "Bem2-", "Bem3" })]
        [DataRow(EFamilyProp.sBem, new object[] { 1, "Bem1-" })]
        [DataRow(EFamilyProp.dAnlDatum, new[] { 1980, 12, 30 })]
        [DataRow(EFamilyProp.dEditDat, new[] { 2024, 01, 02 })]
        [DataRow(EFamilyProp.iPrae, 8)]
        [DataRow(EFamilyProp.iSuf, 9)]
        [DataRow(EFamilyProp.iEltern, 7)]
        [DataRow(EFamilyProp.gUID, "01234560-89AB-CDEF-0123-456789ABCDEF")]
        [DataRow(EFamilyProp.iGgv, 10)]
        [DataRow(EFamilyProp.xAeB, false)]
        [DataRow(EFamilyProp.sPruefen, "Pruefe")]
        public void SetDBValueTest(EFamilyProp eAct, object _)
        {
            testClass.SetDBValue(testRS, new[] { $"{eAct}" });
            _ = testRS.Received().Fields[eAct.ToString()];
        }
        [TestMethod()]
        [DataRow((EFamilyProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EFamilyProp)12, TypeCode.Int32)]
        [DataRow((EFamilyProp)100, TypeCode.Int32)]
        public void SetDBValueTest1(EFamilyProp eAct, object _)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetDBValue(testRS, new[] { $"{eAct}" }));
        }

        [DataTestMethod()]
        [DataRow(EFamilyProp.ID, 4)]
        [DataRow(EFamilyProp.iName, 6)]
        [DataRow(EFamilyProp.sBem, new[] { "", "Bem1-", "Bem2-", "Bem3" })]
        [DataRow(EFamilyProp.sBem, new object[] { 1, "Bem1-" })]
        [DataRow(EFamilyProp.dAnlDatum, new[] { 1980, 12, 30 })]
        [DataRow(EFamilyProp.dEditDat, new[] { 2024, 01, 02 })]
        [DataRow(EFamilyProp.iPrae, 8)]
        [DataRow(EFamilyProp.iSuf, 9)]
        [DataRow(EFamilyProp.iEltern, 7)]
        [DataRow(EFamilyProp.gUID, "01234560-89AB-CDEF-0123-456789ABCDEF")]
        [DataRow(EFamilyProp.iGgv, 10)]
        [DataRow(EFamilyProp.xAeB, false)]
        [DataRow(EFamilyProp.sPruefen, "Pruefe")]
        public void SetDBValueTest2(EFamilyProp eAct, object oExp)
        {
            SetPropValueTest(eAct, oExp);
            testClass.SetDBValue(testRS, null );
            _ = testRS.Received().Fields[eAct.ToString()];
        }

        [DataTestMethod()]
        [DataRow(false)]
        [DataRow(true)]
        public void DeleteTest(bool xAct)
        {
            testRS.NoMatch.Returns(xAct);
            testClass.Delete();
            Assert.AreEqual("Fam", testRS.Index);
            testRS.Received(xAct?0:1).Delete();
            testRS.Received(1).Seek("=",testClass.ID);
        }

        [TestMethod()]
        public void KinderTest()
        {
            testClass.Kinder.Add((1,"A"));
            Assert.AreEqual(1,testClass.Kind[0]);
            Assert.AreEqual("A",testClass.KiAText[0]);
        }

        [DataTestMethod()]
        [DataRow("sName","Text_5")]
        [DataRow("sPrefix", "Text_7")]
        [DataRow("sSuffix", "Text_8")]
        public void GetTextTest(string sProp,string sExp)
        {
            Assert.AreEqual(sExp, testClass.GetProp(sProp));
        }



        [DataTestMethod()]
        [DataRow(EFamilyProp.ID, TypeCode.Int32)]
        [DataRow(EFamilyProp.iName, TypeCode.Int32)]
        [DataRow(EFamilyProp.sBem, TypeCode.Object)]
        [DataRow(EFamilyProp.dAnlDatum, TypeCode.DateTime)]
        [DataRow(EFamilyProp.dEditDat, TypeCode.DateTime)]
        [DataRow(EFamilyProp.iPrae, TypeCode.Int32)]
        [DataRow(EFamilyProp.iSuf, TypeCode.Int32)]
        [DataRow(EFamilyProp.iEltern, TypeCode.Int32)]
        [DataRow(EFamilyProp.gUID, TypeCode.Object)]
        [DataRow(EFamilyProp.iGgv, TypeCode.Int32)]
        [DataRow(EFamilyProp.xAeB, TypeCode.Boolean)]
        [DataRow(EFamilyProp.sPruefen, TypeCode.String)]
        public void GetPropTypeTest(EFamilyProp pAct, TypeCode eExp)
        {
            Assert.AreEqual(eExp, Type.GetTypeCode(testClass.GetPropType(pAct)));
        }

        [TestMethod()]
        [DataRow((EFamilyProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EFamilyProp)12, TypeCode.Int32)]
        [DataRow((EFamilyProp)100, TypeCode.Int32)]
        public void GetPropTypeTest1(EFamilyProp pAct, TypeCode eExp)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropType(pAct));
        }

        [TestMethod()]
        [DataRow(EFamilyProp.ID, 3)]
        [DataRow(EFamilyProp.iName, 5)]
        [DataRow(EFamilyProp.sBem, new[] {"", "Bem1", "Bem2", "Bem3" })]
        [DataRow(EFamilyProp.dAnlDatum,new[] { 1980, 12, 31 })]
        [DataRow(EFamilyProp.dEditDat, new[] { 2024, 01, 01 })]
        [DataRow(EFamilyProp.iPrae, 7)]
        [DataRow(EFamilyProp.iSuf, 8)]
        [DataRow(EFamilyProp.iEltern, 6)]
        [DataRow(EFamilyProp.gUID, "01234567-89AB-CDEF-0123-456789ABCDEF")]
        [DataRow(EFamilyProp.iGgv, 9)]
        [DataRow(EFamilyProp.xAeB, true)]
        [DataRow(EFamilyProp.sPruefen, "Pruefen")]

        public void GetPropValueTest(EFamilyProp eExp, object? oAct)
        {
            if (oAct is int[] aiAct && aiAct.Length == 3 && aiAct[1] <= 12 && aiAct[2] <= 31)
                oAct = new DateTime(aiAct[0], aiAct[1], aiAct[2]);
            else if (oAct is string s && s.Length == 36 && Guid.TryParse(s, out var g))
                oAct = g;
            if (oAct is not string[] aS)
                Assert.AreEqual(oAct, testClass.GetPropValue(eExp));
            else
                AssertAreEqual(aS, (string[])testClass.GetPropValue(eExp));
        }

        [TestMethod()]
        [DataRow((EFamilyProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EFamilyProp)13, TypeCode.Int32)]
        [DataRow((EFamilyProp)100, TypeCode.Int32)]
        public void GetPropValueTest2(EFamilyProp eExp, object? oAct)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropValue(eExp));
        }

        [TestMethod()]
        public void GetPropValueTest1()
        {
            Assert.AreEqual(3, testClass.GetPropValue<int>(EFamilyProp.ID));
        }

        [DataTestMethod()]
        [DataRow(EFamilyProp.ID, 3)]
        [DataRow(EFamilyProp.ID, 4)]
        [DataRow(EFamilyProp.iName, 6)]
        [DataRow(EFamilyProp.sBem, new[] { "", "Bem1-", "Bem2-", "Bem3" })]
        [DataRow(EFamilyProp.sBem, new object[] { 1, "Bem1-"})]
        [DataRow(EFamilyProp.dAnlDatum, new[] { 1980, 12, 30 })]
        [DataRow(EFamilyProp.dEditDat, new[] { 2024, 01, 02 })]
        [DataRow(EFamilyProp.iPrae, 8)]
        [DataRow(EFamilyProp.iSuf, 9)]
        [DataRow(EFamilyProp.iEltern, 7)]
        [DataRow(EFamilyProp.gUID, "01234560-89AB-CDEF-0123-456789ABCDEF")]
        [DataRow(EFamilyProp.iGgv, 10)]
        [DataRow(EFamilyProp.xAeB, false)]
        [DataRow(EFamilyProp.sPruefen, "Pruefe")]
        public void SetPropValueTest(EFamilyProp eAct, object oAct)
        {
            if (oAct is int[] aiAct && aiAct.Length == 3 && aiAct[1] <= 12 && aiAct[2] <= 31)
                oAct = new DateTime(aiAct[0], aiAct[1], aiAct[2]);
            else if (oAct is string s && s.Length == 36 && Guid.TryParse(s, out var g))
                oAct = g;
            else if (oAct is object[] aO && aO.Length == 2 && aO[0] is int iX && aO[1] is string sD)
                oAct = new ListItem<int>(sD,iX);

            testClass.SetPropValue(eAct, oAct);
            if (oAct is ListItem<int> l)
                Assert.AreEqual(l.ItemString, ((string[])testClass.GetPropValue(eAct))[l.ItemData]);
            else if (oAct is not string[] aS)
                Assert.AreEqual(oAct, testClass.GetPropValue(eAct));
            else
                AssertAreEqual(aS, (string[])testClass.GetPropValue(eAct));
        }

        [TestMethod()]
        [DataRow((EFamilyProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EFamilyProp)13, TypeCode.Int32)]
        [DataRow((EFamilyProp)100, TypeCode.Int32)]
        public void SetPropValueTest1(EFamilyProp eExp, object? oAct)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetPropValue(eExp, oAct));
        }

        [TestMethod()]
        public void AddChangedPropTest()
        {
            testClass.AddChangedProp(EFamilyProp.ID);
            testClass.AddChangedProp(EFamilyProp.iName);
            testClass.AddChangedProp(EFamilyProp.ID);
            Assert.AreEqual(2, testClass.ChangedProps.Count);
        }

        [TestMethod()]
        public void ClearChangedPropsTest()
        {
            AddChangedPropTest();
            testClass.ClearChangedProps();
            Assert.AreEqual(0, testClass.ChangedProps.Count);
        }
    }
}