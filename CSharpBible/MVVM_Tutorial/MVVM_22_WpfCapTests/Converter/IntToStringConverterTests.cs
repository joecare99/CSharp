using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_22_WpfCap.Converter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Data;

namespace MVVM_22_WpfCap.Converter.Tests
{
    [TestClass()]
    public class IntToStringConverterTests
    {
        private IntToStringConverter _testConverter;

        [TestInitialize()]
        public void Init()
        {
            _testConverter = new IntToStringConverter();
        }

        [TestMethod()]
        public void SetUpTest()
        {
            Assert.IsNotNull(_testConverter);
            Assert.IsInstanceOfType(_testConverter, typeof(IntToStringConverter));
   //         Assert.IsInstanceOfType(_testConverter, typeof(IValueConverter));
        }
        [DataTestMethod()]
        [DataRow("",null)]
        [DataRow("0", 0)]
        [DataRow("-1", -1)]
        [DataRow("1", 1)]
        [DataRow("2147483647", int.MaxValue)]
        [DataRow("-2147483648", int.MinValue)]
        [DataRow("System.Int32", typeof(int))]
        public void ConvertTest(string sExp,object o)
        {
            Assert.AreEqual(sExp,_testConverter.Convert(o,typeof(string),null,null));
        }

        [TestMethod()]
        public void ConvertBackTest()
        {
            Assert.ThrowsException<NotImplementedException>(() => _testConverter.ConvertBack(null, typeof(string), null, CultureInfo.InvariantCulture));
        }
    }
}