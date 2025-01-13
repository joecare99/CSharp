using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.Model;
using GenFree.Interfaces;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CFamilyTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CFamily testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            var testST = Substitute.For<ISysTime>();
            testST.Now.Returns(new DateTime(2022, 12, 31));
            testClass = new CFamily(() => testRS, testST);
            testRS.NoMatch.Returns(true);
            testRS.Fields[nameof(FamilyFields.FamNr)].Value.Returns(2, 6, 4, 9);
            testRS.Fields[nameof(FamilyFields.Eltern)].Value.Returns('N', 'V', 'V', 'A', 'B');
            testRS.Fields[nameof(FamilyFields.Aeb)].Value.Returns(1, 3, 5);
            testRS.ClearReceivedCalls();
        }

        [TestMethod()]
        public void CFamilyTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(IFamily));
            Assert.IsInstanceOfType(testClass, typeof(CFamily));
        }

        [TestMethod()]
        public void MaxIDTest()
        {
            Assert.AreEqual(2, testClass.MaxID);
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received(1).MoveLast();
            testRS.Received(1).Fields[0].Value = 0;
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void SetNameNrTest(string sName, int iActFamNr,object _, int iName, bool xExp)
        {
            testRS.NoMatch.Returns(iActFamNr is not (> 0 and < 3) || iName / 2 != iActFamNr);
            testClass.SetNameNr(iActFamNr, iName);
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received(1).Seek("=", iActFamNr);
            _ = testRS.Received(xExp ? 2 : 8).Fields[""].Value;
            _ = testRS.Received(xExp ? 2 : 1).NoMatch;
            _ = testRS.Received(0).EOF;
            testRS.Received(xExp ? 0 : 1).AddNew();
            testRS.Received(xExp ? 1 : 0).Edit();
            testRS.Received(1).Update();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, EFamilyProp.sBem, 0, false)]
        [DataRow("1-None-0", 1, EFamilyProp.iPrae, 0, false)]
        [DataRow("1-Name-2", 1, EFamilyProp.xAeB, 2, true)]

        public void SetValueTest(string sName, int iActFamNr, EFamilyProp eFProp, int iName, bool xExp)
        {
            testRS.NoMatch.Returns(iActFamNr is not (> 0 and < 3) || iName / 2 != iActFamNr);
            testClass.SetValue(iActFamNr, iName, new[] { (eFProp,(object)sName) });
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received(1).Seek("=", iActFamNr);
            _ = testRS.Received(xExp ? 3 : 8).Fields[""].Value;
            _ = testRS.Received(xExp ? 2 : 1).NoMatch;
            _ = testRS.Received(0).EOF;
            testRS.Received(xExp ? 0 : 1).AddNew();
            testRS.Received(xExp ? 2 : 1).Edit();
            testRS.Received(2).Update();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]

        public void SeekTest(string sName, int iActFamNr, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActFamNr is not (> 0 and < 3) || iLfNr / 2 != iActFamNr);
            Assert.AreEqual(xExp ? testRS : null, testClass.Seek(iActFamNr, out var xBreak));
            Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received().Seek("=", iActFamNr);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]

        public void ReadDataTest(string sName, int iActFamNr, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActFamNr is not (> 0 and < 3) || iLfNr / 2 != iActFamNr);
            Assert.AreEqual(xExp, testClass.ReadData(iActFamNr, out var cPd));
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received().Seek("=", iActFamNr);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]

        public void ReadAllTest(string sName, int iActFamNr, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActFamNr is not (> 0 and < 3) || iLfNr / 2 != iActFamNr, false, false, true);
            testRS.EOF.Returns(iActFamNr is not (> 0 and < 3) || iLfNr / 2 != iActFamNr, false, false, true);
            var iCnt = 0;
            foreach (var cNm in testClass.ReadAll())
            {
                Assert.IsNotNull(cNm);
                Assert.IsInstanceOfType(cNm, typeof(IFamilyData));
                Assert.IsInstanceOfType(cNm, typeof(CFamilyPersons));
                iCnt++;
            }
            Assert.AreEqual(xExp ? 3 : 0, iCnt);
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received(xExp ? 3 : 0).MoveNext();
            testRS.Received(1).MoveFirst();

        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]

        public void SetDataTest(string sName, int iActFamNr, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActFamNr is not (> 0 and < 3) || iLfNr / 2 != iActFamNr);
            var testPD = Substitute.For<IFamilyData>();
            testClass.SetData(iActFamNr, testPD);
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received().Seek("=", iActFamNr);
        }

        [DataTestMethod()]
        [DataRow(FamilyIndex.Fam, FamilyFields.FamNr)]
        [DataRow(FamilyIndex.Fuid, FamilyFields.Fuid)]
        [DataRow(FamilyIndex.BeaDat, FamilyFields.EditDat)]
        public void GetIndex1FieldTest(FamilyIndex eAct, FamilyFields eExp)
        {
            Assert.AreEqual(eExp, testClass.GetIndex1Field(eAct));
        }

        [DataTestMethod()]
        [DataRow((FamilyIndex)5, FamilyFields.FamNr)]
        [DataRow((FamilyIndex)7, FamilyFields.FamNr)]
        public void GetIndex1FieldTest2(FamilyIndex eAct, FamilyFields eExp)
        {
            Assert.ThrowsException<ArgumentException>(() => testClass.GetIndex1Field(eAct));
        }
    }
}