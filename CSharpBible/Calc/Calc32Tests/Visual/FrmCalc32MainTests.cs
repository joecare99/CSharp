// ***********************************************************************
// Assembly         : Calc32Tests
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 08-28-2022
// ***********************************************************************
// <copyright file="FrmCalc32MainTests.cs" company="HP Inc.">
//     Copyright © HP Inc. 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc32.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void FrmCalc32MainTest()
        {
            Assert.Fail();
        }
    }
}