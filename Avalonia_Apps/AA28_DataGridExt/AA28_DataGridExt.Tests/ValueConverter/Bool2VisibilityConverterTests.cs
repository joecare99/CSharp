using AA28_DataGridExt.ValueConverter;
using System;
using System.Globalization;

namespace AA28_DataGridExt.Tests.ValueConverter;

[TestClass]
public class Bool2VisibilityConverterTests
{
    private readonly Bool2VisibilityConverter _converter = new();

    [TestMethod]
    [DataRow(true, true)]
    [DataRow(false, false)]
    public void ConvertReturnsExpectedVisibilityTest(bool value, bool expected)
    {
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow(true, true)]
    [DataRow(false, false)]
    public void ConvertBackReturnsExpectedBooleanTest(bool value, bool expected)
    {
        var result = _converter.ConvertBack(value, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.AreEqual(expected, result);
    }
}
