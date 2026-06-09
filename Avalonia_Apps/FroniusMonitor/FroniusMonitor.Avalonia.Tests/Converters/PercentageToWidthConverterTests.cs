using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FroniusMonitor.Avalonia.Converters;

namespace FroniusMonitor.Avalonia.Tests.Converters;

[TestClass]
public sealed class PercentageToWidthConverterTests
{
    [TestMethod]
    [DataRow(-10d, 0d)]
    [DataRow(50d, 28d)]
    [DataRow(100d, 56d)]
    [DataRow(120d, 56d)]
    public void Convert_ClampsAndScalesPercent(double fInput, double fExpected)
    {
        PercentageToWidthConverter converter = new();

        object result = converter.Convert(fInput, typeof(double), null, System.Globalization.CultureInfo.InvariantCulture);

        Assert.IsInstanceOfType<double>(result);
        Assert.AreEqual(fExpected, (double)result, 0.0001d);
    }
}
