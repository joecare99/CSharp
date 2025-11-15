using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using Avalonia.Media;

namespace AA22_AvlnCap.Converter.Tests;

[TestClass()]
public class EnumToColorStringConverterTests
{
    private EnumToColorConverter _testConverter = null!;

    private static string Hex(object? result)
    {
        if (result is ISolidColorBrush scb)
            return scb.Color.ToString();
        return string.Empty;
    }

    [TestInitialize()]
    public void Init()
    {
        _testConverter = new EnumToColorConverter();
    }

    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(_testConverter);
        Assert.IsInstanceOfType(_testConverter, typeof(EnumToColorConverter));
        Assert.IsNotNull(_testConverter.colors);
    }

    [DataTestMethod()]
    [DataRow("#FF008000", true, null)]
    [DataRow("#FFFF0000", false, null)]
    [DataRow("#FF008000", true, "345")]
    [DataRow("#FFFF0000", false, "345")]
    [DataRow("", new bool[] { true, false }, null)]
    [DataRow("", new bool[] { false,true }, null)]
    [DataRow("#FF008000", new bool[] { true, true }, "0")]
    [DataRow("#FF008000", new bool[] { true, false }, "0")]
    [DataRow("#FFFF0000", new bool[] { false, true }, "0")]
    [DataRow("#FFFF0000", new bool[] { false, false }, "0")]
    [DataRow("#FF008000", new bool[] { true, true }, "1")]
    [DataRow("#FFFF0000", new bool[] { true, false }, "1")]
    [DataRow("#FF008000", new bool[] { false, true }, "1")]
    [DataRow("#FFFF0000", new bool[] { false, false }, "1")]
    [DataRow("", new bool[] { false, false }, null)]
    [DataRow("#FF000000", new int[] { 0, 1, 2, 3, 4, 5 }, "0")]
    [DataRow("#FFFF0000", new int[] { 0, 1, 2, 3, 4, 5 }, "1")]
    [DataRow("#FF008000", new int[] { 0, 1, 2, 3, 4, 5 }, "2")]
    [DataRow("#FFFFFF00", new int[] { 0, 1, 2, 3, 4, 5 }, "3")]
    [DataRow("#FF0000FF", new int[] { 0, 1, 2, 3, 4, 5 }, "4")]
    [DataRow("#FF000000", new int[] { 0, 1, 2, 3, 4, 5 }, "5")]
    [DataRow("", new int[] { 0, 1, 2, 3, 4, 5 }, "6")]
    [DataRow("#FF000000", new int[] { 0, 1, 2, 3, 4, 5, 6 }, "6")]
    [DataRow("#FF000000", new int[] { 5, 4, 3, 2, 1, 0 }, "0")]
    [DataRow("#FF0000FF", new int[] { 5, 4, 3, 2, 1, 0 }, "1")]
    [DataRow("#FFFFFF00", new int[] { 5, 4, 3, 2, 1, 0 }, "2")]
    [DataRow("#FF008000", new int[] { 5, 4, 3, 2, 1, 0 }, "3")]
    [DataRow("#FFFF0000", new int[] { 5, 4, 3, 2, 1, 0 }, "4")]
    [DataRow("#FF000000", new int[] { 5, 4, 3, 2, 1, 0 }, "5")]
    [DataRow("", new int[] { 5, 4, 3, 2, 1, 0 }, "6")]
    [DataRow("", new int[] { 5, 4, 3, 2, 1, 0 }, null)]
    public void ConvertTest(string sExp, object o,object p)
    {
        var r = _testConverter.Convert(o, typeof(object), p, CultureInfo.InvariantCulture);
        Assert.AreEqual(sExp, Hex(r));
    }

    [DataTestMethod()]
    [DataRow("#FF008000", true, null)]
    [DataRow("#FFFFD700", true, null, true)]
    [DataRow("#FFA52A2A", false, null, true)]
    [DataRow("#FF008000", true, "345")] 
    [DataRow("#FFFF0000", false, "345")] 
    [DataRow("", new bool[] { true, false }, null)]
    [DataRow("", new bool[] { false, true }, null)]
    [DataRow("#FF008000", new bool[] { true, true }, "0")]
    [DataRow("#FF008000", new bool[] { true, false }, "0")]
    [DataRow("#FFFF0000", new bool[] { false, true }, "0")]
    [DataRow("#FFFF0000", new bool[] { false, false }, "0")]
    [DataRow("#FF008000", new bool[] { true, true }, "1")]
    [DataRow("#FFFF0000", new bool[] { true, false }, "1")]
    [DataRow("#FF008000", new bool[] { false, true }, "1")]
    [DataRow("#FFFF0000", new bool[] { false, false }, "1")]
    [DataRow("#00FFFFFF", new int[] { 0, 1, 2, 3, 4, 5 }, "0", true)]
    [DataRow("#FFA52A2A", new int[] { 0, 1, 2, 3, 4, 5 }, "1", true)]
    [DataRow("#FFFFD700", new int[] { 0, 1, 2, 3, 4, 5 }, "2", true)]
    [DataRow("#FF00008B", new int[] { 0, 1, 2, 3, 4, 5 }, "3", true)]
    [DataRow("#FFEE82EE", new int[] { 0, 1, 2, 3, 4, 5 }, "4", true)]
    [DataRow("#FF000000", new int[] { 0, 1, 2, 3, 4, 5 }, "5", true)]
    [DataRow("#FF000000", new int[] { 0, 1, 2, 3, 4, 5 , 6}, "6", true)]
    [DataRow("", new int[] { 0, 1, 2, 3, 4, 5 }, "6", true)]
    [DataRow("#FF000000", new int[] { 5, 4, 3, 2, 1, 0 }, "0", true)]
    [DataRow("#FFEE82EE", new int[] { 5, 4, 3, 2, 1, 0 }, "1", true)]
    [DataRow("#FF00008B", new int[] { 5, 4, 3, 2, 1, 0 }, "2", true)]
    [DataRow("#FFFFD700", new int[] { 5, 4, 3, 2, 1, 0 }, "3", true)]
    [DataRow("#FFA52A2A", new int[] { 5, 4, 3, 2, 1, 0 }, "4", true)]
    [DataRow("#00FFFFFF", new int[] { 5, 4, 3, 2, 1, 0 }, "5", true)]
    [DataRow("#FF000000", new int[] { 6, 5, 4, 3, 2, 1, 0 }, "0", true)]
    public void ConvertTest2(string sExp, object o, object p, bool useColors = false)
    {
        if (useColors)
        {
            _testConverter.colors.Add(Colors.Transparent);
            _testConverter.colors.Add(Colors.Brown);
            _testConverter.colors.Add(Colors.Gold);
            _testConverter.colors.Add(Colors.DarkBlue);
            _testConverter.colors.Add(Colors.Violet);
        }
        var r = _testConverter.Convert(o, typeof(object), p, CultureInfo.InvariantCulture);
        Assert.AreEqual(sExp, Hex(r));
    }

    [TestMethod()]
    public void ConvertBackTest()
    {
        Assert.ThrowsExactly<System.NotImplementedException>(() => _testConverter.ConvertBack(null!, typeof(string), null!, CultureInfo.InvariantCulture));
    }
}
