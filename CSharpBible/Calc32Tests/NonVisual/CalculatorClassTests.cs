using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpBible.Calc32.NonVisual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBible.Calc32.NonVisual.Tests
{
    [TestClass()]
    public class CalculatorClassTests
    {
        private CalculatorClass FCalculatorClass;
        private int nChanges;

        [TestInitialize()]
        public void Init()
        {
            FCalculatorClass = new CalculatorClass();
        }

        [TestMethod()]
        public void TestSetup()
        {
            Assert.IsNotNull(FCalculatorClass);
            Assert.AreEqual(0,FCalculatorClass.Akkumulator);
        }

        [TestMethod()]
        public void CalculatorClassTest()
        {
            Assert.IsInstanceOfType(FCalculatorClass,typeof(CalculatorClass));
        }

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

        private void CalcChange(object sender, EventArgs e)
        {
            nChanges++;
        }

        [TestMethod()]
        public void OnChangeTest1()
        {
            FCalculatorClass.OnChange += new EventHandler(CalcChange);
            AkkumulatorTest();
            Assert.AreEqual(3, nChanges);
        }

        [TestMethod()]
        public void OnChangeTest2()
        {
            FCalculatorClass.OnChange += new EventHandler(CalcChange);
            FCalculatorClass.Button(3);
            Assert.AreEqual(3, FCalculatorClass.Akkumulator);
            FCalculatorClass.Button(2);
            Assert.AreEqual(32, FCalculatorClass.Akkumulator);
            FCalculatorClass.Button(1);
            Assert.AreEqual(321, FCalculatorClass.Akkumulator);
            Assert.AreEqual(3, nChanges);
        }

        [TestMethod()]
        public void ButtonTest()
        {
            FCalculatorClass.Button(4);
            Assert.AreEqual(4, FCalculatorClass.Akkumulator);
            FCalculatorClass.Button(3);
            Assert.AreEqual(43, FCalculatorClass.Akkumulator);
            FCalculatorClass.Button(2);
            Assert.AreEqual(432, FCalculatorClass.Akkumulator);
        }

        [TestMethod()]
        public void ButtonBack()
        {
            FCalculatorClass.OnChange += new EventHandler(CalcChange);
            FCalculatorClass.Button(4);
            Assert.AreEqual(4, FCalculatorClass.Akkumulator);
            FCalculatorClass.Button(3);
            Assert.AreEqual(43, FCalculatorClass.Akkumulator);
            FCalculatorClass.Button(2);
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