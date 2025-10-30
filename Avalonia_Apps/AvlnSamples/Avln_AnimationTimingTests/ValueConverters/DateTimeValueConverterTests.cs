// ***********************************************************************
// Assembly : Avln_AnimationTimingTests
// Author   : Mir
// Created : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="DateTimeValueConverterTests.cs" company="JC-Soft">
//    Copyright (c) JC-Soft 2025. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace Avln_AnimationTiming.ValueConverter.Tests
{
    /// <summary>
 /// Defines test class DateTimeValueConverterTests.
   /// </summary>
    [TestClass()]
    public class DateTimeValueConverterTests
  {
  /// <summary>
        /// The converter
    /// </summary>
     readonly DateTimeValueConverter testConv = new();

   /// <summary>
        /// Converts the correctly formats value.
  /// </summary>
   /// <param name="value">The value.</param>
     /// <param name="param">The parameter.</param>
        /// <param name="expected">The expected.</param>
        [DataTestMethod]
   [DataRow("2023-01-01 22:00", null, "01/01/2023 23:00:00")]
        [DataRow("2023-05-02 12:30", "MM/dd/yyyy", "05.02.2023")]
        [DataRow("Hallo", null, "Hallo")]
        [DataRow(null, null, "")]
  public void ConvertTest(object? value, string? param, string expected)
  {
 if (value is string s && DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt)) 
   value = dt;
    var result = testConv.Convert(value, typeof(string), param, CultureInfo.InvariantCulture);
      Assert.AreEqual(expected, result);
        }

        /// <summary>
      /// Defines the test method ConvertBackTest.
  /// </summary>
     /// <param name="value">The value.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="expected">The expected.</param>
   [DataTestMethod]
        [DataRow("2023-01-01 22:30", null, "01/01/2023 22:30:00")]
        [DataRow("2023-05-02", "MM/dd/yyyy", "05.02.2023")]
        [DataRow("01.01.0001 00:00:00", null, "Hallo")]
  [DataRow("01.01.0001 00:00:00", "", 100)]
        [DataRow("01.01.0001 00:00:00", "", null)]
   public void ConvertBackTest(object? value, string? param, object? expected)
        {
        if (value is string s && DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt)) 
       value = dt;
   var result = testConv.ConvertBack(expected, typeof(DateTime), param, CultureInfo.InvariantCulture);
    Assert.AreEqual(value, result);
        }
    }
}
