using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [TestMethod()]
        public void SetNameNrTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetValueTest()
        {
            Assert.Fail();
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
    }
}