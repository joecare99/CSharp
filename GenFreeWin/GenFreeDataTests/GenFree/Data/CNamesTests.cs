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
using GenFree.Interfaces;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CNamesTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CNames testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testClass = new CNames(() => testRS);
            testRS.NoMatch.Returns(true);
            testRS.Fields[nameof(NameFields.Kennz)].Value.Returns('N', 'V', '-', 'F' ,'A', 'B');
            testRS.Fields[nameof(NameFields.LfNr)].Value.Returns(1, 3, 5, 16);
            testRS.Fields[nameof(NameFields.PersNr)].Value.Returns(2, 6, 4, 9);
            testRS.ClearReceivedCalls();
        }

        [TestMethod()]
        public void CNamesTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(INames));
            Assert.IsInstanceOfType(testClass, typeof(CNames));
        }


        [TestMethod()]
        public void MaxIDTest()
        {
            Assert.AreEqual((2, ETextKennz.tkName, 1), testClass.MaxID);
            Assert.AreEqual(nameof(NameIndex.Vollname), testRS.Index);
            testRS.Received(1).MoveLast();
            testRS.Received(3).Fields[0].Value = 0;
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("2-Name-4", 2, ETextKennz.tkName, 4, true)]

        public void DeleteAllPTest(string sName, int iActPers, ETextKennz eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers, false, false, true);
            testRS.EOF.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers, false, false, true);
            testClass.DeleteAllP(iActPers);
            Assert.AreEqual(nameof(NameIndex.PNamen), testRS.Index);
            testRS.Received().Seek("=", iActPers);
            testRS.Received(xExp ? 1 : 0).Delete();
            testRS.Received(xExp ? 1 : 0).MoveNext();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void DeleteNKTest(string sName, int iActPers, ETextKennz eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers);
            Assert.AreEqual(xExp, testClass.DeleteNK(iActPers, eTKennz));
            Assert.AreEqual(nameof(NameIndex.NamKenn), testRS.Index);
            testRS.Received().Seek("=", iActPers, eTKennz);
            testRS.Received(xExp ? 1 : 0).Delete();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]

        public void ExistTextTest(string sName, int iActPers, ETextKennz eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers);
            Assert.AreEqual(xExp, testClass.ExistText(iActPers));
            Assert.AreEqual(nameof(NameIndex.TxNr), testRS.Index);
            testRS.Received().Seek("=", iActPers);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]

        public void ExistsNKTest(string sName, int iActPers, ETextKennz eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers);
            Assert.AreEqual(xExp, testClass.ExistsNK(iActPers, eTKennz));
            Assert.AreEqual(nameof(NameIndex.NamKenn), testRS.Index);
            testRS.Received().Seek("=", iActPers, eTKennz);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("2-Name-4", 2, ETextKennz.tkName, 4, true)]
        public void ReadAllTest(string sName, int iActPers, ETextKennz eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers, false, false, true);
            testRS.EOF.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers, false, false, true);
            var iCnt = 0;
            foreach (var cNm in testClass.ReadAll())
            {
                Assert.IsNotNull(cNm);
                Assert.IsInstanceOfType(cNm, typeof(INamesData));
                Assert.IsInstanceOfType(cNm, typeof(CNamesData));
                iCnt++;
            }
            Assert.AreEqual(xExp ? 3 : 0, iCnt);
            Assert.AreEqual(nameof(NameIndex.Vollname), testRS.Index);
            testRS.Received(xExp ? 3 : 0).MoveNext();
            testRS.Received(1).MoveFirst();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void ReadDataTest(string sName, int iActPers, ETextKennz eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers);
            Assert.AreEqual(xExp, testClass.ReadData((iActPers, eTKennz, iLfNr), out var cNm));
            if (xExp)
            {
                Assert.IsNotNull(cNm);
                Assert.IsInstanceOfType(cNm, typeof(INamesData));
                Assert.IsInstanceOfType(cNm, typeof(CNamesData));
            }
            else
            {
                Assert.IsNull(cNm);
            }
            Assert.AreEqual(nameof(NameIndex.Vollname), testRS.Index);
            testRS.Received().Seek("=", iActPers, eTKennz, iLfNr);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void SeekTest(string sName, int iActPers, ETextKennz eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers);
            Assert.AreEqual(xExp ? testRS : null, testClass.Seek((iActPers, eTKennz, iLfNr), out var xBreak));
            Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual(nameof(NameIndex.Vollname), testRS.Index);
            testRS.Received().Seek("=", iActPers, eTKennz, iLfNr);

        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("2-Name-4", 2, ETextKennz.tkName, 4, true)]

        public void SetDataTest(string sName, int iActPers, ETextKennz eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers, false, false, true);
            var testND = Substitute.For<INamesData>();
            testClass.SetData((iActPers, eTKennz, iLfNr), testND);
            //          Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual(nameof(NameIndex.Vollname), testRS.Index);
            testRS.Received().Seek("=", iActPers, eTKennz, iLfNr);
            testRS.Received(xExp ? 1 : 0).Edit();
            testRS.Received(xExp ? 1 : 0).Update();
            testND.Received(xExp ? 1 : 0).SetDBValue(testRS, null);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("2-Name-4", 2, ETextKennz.tkName, 4, true)]
        public void ReadPersonNamesTest(string sName, int iActPers, ETextKennz eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers, false, false, false, false, true);
            testRS.Fields[nameof(NameFields.PersNr)].Value.Returns(2, 2, 2, 2, 2, 9);

            Assert.AreEqual(xExp, testClass.ReadPersonNames(iActPers, out var aiNames, out var aVrn));
            //          Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual(nameof(NameIndex.PNamen), testRS.Index);
            testRS.Received().Seek("=", iActPers);
            testRS.Received(xExp ? 4 : 0).MoveNext();
            testRS.Received(0).MoveFirst();
            testRS.Received(xExp ? 25 : 1).Fields[0].Value = 0;

        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("2-Name-4", 2, ETextKennz.tkName, 4, true)]
        [DataRow("2-V_-3", 2, ETextKennz.V_, 3, false)]
        public void UpdateTest(string sName, int iActPers, ETextKennz eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3) || iLfNr / 2 != iActPers, false, false, true);
            testClass.Update(iActPers, iLfNr, eTKennz, iLfNr);
            //          Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual(nameof(NameIndex.Vollname), testRS.Index);
            testRS.Received().Seek("=", iActPers, eTKennz, iLfNr);
            testRS.Received(xExp ? 1 : 0).Edit();
            testRS.Received(xExp ? 0 : 1).AddNew();
            testRS.Received().Update();
            testRS.Received().Fields[nameof(NameFields.LfNr)].Value = iLfNr;
            testRS.Received().Fields[nameof(NameFields.Kennz)].Value = eTKennz;
            testRS.Received(xExp ? 5 : 6).Fields[nameof(NameFields.PersNr)].Value = iActPers;
        }

        [DataTestMethod()]
        [DataRow(NameIndex.PNamen,NameFields.PersNr)]
        [DataRow(NameIndex.TxNr, NameFields.Text)]
        [DataRow(NameIndex.Vollname, NameFields.PersNr)]
        [DataRow(NameIndex.NamKenn, NameFields.Kennz)]
        public void GetIndex1FieldTest(NameIndex eAct,NameFields eExp)
        {
            Assert.AreEqual(eExp, testClass.GetIndex1Field(eAct));
        }

        [DataTestMethod()]
        [DataRow((NameIndex)5, NameFields.PersNr)]
        public void GetIndex1FieldTest1(NameIndex eAct, NameFields eExp)
        {
            Assert.ThrowsException<ArgumentException>(() =>testClass.GetIndex1Field(eAct));
        }
    }
}