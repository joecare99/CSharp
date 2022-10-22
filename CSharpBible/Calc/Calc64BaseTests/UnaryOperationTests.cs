﻿// ***********************************************************************
// Assembly         : Calc64BaseTests
// Author           : Mir
// Created          : 08-27-2022
//
// Last Modified By : Mir
// Last Modified On : 08-28-2022
// ***********************************************************************
// <copyright file="UnaryOperationTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc64Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace Calc64Base.Tests
{

    /// <summary>
    /// Defines test class UnaryOperationTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class UnaryOperationTests
    {
        /// <summary>
        /// Defines the test method UnaryOperationTest.
        /// </summary>
        /// <autogeneratedoc />
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

        /// <summary>
        /// Defines the test method ExecuteTest.
        /// </summary>
        /// <autogeneratedoc />
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