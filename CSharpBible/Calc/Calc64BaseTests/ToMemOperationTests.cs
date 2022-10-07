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
    public class ToMemOperationTests
    {
        [TestMethod()]
        public void ToMemOperationTest()
        {
            var tmco = new ToMemOperation("?4", "Quest4", (a, m) => a ^ m);
            Assert.AreEqual("?4", tmco.ShortDescr);
            Assert.AreEqual("Quest4", tmco.LongDescr);
            Assert.AreEqual(true, tmco.NeedAccumulator);
            Assert.AreEqual(false, tmco.NeedRegister);
            Assert.AreEqual(true, tmco.NeedMemory);
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            var tmco = new ToMemOperation("?4", "Quest4", (a, m) => (a ^ m) - 1);
            var arg = CalcOperation.CreateArguments(tmco);
            arg[0] = 3L;
            arg[1] = 15L;
            Assert.AreEqual(true, tmco.Execute(ref arg));
            Assert.AreEqual(3L, arg[0]);
            Assert.AreEqual(11L, arg[1]);
        }
    }
}