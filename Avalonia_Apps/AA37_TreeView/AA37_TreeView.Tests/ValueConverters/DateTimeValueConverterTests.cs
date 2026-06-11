using System;
using System.Globalization;
using AA37_TreeView.ValueConverters;

namespace AA37_TreeView.Tests.ValueConverters;

[TestClass]
public class DateTimeValueConverterTests
{
    private readonly DateTimeValueConverter _converter = new();

    [TestMethod]
    [DataRow("2023-01-01 22:00", null, "01/01/2023 22:00:00")]
    [DataRow("2023-05-02 12:30", "MM/dd/yyyy", "05/02/2023")]
    [DataRow("Hallo", null, "Hallo")]
    [DataRow(null, null, "")]
    public void ConvertTest(string? input, string? format, string expected)
    {
        object? value = input;
        if (input is not null && DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            value = parsed;
        }

        var result = _converter.Convert(value, typeof(string), format, CultureInfo.InvariantCulture);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("2023-01-01 22:30", "2023-01-01 22:30:00")]
    [DataRow("2023-05-02", "2023-05-02 00:00:00")]
    [DataRow("invalid", "0001-01-01 00:00:00")]
    public void ConvertBackTest(string input, string expected)
    {
        var result = _converter.ConvertBack(input, typeof(DateTime), null, CultureInfo.InvariantCulture);

        Assert.AreEqual(DateTime.Parse(expected, CultureInfo.InvariantCulture), result);
    }
}
