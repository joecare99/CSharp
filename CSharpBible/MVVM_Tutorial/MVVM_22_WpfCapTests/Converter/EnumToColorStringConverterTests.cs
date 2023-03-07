using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_22_WpfCap.Converter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_22_WpfCap.Converter.Tests
{
    [TestClass()]
    public class EnumToColorStringConverterTests
    {
        private EnumToColorStringConverter _testConverter;

        [TestInitialize()]
        public void Init()
        {
            _testConverter = new EnumToColorStringConverter();
        }

        [TestMethod()]
        public void ConvertTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ConvertBackTest()
        {
            Assert.ThrowsException<NotImplementedException>(() => _testConverter.ConvertBack(null, typeof(string), null, CultureInfo.InvariantCulture));
        }
    }
}