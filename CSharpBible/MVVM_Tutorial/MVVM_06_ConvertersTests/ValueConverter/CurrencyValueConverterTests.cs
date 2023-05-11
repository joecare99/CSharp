﻿// ***********************************************************************
// Assembly         : MVVM_06_ConvertersTests
// Author           : Mir
// Created          : 05-11-2023
//
// Last Modified By : Mir
// Last Modified On : 05-11-2023
// ***********************************************************************
// <copyright file="CurrencyValueConverterTests.cs" company="MVVM_06_ConvertersTests">
//    Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_06_Converters.ValueConverter.Tests
{

    /// <summary>
    /// Defines test class CurrencyValueConverterTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class CurrencyValueConverterTests
    {
        /// <summary>
        /// The converter
        /// </summary>
        /// <autogeneratedoc />
        CurrencyValueConverter converter = new();

        /// <summary>
        /// Converts the correctly formats value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="expected">The expected.</param>
        /// <autogeneratedoc />
        [DataTestMethod]
        [DataRow(10.5, "10.50€")]
        [DataRow(0.99, "0.99€")]
        [DataRow("Hallo", "Hallo")]
        [DataRow(null, "")]
        public void Convert_CorrectlyFormatsValue(object? value, string expected)
        {
            if (value is double d) value = (decimal)d;
            var result = converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method ConvertBackTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void ConvertBackTest()
        {
            Assert.ThrowsException<NotImplementedException>(()=> converter.ConvertBack(null,typeof(object),null,null));
        }
    }
}