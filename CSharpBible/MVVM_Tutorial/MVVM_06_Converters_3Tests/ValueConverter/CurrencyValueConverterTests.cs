﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace MVVM_06_Converters_3.ValueConverter.Tests
{
    [TestClass()]
    public class CurrencyValueConverterTests
    {
        /// <summary>
        /// The converter
        /// </summary>
        /// <autogeneratedoc />
        CurrencyValueConverter testConv = new();

        /// <summary>
        /// Converts the correctly formats value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="expected">The expected.</param>
        /// <autogeneratedoc />
        [DataTestMethod]
        [DataRow(10.5, null, "10.5")]
        [DataRow(10.5, "", "10.5")]
        [DataRow(10.5, "0.00$", "10.50$")]
        [DataRow(0.99, null, "0.99")]
        [DataRow(0.99, "", "0.99")]
        [DataRow(0.999, "0.00€", "1.00€")]
        [DataRow(true, null, "True")]
        [DataRow(false, null, "False")]
        [DataRow("Hallo",null, "Hallo")]
        [DataRow(null, null, "")]
        public void ConvertTest(object? value, object? value2, string expected)
        {
            if (value is double d) value = (decimal)d;
            var result = testConv.Convert(value!, typeof(string), value2!, CultureInfo.InvariantCulture);
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method ConvertBackTest.
        /// </summary>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow(0.0, "1", null)]
        [DataRow(0.0, null, "")]
        [DataRow(1.0, "", "1")]
        public void ConvertBackTest(object? value, object? value2, string eVal)
        {
            if (value is double d) value = (decimal)d;
            Assert.AreEqual(value, testConv.ConvertBack(eVal, typeof(object), value2!, CultureInfo.InvariantCulture));
        }
    }
}