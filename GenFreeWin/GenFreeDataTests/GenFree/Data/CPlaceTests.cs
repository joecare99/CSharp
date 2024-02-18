using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Interfaces.Model;
using GenFree.Interfaces;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CPlaceTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CPlace testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testClass = new CPlace(() => testRS);
            testRS.NoMatch.Returns(true);
            testRS.Fields[nameof(PlaceFields.OrtNr)].Value.Returns(2, 6, 4, 9);
            testRS.Fields[nameof(PlaceFields.Ort)].Value.Returns('N', 'V', 'V', 'A', 'B');
            testRS.Fields[nameof(PlaceFields.PLZ)].Value.Returns("F", 'M', '_', 'C', 'B');
            testRS.Fields[nameof(PlaceFields.Bem)].Value.Returns(1, 3, 5);
            testRS.ClearReceivedCalls();
        }

        [TestMethod()]
        public void CPlaceTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(IPlace));
            Assert.IsInstanceOfType(testClass, typeof(CPlace));
        }

        [TestMethod()]
        public void MaxIDTest()
        {
            Assert.AreEqual(2, testClass.MaxID);
            Assert.AreEqual(nameof(PlaceIndex.OrtNr), testRS.Index);
            testRS.Received(1).MoveLast();
            testRS.Received(1).Fields[0].Value = 0;
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("2-Name-4", 1, ETextKennz.tkName, 2, true)]
        [DataRow("2-Name-4", 1, ETextKennz.tkNone, 2, true)]

        public void ForeEachTextDoTest(string sName, int iActPlace, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPlace is not (> 0 and < 3) || iLfNr / 2 != iActPlace, false, false, true);
            testRS.EOF.Returns(iActPlace is not (> 0 and < 3) || iLfNr / 2 != iActPlace, false, false, true);
            var iCnt = 0;
            Action<float, int>? onProgress = ((int)(EEventArt)eTKennz == 0) ? null : (f, i) => _ = i;
            testClass.ForeEachTextDo(i => $"i", (i, aS) => iCnt++, onProgress);
            Assert.AreEqual(xExp ? 3 : 0, iCnt);
            Assert.AreEqual(nameof(PlaceIndex.OrtNr), testRS.Index);
            testRS.Received(xExp ? 3 : 0).MoveNext();
            testRS.Received(1).MoveFirst();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]

        public void SeekTest(string sName, int iActPlace, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPlace is not (> 0 and < 3) || iLfNr / 2 != iActPlace);
            Assert.AreEqual(xExp ? testRS : null, testClass.Seek(iActPlace, out var xBreak));
            Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual(nameof(PlaceIndex.OrtNr), testRS.Index);
            testRS.Received(1).Seek("=", iActPlace);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void ReadDataTest(string sName, int iActPlace, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPlace is not (> 0 and < 3) || iLfNr / 2 != iActPlace);
            Assert.AreEqual(xExp, testClass.ReadData(iActPlace, out var cPd));
            Assert.AreEqual(nameof(PlaceIndex.OrtNr), testRS.Index);
            testRS.Received().Seek("=", iActPlace);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("2-Name-4", 1, ETextKennz.tkName, 2, true)]

        public void ReadAllTest(string sName, int iActPlace, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPlace is not (> 0 and < 3) || iLfNr / 2 != iActPlace, false, false, true);
            testRS.EOF.Returns(iActPlace is not (> 0 and < 3) || iLfNr / 2 != iActPlace, false, false, true);
            var iCnt = 0;
            foreach (var cNm in testClass.ReadAll())
            {
                Assert.IsNotNull(cNm);
                Assert.IsInstanceOfType(cNm, typeof(IPlaceData));
                Assert.IsInstanceOfType(cNm, typeof(CPlaceData));
                iCnt++;
            }
            Assert.AreEqual(xExp ? 3 : 0, iCnt);
            Assert.AreEqual(nameof(PlaceIndex.OrtNr), testRS.Index);
            testRS.Received(xExp ? 3 : 0).MoveNext();
            testRS.Received(1).MoveFirst();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void SetDataTest(string sName, int iActPlace, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPlace is not (> 0 and < 3) || iLfNr / 2 != iActPlace);
            var testPD = Substitute.For<IPlaceData>();
            testClass.SetData(iActPlace, testPD);
            Assert.AreEqual(nameof(PlaceIndex.OrtNr), testRS.Index);
            testRS.Received().Seek("=", iActPlace);
        }

        [DataTestMethod()]
        [DataRow(PlaceIndex.OrtNr,PlaceFields.OrtNr)]
        [DataRow(PlaceIndex.Orte, PlaceFields.Ort)]
        [DataRow(PlaceIndex.OT, PlaceFields.Ortsteil)]
        [DataRow(PlaceIndex.K, PlaceFields.Kreis)]
        [DataRow(PlaceIndex.L, PlaceFields.Land)]
        [DataRow(PlaceIndex.S, PlaceFields.Staat)]
        public void GetIndex1FieldTest(PlaceIndex eAct,PlaceFields eExp)
        {
            Assert.AreEqual(eExp, testClass.GetIndex1Field(eAct));
        }

        [DataTestMethod()]
        [DataRow(PlaceIndex.Pol, PlaceFields.OrtNr)]
        [DataRow(PlaceIndex.O, PlaceFields.OrtNr)]
        [DataRow((PlaceIndex)100, PlaceFields.OrtNr)]
        public void GetIndex1FieldTest2(PlaceIndex eAct, PlaceFields eExp)
        {
            Assert.ThrowsException<ArgumentException>(()=>testClass.GetIndex1Field(eAct));
        }
    }
}