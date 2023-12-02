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
using GenFree.Interfaces.Model;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CEventTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CEvent testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testClass = new CEvent(() => testRS);
            testRS.NoMatch.Returns(true);
            testRS.Fields[nameof(EventFields.Art)].Value.Returns(101, 102, 502, 503);
            testRS.Fields[nameof(EventFields.PerFamNr)].Value.Returns(1, 3, 5);
            testRS.Fields[nameof(EventFields.LfNr)].Value.Returns(2, 6, 4, 9);
            testRS.Fields[nameof(EventFields.DatumV)].Value.Returns(19000101);
            testRS.Fields[nameof(EventFields.DatumB)].Value.Returns(19101231);
        }

        [TestMethod()]
        public void CEventTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(IEvent));
            Assert.IsInstanceOfType(testClass, typeof(CEvent));
        }

        [TestMethod()]
        public void MaxIDTest()
        {
            Assert.AreEqual((EEventArt.eA_Birth, 1, 2), testClass.MaxID);
            testRS.Received().MoveLast();
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);   
        }

        [TestMethod()]
        public void GetPersonDatesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReadFamDatesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetPersonBirthOrBaptTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDateTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDateBTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDateBTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PersonDatTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReadDataPlTest()
        {
            Assert.Fail();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 1, 2, EEventArt.eA_Birth, true)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, false)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void ReadDataTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            Assert.AreEqual(xExp, testClass.ReadData((eArt, iActFam, (short)iLfdNr),out var cEv));
            if (xExp)
            {
                Assert.AreEqual(eArt, cEv!.eArt);
                Assert.AreEqual(iActFam, cEv!.iPerFamNr);
                Assert.AreEqual(iLfdNr, cEv!.iLfNr);
                Assert.AreEqual(new DateTime(1900,1,1), cEv!.dDatumV);
                Assert.AreEqual(new DateTime(1910,12,31), cEv!.dDatumB);
            }
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam, (short)iLfdNr);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-0eA_Birth", 1, 0, EEventArt.eA_Birth, false)]
        [DataRow("1-2eA_Unknown", 1, 2, EEventArt.eA_Unknown, true)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]
        public void ExistsTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            Assert.AreEqual(xExp, testClass.Exists(eArt, iActFam, iLfdNr));
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam, (short)iLfdNr);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-0eA_Unknown", 1, 2, EEventArt.eA_Unknown, true)]
        [DataRow("1-2eA_Birth", 1, 0, EEventArt.eA_Birth, false)]
        [DataRow("1-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]
        public void DeleteBeSuTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            //            Assert.AreEqual(xExp, 
            testClass.DeleteBeSu(iActFam, eArt);//);
            Assert.AreEqual(nameof(EventIndex.BeSu), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam);
        }

        [TestMethod()]
        public void DeleteAllTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ChgEventTest()
        {
            Assert.Fail();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-0eA_Unknown", 1, 2, EEventArt.eA_Unknown, true)]
        [DataRow("1-2eA_Birth", 1, 0, EEventArt.eA_Birth, false)]
        [DataRow("1-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]
        public void SeekBeSuTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            Assert.AreEqual(xExp ? testRS : null, testClass.SeekBeSu(iActFam, eArt, out var xBreak));
            Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual(nameof(EventIndex.BeSu), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam);
        }

        [TestMethod()]
        public void ReadEventsBeSuTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReadAllTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteEmptyFamTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PersLebDatlesTest()
        {
            Assert.Fail();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-0eA_Unknown", 1, 2, EEventArt.eA_Unknown, true)]
        [DataRow("1-2eA_Birth", 1, 0, EEventArt.eA_Birth, false)]
        [DataRow("1-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]
        public void SeekTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            Assert.AreEqual(xExp ? testRS : null, testClass.Seek((eArt, iActFam, (short)iLfdNr), out var xBreak));
            Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam, (short)iLfdNr);
        }

        [TestMethod()]
        public void ReadDataTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetDataTest()
        {
            Assert.Fail();
        }
    }
}