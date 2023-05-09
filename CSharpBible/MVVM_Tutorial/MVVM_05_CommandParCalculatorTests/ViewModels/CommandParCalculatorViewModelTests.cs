using BaseLib.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using MVVM_05_CommandParCalculator.Model;
using System;
using System.ComponentModel;
using System.Globalization;

namespace MVVM_05_CommandParCalculator.ViewModels.Tests
{
    [TestClass()]
    public class CommandParCalculatorViewModelTests
    {
        CommandParCalculatorViewModel testModel;

        public string DebugOut = "";
        private Func<ICalculatorModel> _oldModel;
        private TestCalcModel testCalcModel;

        [TestInitialize]
        public void Init()
        {
            _oldModel = CommandParCalculatorViewModel.GetModel;
            testCalcModel = new TestCalcModel(DoLog);
            CommandParCalculatorViewModel.GetModel = () => testCalcModel; 
            testModel = new();
            testModel.PropertyChanged += OnPropertyChanged;
            Assert.AreEqual(false, testModel.canOperator(EOperations.CalcResult));
            Assert.AreEqual(false, testModel.canOperator(EOperations.Add));
            Assert.AreEqual(false, testModel.canCommand(ECommands.DecMode));
            Assert.AreEqual(false, testModel.canCommand(ECommands.MR));
            DebugOut = "";
        }
        [TestCleanup]
        public void Cleanup()
        {
            CommandParCalculatorViewModel.GetModel = _oldModel;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            DoLog($"PropChange({sender},{e.PropertyName})={sender.GetProp(e.PropertyName)}");
        }

        private void DoLog(string v)
        {
            DebugOut += $"{v}{Environment.NewLine}";
        }

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
            Assert.AreEqual("canOperator(CalcResult=False)\r\ncanOperator(Add=False)\r\ncanCommand(DecMode)=False\r\ncanCommand(MR)=False\r\n", DebugOut);
        }

        [DataTestMethod()]
        [DataRow("0",new double[] {0d }, new string[] { "0", "" })]
        [DataRow("25", new double[] { 25d }, new string[] { "25", @"canOperator(CalcResult=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=25
" })]
#if NET5_0_OR_GREATER
        [DataRow("PI,E", new double[] { Math.PI,Math.E }, new string[] { "2.718281828459045", @"canOperator(CalcResult=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=3,141592653589793
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=2,718281828459045
" })]
        [DataRow("Min,NaN,+Inf", new double[] { double.MinValue, double.NaN,double.PositiveInfinity,double.Epsilon }, new string[] { "5E-324", @"canOperator(CalcResult=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=-1,7976931348623157E+308
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=NaN
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=∞
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=5E-324
" })]
#else
        [DataRow("PI,E", new double[] { Math.PI,Math.E }, new string[] { "2.71828182845905", @"canOperator(CalcResult=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=3,14159265358979
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=2,71828182845905
" })]
        [DataRow("Min,NaN,+Inf", new double[] { double.MinValue, double.NaN,double.PositiveInfinity,double.Epsilon }, new string[] { "4.94065645841247E-324", @"canOperator(CalcResult=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=-1,79769313486232E+308
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=NaN
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=∞
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Accumulator)=4,94065645841247E-324
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
            Assert.AreEqual(asExp[1], DebugOut, "DebugOut");
        }

        [DataTestMethod()]
        [DataRow("0", new double[] { 0d }, new string[] { "0", @"canOperator(CalcResult=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=0
" })]
        [DataRow("25", new double[] { 25d }, new string[] { "25", @"canOperator(CalcResult=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=25
" })]
#if NET5_0_OR_GREATER
        [DataRow("PI,E", new double[] { Math.PI, Math.E }, new string[] { "2.718281828459045", @"canOperator(CalcResult=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=3,141592653589793
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=2,718281828459045
" })]
        [DataRow("Min,NaN,+Inf", new double[] { double.MinValue, double.NaN, double.PositiveInfinity, double.Epsilon }, new string[] { "5E-324", @"canOperator(CalcResult=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=-1,7976931348623157E+308
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=NaN
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=∞
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=5E-324
" })]
#else
        [DataRow("PI,E", new double[] { Math.PI, Math.E }, new string[] { "2.71828182845905", @"canOperator(CalcResult=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=3,14159265358979
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=2,71828182845905
" })]
        [DataRow("Min,NaN,+Inf", new double[] { double.MinValue, double.NaN, double.PositiveInfinity, double.Epsilon }, new string[] { "4.94065645841247E-324", @"canOperator(CalcResult=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,canOperator)=
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=-1,79769313486232E+308
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=NaN
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=∞
canOperator(CalcResult=False)
canOperator(Add=False)
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Register)=4,94065645841247E-324
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
            Assert.AreEqual(asExp[1], DebugOut, "DebugOut");
        }

        [DataTestMethod()]
        [DataRow("0", new double[] { 0d }, new string[] { "0", "PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=0\r\n" })]
        [DataRow("25", new double[] { 25d }, new string[] { "25", @"PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=25
" })]
#if NET5_0_OR_GREATER
        [DataRow("PI,E", new double[] { Math.PI, Math.E }, new string[] { "2.718281828459045", @"PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=3,141592653589793
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=2,718281828459045
" })]
        [DataRow("Min,NaN,+Inf", new double[] { double.MinValue, double.NaN, double.PositiveInfinity, double.Epsilon }, new string[] { "5E-324", @"PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=-1,7976931348623157E+308
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=NaN
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=∞
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=5E-324
" })]
#else
        [DataRow("PI,E", new double[] { Math.PI, Math.E }, new string[] { "2.71828182845905", @"PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=3,14159265358979
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=2,71828182845905
" })]
        [DataRow("Min,NaN,+Inf", new double[] { double.MinValue, double.NaN, double.PositiveInfinity, double.Epsilon }, new string[] { "4.94065645841247E-324", @"PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=-1,79769313486232E+308
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=NaN
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=∞
PropChange(MVVM_05_CommandParCalculator.ViewModels.CommandParCalculatorViewModel,Memory)=4,94065645841247E-324
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
            Assert.AreEqual(asExp[1], DebugOut, "DebugOut");
        }
    }
}