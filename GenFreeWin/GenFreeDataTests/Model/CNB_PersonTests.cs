using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Interfaces.Model;

namespace GenFree.Model.Tests
{
    [TestClass()]
    public class CNB_PersonTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CNB_Person testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        int iRes = 0;

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testClass = new(() => testRS, aPAct);
            testRS.NoMatch.Returns(true);
            testRS.Fields["Per"].Value.Returns(1, 2, 3);
            iRes = 0;
        }


        private void aPAct(int obj)
        {
            iRes = obj;
        }

        [TestMethod()]
        public void CNB_PersonTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsNotNull(testRS);
            Assert.IsInstanceOfType(testRS, typeof(IRecordset));
            Assert.IsInstanceOfType(testClass, typeof(INB_Person));
        }

        [DataTestMethod()]
        [DataRow(0, false)]
        [DataRow(1, true)]
        public void SeekTest(int iAct, bool xExt)
        {
            testRS.NoMatch.Returns(iAct is not (> 0 and < 3));
            var tRS = testClass.Seek(iAct, out var xBreak);
            Assert.AreEqual(!xExt, xBreak);
            Assert.AreEqual(xBreak ? null : testRS, tRS);
        }

        [DataTestMethod()]
        [DataRow(0, false)]
        [DataRow(1, true)]
        public void AppendTest(int iAct,bool xAct)
        {
            testClass.Append(iAct,xAct);
            if (xAct)
            {
                Assert.AreEqual(iAct, iRes);
            }
            else
            {
                Assert.AreEqual(0, iRes);
            }
        }

        [TestMethod]
        public void MaxIDTest()
        {
            Assert.AreEqual(0, testClass.MaxID);
        }
    }
}