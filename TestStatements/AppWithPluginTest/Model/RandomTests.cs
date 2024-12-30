using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppWithPlugin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginBase.Interfaces;

namespace AppWithPlugin.Model.Tests
{
    [TestClass()]
    public class RandomTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private Random _testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

        [TestInitialize]
        public void Initialize()
        {
            _testClass = new Random();
        }

        [TestMethod()]
        public void RandomTest()
        {
            // Assert
            Assert.IsNotNull((Random?)_testClass);
            Assert.IsInstanceOfType((Random?)_testClass, typeof(Random));
            Assert.IsInstanceOfType((Random?)_testClass, typeof(IRandom));
        }

        [TestMethod()]
        public void NextTest()
        {
            // Act
            var result = _testClass.Next();

            // Assert
            Assert.IsInstanceOfType(result, typeof(int));

        }

        [TestMethod()]
        public void NextTest1()
        {
            // Act
            int minValue = 1;
            int maxValue = 10;
            var result = _testClass.Next(minValue, maxValue);

            // Assert
            Assert.IsInstanceOfType(result, typeof(int));
            Assert.IsTrue(result >= minValue && result < maxValue);
        }

        [TestMethod()]
        public void NextTest2()
        {
            // Act
            int maxValue = 10;
            var result = _testClass.Next(maxValue);

            // Assert
            Assert.IsInstanceOfType(result, typeof(int));
            Assert.IsTrue(result >= 0 && result < maxValue);
        }

        [TestMethod()]
        public void NextBytesTest()
        {
            // Arrange
            byte[] buffer = new byte[10];

            // Act
            _testClass.NextBytes(buffer);

            // Assert
            Assert.IsNotNull(buffer);
            Assert.AreEqual(10, buffer.Length);
            Assert.IsTrue(buffer.Any(b => b != 0)); // Check that at least one byte is not zero
        }

        [TestMethod()]
        public void NextDoubleTest()
        {
            // Act
            var result = _testClass.NextDouble();

            // Assert
            Assert.IsInstanceOfType(result, typeof(double));
            Assert.IsTrue(result >= 0.0 && result < 1.0);
        }

        [TestMethod()]
        [DataRow(12345)]
        [DataRow(1)]
        [DataRow(-1)]
        [DataRow(int.MaxValue- 1)]
       // [DataRow(int.MaxValue)]
       // [DataRow(int.MinValue)]
        public void SeedTest(int iSeed)
        {
            // Act
            _testClass.Seed(iSeed);
            var firstResult = _testClass.Next();

            _testClass.Seed(iSeed);
            var secondResult = _testClass.Next();

            _testClass.Seed(unchecked(iSeed+1));
            var thirdResult = _testClass.Next();

            // Assert
            Assert.AreEqual(firstResult, secondResult);
            Assert.AreNotEqual(thirdResult, firstResult);
            Assert.AreNotEqual(thirdResult, secondResult);
        }
    }
}