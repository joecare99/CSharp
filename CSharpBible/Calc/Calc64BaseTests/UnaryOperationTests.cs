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
    public class UnaryOperationTests
    {
        [TestMethod()]
        public void UnaryOperationTest()
        {
            var uco= new UnaryOperation("?1", "Quest1", (a) => a * 2);
            Assert.AreEqual("?1", uco.ShortDescr);
            Assert.AreEqual("Quest1", uco.LongDescr);
            Assert.AreEqual(true, uco.NeedAccumulator);
            Assert.AreEqual(false, uco.NeedRegister);
            Assert.AreEqual(false, uco.NeedMemory);

        }

        [TestMethod()]
        public void ExecuteTest()
        {
            var uco = new UnaryOperation("?", "Quest", (a) => a * 2);
            var arg = CalcOperation.CreateArguments(uco);
            arg[0] = 5L;
            Assert.AreEqual(true,uco.Execute(ref arg));
            Assert.AreEqual(10L,arg[0]);
        }
    }
}