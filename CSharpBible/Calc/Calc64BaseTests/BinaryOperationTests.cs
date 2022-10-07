using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc64Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc64Base.Tests
{
    [TestClass()]
    public class BinaryOperationTests
    {
        [TestMethod()]
        public void BinaryOperationTest()
        {
            var bco = new BinaryOperation("?2", "Quest2", (a, r) => a ^ r);
            Assert.AreEqual("?2",bco.ShortDescr);
            Assert.AreEqual("Quest2", bco.LongDescr);
            Assert.AreEqual(true, bco.NeedAccumulator);
            Assert.AreEqual(true, bco.NeedRegister);
            Assert.AreEqual(false, bco.NeedMemory);
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            var bco = new BinaryOperation("?", "Quest", (a,r) => a ^ r);
            var arg = CalcOperation.CreateArguments(bco);
            arg[0] = 5L;
            arg[1] = 6L;
            Assert.AreEqual(true, bco.Execute(ref arg));
            Assert.AreEqual(3L, arg[0]);
            Assert.AreEqual(6L, arg[1]);
        }
    }
}