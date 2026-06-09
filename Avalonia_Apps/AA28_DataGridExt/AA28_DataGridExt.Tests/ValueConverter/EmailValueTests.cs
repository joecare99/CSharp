using AA28_DataGridExt.ValueConverter;
using System.Globalization;

namespace AA28_DataGridExt.Tests.ValueConverter;

[TestClass]
public class EmailValueTests
{
    private readonly EmailValue _converter = new();

    [TestMethod]
    [DataRow(null, "")]
    [DataRow("", "")]
    [DataRow("info@muster.com", "mailto:info@muster.com")]
    public void ConvertReturnsMailtoUriTest(string? value, string expected)
    {
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("", "")]
    [DataRow("", null)]
    [DataRow("info@muster.com", "mailto:info@muster.com")]
    public void ConvertBackReturnsPlainAddressTest(string expected, string? value)
    {
        var result = _converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.AreEqual(expected, result);
    }
}
