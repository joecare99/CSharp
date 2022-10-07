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
    public class CalcOperationTests
    {
        [TestMethod()]
        public void CalcOpDefautTest()
        {
            var co = new TestCalcOp();
            Assert.IsNotNull(co);
            Assert.AreEqual("?", co.ShortDescr);
            Assert.AreEqual("SomeQuest", co.LongDescr);
            Assert.AreEqual(false, co.NeedAccumulator);
            Assert.AreEqual(false, co.NeedRegister);
            Assert.AreEqual(false, co.NeedMemory);
        }

        [DataTestMethod()]
        [DataRow("1",3, new object[] { 1, 2, 3 },new object[] { 4, 5, 6 },true)]
        [DataRow("2", 2, new object[] { 1, 2 }, new object[] { 5, 6 }, false)]
        public void ExecuteTest(string Name,int iExpArgCount, object[] oInData, object[] oOutData,bool xExpResult)
        {
            var co = new TestCalcOp();
            co.oExpData = oInData;
            co.oSetData = oOutData;
            co.iExpLength = iExpArgCount;
            co.xSetResult = xExpResult;
            var arg=oInData;
            Assert.AreEqual(xExpResult, co.Execute(ref arg));
            Assert.AreEqual(iExpArgCount, arg.Length);
            for (var i = 0; i < arg.Length; i++)
                Assert.AreEqual(oOutData[i], arg[i]);
        }

        [TestMethod()]
        public void CreateArgumentsTest()
        {
            var co = new TestCalcOp();
            for (int i = 0;i<8;i++)
            {
                co.SetNeedAccumulator = ((i & 1) != 0);
                co.SetNeedRegister = (i & 2) != 0;
                co.SetNeedMemory = (i & 4) != 0;

                var arg = CalcOperation.CreateArguments(co);
                
                var argCount = 2;
                if (i == 0) argCount = 0;
                else if (i == 7) argCount = 3;
                else if (i == 1 || i == 2 || i == 4) argCount = 1; 
                Assert.AreEqual(argCount, arg.Length);
                Assert.IsInstanceOfType(arg, typeof(object));
            }
        }
    }
}