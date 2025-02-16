﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace MVVM.View.ValueConverter.Tests;

[TestClass()]
public class DoubleValueConverterTests
{
    /// <summary>
    /// The converter
    /// </summary>
    /// <autogeneratedoc />
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    DoubleValueConverter testConv;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    [TestInitialize]
    public void Init() {
        testConv = new();
    }

    /// <summary>
    /// Converts the correctly formats value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="expected">The expected.</param>
    /// <autogeneratedoc />
    [DataTestMethod]
    [DataRow(null,null, "")]
    [DataRow(true,null, "True")]
    [DataRow(1.0f, null, "1")]
    [DataRow(2.0f, "F2", "2.00")]
    [DataRow(3.0d, null, "3")]
    [DataRow(4.0d, "F2", "4.00")]
    public void ConvertTest(object? value,object? par, string expected)
    {
        var result = testConv.Convert(value, typeof(string), par, CultureInfo.InvariantCulture);
        Assert.AreEqual(expected, result);
    }
    
    /// <summary>
    /// Converts the correctly formats value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="expected">The expected.</param>
    /// <autogeneratedoc />
    [DataTestMethod]
    [DataRow(null,null, "")]
    [DataRow(true,null, "True")]
    [DataRow(1.0f, null, "2")]
    [DataRow(2.0f, "F2", "4.00")]
    [DataRow(3.0d, null, "6")]
    [DataRow(4.0d, "F2", "8.00")]
    public void ConvertTest2(object? value,object? par, string expected)
    {
        testConv.FixedFactor = 2.0d;
        var result = testConv.Convert(value, typeof(string), par, CultureInfo.InvariantCulture);
        Assert.AreEqual(expected, result);
    }

    [TestMethod()]
    [DataRow(null, double.NaN)]
    [DataRow("", double.NaN)]
    [DataRow("1", 1.0)]
    [DataRow("2.5", 2.5)]
    [DataRow("invalid", double.NaN)]
    [DataRow("3.14", 3.14)]
    public void ConvertBackTest(object? value, object? expected)
    {
        // Act
        var result = testConv.ConvertBack(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        if (expected == null)
        {
            Assert.IsNull(result);
        }
        else
        {
            Assert.AreEqual(expected, result);
        }
    }

    [TestMethod()]
    [DataRow("2.5 mm", "0.0 mm", 2.5)]
    [DataRow("invalid", "{0}", double.NaN)]
    [DataRow("invalid#", "0#", double.NaN)]
    [DataRow("3.14-", "0.00-", 3.14)]
    public void ConvertBackTest2(object? value, object? par, object? expected)
    {
        // Act
        var result = testConv.ConvertBack(value, typeof(double), par, CultureInfo.InvariantCulture);

        // Assert
        if (expected == null)
        {
            Assert.IsNull(result);
        }
        else
        {
            Assert.AreEqual(expected, result);
        }
    }
}