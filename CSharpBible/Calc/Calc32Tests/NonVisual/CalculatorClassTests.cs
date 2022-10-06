// ***********************************************************************
// Assembly         : Calc32Tests
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="CalculatorClassTests.cs" company="HP Inc.">
//     Copyright © HP Inc. 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc32.NonVisual;
using System;

namespace CSharpBible.Calc32.NonVisual.Tests
{
    /// <summary>
    /// Defines test class CalculatorClassTests.
    /// </summary>
    [TestClass()]
    public class CalculatorClassTests
    {
        /// <summary>
        /// The f calculator class
        /// </summary>
        private CalculatorClass FCalculatorClass;
        /// <summary>
        /// The n changes
        /// </summary>
        private int nChanges;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            FCalculatorClass = new CalculatorClass();
        }

        /// <summary>
        /// Defines the test method TestSetup.
        /// </summary>
        [TestMethod()]
        public void TestSetup()
        {
            Assert.IsNotNull(FCalculatorClass);
            Assert.AreEqual(0,FCalculatorClass.Akkumulator);
        }

        /// <summary>
        /// Defines the test method CalculatorClassTest.
        /// </summary>
        [TestMethod()]
        public void CalculatorClassTest()
        {
            Assert.IsInstanceOfType(FCalculatorClass,typeof(CalculatorClass));
        }

        /// <summary>
        /// Defines the test method AkkumulatorTest.
        /// </summary>
        [TestMethod()]
        public void AkkumulatorTest()
        {
            Assert.AreEqual(0, FCalculatorClass.Akkumulator);
            FCalculatorClass.Akkumulator = 1;
            Assert.AreEqual(1, FCalculatorClass.Akkumulator);
            FCalculatorClass.Akkumulator = int.MaxValue;
            Assert.AreEqual(int.MaxValue, FCalculatorClass.Akkumulator);
            FCalculatorClass.Akkumulator = int.MinValue;
            Assert.AreEqual(int.MinValue, FCalculatorClass.Akkumulator);
        }

        /// <summary>
        /// Calculates the change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CalcChange(object sender, EventArgs e)
        {
            nChanges++;
        }

        /// <summary>
        /// Defines the test method OnChangeTest1.
        /// </summary>
        [TestMethod()]
        public void OnChangeTest1()
        {
            FCalculatorClass.OnChange += new EventHandler(CalcChange);
            AkkumulatorTest();
            Assert.AreEqual(3, nChanges);
        }

        /// <summary>
        /// Defines the test method OnChangeTest2.
        /// </summary>
        [TestMethod()]
        public void OnChangeTest2()
        {
            FCalculatorClass.OnChange += new EventHandler(CalcChange);
            FCalculatorClass.NumberButton(3);
            Assert.AreEqual(3, FCalculatorClass.Akkumulator);
            FCalculatorClass.NumberButton(2);
            Assert.AreEqual(32, FCalculatorClass.Akkumulator);
            FCalculatorClass.NumberButton(1);
            Assert.AreEqual(321, FCalculatorClass.Akkumulator);
            Assert.AreEqual(3, nChanges);
        }

        /// <summary>
        /// Defines the test method ButtonTest.
        /// </summary>
        [TestMethod()]
        public void ButtonTest()
        {
            FCalculatorClass.NumberButton(4);
            Assert.AreEqual(4, FCalculatorClass.Akkumulator);
            FCalculatorClass.NumberButton(3);
            Assert.AreEqual(43, FCalculatorClass.Akkumulator);
            FCalculatorClass.NumberButton(2);
            Assert.AreEqual(432, FCalculatorClass.Akkumulator);
        }

        /// <summary>
        /// Defines the test method ButtonBack.
        /// </summary>
        [TestMethod()]
        public void ButtonBack()
        {
            FCalculatorClass.OnChange += new EventHandler(CalcChange);
            FCalculatorClass.NumberButton(4);
            Assert.AreEqual(4, FCalculatorClass.Akkumulator);
            FCalculatorClass.NumberButton(3);
            Assert.AreEqual(43, FCalculatorClass.Akkumulator);
            FCalculatorClass.NumberButton(2);
            Assert.AreEqual(432, FCalculatorClass.Akkumulator);
            FCalculatorClass.BackSpace();
            Assert.AreEqual(43, FCalculatorClass.Akkumulator);
            FCalculatorClass.BackSpace();
            Assert.AreEqual(4, FCalculatorClass.Akkumulator);
            FCalculatorClass.BackSpace();
            Assert.AreEqual(0, FCalculatorClass.Akkumulator);
        }

    }
}