using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FroniusMonitor.Avalonia.Converters;

namespace FroniusMonitor.Avalonia.Tests.Converters;

[TestClass]
public sealed class AbsolutePowerToLogSizeConverterTests
{
    [TestMethod]
    [DataRow(0d, 8d)]
    [DataRow(10000d, 24d)]
    public void Convert_ReturnsExpectedRange(double fInput, double fExpectedBoundary)
    {
        AbsolutePowerToLogSizeConverter converter = new();

        object result = converter.Convert(fInput, typeof(double), null, System.Globalization.CultureInfo.InvariantCulture);

        Assert.IsTrue(result is double);
        double fResult = (double)result;
        if (fInput == 0d)
        {
            Assert.AreEqual(fExpectedBoundary, fResult, 0.0001d);
        }
        else
        {
            Assert.AreEqual(fExpectedBoundary, fResult, 0.0001d);
        }
    }
}
