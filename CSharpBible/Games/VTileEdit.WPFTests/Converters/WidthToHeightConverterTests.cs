using System.Globalization;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VTileEdit.WPF.Converters;

namespace VTileEdit.WPF.Tests.Converters;

[TestClass]
public sealed class WidthToHeightConverterTests
{
    [TestMethod]
    public void Convert_ShouldApplyDefaultRatio()
    {
        var converter = new WidthToHeightConverter();

        var result = converter.Convert(10d, typeof(double), parameter: null, culture: CultureInfo.InvariantCulture);

        Assert.AreEqual(20d, result);
    }

    [TestMethod]
    public void Convert_ShouldRespectCustomRatio()
    {
        var converter = new WidthToHeightConverter { Ratio = 1.5d };

        var result = converter.Convert(8d, typeof(double), parameter: null, culture: CultureInfo.InvariantCulture);

        Assert.AreEqual(12d, result);
    }

    [TestMethod]
    [DataRow(double.NaN)]
    [DataRow(double.PositiveInfinity)]
    [DataRow("not-a-double")]
    [DataRow(null)]
    public void Convert_InvalidValues_ShouldReturnZero(object value)
    {
        var converter = new WidthToHeightConverter();

        var result = converter.Convert(value, typeof(double), parameter: null, culture: CultureInfo.InvariantCulture);

        Assert.AreEqual(0d, result);
    }

    [TestMethod]
    public void ConvertBack_ShouldReturnBindingDoNothing()
    {
        var converter = new WidthToHeightConverter();

        var result = converter.ConvertBack(5d, typeof(double), parameter: null, culture: CultureInfo.InvariantCulture);

        Assert.AreEqual(Binding.DoNothing, result);
    }
}
