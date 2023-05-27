﻿// ***********************************************************************
// Assembly         : Calc32Tests
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-07-2022
// ***********************************************************************
// <copyright file="CalculatorClassTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc32.NonVisual;
using System;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
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
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CalculatorClass FCalculatorClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        /// <summary>
        /// The n changes
        /// </summary>
        private int nChanges;

        public string DebugLog = "";

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            FCalculatorClass = new CalculatorClass();
            DebugLog = "";
        }

        /// <summary>
        /// Defines the test method TestSetup.
        /// </summary>
        [TestMethod()]
        public void TestSetup()
        {
            Assert.IsNotNull(FCalculatorClass);
            Assert.AreEqual(0, FCalculatorClass.Accumulator);
            Assert.AreEqual(0, FCalculatorClass.Memory);
            Assert.AreEqual("", FCalculatorClass.OperationText);
            Assert.AreEqual("", DebugLog);
        }

        /// <summary>
        /// Defines the test method CalculatorClassTest.
        /// </summary>
        [TestMethod()]
        public void CalculatorClassTest()
        {
            Assert.IsInstanceOfType(FCalculatorClass, typeof(CalculatorClass));
        }

        /// <summary>
        /// Defines the test method AccumulatorTest.
        /// </summary>
        [TestMethod()]
        public void AccumulatorTest()
        {
            Assert.AreEqual(0, FCalculatorClass.Accumulator);
            FCalculatorClass.Accumulator = 1;
            Assert.AreEqual(1, FCalculatorClass.Accumulator);
            FCalculatorClass.Accumulator = int.MaxValue;
            Assert.AreEqual(int.MaxValue, FCalculatorClass.Accumulator);
            FCalculatorClass.Accumulator = int.MinValue;
            Assert.AreEqual(int.MinValue, FCalculatorClass.Accumulator);
        }

        /// <summary>
        /// Calculates the change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void CalcChange(object? sender, (string, object?, object?) e)
        {
            DebugLog += $"{sender}.Change({e.Item1},{e.Item2}=>{e.Item3}){Environment.NewLine}";
            nChanges++;
        }

        /// <summary>
        /// Defines the test method OnChangeTest1.
        /// </summary>
        [TestMethod()]
        public void OnChangeTest1()
        {
            FCalculatorClass.OnChange += CalcChange;
            AccumulatorTest();
            Assert.AreEqual(3, nChanges);
        }


        /// <summary>
        /// Defines the test method OnChangeTest2.
        /// </summary>
        [TestMethod()]
        public void OnChangeTest2()
        {
            FCalculatorClass.OnChange += CalcChange;
            FCalculatorClass.NumberButton(3);
            Assert.AreEqual(3, FCalculatorClass.Accumulator);
            FCalculatorClass.NumberButton(2);
            Assert.AreEqual(32, FCalculatorClass.Accumulator);
            FCalculatorClass.NumberButton(1);
            Assert.AreEqual(321, FCalculatorClass.Accumulator);
            Assert.AreEqual(3, nChanges);
        }

        /// <summary>
        /// Defines the test method ButtonTest.
        /// </summary>
        [DataTestMethod()]
        [DataRow("432", new int[] { 4, 3, 2 }, new int[] { 4, 43, 432 })]
        [DataRow("1234", new int[] { 1, 2, 3, 4 }, new int[] { 1, 12, 123, 1234 })]
        [DataRow("1234", new int[] { 1, 2, 3, 4 }, new int[] { 1, 12, 123, 1234 })]
        [DataRow("999999999", new int[] { 9, 9, 9, 9, 9, 9, 9, 9 }, new int[] { 9, 99, 999, 9999, 99999, 999999, 9999999, 99999999, 999999999, 999999999 })]
        public void ButtonTest(string name, int[] aiButtons, int[] aiExp)
        {
            for (int i = 0; i < aiButtons.Length; i++)
            {
                FCalculatorClass.NumberButton(aiButtons[i]);
                Assert.AreEqual(aiExp[i], FCalculatorClass.Accumulator, $"{name}[{i}]");
            }

        }

        /// <summary>
        /// Defines the test method ButtonBack.
        /// </summary>
        [DataTestMethod()]
        [DataRow("4", 4, false, 0, "Calc32.NonVisual.CalculatorClass.Change(Accumulator,4=>0)\r\n")]
        [DataRow("431", 431, false, 0, "Calc32.NonVisual.CalculatorClass.Change(Accumulator,431=>0)\r\n")]
        [DataRow("4", 4, true, 0, "Calc32.NonVisual.CalculatorClass.Change(Accumulator,4=>0)\r\n")]
        [DataRow("431", 431, true, 43, "Calc32.NonVisual.CalculatorClass.Change(Accumulator,431=>43)\r\n")]
        [DataRow("-4", -4, true, 0, "Calc32.NonVisual.CalculatorClass.Change(Accumulator,-4=>0)\r\n")]
        [DataRow("-456", -456, true, -45, "Calc32.NonVisual.CalculatorClass.Change(Accumulator,-456=>-45)\r\n")]
        [DataRow("0", 0, true, 0, "")]
        public void ButtonBack(string name, int iAkk, bool xEdit, int iExp, string sExp)
        {
            FCalculatorClass.OnChange += CalcChange;

            if (!xEdit) FCalculatorClass.Accumulator = iAkk;
            else
                Enter(iAkk, FCalculatorClass);
            DebugLog = "";
            FCalculatorClass.BackSpace();
            Assert.AreEqual(iExp, FCalculatorClass.Accumulator, $"Calc({name}).result");
            Assert.AreEqual(sExp, DebugLog, $"Calc({name}).DebugLog");
        }

        private void Enter(int iAkk, CalculatorClass fCalculatorClass)
        {
            var s = iAkk.ToString();
            fCalculatorClass.Operation(1);
            bool xNeg = false;
            if (s[0] == '-')
            {
                xNeg = true;
                s = s.Remove(0, 1);
            }
            while (s.Length > 0)
            {
                fCalculatorClass.NumberButton(s[0] - 48);
                s = s.Remove(0, 1);
            }
            if (xNeg)
                fCalculatorClass.Operation((int)CalculatorClass.eOpMode.Negate);

        }

        /// <summary>
        /// Defines the test method ButtonTest.
        /// </summary>
        [DataTestMethod()]
        [DataRow("4+3", CalculatorClass.eOpMode.Plus, new int[] { 4, 3 }, 7, new string[] { @"Calc32.NonVisual.CalculatorClass.Change(OperationText,=>=)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,0=>4)
Calc32.NonVisual.CalculatorClass.Change(OperationText,==>+)
Calc32.NonVisual.CalculatorClass.Change(Memory,0=>4)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,4=>3)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,3=>7)
Calc32.NonVisual.CalculatorClass.Change(OperationText,+=>=)
Calc32.NonVisual.CalculatorClass.Change(Memory,4=>0)
" })]
        [DataRow("12-4", CalculatorClass.eOpMode.Minus, new int[] { 12, 4 }, 8, new string[] { @"Calc32.NonVisual.CalculatorClass.Change(OperationText,=>=)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,0=>12)
Calc32.NonVisual.CalculatorClass.Change(OperationText,==>-)
Calc32.NonVisual.CalculatorClass.Change(Memory,0=>12)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,12=>4)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,4=>8)
Calc32.NonVisual.CalculatorClass.Change(OperationText,-=>=)
Calc32.NonVisual.CalculatorClass.Change(Memory,12=>0)
" })]
        [DataRow("7*6", CalculatorClass.eOpMode.Multiply, new int[] { 7, 6 }, 42, new string[] { @"Calc32.NonVisual.CalculatorClass.Change(OperationText,=>=)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,0=>7)
Calc32.NonVisual.CalculatorClass.Change(OperationText,==>*)
Calc32.NonVisual.CalculatorClass.Change(Memory,0=>7)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,7=>6)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,6=>42)
Calc32.NonVisual.CalculatorClass.Change(OperationText,*=>=)
Calc32.NonVisual.CalculatorClass.Change(Memory,7=>0)
" })]
        [DataRow("99/11", CalculatorClass.eOpMode.Divide, new int[] { 99, 11 }, 9, new string[] { @"Calc32.NonVisual.CalculatorClass.Change(OperationText,=>=)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,0=>99)
Calc32.NonVisual.CalculatorClass.Change(OperationText,==>/)
Calc32.NonVisual.CalculatorClass.Change(Memory,0=>99)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,99=>11)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,11=>9)
Calc32.NonVisual.CalculatorClass.Change(OperationText,/=>=)
Calc32.NonVisual.CalculatorClass.Change(Memory,99=>0)
" })]
        [DataRow("99 & 7", CalculatorClass.eOpMode.BinaryAnd, new int[] { 99, 7 }, 3, new string[] { @"Calc32.NonVisual.CalculatorClass.Change(OperationText,=>=)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,0=>99)
Calc32.NonVisual.CalculatorClass.Change(OperationText,==>&)
Calc32.NonVisual.CalculatorClass.Change(Memory,0=>99)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,99=>7)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,7=>3)
Calc32.NonVisual.CalculatorClass.Change(OperationText,&=>=)
Calc32.NonVisual.CalculatorClass.Change(Memory,99=>0)
" })]
        [DataRow("12 | 7", CalculatorClass.eOpMode.BinaryOr, new int[] { 12, 7 }, 15, new string[] { @"Calc32.NonVisual.CalculatorClass.Change(OperationText,=>=)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,0=>12)
Calc32.NonVisual.CalculatorClass.Change(OperationText,==>|)
Calc32.NonVisual.CalculatorClass.Change(Memory,0=>12)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,12=>7)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,7=>15)
Calc32.NonVisual.CalculatorClass.Change(OperationText,|=>=)
Calc32.NonVisual.CalculatorClass.Change(Memory,12=>0)
" })]
        [DataRow("12 x 7", CalculatorClass.eOpMode.BinaryXor, new int[] { 12, 7 }, 11, new string[] { @"Calc32.NonVisual.CalculatorClass.Change(OperationText,=>=)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,0=>12)
Calc32.NonVisual.CalculatorClass.Change(OperationText,==>x)
Calc32.NonVisual.CalculatorClass.Change(Memory,0=>12)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,12=>7)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,7=>11)
Calc32.NonVisual.CalculatorClass.Change(OperationText,x=>=)
Calc32.NonVisual.CalculatorClass.Change(Memory,12=>0)
" })]
        [DataRow("12 ! 7", CalculatorClass.eOpMode.BinaryNot, new int[] { 12, -2 }, -2, new string[] { @"Calc32.NonVisual.CalculatorClass.Change(OperationText,=>=)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,0=>12)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,12=>-13)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,-13=>-2)
" })]
        [DataRow("12 ~ 7", CalculatorClass.eOpMode.Negate, new int[] { 12, -2 }, -2, new string[] { @"Calc32.NonVisual.CalculatorClass.Change(OperationText,=>=)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,0=>12)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,12=>-12)
Calc32.NonVisual.CalculatorClass.Change(Accumulator,-12=>-2)
" })]
        public void OperationTest(string name, CalculatorClass.eOpMode eO, int[] aiData, int iExp, string[] asExp)
        {
            FCalculatorClass.OnChange += CalcChange;
            FCalculatorClass.Operation(1);
            FCalculatorClass.Accumulator = aiData[0];
            FCalculatorClass.Operation((int)eO);
            FCalculatorClass.Accumulator = aiData[1];
            FCalculatorClass.Operation(1);
            Assert.AreEqual(iExp, FCalculatorClass.Accumulator, $"Calc({name}).result");
            Assert.AreEqual(asExp[0], DebugLog);
        }

    }
}