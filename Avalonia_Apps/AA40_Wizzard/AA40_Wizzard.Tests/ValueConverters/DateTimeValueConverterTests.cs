using System;
using System.Globalization;
using AA40_Wizzard.ValueConverters;
using Avalonia.Data;

namespace AA40_Wizzard.Tests.ValueConverters;

[TestClass]
public class DateTimeValueConverterTests
{
    private readonly DateTimeValueConverter _converter = new();

    [TestMethod]
    public void ConvertFormatsDateTimeTest()
    {
        var result = _converter.Convert(new DateTime(2025, 1, 2, 3, 4, 5), typeof(string), "yyyy-MM-dd", CultureInfo.InvariantCulture);

        Assert.AreEqual("2025-01-02", result);
    }

    [TestMethod]
    public void ConvertBackParsesDateTimeTest()
    {
        var result = _converter.ConvertBack("2025-01-02", typeof(DateTime), "yyyy-MM-dd", CultureInfo.InvariantCulture);

        Assert.AreEqual(new DateTime(2025, 1, 2), result);
    }

    [TestMethod]
    public void ConvertBackReturnsDoNothingForInvalidInputTest()
    {
        var result = _converter.ConvertBack("invalid", typeof(DateTime), null, CultureInfo.InvariantCulture);

        Assert.AreSame(BindingOperations.DoNothing, result);
    }
}
