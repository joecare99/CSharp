﻿// ***********************************************************************
// Assembly         : MVVM_06_ConvertersTests
// Author           : Mir
// Created          : 05-11-2023
//
// Last Modified By : Mir
// Last Modified On : 05-11-2023
// ***********************************************************************
// <copyright file="CurrencyValueConverterTests.cs" company="MVVM_28_1_DataGridExt">
//    Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_28_1_DataGridExt.ValueConverter.Tests;


/// <summary>
/// Defines test class CurrencyValueConverterTests.
/// </summary>
/// <autogeneratedoc />
[TestClass()]
public class Bool2VisibilityConverterTests
{
    /// <summary>
    /// The converter
    /// </summary>
    /// <autogeneratedoc />
    Bool2VisibilityConverter testConv = new();

    /// <summary>
    /// Converts the correctly formats value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="expected">The expected.</param>
    /// <autogeneratedoc />
    [DataTestMethod]
    [DataRow(10.5, System.Windows.Visibility.Visible)]
    [DataRow(0.99, System.Windows.Visibility.Visible)]
    [DataRow(true, System.Windows.Visibility.Visible)]
    [DataRow(false, System.Windows.Visibility.Hidden)]
    [DataRow("Hallo", System.Windows.Visibility.Visible)]
    [DataRow(null, System.Windows.Visibility.Visible)]
    public void ConvertTest(object? value, System.Windows.Visibility expected)
    {
        if (value is double d) value = (decimal)d;
        var result = testConv.Convert(value!, typeof(string), null!, CultureInfo.InvariantCulture);
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// Defines the test method ConvertBackTest.
    /// </summary>
    /// <autogeneratedoc />
    [DataTestMethod()]
    [DataRow(true, System.Windows.Visibility.Visible)]
    [DataRow(false, System.Windows.Visibility.Hidden)]
    [DataRow(false, null)]
    public void ConvertBackTest(bool xExp, object? eVal)
    {
        Assert.AreEqual(xExp, testConv.ConvertBack(eVal!,typeof(object),null!,CultureInfo.InvariantCulture));
    }
}