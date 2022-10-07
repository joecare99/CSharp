﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc64Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc64Base.Tests
{

    [TestClass()]
    public class Calc64Tests
    {
        private static TestCalcOp testCO;
        private Calc64 testCalc64;

        static Calc64Tests(){

            testCO = new TestCalcOp();
            Calc64.RegisterOperation(testCO);
        }

        [TestInitialize]
        public void Init()
        {
            testCalc64 = new Calc64();
        }

        [TestMethod()]
        public void RegisterOperationTest()
        {
            Assert.IsTrue(testCalc64.ShortDesciptions.Contains("?"));
            Assert.IsTrue(testCalc64.IDs.Contains(1));
            Assert.IsTrue(testCalc64.Operations.Contains(testCO));
            Assert.AreEqual(1, testCO.ID);
        }

        [DataTestMethod()]
        [DataRow("1", 3, new long[] { 1, 2, 3 }, new long[] { 4, 5, 6 }, true)]
        [DataRow("1-", 3, new long[] { 1, 2, 3 }, new long[] { 4, 5, 6 }, false)]
        [DataRow("2", 2, new long[] { 1, 2 }, new long[] { 5, 6 }, true)]
        [DataRow("2-", 2, new long[] { 1, 2 }, new long[] { 5, 6 }, false)]
        [DataRow("3", 1, new long[] { 1 }, new long[] { 5 }, true)]
        [DataRow("3-", 1, new long[] { 1 }, new long[] { 5 }, false)]
        public void DoOpeationTest(string Name, int iExpArgCount, long[] oInData, long[] oOutData, bool xExpResult)
        {
            {
                var co = new TestCalcOp();
                co.SetNeedAccumulator=true;
                testCalc64.Accumulator = oInData[0];
                if (iExpArgCount > 1)
                {
                    co.SetNeedRegister = true;
                    testCalc64.Register = oInData[1];
                }
                if (iExpArgCount > 2) { 
                    co.SetNeedMemory=true;
                    testCalc64.Memory = oInData[2];
                }
                co.oExpData = new object[iExpArgCount];
                co.oSetData = new object[iExpArgCount];
                for (var i = 0; i < iExpArgCount; i++) {
                    co.oExpData[i]=oInData[i];
                    co.oSetData[i] = oOutData[i];
                }
                co.iExpLength = iExpArgCount;
                co.xSetResult = xExpResult;
                Assert.AreEqual(xExpResult, testCalc64.DoOpeation(co));
                if (!xExpResult) oOutData = oInData;
                Assert.AreEqual((object)oOutData[0], testCalc64.Accumulator);
                if (iExpArgCount > 1)
                    Assert.AreEqual((object)oOutData[1], testCalc64.Register);
                else
                    Assert.AreEqual(0, testCalc64.Memory);
                if (iExpArgCount > 2)
                    Assert.AreEqual((object)oOutData[2], testCalc64.Memory);
                else
                    Assert.AreEqual(0, testCalc64.Memory);
            }
        }

        [DataTestMethod()]
        [DataRow("1", "?", 3, new long[] { 1, 2, 3 }, new long[] { 4, 5, 6 }, true)]
        [DataRow("1-", "?", 3, new long[] { 1, 2, 3 }, new long[] { 4, 5, 6 }, false)]
        [DataRow("2", "?", 2, new long[] { 1, 2 }, new long[] { 5, 6 }, true)]
        [DataRow("2-", "?", 2, new long[] { 1, 2 }, new long[] { 5, 6 }, false)]
        [DataRow("3", "?", 1, new long[] { 1 }, new long[] { 5 }, true)]
        [DataRow("3-", "?", 1, new long[] { 1 }, new long[] { 5 }, false)]
        [DataRow("4+", "+", 2, new long[] { 7, 1 }, new long[] { 8, 1 }, true)]
        [DataRow("5-", "-", 2, new long[] { -7, 1 }, new long[] { 8, 1 }, true)]
        [DataRow("6*", "*", 2, new long[] { 7, 3 }, new long[] { 21, 3 }, true)]
        [DataRow("7/", "/", 2, new long[] { -7, 21 }, new long[] { -3, 21 }, true)]
        [DataRow("8^", "^", 2, new long[] { 3, 7 }, new long[] { 343, 7 }, true)]
        public void DoOpeationTest1(string Name, string Op, int iExpArgCount, long[] oInData, long[] oOutData, bool xExpResult)
        {
            var co = testCO;
            co.SetNeedAccumulator = true;
            testCalc64.Accumulator = oInData[0];
            if (co.SetNeedRegister = iExpArgCount > 1)
            {
                testCalc64.Register = oInData[1];
            }
            if (co.SetNeedMemory = iExpArgCount > 2)
            {
                testCalc64.Memory = oInData[2];
            }
            co.oExpData = new object[iExpArgCount];
            co.oSetData = new object[iExpArgCount];
            for (var i = 0; i < iExpArgCount; i++)
            {
                co.oExpData[i] = oInData[i];
                co.oSetData[i] = oOutData[i];
            }
            co.iExpLength = iExpArgCount;
            co.xSetResult = xExpResult;
            Assert.AreEqual(xExpResult, testCalc64.DoOpeation(Op));
            Assert.AreEqual(null, testCalc64.LastError);
            if (!xExpResult) oOutData = oInData;
            Assert.AreEqual((object)oOutData[0], testCalc64.Accumulator);
            if (iExpArgCount > 1)
                Assert.AreEqual((object)oOutData[1], testCalc64.Register);
            else
                Assert.AreEqual(0, testCalc64.Memory);
            if (iExpArgCount > 2)
                Assert.AreEqual((object)oOutData[2], testCalc64.Memory);
            else
                Assert.AreEqual(0, testCalc64.Memory);
        }

        [DataTestMethod()]
        [DataRow("1", 1, 3, new long[] { 1, 2, 3 }, new long[] { 4, 5, 6 }, true)]
        [DataRow("1-", 1, 3, new long[] { 1, 2, 3 }, new long[] { 4, 5, 6 }, false)]
        [DataRow("2", 1, 2, new long[] { 1, 2 }, new long[] { 5, 6 }, true)]
        [DataRow("2-", 1, 2, new long[] { 1, 2 }, new long[] { 5, 6 }, false)]
        [DataRow("3", 1, 1, new long[] { 1 }, new long[] { 5 }, true)]
        [DataRow("3-", 1, 1, new long[] { 1 }, new long[] { 5 }, false)]
        [DataRow("4-2", 2, 2, new long[] { 7, 1 }, new long[] { 8, 1 }, true)]
        [DataRow("5-3", 3, 2, new long[] { -7, 1 }, new long[] { 8, 1 }, true)]
        [DataRow("6-4", 4, 2, new long[] { 7, 3 }, new long[] { 21, 3 }, true)]
        [DataRow("7-5", 5, 2, new long[] { -7, 21 }, new long[] { -3, 21 }, true)]
        [DataRow("8-10", 10, 2, new long[] { 3, 7 }, new long[] { 343, 7 }, true)]
        [DataRow("9-6", 6, 2, new long[] { 3, 14 }, new long[] { 2, 14 }, true)]
        [DataRow("10-7", 7, 2, new long[] { 3, 14 }, new long[] { 15, 14 }, true)]
        [DataRow("11-8", 8, 2, new long[] { 3, 14 }, new long[] { 13, 14 }, true)]
        [DataRow("12-13", 13, 2, new long[] { 3, 14 }, new long[] { 343, 14 }, true)]
        [DataRow("13-14", 14, 2, new long[] { 3, 14 }, new long[] { 343, 14 }, true)]
        [DataRow("14-15", 15, 2, new long[] { 3, 14 }, new long[] { 343, 14 }, true)]
        public void DoOpeationTest1(string Name, int Op, int iExpArgCount, long[] oInData, long[] oOutData, bool xExpResult)
        {
            var co = testCO;
            co.SetNeedAccumulator = true;
            testCalc64.Accumulator = oInData[0];
            if (co.SetNeedRegister = iExpArgCount > 1)
            {
                testCalc64.Register = oInData[1];
            }
            if (co.SetNeedMemory = iExpArgCount > 2)
            {
                testCalc64.Memory = oInData[2];
            }
            co.oExpData = new object[iExpArgCount];
            co.oSetData = new object[iExpArgCount];
            for (var i = 0; i < iExpArgCount; i++)
            {
                co.oExpData[i] = oInData[i];
                co.oSetData[i] = oOutData[i];
            }
            co.iExpLength = iExpArgCount;
            co.xSetResult = xExpResult;
            Assert.AreEqual(xExpResult, testCalc64.DoOpeation(Op));
            Assert.AreEqual(null, testCalc64.LastError);
            if (!xExpResult) oOutData = oInData;
            Assert.AreEqual((object)oOutData[0], testCalc64.Accumulator);
            if (iExpArgCount > 1)
                Assert.AreEqual((object)oOutData[1], testCalc64.Register);
            else
                Assert.AreEqual(0, testCalc64.Memory);
            if (iExpArgCount > 2)
                Assert.AreEqual((object)oOutData[2], testCalc64.Memory);
            else
                Assert.AreEqual(0, testCalc64.Memory);
        }


        [DataTestMethod()]
        [DataRow("1+", 1, "?", true)]
        [DataRow("1-", 1, "?", false)]
        public void IsRegisterOpeationTest(string Name, int iOp, string sOp, bool xExpResult)
        {
            testCO.SetNeedRegister = xExpResult;
            Assert.AreEqual(xExpResult, testCalc64.IsRegisterOpeation(testCO));
        }

        [DataTestMethod()]
        [DataRow("1+", 1, "?", true)]
        [DataRow("1-", 1, "?", false)]
        [DataRow("2", 2, "+", true)]
        [DataRow("3", 3, "-", true)]
        [DataRow("4", 4, "*", true)]
        [DataRow("5", 5, "/", true)]
        [DataRow("6", 6, "&", true)]
        [DataRow("7", 7, "|", true)]
        [DataRow("8", 8, "x", true)]
        [DataRow("9", 9, "~", false)]
        [DataRow("10", 10, "^", true)]
        [DataRow("11", 11, "±", false)]
        [DataRow("12", 12, "%", true)]
        [DataRow("13", 13, "&~", true)]
        [DataRow("14", 14, "|~", true)]
        [DataRow("15", 15, "x~", true)]
        [DataRow("16", 16, "==", true)]
        public void IsRegisterOpeationTest1(string Name, int iOp, string sOp, bool xExpResult)
        {
            testCO.SetNeedRegister = xExpResult;
            Assert.AreEqual(xExpResult, testCalc64.IsRegisterOpeation(sOp));
        }

        [DataTestMethod()]
        [DataRow("1+", 1, "?", true)]
        [DataRow("1-", 1, "?", false)]
        [DataRow("2", 2, "+", true)]
        [DataRow("3", 3, "-", true)]
        [DataRow("4", 4, "*", true)]
        [DataRow("5", 5, "/", true)]
        [DataRow("6", 6, "&", true)]
        [DataRow("7", 7, "|", true)]
        [DataRow("8", 8, "x", true)]
        [DataRow("9", 9, "~", false)]
        [DataRow("10", 10, "^", true)]
        [DataRow("11", 11, "±", false)]
        [DataRow("12", 12, "%", true)]
        [DataRow("13", 13, "&~", true)]
        [DataRow("14", 14, "|~", true)]
        [DataRow("15", 15, "x~", true)]
        [DataRow("16", 16, "==", true)]
        public void IsRegisterOpeationTest2(string Name, int iOp, string sOp,bool xExpResult)
        {
            testCO.SetNeedRegister = xExpResult;
            Assert.AreEqual(xExpResult, testCalc64.IsRegisterOpeation(iOp));
        }

    [DataTestMethod()]
        [DataRow("1", 1, "?")]
        [DataRow("2", 2, "+")]
        [DataRow("3", 3, "-")]
        [DataRow("4", 4, "*")]
        [DataRow("5", 5, "/")]
        [DataRow("6", 6, "&")]
        [DataRow("7", 7, "|")]
        [DataRow("8", 8, "x")]
        [DataRow("9", 9, "~")]
        [DataRow("10", 10, "^")]
        [DataRow("11", 11, "±")]
        [DataRow("12", 12, "%")]
        [DataRow("13", 13, "&~")]
        [DataRow("14", 14, "|~")]
        [DataRow("15", 15, "x~")]
        [DataRow("16", 16, "==")]
        public void ToCalcOperationTest(string Name, int iOp, string sOp)
        {
            CalcOperation co;
            Assert.IsNotNull(co = testCalc64.ToCalcOperation(sOp));
            Assert.AreEqual(sOp, co.ShortDescr);
            Assert.AreEqual(iOp, co.ID);
        }

        [DataTestMethod()]
        [DataRow("1", 1, "?")]
        [DataRow("2", 2, "+")]
        [DataRow("3", 3, "-")]
        [DataRow("4", 4, "*")]
        [DataRow("5", 5, "/")]
        [DataRow("6", 6, "&")]
        [DataRow("7", 7, "|")]
        [DataRow("8", 8, "x")]
        [DataRow("9", 9, "~")]
        [DataRow("10", 10, "^")]
        [DataRow("11", 11, "±")]
        [DataRow("12", 12, "%")]
        [DataRow("13", 13, "&~")]
        [DataRow("14", 14, "|~")]
        [DataRow("15", 15, "x~")]
        [DataRow("16", 16, "==")]
        public void ToCalcOperationTest1(string Name, int iOp, string sOp)
        {
            CalcOperation co;
            Assert.IsNotNull(co=testCalc64.ToCalcOperation(iOp));
            Assert.AreEqual(sOp, co.ShortDescr);
            Assert.AreEqual(iOp, co.ID);
        }
    }
}