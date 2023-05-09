using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Text;

namespace MVVM_05_CommandParCalculator.Views.ValueConverter.Tests
{

    [TestClass]
    public class DoubleValueConverterTests
    {
        [DataTestMethod]
        [DataRow(123.45d, "F2", 2d, "246.90")]
        [DataRow(123.45d, "C2", 2d, "¤246.90")]
        [DataRow("Hallo", "C2", 2d, "Hallo")]
        [DataRow(null, "C2", 2d, "")]
        public void ConvertTest(object value, string parameter, double fixedFactor, string expectedResult)
        {
            // Arrange
            var converter = new DoubleValueConverter();
            converter.FixedFactor = fixedFactor;

            // Act
            var result = converter.Convert(value, typeof(string), parameter, CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("246.90", "F2", 2d, 123.45d)]
        [DataRow("246.90-", "{0}-", 2d, double.NaN)]
        [DataRow("246.90 ¤", "0.00 ¤", 2d, 123.45d)]
        [DataRow("246.X ¤", "0.00 ¤", 2d, double.NaN)]
        [DataRow(null, "0.00 ¤", 2d, 0d)]
        public void ConvertBackTest(string value, string parameter, double fixedFactor, double expectedResult)
        {
            // Arrange
            var converter = new DoubleValueConverter();
            converter.FixedFactor = fixedFactor;

            // Act
            var result = converter.ConvertBack(value, typeof(double), parameter, CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
