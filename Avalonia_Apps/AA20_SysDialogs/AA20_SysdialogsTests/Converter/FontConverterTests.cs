using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using Avalonia.Media;

namespace AA20_SysDialogs.Converter.Tests;

[TestClass]
public class FontConverterTests
{
#pragma warning disable CS8618
 private FontConverter _testConverter;
#pragma warning restore CS8618

 [TestInitialize()]
 public void Init()
 {
 _testConverter = new FontConverter();
 }

#pragma warning disable CA1416 // Platform compatibility
 [TestMethod()]
 public void ConvertTest_Arial()
 {
 var result = _testConverter.Convert(new System.Drawing.Font("Arial", 12f), typeof(FontFamily), null!, CultureInfo.InvariantCulture);
 Assert.IsInstanceOfType(result, typeof(FontFamily));
 var fontFamily = (FontFamily)result!;
 Assert.AreEqual("Arial", fontFamily.Name);
 }
#pragma warning restore CA1416

 [TestMethod()]
 public void ConvertTest_Null()
 {
 var result = _testConverter.Convert(null!, typeof(FontFamily), null!, CultureInfo.InvariantCulture);
 Assert.IsNull(result);
 }

 [TestMethod()]
 public void ConvertBackTest()
 {
 Assert.ThrowsExactly<NotImplementedException>(() => _testConverter.ConvertBack(null!, typeof(string), null!, CultureInfo.InvariantCulture));
 }
}
