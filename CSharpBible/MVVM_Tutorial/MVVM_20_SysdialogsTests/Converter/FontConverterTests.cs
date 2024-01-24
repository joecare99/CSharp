using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MVVM_20_Sysdialogs.Converter.Tests
{
    [TestClass]
    public class FontConverterTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private FontConverter _testConverter;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        static IEnumerable<object[]> FontConverterData => new[] {
                    new object[] { new System.Drawing.Font("Arial", 12f), new System.Windows.Media.FontFamily("Arial") },
                    new object[] { null!, null! },
        };

        [TestInitialize()]
        public void Init()
        {
            _testConverter = new FontConverter();
        }


        [DataTestMethod()]
        [DynamicData(nameof(FontConverterData))]
        public void ConvertTest(object value, object sExp)
        {
            Assert.AreEqual(sExp, _testConverter.Convert(value, typeof(string), null!, CultureInfo.InvariantCulture));
        }
        [TestMethod()]
        public void ConvertBackTest()
        {
            Assert.ThrowsException<NotImplementedException>(() => _testConverter.ConvertBack(null!, typeof(string), null!, CultureInfo.InvariantCulture));
        }
    }
}
