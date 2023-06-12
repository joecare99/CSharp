﻿// ***********************************************************************
// Assembly         : MVVM_05_CommandParCalculator_netTests
// Author           : Mir
// Created          : 05-11-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="CommandParCalculatorViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using MVVM_05_CommandParCalculator.Model;
using System;
using System.Globalization;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_05_CommandParCalculator.ViewModels.Tests
{
    /// <summary>
    /// Defines test class CommandParCalculatorViewModelTests.
    /// Implements the <see cref="BaseTestViewModel" />
    /// </summary>
    /// <seealso cref="BaseTestViewModel" />
    /// <autogeneratedoc />
    [TestClass()]
    public class CommandParCalculatorViewModelTests : BaseTestViewModel
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        /// <summary>
        /// The test model
        /// </summary>
        /// <autogeneratedoc />
        CommandParCalculatorViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        /// <summary>
        /// The old model
        /// </summary>
        /// <autogeneratedoc />
        private Func<ICalculatorModel> _oldModel;
        /// <summary>
        /// The test calculate model
        /// </summary>
        /// <autogeneratedoc />
        private TestCalcModel testCalcModel;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <autogeneratedoc />
        [TestInitialize]
        public void Init()
        {
            _oldModel = CommandParCalculatorViewModel.GetModel;
            testCalcModel = new TestCalcModel(DoLog);
            CommandParCalculatorViewModel.GetModel = () => testCalcModel; 
            testModel = new();
            testModel.PropertyChanged += OnVMPropertyChanged;
            Assert.AreEqual(false, testModel.canOperator(EOperations.CalcResult));
            Assert.AreEqual(false, testModel.canOperator(EOperations.Add));
            Assert.AreEqual(false, testModel.canCommand(ECommands.DecMode));
            Assert.AreEqual(false, testModel.canCommand(ECommands.MR));
            ClearLog();
        }
        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        /// <autogeneratedoc />
        [TestCleanup]
        public void Cleanup()
        {
            CommandParCalculatorViewModel.GetModel = _oldModel;
        }

        /// <summary>
        /// Defines the test method CommandParCalculatorViewModelTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod]
        public void CommandParCalculatorViewModelTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModel));
            Assert.IsInstanceOfType(testModel, typeof(CommandParCalculatorViewModel));
            Assert.AreEqual(0d, testModel.Accumulator);
            Assert.AreEqual(double.NaN, testModel.Register);
            Assert.AreEqual(double.NaN, testModel.Memory);
            Assert.AreEqual("None Grad ", testModel.Status);
            Assert.AreEqual(false, testModel.canOperator(EOperations.CalcResult));
            Assert.AreEqual(false, testModel.canOperator(EOperations.Add));
            Assert.AreEqual(false, testModel.canCommand(ECommands.DecMode));
            Assert.AreEqual(false, testModel.canCommand(ECommands.MR));
            Assert.IsNotNull( testModel.NumberCommand);
            Assert.IsNotNull( testModel.OperatorCommand);
            Assert.IsNotNull( testModel.CalculatorCommand);
            Assert.AreEqual("canOperator(CalcResult=False)\r\ncanOperator(Add=False)\r\ncanCommand(DecMode)=False\r\ncanCommand(MR)=False\r\n", DebugLog);
        }

        /// <summary>
        /// Accumulators the test.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="adVal">The ad value.</param>
        /// <param name="asExp">As exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow("0",new double[] {0d }, new string[] { "0", "" })]
        [DataRow("25", new double[] { 25d }, new string[] { "25", @"canOperator(CalcResult=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=25
" })]
#if NET5_0_OR_GREATER
        [DataRow("PI,E", new double[] { Math.PI,Math.E }, new string[] { "2.718281828459045", @"canOperator(CalcResult=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=3,141592653589793
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=2,718281828459045
" })]
        [DataRow("Min,NaN,+Inf", new double[] { double.MinValue, double.NaN,double.PositiveInfinity,double.Epsilon }, new string[] { "5E-324", @"canOperator(CalcResult=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=-1,7976931348623157E+308
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=NaN
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=∞
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=5E-324
" })]
#else
        [DataRow("PI,E", new double[] { Math.PI,Math.E }, new string[] { "2.71828182845905", @"canOperator(CalcResult=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=3,14159265358979
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=2,71828182845905
" })]
        [DataRow("Min,NaN,+Inf", new double[] { double.MinValue, double.NaN,double.PositiveInfinity,double.Epsilon }, new string[] { "4.94065645841247E-324", @"canOperator(CalcResult=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=-1,79769313486232E+308
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=NaN
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=∞
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=4,94065645841247E-324
" })]
#endif
        public void AccumulatorTest(string name, double[] adVal,  string[] asExp)
        {
            // Arrange
            // Act    
            foreach (var item in adVal) 
                testCalcModel.Accumulator = item;
            // Assert
            Assert.AreEqual(asExp[0], testModel.Accumulator.ToString(CultureInfo.InvariantCulture), "Accumulator");
            Assert.AreEqual(asExp[1], DebugLog, "DebugOut");
        }

        /// <summary>
        /// Registers the test.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="adVal">The ad value.</param>
        /// <param name="asExp">As exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow("0", new double[] { 0d }, new string[] { "0", @"canOperator(CalcResult=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=0
" })]
        [DataRow("25", new double[] { 25d }, new string[] { "25", @"canOperator(CalcResult=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=25
" })]
#if NET5_0_OR_GREATER
        [DataRow("PI,E", new double[] { Math.PI, Math.E }, new string[] { "2.718281828459045", @"canOperator(CalcResult=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=3,141592653589793
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=2,718281828459045
" })]
        [DataRow("Min,NaN,+Inf", new double[] { double.MinValue, double.NaN, double.PositiveInfinity, double.Epsilon }, new string[] { "5E-324", @"canOperator(CalcResult=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=-1,7976931348623157E+308
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=NaN
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=∞
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=5E-324
" })]
#else
        [DataRow("PI,E", new double[] { Math.PI, Math.E }, new string[] { "2.71828182845905", @"canOperator(CalcResult=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=3,14159265358979
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=2,71828182845905
" })]
        [DataRow("Min,NaN,+Inf", new double[] { double.MinValue, double.NaN, double.PositiveInfinity, double.Epsilon }, new string[] { "4.94065645841247E-324", @"canOperator(CalcResult=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=-1,79769313486232E+308
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=NaN
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=∞
canOperator(CalcResult=False)
canOperator(Add=False)
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=4,94065645841247E-324
" })]
#endif
        public void RegisterTest(string name, double[] adVal, string[] asExp)
        {
            // Arrange
            // Act    
            foreach (var item in adVal)
                testCalcModel.Register = item;
            // Assert
            Assert.AreEqual(asExp[0], testModel.Register.ToString(CultureInfo.InvariantCulture), "Accumulator");
            Assert.AreEqual(asExp[1], DebugLog, "DebugOut");
        }

        /// <summary>
        /// Memories the test.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="adVal">The ad value.</param>
        /// <param name="asExp">As exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow("0", new double[] { 0d }, new string[] { "0", "PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=0\r\n" })]
        [DataRow("25", new double[] { 25d }, new string[] { "25", @"PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=25
" })]
#if NET5_0_OR_GREATER
        [DataRow("PI,E", new double[] { Math.PI, Math.E }, new string[] { "2.718281828459045", @"PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=3,141592653589793
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=2,718281828459045
" })]
        [DataRow("Min,NaN,+Inf", new double[] { double.MinValue, double.NaN, double.PositiveInfinity, double.Epsilon }, new string[] { "5E-324", @"PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=-1,7976931348623157E+308
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=NaN
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=∞
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=5E-324
" })]
#else
        [DataRow("PI,E", new double[] { Math.PI, Math.E }, new string[] { "2.71828182845905", @"PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=3,14159265358979
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=2,71828182845905
" })]
        [DataRow("Min,NaN,+Inf", new double[] { double.MinValue, double.NaN, double.PositiveInfinity, double.Epsilon }, new string[] { "4.94065645841247E-324", @"PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=-1,79769313486232E+308
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=NaN
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=∞
PropChg(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=4,94065645841247E-324
" })]
#endif
        public void MemoryTest(string name, double[] adVal, string[] asExp)
        {
            // Arrange
            // Act    
            foreach (var item in adVal)
                testCalcModel.Memory = item;
            // Assert
            Assert.AreEqual(asExp[0], testModel.Memory.ToString(CultureInfo.InvariantCulture), "Accumulator");
            Assert.AreEqual(asExp[1], DebugLog, "DebugOut");
        }
    }
}