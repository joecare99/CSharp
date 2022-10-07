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
    public class FromMemOperationTests
    {
        [TestMethod()]
        public void FromMemOperationTest()
        {
            var fmco = new FromMemOperation("?3", "Quest3", (a, m) => a << (int)(m%64));
            Assert.AreEqual("?3", fmco.ShortDescr);
            Assert.AreEqual("Quest3", fmco.LongDescr);
            Assert.AreEqual(true, fmco.NeedAccumulator);
            Assert.AreEqual(false, fmco.NeedRegister);
            Assert.AreEqual(true, fmco.NeedMemory);
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            var fmco = new FromMemOperation("?", "Quest", (a, m) => a << (int)(m % 64));
            var arg = CalcOperation.CreateArguments(fmco);
            arg[0] = 5L;
            arg[1] = 2L;
            Assert.AreEqual(true, fmco.Execute(ref arg));
            Assert.AreEqual(20L, arg[0]);
            Assert.AreEqual(2L, arg[1]);
        }

    }
}