using GenFree.Model;
using GenFree.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Model.Tests
{
    [TestClass()]
    public class COFBTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private COFB testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testClass = new COFB(() => testRS);
            testRS.NoMatch.Returns(true);
            testRS.Fields[nameof(OFBFields.PerNr)].Value.Returns(1, 2, 3);
            testRS.Fields[nameof(OFBFields.Kennz)].Value.Returns("AA", "BB", "OO");
            testRS.Fields[nameof(OFBFields.TextNr)].Value.Returns(2, 3, 4);
            testRS.ClearReceivedCalls();
        }

        [TestMethod()]
        public void COFBTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(IOFB));
            Assert.IsInstanceOfType(testClass, typeof(COFB));
        }

        [TestMethod()]
        public void MaxIDTest()
        {
            Assert.AreEqual((1, "AA", 2), testClass.MaxID);
            testRS.Received().MoveLast();
            Assert.AreEqual(nameof(OFBIndex.Indn), testRS.Index);
        }

        [DataTestMethod()]
        [DataRow(OFBIndex.InDNr, OFBFields.PerNr)]
        [DataRow(OFBIndex.IndNum, OFBFields.TextNr)]
        public void GetIndex1FieldTest(OFBIndex eAct, OFBFields eExp)
        {
            Assert.AreEqual(eExp, testClass.GetIndex1Field(eAct));
        }

        [DataTestMethod()]
        [DataRow(OFBIndex.Indn, OFBFields.PerNr)]
        public void GetIndex1FieldTest1(OFBIndex eAct, OFBFields eExp)
        {
            Assert.ThrowsException<ArgumentException>(() => testClass.GetIndex1Field(eAct));
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void ReadDataTest(string sName, int iActOFB, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActOFB is not (> 0 and < 3) || iLfNr / 2 != iActOFB);
            Assert.AreEqual(xExp, testClass.ReadData((iActOFB, "AA", iLfNr), out var cPd));

            Assert.AreEqual(nameof(OFBIndex.Indn), testRS.Index);
            testRS.Received().Seek("=", iActOFB, "AA", iLfNr);

            if (xExp)
            {
                Assert.IsNotNull(cPd);
                Assert.AreEqual(iActOFB, cPd.iPerNr);
                Assert.AreEqual("AA", cPd.sKennz);
                Assert.AreEqual(iLfNr, cPd.iTextNr);
            }
            else
            {
                Assert.IsNull(cPd);
            }
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void SeekTest(string sName, int iActOFB, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActOFB is not (> 0 and < 3) || iLfNr / 2 != iActOFB);
            Assert.AreEqual(xExp ? testRS : null, testClass.Seek((iActOFB, "AA", iLfNr), out var xBr));
            Assert.AreEqual(!xExp, xBr);
            Assert.AreEqual(nameof(OFBIndex.Indn), testRS.Index);
            testRS.Received().Seek("=", iActOFB, "AA", iLfNr);

        }
    }
}