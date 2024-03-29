﻿// ***********************************************************************
// Assembly         : Calc64BaseTests
// Author           : Mir
// Created          : 08-27-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="CalcOperationTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace Calc64Base.Tests
{

    /// <summary>
    /// Defines test class CalcOperationTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class CalcOperationTests
    {
        /// <summary>
        /// Defines the test method CalcOpDefautTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void CalcOpDefautTest()
        {
            var co = new TestCalcOp();
            Assert.IsNotNull(co);
            Assert.AreEqual("?", co.ShortDesc);
            Assert.AreEqual("SomeQuest", co.LongDesc);
            Assert.AreEqual(false, co.NeedAccumulator);
            Assert.AreEqual(false, co.NeedRegister);
            Assert.AreEqual(false, co.NeedMemory);
        }

        /// <summary>
        /// Executes the test.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="iExpArgCount">The i exp argument count.</param>
        /// <param name="oInData">The o in data.</param>
        /// <param name="oOutData">The o out data.</param>
        /// <param name="xExpResult">if set to <c>true</c> [x exp result].</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow("1", 3, new object[] { 1, 2, 3 }, new object[] { 4, 5, 6 }, true)]
        [DataRow("2", 2, new object[] { 1, 2 }, new object[] { 5, 6 }, false)]
        public void ExecuteTest(string Name, int iExpArgCount, object[] oInData, object[] oOutData, bool xExpResult)
        {
            var co = new TestCalcOp();
            co.oExpData = oInData;
            co.oSetData = oOutData;
            co.iExpLength = iExpArgCount;
            co.xSetResult = xExpResult;
            var arg = oInData;
            Assert.AreEqual(xExpResult, co.Execute(ref arg));
            Assert.AreEqual(iExpArgCount, arg.Length);
            for (var i = 0; i < arg.Length; i++)
                Assert.AreEqual(oOutData[i], arg[i]);
        }

        /// <summary>
        /// Defines the test method CreateArgumentsTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void CreateArgumentsTest()
        {
            var co = new TestCalcOp();
            for (int i = 0; i < 8; i++)
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

        [TestMethod]
        public void ClcOpSettingTest2()
        {
            for (int i = 0; i < 8; i++)
            {
                var s = new CalcOperation.ClcOpSetting((i & 1) != 0, (i & 2) != 0, (i & 4) != 0);
                Assert.AreEqual((i & 1) != 0, s.NeedAccumulator);
                Assert.AreEqual((i & 2) != 0, s.NeedRegister);
                Assert.AreEqual((i & 4) != 0, s.NeedMemory);
            }
        }

        [TestMethod]
        public void SetIDTest()
        {
            var co = new TestCalcOp();
            Assert.AreEqual(1, co.ID);
            co.SetID(-1);    
            Assert.AreEqual(-1, co.ID);
        }

    }
}