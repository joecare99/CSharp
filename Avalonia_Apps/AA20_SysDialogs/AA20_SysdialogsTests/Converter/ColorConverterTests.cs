using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Collections.Generic;
using System;
using Avalonia.Media;

namespace AA20_SysDialogs.Converter.Tests;

[TestClass()]
public class ColorConverterTests
{
    static IEnumerable<object[]> ColorConverterData => new[] {
                new object[] { System.Drawing.Color.Red , "#FFFF0000" },
					new object[] { System.Drawing.Color.Lime, "#FF00FF00" },
                new object[] { System.Drawing.Color.Blue, "#FF0000FF" },
                new object[] { System.Drawing.Color.White, "#FFFFFFFF" },
                new object[] { System.Drawing.Color.Transparent, "#00FFFFFF" },
                new object[] { null!, "#FFFFFFFF" }, // Returns White as fallback
    };

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private ColorConverter _testConverter;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize()]
    public void Init()
    {
        _testConverter = new ColorConverter();
    }
    
    [TestMethod()]
    public void ConvertTest_Red()
    {
        var result = _testConverter.Convert(System.Drawing.Color.Red, typeof(Brush), null!, CultureInfo.InvariantCulture);
        Assert.IsInstanceOfType(result, typeof(SolidColorBrush));
        var brush = (SolidColorBrush)result!;
        Assert.AreEqual("#FFFF0000", brush.Color.ToString());
    }

    [TestMethod()]
    public void ConvertBackTest()
    {
        Assert.ThrowsExactly<NotImplementedException>(()=> _testConverter.ConvertBack(null!,typeof(string),null!,CultureInfo.InvariantCulture));
    }
}