// Pseudocode-Plan:
// - Tests von xUnit auf MSTest umstellen
// - Test `Next`:
//   - Wert liegt innerhalb [min, max) über viele Iterationen
//   - Gibt `min` zurück, wenn `min == max`
//   - Wirft ArgumentOutOfRangeException, wenn `min > max`
//   - Bei Intervallbreite 1 (z. B. [5,6)) immer 5
// - Test `NextSingle`:
//   - Wert liegt in [0.0f, 1.0f) über viele Iterationen

using System;
using AsteroidsModern.Engine.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AsteroidsModern.Engine.Model.Tests;

[TestClass]
public class CRandomTests
{
    [TestMethod]
    public void Next_ReturnsValuesWithinRange()
    {
        var rnd = new CRandom();
        for (int i = 0; i < 1000; i++)
        {
            int value = rnd.Next(-10, 10);
            Assert.IsTrue(value >= -10 && value <= 9, $"Wert {value} außerhalb des erwarteten Bereichs.");
        }
    }

    [TestMethod]
    public void Next_ReturnsMin_WhenMinEqualsMax()
    {
        var rnd = new CRandom();
        Assert.AreEqual(42, rnd.Next(42, 42));
    }

    [TestMethod]
    public void Next_Throws_WhenMinGreaterThanMax()
    {
        var rnd = new CRandom();
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => rnd.Next(10, 0));
    }

    [TestMethod]
    public void Next_WithUnitRange_ReturnsOnlyMin()
    {
        var rnd = new CRandom();
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual(5, rnd.Next(5, 6));
        }
    }

    [TestMethod]
    public void NextSingle_ReturnsValuesWithinRange()
    {
        var rnd = new CRandom();
        for (int i = 0; i < 5000; i++)
        {
            float value = rnd.NextSingle();
            Assert.IsTrue(value >= 0f && value < 1f, $"Wert {value} außerhalb des erwarteten Bereichs.");
        }
    }
}