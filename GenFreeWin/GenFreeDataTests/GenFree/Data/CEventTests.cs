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
            testRS.Fields[nameof(EventFields.PerFamNr)].Value.Returns(2, 3, 5);
            testRS.Fields[nameof(EventFields.LfNr)].Value.Returns(1, 6, 4, 9);
            testRS.Fields[nameof(EventFields.DatumV)].Value.Returns(19000101);
            testRS.Fields[nameof(EventFields.DatumB)].Value.Returns(19101231);
            testRS.Fields[nameof(EventFields.DatumV_S)].Value.Returns("<V>");
            testRS.Fields[nameof(EventFields.DatumB_S)].Value.Returns("<B>");
            testRS.Fields[nameof(EventFields.VChr)].Value.Returns("0");
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
            Assert.AreEqual((EEventArt.eA_Birth, 2, 1), testClass.MaxID);
            testRS.Received().MoveLast();
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 2, 4, EEventArt.eA_Birth, true)]
        [DataRow("1-2eA_Birth", 2, 4, EEventArt.eA_Unknown, true)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, false)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void GetPersonDatesTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.Fields[nameof(EventFields.Art)].Value.Returns(101, 101);
            testRS.Fields[nameof(EventFields.LfNr)].Value.Returns(1, 0, 1, 0);
            if (xExp)
                testRS.Fields[nameof(EventFields.PerFamNr)].Value.Returns(iActFam);
            var iCnt = 0;
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true, false, true);
            Action<EEventArt, int, string>? testAct = eArt switch
            {
                EEventArt.eA_Birth => (eArt, iActFam, s) => iCnt++,
                _ => null
            };
            var adAct = testClass.GetPersonDates(iActFam, out var xBC1, testAct);
            testRS.Received(1).Seek("=", EEventArt.eA_Birth, iActFam);
            testRS.Received(1).Seek("=", EEventArt.eA_Baptism, iActFam);
            testRS.Received(1).Seek("=", EEventArt.eA_Death, iActFam);
            testRS.Received(1).Seek("=", EEventArt.eA_Burial, iActFam);
            Assert.AreEqual(xExp && eArt == EEventArt.eA_Birth ? 2 : 0, iCnt);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 2, 4, EEventArt.eA_Marriage, true)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, false)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void ReadFamDatesTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.Fields[nameof(EventFields.Art)].Value.Returns(502, 502);
            testRS.Fields[nameof(EventFields.PerFamNr)].Value.Returns(2, 2);
            testRS.Fields[nameof(EventFields.LfNr)].Value.Returns(1, 1, 0);
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, false, true);
            var adAct = testClass.ReadFamDates(iActFam);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 2, 4, EEventArt.eA_Birth, true)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, false)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]
        public void PersonDatTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.Fields[nameof(EventFields.Art)].Value.Returns(101, 101, 101, 101, 103);
            testRS.Fields[nameof(EventFields.LfNr)].Value.Returns(1, 0, 1, 0);
            if (xExp)
                testRS.Fields[nameof(EventFields.PerFamNr)].Value.Returns(iActFam);
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true, true, false, false, true);
            testClass.PersonDat(iActFam, out var dt1, out var dt2);
            testRS.Received().Seek("=", EEventArt.eA_Birth, iActFam);
            testRS.Received().Seek("=", EEventArt.eA_Baptism, iActFam);
            testRS.Received().Seek("=", EEventArt.eA_Death, iActFam);
            testRS.Received().Seek("=", EEventArt.eA_Burial, iActFam);

        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 1, 2, EEventArt.eA_Birth, true)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, false)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void GetPersonBirthOrBaptTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, !xExp, true);
            Assert.AreEqual(xExp ? new DateTime(1900, 1, 1) : default, testClass.GetPersonBirthOrBapt(iActFam));
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 1, 2, EEventArt.eA_Birth, true)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, false)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void GetDateTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            Assert.AreEqual(xExp ? new DateTime(1900, 1, 1) : default, testClass.GetDate(eArt, iActFam));
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam, (short)0);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 1, 2, EEventArt.eA_Birth, true)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, false)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void GetDateTest1(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            Assert.AreEqual(xExp ? new DateTime(1900, 1, 1) : default, testClass.GetDate(eArt, iActFam, out var s));
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam, (short)0);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 1, 2, EEventArt.eA_Birth, true)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, false)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void GetDateBTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            Assert.AreEqual(xExp ? new DateTime(1910, 12, 31) : default, testClass.GetDateB(eArt, iActFam));
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam, (short)0);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 1, 2, EEventArt.eA_Birth, true)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, false)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void GetDateBTest1(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            Assert.AreEqual(xExp ? new DateTime(1910, 12, 31) : default, testClass.GetDateB(eArt, iActFam, out var s));
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam, (short)0);
        }


        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 1, 2, EEventArt.eA_Birth, true)]
        [DataRow("1-2eA_Unknown", 1, 2, EEventArt.eA_Unknown, false)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, false)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void ReadDataPlTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            if (xExp)
                testRS.Fields[nameof(EventFields.Ort)].Value.Returns(15, 17, 19);
            var cEv = testClass.ReadDataPl(eArt, iActFam, out var xBreak, (short)iLfdNr);
            Assert.AreEqual(!xExp, xBreak);
            if (xExp)
            {
                Assert.AreEqual(eArt, cEv!.eArt);
                Assert.AreEqual(2, cEv!.iPerFamNr);
                Assert.AreEqual(1, cEv!.iLfNr);
                Assert.AreEqual(new DateTime(1900, 1, 1), cEv!.dDatumV);
                Assert.AreEqual(new DateTime(1910, 12, 31), cEv!.dDatumB);
            }
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam, (short)iLfdNr);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 1, 2, EEventArt.eA_Birth, true)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, false)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void ReadDataTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            Assert.AreEqual(xExp, testClass.ReadData((eArt, iActFam, (short)iLfdNr), out var cEv));
            if (xExp)
            {
                Assert.AreEqual(eArt, cEv!.eArt);
                Assert.AreEqual(2, cEv!.iPerFamNr);
                Assert.AreEqual(1, cEv!.iLfNr);
                Assert.AreEqual(new DateTime(1900, 1, 1), cEv!.dDatumV);
                Assert.AreEqual(new DateTime(1910, 12, 31), cEv!.dDatumB);
            }
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam, (short)iLfdNr);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 1, 2, EEventArt.eA_Birth, true)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, false)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void ReadDataTest1(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            Assert.AreEqual(xExp, testClass.ReadData(eArt, iActFam, out var cEv, (short)iLfdNr));
            if (xExp)
            {
                Assert.AreEqual(eArt, cEv!.eArt);
                Assert.AreEqual(2, cEv!.iPerFamNr);
                Assert.AreEqual(1, cEv!.iLfNr);
                Assert.AreEqual(new DateTime(1900, 1, 1), cEv!.dDatumV);
                Assert.AreEqual(new DateTime(1910, 12, 31), cEv!.dDatumB);
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
            testClass.DeleteBeSu(eArt, iActFam);//);
            Assert.AreEqual(nameof(EventIndex.BeSu), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 2, 4, EEventArt.eA_Birth, true)]
        [DataRow("1-2eA_Baptism", 1, 2, EEventArt.eA_Baptism, false)]

        public void DeleteAllTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            // Assert.AreEqual(xExp ? testRS : null, 
            testClass.DeleteAll(eArt, iActFam);
            //            Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual(nameof(EventIndex.BeSu), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam);
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
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, EEventArt.eA_Unknown, 0, false)]
        [DataRow("1-0eA_Unknown", 1, 0, EEventArt.eA_Unknown, EEventArt.eA_Unknown, 0, false)]
        [DataRow("1-2eA_Birth", 2, 4, EEventArt.eA_Birth, EEventArt.eA_Birth, 0, false)]
        [DataRow("1-2eA_Birth", 2, 4, EEventArt.eA_Death, EEventArt.eA_Death, 2, false)]
        [DataRow("1-2eA_Birth", 2, 4, EEventArt.eA_Birth, EEventArt.eA_Baptism, 0, true)]
        [DataRow("1-2eA_Baptism", 1, 2, EEventArt.eA_Baptism, EEventArt.eA_Death, 3, true)]
        [DataRow("1-2eA_Baptism", 1, 2, EEventArt.eA_Birth, EEventArt.eA_Death, 1, true)]

        public void ChgEventTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, EEventArt eArt2, int iActFam2, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            // Assert.AreEqual(xExp ? testRS : null, 
            testClass.ChgEvent(eArt, iActFam, eArt2, iActFam2);
            //            Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual(nameof(EventIndex.BeSu), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam);
            if (xExp)
            {
                testRS.Received().Edit();
                testRS.Received().Update();
            }
            else
            {
                testRS.DidNotReceive().Delete();
                testRS.DidNotReceive().MoveNext();
            }
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-0eA_Unknown", 1, 2, EEventArt.eA_Unknown, true)]
        [DataRow("1-2eA_Birth", 1, 0, EEventArt.eA_Birth, false)]
        [DataRow("1-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]
        public void SeekBeSuTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            Assert.AreEqual(xExp ? testRS : null, testClass.SeekBeSu(eArt, iActFam, out var xBreak));
            Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual(nameof(EventIndex.BeSu), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-0eA_Unknown", 1, 2, EEventArt.eA_Unknown, true)]
        [DataRow("1-2eA_Birth", 1, 0, EEventArt.eA_Birth, false)]
        [DataRow("1-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void ReadEventsBeSuTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, true);
            foreach (var cEv in testClass.ReadEventsBeSu(iActFam, eArt))
            {
                Assert.AreEqual(xExp ? new DateTime(1900, 1, 1) : default, cEv.dDatumV);
                Assert.AreEqual(xExp ? new DateTime(1910, 12, 31) : default, cEv.dDatumB);
                Assert.AreEqual(xExp ? eArt : EEventArt.eA_Unknown, cEv.eArt);
                Assert.AreEqual(xExp ? iActFam : 0, cEv.iPerFamNr);
                Assert.AreEqual(xExp ? iLfdNr : 0, cEv.iLfNr);
            }
            Assert.AreEqual(nameof(EventIndex.BeSu), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-0eA_Unknown", 1, 2, EEventArt.eA_Birth, true)]
        [DataRow("1-2eA_Birth", 1, 0, EEventArt.eA_Birth, false)]
        [DataRow("1-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void ReadAllTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, true);
            foreach (var cEv in testClass.ReadAll())
            {
                Assert.AreEqual(xExp ? new DateTime(1900, 1, 1) : default, cEv.dDatumV);
                Assert.AreEqual(xExp ? new DateTime(1910, 12, 31) : default, cEv.dDatumB);
                Assert.AreEqual(EEventArt.eA_Birth, cEv.eArt);
                Assert.AreEqual(2, cEv.iPerFamNr);
                Assert.AreEqual(1, cEv.iLfNr);
            }
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);
            testRS.Received().Seek(">=", 0);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-2eA_Birth", 1, 2, EEventArt.eA_Birth, true)]
        [DataRow("1-2eA_Unknown", 1, 2, EEventArt.eA_Unknown, false)]
        [DataRow("1-0eA_Birth", 1, 0, EEventArt.eA_Birth, false)]
        [DataRow("2-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void DeleteEmptyFamTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, true);
            if (eArt == EEventArt.eA_Unknown)
            {
                testRS.Fields[nameof(EventFields.LfNr)].Value.Returns(0);
                testRS.Fields[nameof(EventFields.DatumV)].Value.Returns(0);
                testRS.Fields[nameof(EventFields.DatumB)].Value.Returns(0);
                testRS.Fields[nameof(EventFields.DatumV_S)].Value.Returns("");
                testRS.Fields[nameof(EventFields.DatumB_S)].Value.Returns("");
            }
            Assert.AreEqual(!xExp, testClass.DeleteEmptyFam(iActFam, eArt));
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);
            testRS.Received().Seek("=", eArt, iActFam, (short)0);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, false)]
        [DataRow("1-0eA_Unknown", 1, 2, EEventArt.eA_Unknown, true)]
        [DataRow("1-2eA_Birth", 1, 0, EEventArt.eA_Birth, false)]
        [DataRow("1-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, false)]

        public void PersLebDatlesTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, !xExp, !xExp, true);
            var testP = Substitute.For<IPersonData>();
            testClass.PersLebDatles(iActFam, testP);
            Assert.AreEqual(nameof(EventIndex.ArtNr), testRS.Index);
            testRS.Received().Seek("=", EEventArt.eA_Birth, iActFam, (short)0);
            testRS.Received().Seek("=", EEventArt.eA_Baptism, iActFam, (short)0);
            testRS.Received().Seek("=", EEventArt.eA_Death, iActFam, (short)0);
            testRS.Received().Seek("=", EEventArt.eA_Burial, iActFam, (short)0);
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

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, null, false)]
        [DataRow("1-0eA_Unknown", 1, 2, EEventArt.eA_Unknown, null, true)]
        [DataRow("1-2eA_Birth", 1, 0, EEventArt.eA_Birth, null, false)]
        [DataRow("1-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, null, false)]

        public void SetDataTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, string[]? asAct, bool xExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, !xExp, !xExp, true);
            var testP = Substitute.For<IEventData>();
            testClass.SetData((eArt, iActFam, (short)iLfdNr), testP, asAct);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, EventFields.ArtText, null, 0)]
        [DataRow("1-0eA_Unknown", 1, 2, EEventArt.eA_Unknown, EventFields.ArtText, null, 0)]
        [DataRow("1-2eA_Birth", 1, 0, EEventArt.eA_Birth, EventFields.ArtText, null, 0)]
        [DataRow("1-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, EventFields.ArtText, null, 0)]
        public void GetValueTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, EventFields eDataField, string[]? asAct, int iExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, false, true);
            Assert.AreEqual(iExp, testClass.GetValue((eArt, iActFam, (short)iLfdNr), eDataField, 0));
        }

        [DataTestMethod()]
        [DataRow("Null", 0, 0, EEventArt.eA_Unknown, EventFields.ArtText, null, 0)]
        [DataRow("1-0eA_Unknown", 1, 2, EEventArt.eA_Unknown, EventFields.ArtText, null, 0)]
        [DataRow("1-2eA_Birth", 1, 0, EEventArt.eA_Birth, EventFields.ArtText, null, 0)]
        [DataRow("1-2eA_Baptism", 2, 2, EEventArt.eA_Baptism, EventFields.ArtText, null, 0)]
        public void SetValuesTest(string sName, int iActFam, int iLfdNr, EEventArt eArt, EventFields eDataField, string[]? asAct, int iExp)
        {
            testRS.NoMatch.Returns(iActFam is not (> 0 and < 3) || iLfdNr / 2 != iActFam, false, false, true);
            testClass.SetValues((eArt, iActFam, (short)iLfdNr), new[]{ (eDataField, (object)iExp) });    
            Assert.Fail();
        }

        [DataTestMethod()]
        [DataRow("NText", EventIndex.NText, EventFields.ArtText)]
        [DataRow("PText", EventIndex.PText, EventFields.Platz)]
        [DataRow("CText", EventIndex.CText, EventFields.Causal)]
        [DataRow("KText", EventIndex.KText, EventFields.KBem)]
        [DataRow("Ort", EventIndex.EOrt, EventFields.Ort)]
        [DataRow("Reg", EventIndex.Reg, EventFields.Reg)]
        [DataRow("HaNu", EventIndex.HaNu, EventFields.Hausnr)]
        public void GetIndex1FieldTest(string sName, EventIndex eIx, EventFields eExp)
        {
            Assert.AreEqual(eExp,testClass.GetIndex1Field(eIx));
        }

        [TestMethod()]
        public void UpdateClearPredTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateAllSetValTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateValuesTest()
        {
            Assert.Fail();
        }
    }
}