using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Interfaces.Model;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CPersonTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CPerson testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            var testST = Substitute.For<ISysTime>();
            testST.Now.Returns(new DateTime(2022, 12, 31));
            testClass = new CPerson(() => testRS, testST);
            testRS.NoMatch.Returns(true);
            testRS.Fields[nameof(PersonFields.PersNr)].Value.Returns(2, 6, 4, 9);
            testRS.Fields[nameof(PersonFields.Bem1)].Value.Returns('N', 'V', 'V', 'A', 'B');
            testRS.Fields[nameof(PersonFields.Sex)].Value.Returns("F", 'M', '_', 'C', 'B');
            testRS.Fields[nameof(PersonFields.OFB)].Value.Returns(1, 3, 5);
            testRS.ClearReceivedCalls();
        }

        [TestMethod()]
        public void CPersonTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(IPerson));
            Assert.IsInstanceOfType(testClass, typeof(CPerson));

        }

        [TestMethod()]
        public void MaxIDTest()
        {
            Assert.AreEqual(2, testClass.MaxID);
            Assert.AreEqual(nameof(PersonIndex.PerNr), testRS.Index);
            testRS.Received(1).MoveLast();
            testRS.Received(1).Fields[0].Value = 0;

        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void AllSetEditDateTest(string sName, int iActPers, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.RecordCount.Returns(iActPers);
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers, false, false, true);
            testRS.EOF.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers, false, false, true);
            testClass.AllSetEditDate();
            Assert.AreEqual(nameof(PersonIndex.PerNr), testRS.Index);
            testRS.Received(xExp ? 3 : 0).MoveNext();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, -2)]
        [DataRow("1-None-0", 5, ETextKennz.tkNone, 0, -1)]
        [DataRow("2-Name-4", 1, ELinkKennz.lkFather, 2, -3)]
        [DataRow("2-Name-4", 1, ELinkKennz.lkFather, 3, 0)]
        [DataRow("2-Name-4", 1, ELinkKennz.lkMother, 2, 0)]
        [DataRow("2-Name-4", 1, ELinkKennz.lkNone, 2, 0)]

        public void CheckIDTest(string sName, int iActPers, Enum eTKennz, int iLfNr, int iExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers);
            if (iLfNr == 3) testRS.Fields[nameof(PersonFields.Sex)].Value.Returns("M");
            Assert.AreEqual(iExp, testClass.CheckID(iActPers, (int)(ELinkKennz)eTKennz == 0, (ELinkKennz)eTKennz));
            Assert.AreEqual(nameof(PersonIndex.PerNr), testRS.Index);
            testRS.Received(iExp is -1 ? 0 : 1).Seek("=", iActPers);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, 0, 0)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, 0, 0)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, 0, 1)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 3, 0, 1)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 4, 0, 0)]
        [DataRow("1-Name-2", 2, ETextKennz.tkNone, 4, 0, 2)]
        [DataRow("1-Name-2", 2, ETextKennz.tkName, 4, 0, 0)]
        [DataRow("1-Name-2", 2, ETextKennz.tkNone, 4, 3, 2)]
        [DataRow("1-Name-2", 2, ETextKennz.tkName, 4, 3, 0)]
        [DataRow("1-Name-2", 2, ETextKennz.tkName, 4, 2, 2)]

        public void ValidateIDTest(string sName, int iActPers, Enum eTKennz, int iLfNr, int schalt, int iExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers, false, false, true);
            testRS.RecordCount.Returns(iLfNr);
            if (iLfNr >= 4) testRS.Fields[nameof(PersonFields.Pruefen)].Value.Returns("G", "G", "G", "");
            Assert.AreEqual(iExp, testClass.ValidateID(iActPers, (short)schalt, iLfNr, ELinkKennz.lkNone, i => (ELinkKennz)eTKennz));
            Assert.AreEqual(nameof(PersonIndex.PerNr), testRS.Index);
            testRS.Received().Seek("=", iActPers);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, "")]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, "")]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, "F")]
        public void GetSexTest(string sName, int iActPers, Enum eTKennz, int iLfNr, string xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers);
            Assert.AreEqual(xExp, testClass.GetSex(iActPers));
            Assert.AreEqual(nameof(PersonIndex.PerNr), testRS.Index);
            testRS.Received().Seek("=", iActPers);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]

        public void SeekTest(string sName, int iActPers, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers);
            Assert.AreEqual(xExp ? testRS : null, testClass.Seek(iActPers, out var xBreak));
            Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual(nameof(PersonIndex.PerNr), testRS.Index);
            testRS.Received().Seek("=", iActPers);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void ReadDataTest(string sName, int iActPers, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers);
            Assert.AreEqual(xExp, testClass.ReadData(iActPers, out var cPd));
            Assert.AreEqual(nameof(PersonIndex.PerNr), testRS.Index);
            testRS.Received().Seek("=", iActPers);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("2-Name-4", 1, ETextKennz.tkName, 2, true)]

        public void ReadAllTest(string sName, int iActPers, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers, false, false, true);
            testRS.EOF.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers, false, false, true);
            var iCnt = 0;
            foreach (var cNm in testClass.ReadAll())
            {
                Assert.IsNotNull(cNm);
                Assert.IsInstanceOfType(cNm, typeof(IPersonData));
                Assert.IsInstanceOfType(cNm, typeof(CPersonData));
                iCnt++;
            }
            Assert.AreEqual(xExp ? 3 : 0, iCnt);
            Assert.AreEqual(nameof(PersonIndex.PerNr), testRS.Index);
            testRS.Received(xExp ? 3 : 0).MoveNext();
            testRS.Received(1).MoveFirst();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void SetDataTest(string sName, int iActPers, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers);
            var testPD = Substitute.For<IPersonData>();
            testClass.SetData(iActPers, testPD);
            Assert.AreEqual(nameof(PersonIndex.PerNr), testRS.Index);
            testRS.Received().Seek("=", iActPers);
        }

        [DataTestMethod()]
        [DataRow(PersonIndex.PerNr,PersonFields.PersNr)]
        [DataRow(PersonIndex.Puid, PersonFields.PUid)]
        [DataRow(PersonIndex.BeaDat, PersonFields.EditDat)]
        [DataRow(PersonIndex.reli, PersonFields.religi)]
        public void GetIndex1FieldTest(PersonIndex eAct,PersonFields eExp)
        {
            Assert.AreEqual(eExp, testClass.GetIndex1Field(eAct));
        }

        [DataTestMethod()]
        [DataRow(PersonIndex.Such1, PersonFields.PersNr)]
        [DataRow(PersonIndex.Such2, PersonFields.PersNr)]
        [DataRow(PersonIndex.Such3, PersonFields.PersNr)]
        public void GetIndex1FieldTest2(PersonIndex eAct, PersonFields eExp)
        {
            Assert.ThrowsException<ArgumentException>(()=> testClass.GetIndex1Field(eAct));
        }

    }
}