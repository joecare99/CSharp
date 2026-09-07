using ConsoleLib.Showcase.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Tests;

[TestClass]
public sealed class CalculatorEngineTests
{
    [TestMethod]
    public void Addition_ComputesExpectedResult()
    {
        var calculator = new CalculatorEngine();

        calculator.Digit(1);
        calculator.Digit(2);
        calculator.SetOperation(CalculatorOperation.Add);
        calculator.Digit(3);
        calculator.Equals();

        Assert.AreEqual("15", calculator.Display);
        Assert.IsFalse(calculator.HasError);
    }

    [TestMethod]
    public void ChainedOperations_ApplyPreviousOperationBeforeNext()
    {
        var calculator = new CalculatorEngine();

        calculator.Digit(8);
        calculator.SetOperation(CalculatorOperation.Multiply);
        calculator.Digit(2);
        calculator.SetOperation(CalculatorOperation.Add);
        calculator.Digit(1);
        calculator.Equals();

        Assert.AreEqual("17", calculator.Display);
    }

    [TestMethod]
    public void DivisionByZero_ReportsRecoverableError()
    {
        var calculator = new CalculatorEngine();

        calculator.Digit(9);
        calculator.SetOperation(CalculatorOperation.Divide);
        calculator.Digit(0);
        calculator.Equals();

        Assert.IsTrue(calculator.HasError);
        Assert.AreEqual("Cannot divide by zero", calculator.Display);

        calculator.Digit(4);

        Assert.IsFalse(calculator.HasError);
        Assert.AreEqual("4", calculator.Display);
    }

    [TestMethod]
    public void DecimalBackspaceAndSign_UpdateDisplay()
    {
        var calculator = new CalculatorEngine();

        calculator.Digit(1);
        calculator.DecimalPoint();
        calculator.Digit(5);
        calculator.ToggleSign();
        calculator.Backspace();

        Assert.AreEqual("-1.", calculator.Display);
    }
}
