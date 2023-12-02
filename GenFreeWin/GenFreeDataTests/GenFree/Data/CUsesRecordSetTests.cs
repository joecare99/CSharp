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

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CUsesRecordSetTests : CUsesRecordSet<int>
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        protected override string _keyIndex => "_MyKeyIndex";

        protected override IRecordset _db_Table => testRS;

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testRS.NoMatch.Returns(true);
            testRS.Fields["idTable"].Value.Returns(1, 2, 3);
            testRS.Fields["Description"].Value.Returns("one", "two", "three");
            testRS.Fields["Data"].Value.Returns(2, 6, 4);
            testRS.EOF.Returns(false, false, true);
        }

        [TestMethod]
        public void SetUpTest()
        {
            Assert.IsNotNull(testRS);
            Assert.IsInstanceOfType(testRS,typeof(IRecordset));
            Assert.AreEqual(0, Count);
            Assert.AreEqual("", testRS.Index);
            Assert.AreEqual(true, testRS.NoMatch);
            Assert.AreEqual(0, testRS.RecordCount);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, false)]
        [DataRow("1-0None", 1, true)]
        [DataRow("1-1Father", 1, true)]
        public void SeekTest(string sName, int iActPers, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3));
            Assert.AreEqual(xExp ? testRS : null, this.Seek(iActPers, out var xBreak));
            Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual("_MyKeyIndex", testRS.Index);
            testRS.Received().Seek("=", iActPers);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, false)]
        [DataRow("1-0None", 1, true)]
        [DataRow("1-1Father", 1, true)]
        public void SeekTest1(string sName, int iActPers, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3));
            Assert.AreEqual(xExp ? testRS : null, this.Seek(iActPers));
            Assert.AreEqual("_MyKeyIndex", testRS.Index);
            testRS.Received().Seek("=", iActPers);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, false)]
        [DataRow("1-0None", 1, true)]
        [DataRow("1-1Father", 1, true)]
        public void DeleteTest(string sName, int iActPers, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3));
            Assert.AreEqual(xExp, this.Delete(iActPers));
            Assert.AreEqual("_MyKeyIndex", testRS.Index);
            testRS.Received().Seek("=", iActPers);
            if (xExp)
            {
                testRS.Received().Delete();
            }
            else
            {
                testRS.DidNotReceive().Delete();
            }
        }

        [DataTestMethod()]
        [DataRow("Null", 0, false)]
        [DataRow("1-0None", 1, true)]
        [DataRow("1-1Father", 1, true)]
        public void ExistTest(string sName, int iActPers, bool xExp)
        {
            testRS.NoMatch.Returns(iActPers is not (> 0 and < 3));
            Assert.AreEqual(xExp, this.Exists(iActPers));
            Assert.AreEqual("_MyKeyIndex", testRS.Index);
            testRS.Received().Seek("=", iActPers);
        }


        [TestMethod()]
        public void ForEachDoTest()
        {
            var iCnt = 0;
            this.ForEachDo(rs =>
            {
                Assert.AreEqual(testRS, rs);
                iCnt++;
            });
            Assert.AreEqual(2, iCnt);
            Assert.AreEqual("_MyKeyIndex", testRS.Index);
            testRS.Received(1).MoveFirst();
            testRS.Received(2).MoveNext();
        }

        [TestMethod()]
        public void ForEachDo2Test()
        {
            var iCnt = 0;
            this.ForEachDo(null);
            Assert.AreEqual("_MyKeyIndex", testRS.Index);
            testRS.Received(1).MoveFirst();
            testRS.Received(2).MoveNext();
        }

        [TestMethod()]
        public void ForEachDo3Test()
        {
            var iCnt = 0;
            this.ForEachDo(rs =>
            {
                iCnt++;
                throw new AccessViolationException();
            });
            Assert.AreEqual(2, iCnt);
            Assert.AreEqual("_MyKeyIndex", testRS.Index);
            testRS.Received(1).MoveFirst();
            testRS.Received(2).MoveNext();
        }

        [TestMethod()]
        public void MaxIDTest()
        {
            Assert.AreEqual(1, MaxID);
            Assert.AreEqual("_MyKeyIndex", testRS.Index);
            testRS.Received(1).MoveLast();
        }

        [TestMethod()]
        public void CountTest()
        {
            testRS.RecordCount.Returns(99);
            Assert.AreEqual(99, Count);
            Assert.AreEqual("", testRS.Index);
        }


        protected override int GetID(IRecordset recordset)
        {
            return recordset.Fields["idTable"].AsInt();
        }

        public override IRecordset? Seek(int key, out bool xBreak)
        {
            _db_Table.Index = _keyIndex;
            _db_Table.Seek("=", key);
            xBreak = _db_Table.NoMatch;
            return xBreak ? null : _db_Table;
        }
    }
}