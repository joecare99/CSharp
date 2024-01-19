using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Helper;
using GenFree.Interfaces;
using static BaseLib.Helper.TestHelper;


namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CPersonDataTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CPersonData testClass;
        private IRecordset testRS;
        private Guid _guid;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testRS.NoMatch.Returns(true);
            testRS.Fields[PersonFields.PersNr.AsFld()].Value.Returns(1, 2, 3);
            testRS.Fields[PersonFields.OFB.AsFld()].Value.Returns("OFB");
            testRS.Fields[PersonFields.Pruefen.AsFld()].Value.Returns("Pruefen");
            testRS.Fields[PersonFields.Such1.AsFld()].Value.Returns("Such1");
            testRS.Fields[PersonFields.Such2.AsFld()].Value.Returns("Such2");
            testRS.Fields[PersonFields.Such3.AsFld()].Value.Returns("Such3");
            testRS.Fields[PersonFields.Such4.AsFld()].Value.Returns("Such4");
            testRS.Fields[PersonFields.Such5.AsFld()].Value.Returns("Such5");
            testRS.Fields[PersonFields.Such6.AsFld()].Value.Returns("Such6");
            testRS.Fields[PersonFields.Sex.AsFld()].Value.Returns("Sex");
            testRS.Fields[PersonFields.Konv.AsFld()].Value.Returns("Konv");
            testRS.Fields[PersonFields.religi.AsFld()].Value.Returns(2, 3, 4);
            testRS.Fields[PersonFields.Bem1.AsFld()].Value.Returns("Bem1");
            testRS.Fields[PersonFields.Bem2.AsFld()].Value.Returns("Bem2");
            testRS.Fields[PersonFields.Bem3.AsFld()].Value.Returns("Bem3");
            testRS.Fields[PersonFields.PUid.AsFld()].Value.Returns(_guid = new Guid("0123456789ABCDEF0123456789ABCDEF"));
            testRS.Fields[PersonFields.EditDat.AsFld()].Value.Returns(20000304, 20100506, 5);
            testRS.Fields[PersonFields.AnlDatum.AsFld()].Value.Returns(19990101, 20050708, 6);
            testClass = new(testRS);
            CPersonData.SetGetText(getTextFnc);
            CPersonData.SetGetText2(getTextFnc2);
            testRS.ClearReceivedCalls();
        }
        private string getTextFnc(int arg)
        {
            return $"Text_{arg}";
        }
        private (string,string) getTextFnc2(int arg)
        {
            return ($"Text_{arg}", $"Text2_{arg}");
        }

        [TestMethod()]
        public void CPersonDataTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(CPersonData));
            Assert.IsInstanceOfType(testClass, typeof(IPersonData));

        }

        [DataTestMethod()]
        [DataRow(0, 2)]
        [DataRow(2, 3)]
        public void FillDataTest(EPersonProp eProp, object oExp)
        {
            testClass.FillData(testRS);
            Assert.AreEqual(oExp, testClass.GetPropValue(eProp));
        }

        [TestMethod()]
        public void CPersonDataTest1()
        {
            CPersonData.SetDataFkc(() => testRS);
            var testClass = new CPersonData(1); 
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(CPersonData));
            Assert.IsInstanceOfType(testClass, typeof(IPersonData));
        }

        [TestMethod()]
        public void CPersonDataTest2()
        {
            CPersonData.SetDataFkc(() => testRS);
            var testClass = new CPersonData();
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(CPersonData));
            Assert.IsInstanceOfType(testClass, typeof(IPersonData));
        }

        [TestMethod()]
        public void SetPersonNrTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetFullSurnameTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetFullTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetDatesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetDatesTest1()
        {
            testClass.SetDates(new DateTime[] {default,
                new DateTime(1900, 1, 2), new DateTime(1901, 3, 4), new DateTime(1980, 5, 6), new DateTime(1981, 7, 8) });
            Assert.AreEqual(new DateTime(1900, 1, 2), testClass.dBirth);
            Assert.AreEqual(new DateTime(1901, 3, 4), testClass.dBaptised);
            Assert.AreEqual(new DateTime(1980, 5, 6), testClass.dDeath);
            Assert.AreEqual(new DateTime(1981, 7, 8), testClass.dBurial);
        }

        [TestMethod()]
        public void ClearTest()
        {
            testClass.Clear();
            Assert.AreEqual(1, testClass.ID);
        }

        [DataTestMethod()]
        [DataRow(new[] {0,1,2,3,4,5,6,7,8 }, new object[] { new object[] {2,false,false } },false, "Text_2")]
        [DataRow(new[] {2,3,4,5,6,7,8,9 }, new object[] { new object[] {1,false,false }, new object[] { 4, false, false } },false, "Text_1 Text_4")]
        [DataRow(new[] {2,3,4,5,6,7,8,9 }, new object[] { new object[] {1,false,false }, new object[] { 4, false, false } },true, "Text_1 >Text2_1< Text_4 >Text2_4<")]
        [DataRow(new[] {1,2,3,4,5,6,7,8 }, new object[] { new object[] {2,false,true }, new object[] { 3, true, false }, },false, "'Text_2' \"Text_3\"")]
        public void SetPersonNamesTest(int[] aiAct, object[] aoAct,bool x,string sExp)
        {
            List<(int,bool,bool)> alVN = new() { default };
            foreach (var o in aoAct)
                if (o is object[] ao && ao.Length==3)
                    alVN.Add((ao[0].AsInt(), ao[1].AsBool(), ao[2].AsBool()));
            testClass.SetPersonNames(aiAct, alVN.ToArray(),x);
            Assert.AreEqual($"Text_{aiAct[1]}",testClass.SurName);
            Assert.AreEqual($"Text_{aiAct[2]}", testClass.Prefix);
            Assert.AreEqual($"Text_{aiAct[3]}",testClass.Suffix);
            Assert.AreEqual(sExp,testClass.Givennames);
        }

        [DataTestMethod()]
        [DataRow(EEventArt.eA_Birth, new[] { 1980, 12, 30 })]
        [DataRow(EEventArt.eA_Baptism, new[] { 1981, 11, 29 })]
        [DataRow(EEventArt.eA_Death, new[] { 1982, 10, 28 })]
        [DataRow(EEventArt.eA_Death, new[] { 1982, 10, 28 },true)]
        [DataRow(EEventArt.eA_Burial, new[] { 1983, 9, 27 })]
        [DataRow(EEventArt.eA_105, new[] { 1984, 8, 26 })]
        public void SetDataTest(EEventArt eArt,object oAct,bool xD=false)
        {
            var cEv = Substitute.For<IEventData>();
            cEv.eArt.Returns(eArt);
            if (oAct is int[] aiAct && aiAct.Length == 3 && aiAct[1] <= 12 && aiAct[2] <= 31)
                oAct = new DateTime(aiAct[0], aiAct[1], aiAct[2]);
            cEv.dDatumV.Returns((DateTime)oAct);
            cEv.xIsDead.Returns(xD);
            testClass.SetData(cEv);
            switch (eArt)
            {
                case EEventArt.eA_Birth:
                    Assert.AreEqual(oAct, testClass.dBirth);
                    break;
                case EEventArt.eA_Baptism:
                    Assert.AreEqual(oAct, testClass.dBaptised);
                    break;
                case EEventArt.eA_Death:
                    Assert.AreEqual(true, testClass.xDead);
                    Assert.AreEqual(oAct, testClass.dDeath);
                    break;
                case EEventArt.eA_Burial:
                    Assert.AreEqual("J", testClass.sBurried);
                    Assert.AreEqual(oAct, testClass.dBurial);
                    break;
                default:
                    break;
            }
        }

        [TestMethod()]
        public void UpdateTest()
        {
            Assert.Fail();
        }

        [DataTestMethod()]
        [DataRow("Sex")]
        [DataRow("M")]
        [DataRow("F")]
        [DataRow("D")]
        public void SetSexTest(string sAct,bool xCH=true)
        {
            testClass.SetSex(sAct);
            Assert.AreEqual(sAct,testClass.sSex);
            Assert.AreEqual(xCH?1:0, testClass.ChangedProps.Count);
        }

        [DataTestMethod()]
        [DataRow(EPersonProp.ID, 1)]
        [DataRow(EPersonProp.ID, 3)]
        [DataRow(EPersonProp.sSex, "D")]
        [DataRow(EPersonProp.iReligi, 6)]
        [DataRow(EPersonProp.sBem, new[] { "", "Bem1-", "Bem2-", "Bem3" })]
        [DataRow(EPersonProp.sBem, new object[] { 1, "Bem1-" })]
        [DataRow(EPersonProp.sKonv, "D")]
        [DataRow(EPersonProp.SurName, "Surname",false)]
        [DataRow(EPersonProp.Givennames, "GivenNames", false)]
        [DataRow(EPersonProp.sSuch, new[] { "", "Bem1-", "Bem2-", "Bem3" })]
        [DataRow(EPersonProp.sSuch, new object[] { 1, "Bem1-" })]
        [DataRow(EPersonProp.dBirth, new[] { 1980, 12, 30 }, false)]
        [DataRow(EPersonProp.dBaptised, new[] { 1980, 12, 30 }, false)]
        [DataRow(EPersonProp.dDeath, new[] { 1980, 12, 30 }, false)]
        [DataRow(EPersonProp.dBurial, new[] { 1980, 12, 30 }, false)]
        [DataRow(EPersonProp.dAnlDatum, new[] { 1980, 12, 30 })]
        [DataRow(EPersonProp.dEditDat, new[] { 2024, 01, 02 })]
        [DataRow(EPersonProp.sOFB, "")]
        [DataRow(EPersonProp.gUid, "01234560-89AB-CDEF-0123-456789ABCDEF")]
        [DataRow(EPersonProp.sPruefen, "Pruefe")]
        public void SetDBValueTest(EPersonProp eAct, object _, bool xSetVal = true)
        {
            testClass.SetDBValue(testRS, new[] { $"{eAct}" });
            if (xSetVal)
                _ = testRS.Received().Fields[eAct.ToString()];
            else
                _ = testRS.DidNotReceive().Fields[eAct.ToString()];
        }

        [DataTestMethod()]
        [DataRow((EPersonProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EPersonProp)17, TypeCode.Int32)]
        [DataRow((EPersonProp)100, TypeCode.Int32)]
        public void SetDBValueTest1(EPersonProp eAct, object _)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetDBValue(testRS, new[] { $"{eAct}" }));
        }

        [DataTestMethod()]
        [DataRow(EPersonProp.ID, 1,false)]
        [DataRow(EPersonProp.sSex, "D")]
        [DataRow(EPersonProp.iReligi, 6)]
        [DataRow(EPersonProp.sBem, new[] { "", "Bem1-", "Bem2-", "Bem3" })]
        [DataRow(EPersonProp.sBem, new object[] { 1, "Bem1-" })]
        [DataRow(EPersonProp.sKonv, "D")]
        [DataRow(EPersonProp.SurName, "Surname", false)]
        [DataRow(EPersonProp.Givennames, "GivenNames", false)]
        [DataRow(EPersonProp.sSuch, new object[] { 1, "Bem1-" })]
        [DataRow(EPersonProp.dBirth, new[] { 1980, 12, 30 }, false)]
        [DataRow(EPersonProp.dBaptised, new[] { 1980, 12, 30 }, false)]
        [DataRow(EPersonProp.dDeath, new[] { 1980, 12, 30 }, false)]
        [DataRow(EPersonProp.dBurial, new[] { 1980, 12, 30 }, false)]
        [DataRow(EPersonProp.dAnlDatum, new[] { 1980, 12, 30 })]
        [DataRow(EPersonProp.dEditDat, new[] { 2024, 01, 02 })]
        [DataRow(EPersonProp.sOFB, "")]
        [DataRow(EPersonProp.gUid, "01234560-89AB-CDEF-0123-456789ABCDEF")]
        [DataRow(EPersonProp.sPruefen, "Pruefe")]
        public void SetDBValueTest2(EPersonProp eAct, object oVal, bool xSetVal = true)
        {
            SetPropValueTest(eAct, oVal);
            testClass.SetDBValue(testRS, null);
            if (xSetVal)
                _ = testRS.Received().Fields[eAct.ToString()];
            else
                _ = testRS.DidNotReceive().Fields[eAct.ToString()];

        }


        [DataTestMethod()]
        [DataRow(EPersonProp.ID, TypeCode.Int32)]
        [DataRow(EPersonProp.sSex, TypeCode.String)]
        [DataRow(EPersonProp.sBem, TypeCode.Object)]
        [DataRow(EPersonProp.sKonv, TypeCode.String)]
        [DataRow(EPersonProp.sSuch, TypeCode.Object)]
        [DataRow(EPersonProp.sPruefen, TypeCode.String)]
        [DataRow(EPersonProp.gUid, TypeCode.Object)]
        [DataRow(EPersonProp.dBirth, TypeCode.DateTime)]
        [DataRow(EPersonProp.dBaptised, TypeCode.DateTime)]
        [DataRow(EPersonProp.dDeath, TypeCode.DateTime)]
        [DataRow(EPersonProp.dBurial, TypeCode.DateTime)]
        [DataRow(EPersonProp.SurName, TypeCode.String)]
        [DataRow(EPersonProp.Givennames, TypeCode.String)]
        [DataRow(EPersonProp.iReligi, TypeCode.Int32)]
        [DataRow(EPersonProp.sOFB, TypeCode.String)]
        public void GetPropTypeTest(EPersonProp pAct, TypeCode eExp)
        {
            Assert.AreEqual(eExp, Type.GetTypeCode(testClass.GetPropType(pAct)));
        }

        [DataTestMethod()]
        [DataRow((EPersonProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EPersonProp)17, TypeCode.Int32)]
        [DataRow((EPersonProp)100, TypeCode.Int32)]
        public void GetPropTypeTest2(EPersonProp pAct, TypeCode eExp)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropType(pAct));
        }

        [DataTestMethod()]
        [DataRow(EPersonProp.ID, 1)]
        [DataRow(EPersonProp.sSex, "Sex")]
        [DataRow(EPersonProp.sBem, new[] { "", "Bem1", "Bem2", "Bem3" })]
        [DataRow(EPersonProp.sKonv, "Konv")]
        [DataRow(EPersonProp.sSuch, new[] { "", "Such1", "Such2", "Such3", "Such4", "Such5", "Such6" })]
        [DataRow(EPersonProp.sPruefen, "Pruefen")]
        [DataRow(EPersonProp.gUid, "01234567-89AB-CDEF-0123-456789ABCDEF")]
        [DataRow(EPersonProp.dBirth, new[] { 1900, 1, 2 })]
        [DataRow(EPersonProp.dBaptised, new[] { 1901, 3, 4 })]
        [DataRow(EPersonProp.dDeath, new[] { 1980, 5, 6 })]
        [DataRow(EPersonProp.dBurial, new[] { 1981, 7, 8 })]
        [DataRow(EPersonProp.dEditDat, new[] { 2000, 3, 4 })]
        [DataRow(EPersonProp.dAnlDatum, new[] { 1999, 1, 1 })]
        [DataRow(EPersonProp.SurName, null)]
        [DataRow(EPersonProp.Givennames, null)]
        [DataRow(EPersonProp.iReligi, 2)]
        [DataRow(EPersonProp.sOFB, "OFB")]
        public void GetPropValueTest(EPersonProp eExp, object oAct)
        {
            SetDatesTest1();
            if (oAct is int[] aiAct && aiAct.Length == 3 && aiAct[1] <= 12 && aiAct[2] <= 31)
                oAct = new DateTime(aiAct[0], aiAct[1], aiAct[2]);
            else if (oAct is string s && s.Length == 36 && Guid.TryParse(s, out var g))
                oAct = g;
            if (oAct is not string[] aS)
                Assert.AreEqual(oAct, testClass.GetPropValue(eExp));
            else
                AssertAreEqual(aS, (string[])testClass.GetPropValue(eExp));
        }

        [DataTestMethod()]
        [DataRow((EPersonProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EPersonProp)17, TypeCode.Int32)]
        [DataRow((EPersonProp)100, TypeCode.Int32)]
        public void GetPropValueTest2(EPersonProp eExp, object oAct)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropValue(eExp));
        }


        [DataTestMethod()]
        [DataRow(EPersonProp.ID, 1)]
        [DataRow(EPersonProp.sSex, "D")]
        [DataRow(EPersonProp.iReligi, 6)]
        [DataRow(EPersonProp.sBem, new[] { "", "Bem1-", "Bem2-", "Bem3" })]
        [DataRow(EPersonProp.sBem, new object[] { 1, "Bem1-" })]
        [DataRow(EPersonProp.sKonv, "D")]
        [DataRow(EPersonProp.SurName, "Surname")]
        [DataRow(EPersonProp.Givennames, "GivenNames")]
        [DataRow(EPersonProp.sSuch, new object[] { 1, "Bem1-" })]
        [DataRow(EPersonProp.dBirth, new[] { 1980, 12, 30 })]
        [DataRow(EPersonProp.dBaptised, new[] { 1980, 12, 30 })]
        [DataRow(EPersonProp.dDeath, new[] { 1980, 12, 30 })]
        [DataRow(EPersonProp.dBurial, new[] { 1980, 12, 30 })]
        [DataRow(EPersonProp.dAnlDatum, new[] { 1980, 12, 30 })]
        [DataRow(EPersonProp.dEditDat, new[] { 2024, 01, 02 })]
        [DataRow(EPersonProp.sOFB, "")]
        [DataRow(EPersonProp.gUid, "01234560-89AB-CDEF-0123-456789ABCDEF")]
        [DataRow(EPersonProp.sPruefen, "Pruefe")]
        public void SetPropValueTest(EPersonProp eAct, object oAct)
        {
            if (oAct is int[] aiAct && aiAct.Length == 3 && aiAct[1] <= 12 && aiAct[2] <= 31)
                oAct = new DateTime(aiAct[0], aiAct[1], aiAct[2]);
            else if (oAct is string s && s.Length == 36 && Guid.TryParse(s, out var g))
                oAct = g;
            else if (oAct is object[] aO && aO.Length == 2 && aO[0] is int iX && aO[1] is string sD)
                oAct = new ListItem<int>(sD, iX);

            testClass.SetPropValue(eAct, oAct);
            if (oAct is ListItem<int> l)
                Assert.AreEqual(l.ItemString, ((string[])testClass.GetPropValue(eAct))[l.ItemData]);
            else if (oAct is not string[] aS)
                Assert.AreEqual(oAct, testClass.GetPropValue(eAct));
            else
                AssertAreEqual(aS, (string[])testClass.GetPropValue(eAct));
        }

        [TestMethod()]
        [DataRow((EPersonProp)(0 - 1), TypeCode.Int32)]
        [DataRow(EPersonProp.ID, 4)]
        [DataRow(EPersonProp.sBem, "X")]
        [DataRow(EPersonProp.sSuch, "X")]
        [DataRow((EPersonProp)17, TypeCode.Int32)]
        [DataRow((EPersonProp)100, TypeCode.Int32)]
        public void SetPropValueTest1(EPersonProp eExp, object? oAct)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetPropValue(eExp, oAct));
        }


        [DataTestMethod()]
        [DataRow(false)]
        [DataRow(true)]
        public void DeleteTest(bool xAct)
        {
            testRS.NoMatch.Returns(xAct);
            testClass.Delete();
            Assert.AreEqual(PersonIndex.PerNr.AsFld(), testRS.Index);
            testRS.Received(xAct ? 0 : 1).Delete();
            testRS.Received(1).Seek("=", testClass.ID);
        }
    }
}