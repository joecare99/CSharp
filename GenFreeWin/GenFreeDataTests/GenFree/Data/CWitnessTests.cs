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

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CWitnessTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CWitness testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testClass = new CWitness(() => testRS);
            testRS.NoMatch.Returns(true);
            testRS.Fields[nameof(WitnessFields.FamNr)].Value.Returns(1, 2, 3);
            testRS.Fields[nameof(WitnessFields.PerNr)].Value.Returns(2, 3, 5);
            testRS.Fields[nameof(WitnessFields.Kennz)].Value.Returns(3, 4, 5);
            testRS.Fields[nameof(WitnessFields.Art)].Value.Returns(101, 102, 502, 503);
            testRS.Fields[nameof(WitnessFields.LfNr)].Value.Returns(1, 6, 4, 9);
            testRS.ClearReceivedCalls();
        }


        [TestMethod()]
        public void CWitnessTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(IWitness));
            Assert.IsInstanceOfType(testClass, typeof(CWitness));
        }

        [TestMethod()]
        public void MaxIDTest()
        {
            Assert.AreEqual((1, 2, 3, EEventArt.eA_Birth, (short)1), testClass.MaxID);
            testRS.Received().MoveLast();
            Assert.AreEqual(nameof(WitnessIndex.Fampruef), testRS.Index);
        }

        [DataTestMethod()]
        [DataRow(WitnessIndex.ElSu, WitnessFields.PerNr)]
        [DataRow(WitnessIndex.FamSu, WitnessFields.FamNr)]
        public void GetIndex1FieldTest(WitnessIndex eAct, WitnessFields eExp)
        {
            Assert.AreEqual(eExp, testClass.GetIndex1Field(eAct));
        }

        [DataTestMethod()]
        [DataRow(WitnessIndex.Zeug, WitnessFields.PerNr)]
        public void GetIndex1FieldTest1(WitnessIndex eAct, WitnessFields eExp)
        {
            Assert.ThrowsException<ArgumentException>(() => testClass.GetIndex1Field(eAct));
        }

        [TestMethod()]
        [DataRow(0, 0)]
        [DataRow(1, 3, true)]
        public void ReadAllFamsTest(int iAct1, int iAct2, bool xExp = false)
        {
            testRS.NoMatch.Returns(iAct1 is not (> 0 and < 3) || iAct2 / 2 != iAct1, false, false, true);
            testRS.EOF.Returns(iAct1 is not (> 0 and < 3) || iAct2 / 2 != iAct1, false, false, true);
            var iCnt = 0;
            foreach (var item in testClass.ReadAllFams(iAct1, iAct2))
            {
                Assert.IsNotNull(item);
                Assert.IsInstanceOfType(item, typeof(IWitnessData));
                Assert.AreEqual((2, 2, 4, EEventArt.eA_Birth, (short)1), item.ID);
                iCnt++;
            }
            Assert.AreEqual(xExp ? 1 : 0, iCnt);
            Assert.AreEqual(nameof(WitnessIndex.FamSu), testRS.Index);
            testRS.Received(xExp ? 1 : 0).MoveNext();
            testRS.Received(0).MoveFirst();
            testRS.Received(1).Seek("=", iAct1, iAct2);

        }

        [DataTestMethod()]
        [DataRow(0, EEventArt.eA_Birth, (short)1, 10, false)]
        [DataRow(1, EEventArt.eA_Birth, (short)2, 10, true)]
        public void ExistZeugTest(int iAct, EEventArt eAct, short sAct, int iK, bool xExp)
        {
            testRS.NoMatch.Returns(iAct is not (> 0 and < 3) || sAct / 2 != iAct, false, false, true);
            Assert.AreEqual(xExp, testClass.ExistZeug(iAct, eAct, sAct, iK));
            testRS.Received(1).Seek("=", iAct, iK, eAct, sAct);
            Assert.AreEqual(nameof(WitnessIndex.ZeugSu), testRS.Index);
        }

        [DataTestMethod()]
        [DataRow(0, 0, false)]
        [DataRow(1, 2, false)]
        [DataRow(2, 4, true)]
        public void DeleteAllETest(int iAct, int iK, bool xExp)
        {
            testRS.NoMatch.Returns(iAct is not (> 0 and < 3) || iK / 2 != iAct, false, false, true);
            testRS.Fields[nameof(WitnessFields.Kennz)].Value.Returns(4, 6, 5);
            testClass.DeleteAllE(iAct, iK);
            Assert.AreEqual(nameof(WitnessIndex.ElSu), testRS.Index);
            testRS.Received(1).Seek("=", iAct, iK);
        }

        [DataTestMethod()]
        [DataRow(0, 0, false)]
        [DataRow(1, 3, true)]
        [DataRow(2, 4, false)]
        public void DeleteAllFTest(int iAct, int iK, bool xExp)
        {
            testRS.NoMatch.Returns(iAct is not (> 0 and < 3) || iK / 2 != iAct, false, false, true);
            testClass.DeleteAllF(iAct, iK);
            Assert.AreEqual(nameof(WitnessIndex.FamSu), testRS.Index);
            testRS.Received(1).Seek("=", iAct, iK);
        }

        [DataTestMethod()]
        [DataRow(0, EEventArt.eA_Birth, (short)1, 10, false)]
        [DataRow(2, EEventArt.eA_Birth, (short)1, 4, true)]
        public void DeleteAllZeugTest(int iAct, EEventArt eAct, short sAct, int iK, bool xExp)
        {
            testRS.NoMatch.Returns(iAct is not (> 0 and < 3) || iK / 2 != iAct, false, false, true);
            testRS.Fields[nameof(WitnessFields.Kennz)].Value.Returns(4, 6, 5);
            testClass.DeleteAllZ(iAct, iK, eAct, sAct);
            testRS.Received(1).Seek("=", iAct, iK, eAct, sAct);
            Assert.AreEqual(nameof(WitnessIndex.ZeugSu), testRS.Index);
        }

        [DataTestMethod()]
        [DataRow(0, EEventArt.eA_Birth, (short)1, 10, false)]
        [DataRow(2, EEventArt.eA_Birth, (short)1, 4, true)]
        public void DeleteAllFamPredTest(int iAct, EEventArt eAct, short sAct, int iK, bool xExp)
        {
            bool testPred(int i) => i % 2 == 0;
            testRS.EOF.Returns(iAct is not (> 0 and < 3) || iK / 2 != iAct, false, false, true);
            testRS.Fields[nameof(WitnessFields.Art)].Value.Returns(101, 502, 503);
            testClass.DeleteAllFamPred(testPred);
            testRS.ReceivedWithAnyArgs(0).Seek("=");
            testRS.Received(1).MoveFirst();
            Assert.AreEqual(nameof(WitnessIndex.Fampruef), testRS.Index);
        }

        [DataTestMethod()]
        [DataRow(0, 0, EEventArt.eA_Birth, (short)1, 10, true)]
        [DataRow(1, 2, EEventArt.eA_Birth, (short)1, 4, false)]
        public void AppendTest(int iAct, int iPers, EEventArt eAct, short sAct, int iK, bool xExp)
        {
            testRS.NoMatch.Returns(iAct is not (> 0 and < 3) || iPers / 2 != iAct, false, false, true);
            testClass.Append(iAct, iPers, iK, eAct, sAct);
            Assert.AreEqual(nameof(WitnessIndex.Fampruef), testRS.Index);
            testRS.Received(1).Seek("=", iAct, iPers, iK, eAct, sAct);
            if (xExp)
            {
                testRS.Received(1).AddNew();
                _ = testRS.Received(5).Fields[""];
                testRS.Received(1).Update();
            }
            else
            {
                testRS.Received(0).AddNew();
                _ = testRS.Received(0).Fields[""];
                testRS.Received(0).Update();
            }
        }

        [DataTestMethod()]
        [DataRow(0, 0, EEventArt.eA_Birth, (short)1, 10, false)]
        [DataRow(1, 2, EEventArt.eA_Birth, (short)1, 4, true)]
        public void AddTest(int iAct, int iPers, EEventArt eAct, short sAct, int iK, bool xExp)
        {
            testRS.NoMatch.Returns(iAct is not (> 0 and < 3) || iPers / 2 != iAct, false, false, true);
            testClass.Add(iAct, iPers, eAct, sAct, iK);
            testRS.Received(0).Seek("=", iAct, iPers, iK, eAct, sAct);
            testRS.Received(1).AddNew();
            _ = testRS.Received(5).Fields[""];
            testRS.Received(1).Update();
        }
    }
}