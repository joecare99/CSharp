﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;
using System.Globalization;
using System.Collections.Generic;
using System;

namespace MVVM_20_Sysdialogs.Converter.Tests;

[TestClass()]
public class ColorConverterTests
{
    static IEnumerable<object[]> ColorConverterData => new[] {
                new object[] { System.Drawing.Color.Red , "#FFFF0000" },
					new object[] { System.Drawing.Color.Lime, "#FF00FF00" },
                new object[] { System.Drawing.Color.Blue, "#FF0000FF" },
                new object[] { System.Drawing.Color.White, "#FFFFFFFF" },
                new object[] { System.Drawing.Color.Transparent, "#00FFFFFF" },
                new object[] { Colors.Red, "#FFFF0000" },
                new object[] { Colors.Lime, "#FF00FF00" },
                new object[] { Colors.Blue, "#FF0000FF" },
                new object[] { Colors.White, "#FFFFFFFF" },
                new object[] { Colors.Transparent, "#00FFFFFF" },
                new object[] { null!, "" },
    };

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private ColorConverter _testConverter;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize()]
    public void Init()
    {
        _testConverter = new ColorConverter();
    }
    [DataTestMethod()]
    [DynamicData(nameof(ColorConverterData))]
    public void ConvertTest(object value, string sExp)
    {
        Assert.AreEqual(sExp,_testConverter.Convert(value,typeof(string),null!,CultureInfo.InvariantCulture));
    }

    [TestMethod()]
    public void ConvertBackTest()
    {
        Assert.ThrowsException<NotImplementedException>(()=> _testConverter.ConvertBack(null!,typeof(string),null!,CultureInfo.InvariantCulture));
    }
}