﻿// ***********************************************************************
// Assembly         : Calc32Tests
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-07-2022
// ***********************************************************************
// <copyright file="FrmCalc32MainTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace Calc32.Visual.Tests
{
    /// <summary>
    /// Defines test class FrmCalc32MainTests.
    /// </summary>
    [TestClass()]
    public class FrmCalc32MainTests
    {
        /// <summary>
        /// The test frame
        /// </summary>
        private FrmCalc32Main testFrame;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            testFrame = new FrmCalc32Main();
        }

        /// <summary>
        /// Defines the test method FrmCalc32MainTest.
        /// </summary>
        [TestMethod()]
        public void SetUpTest()
        {
            Assert.IsNotNull(testFrame);
            Assert.IsInstanceOfType(testFrame,typeof(FrmCalc32Main));
        }
        /// <summary>
        /// Defines the test method FrmCalc32MainTest.
        /// </summary>
        [TestMethod()]
        public void FrmCalc32MainTest()
        {
            Assert.Fail();
        }
    }
}