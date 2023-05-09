using MVVM_05_CommandParCalculator.Model;
using BaseLib.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;
using System.ComponentModel;

namespace MVVM_05_CommandParCalculator.Model.Tests
{
    [TestClass]
    public class CalculatorModelTests
    {
        CalculatorModel testModel;

        public string DebugOut = "";

        [TestInitialize]
        public void Init()
        {
            testModel = new();
            testModel.PropertyChanged += OnPropertyChanged;
            DebugOut = "";
        }
        [TestCleanup]
        public void Cleanup()
        {
            // Nothing to do 
        }

        [TestMethod]
        public void SetUpTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(NotificationObject));
            Assert.IsInstanceOfType(testModel, typeof(CalculatorModel));
            Assert.IsInstanceOfType(CalculatorModel.Instance, typeof(CalculatorModel));
            Assert.AreEqual(0d, testModel.Accumulator);
            Assert.AreEqual(null, testModel.Register);
            Assert.AreEqual(null, testModel.Memory);
            Assert.AreEqual(false, testModel.DecMode);
            Assert.AreEqual(false, testModel.canOperator(EOperations.CalcResult));
            Assert.AreEqual(false, testModel.canOperator(EOperations.Add));
            Assert.AreEqual(true, testModel.canCommand(ECommands.DecMode));
            Assert.AreEqual(true, testModel.canCommand(ECommands.MR));
            Assert.AreEqual(ETrigMode.Grad, testModel.TrigMode);
            Assert.AreEqual(ECalcError.None, testModel.CalcError);
            Assert.AreEqual(0, testModel.StackSize);
            Assert.AreEqual("", DebugOut);
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            DoLog($"PropChange({sender},{e.PropertyName})={sender.GetProp(e.PropertyName)}");
        }

        private void DoLog(string v)
        {
            DebugOut += $"{v}{Environment.NewLine}";
        }

        [DataTestMethod]
        [DataRow("0", new ENumbers[] { ENumbers._0 }, 0d, new string[] { "" })]
        [DataRow("123", new ENumbers[] { ENumbers._1, ENumbers._2, ENumbers._3 }, 123d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=1
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=12
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=123
" })]
        [DataRow("987", new ENumbers[] { ENumbers._9, ENumbers._8, ENumbers._7 }, 987d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=9
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=98
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=987
" })]
        public void NumberCmdTest(string name, ENumbers[] aeVal, double dExp, string[] asExp)
        {
            // Arrange

            // Act
            foreach (var ae in aeVal)
                testModel.NumberCmd(ae);

            // Assert
            Assert.AreEqual(dExp, testModel.Accumulator);
            Assert.AreEqual(true, testModel.canOperator(EOperations.Add));
            Assert.AreEqual(asExp[0], DebugOut);
        }

        [DataTestMethod]
        [DataRow("0", new ENumbers[] { ENumbers._0 }, 0d, new string[] { "" })]
#if NET5_0_OR_GREATER
        [DataRow("123", new ENumbers[] { ENumbers._1, ENumbers._2, ENumbers._3 }, 0.123d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0,1
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0,12000000000000001
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0,12300000000000001
" })]
#else
        [DataRow("123", new ENumbers[] { ENumbers._1, ENumbers._2, ENumbers._3 }, 0.123d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0,1
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0,12
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0,123
" })]
#endif
        [DataRow("987", new ENumbers[] { ENumbers._9, ENumbers._8, ENumbers._7 }, 0.987d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0,9
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0,98
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0,987
" })]
        public void NumberCmd2Test(string name, ENumbers[] aeVal, double dExp, string[] asExp)
        {
            // Arrange
            testModel.NumberCmd(ENumbers._0);
            testModel.DecMode = true;
            // Act
            foreach (var ae in aeVal)
                testModel.NumberCmd(ae);

            // Assert
            Assert.AreEqual(dExp, testModel.Accumulator, 1e-10);
            Assert.AreEqual(asExp[0], DebugOut);
        }

        [DataTestMethod]
        [DataRow(EOperations.Add, 1d, 2d, 3d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=1
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Register)=1
", @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=2
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=3
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Register)=
" })]
        [DataRow(EOperations.Subtract, 3d, 2d, 1d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=3
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Register)=3
", @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=2
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=1
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Register)=
" })]
        [DataRow(EOperations.Multiply, 2d, 3d, 6d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=2
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Register)=2
", @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=3
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=6
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Register)=
" })]
        [DataRow(EOperations.Divide, 6d, 3d, 2d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=6
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Register)=6
", @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=3
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=2
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Register)=
" })]
        [DataRow(EOperations.Power, 2d, 3d, 8d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=2
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Register)=2
", @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=3
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=8
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Register)=
" })]
        public void OperatorCmdTest(EOperations eOp, double dVal1, double dVal2, double dExp, string[] asExp)
        {
            // Arrange
            testModel.Accumulator = dVal1;

            // Act
            testModel.OperatorCmd(eOp);

            // Assert
            Assert.AreEqual(dVal1, testModel.Accumulator);
            Assert.AreEqual(dVal1, testModel.Register);
            Assert.AreEqual(asExp[0], DebugOut);
            DebugOut = "";

            // Arrange
            testModel.Accumulator = dVal2;
            // Act2
            testModel.OperatorCmd(EOperations.CalcResult);
            // Assert2
            Assert.AreEqual(dExp, testModel.Accumulator);
            Assert.AreEqual(null, testModel.Register);
            Assert.AreEqual(asExp[1], DebugOut);
        }

        [DataTestMethod]
        [DataRow(EOperations.Negate, 1d, 2d, -1d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=1
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=-1
", @"" })]
        [DataRow(EOperations.Square, 3d, 2d, 9d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=3
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=9
", @"" })]
        [DataRow(EOperations.SquareRt, 4d, 3d, 2d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=4
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=2
", @"" })]
        [DataRow(EOperations.Inverse, 4d, 3d, 0.25d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=4
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0,25
", @"" })]
#if NET5_0_OR_GREATER
        [DataRow(EOperations.Sin, Math.PI * 0.5d, 3d, 1d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=1,5707963267948966
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=1
", @"" })]
        [DataRow(EOperations.Cos, Math.PI, 3d, -1d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=3,141592653589793
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=-1
", @"" })]
        [DataRow(EOperations.Tan, Math.PI * 0.25, 0d, 1d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0,7853981633974483
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0,9999999999999999
", @"" })]
#else
        [DataRow(EOperations.Sin, Math.PI * 0.5d, 3d, 1d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=1,5707963267949
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=1
", @"" })]
        [DataRow(EOperations.Cos, Math.PI, 3d, -1d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=3,14159265358979
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=-1
", @"" })]
        [DataRow(EOperations.Tan, Math.PI * 0.25, 0d, 1d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0,785398163397448
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=1
", @"" })]
#endif
        [DataRow(EOperations.ExpN, 0d, 3d, 1d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=1
", @"" })]
        [DataRow(EOperations.LogN, 1d, 0d, 0d, new string[] { @"PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=1
PropChange(MVVM_05_CommandParCalculator.Model.CalculatorModel,Accumulator)=0
", @"" })]
        public void OperatorCmd2Test(EOperations eOp, double dVal1, double _, double dExp, string[] asExp)
        {
            // Arrange
            testModel.Accumulator = dVal1;

            // Act
            testModel.OperatorCmd(eOp);

            // Assert
            Assert.AreEqual(dExp, testModel.Accumulator, 1e-10);
            Assert.AreEqual(null, testModel.Register);
            Assert.AreEqual(asExp[0], DebugOut);

        }

        [DataTestMethod]
        [DataRow(ECommands.Nop, 123d, 123d, 123d)]
        [DataRow(ECommands.Clear, 123d, 0d, 123d)]
        [DataRow(ECommands.ClearAll, 123d, 0d, 123d)]
        [DataRow(ECommands.e, 0d, Math.E, 0d)]
        [DataRow(ECommands.Pi, 0d, Math.PI, 0d)]
        [DataRow(ECommands.MS, 123d, 123d, 123d)]
        [DataRow(ECommands.MR, 123d, 123d, 123d)]
        [DataRow(ECommands.MC, 123d, 123d, null)]
        [DataRow(ECommands.Mp, 123d, 123d, 246d)]
        [DataRow(ECommands.Mm, 123d, 123d, 0d)]
        public void CalcCmdTest(ECommands eCmd, double dVal1,
       double dAExp,
       double? dMExp)
        {
            // Arrange
            testModel.Accumulator = dVal1;
            testModel.Memory = dVal1;

            // Act
            testModel.CalcCmd(eCmd);

            // Assert
            Assert.AreEqual(dAExp, testModel.Accumulator);
            Assert.AreEqual(dMExp, testModel.Memory);
        }

        [TestMethod]
        public void CalcCmd2Test()
        {
            testModel.CalcCmd(ECommands.DecMode);
            Assert.IsTrue(testModel.DecMode);
        }

        [TestMethod()]
        [DataRow((EOperations)(-1), EOperations.Nop, 0d, false)]
        [DataRow(EOperations.Nop,EOperations.Add,3d,true)]
        [DataRow(EOperations.CalcResult, EOperations.Add, 3d, true)]
        [DataRow(EOperations.Subtract, EOperations.Add, 3d, true)]
        public void canOperatorTest(EOperations eVal, EOperations ePre,double dAkk,bool xResult)
        {
            // Arrange
            testModel.Accumulator = dAkk;
            testModel.OperatorCmd(ePre);
            // Act & Assert
            Assert.AreEqual(xResult,testModel.canOperator(eVal));
        }
    }
}
