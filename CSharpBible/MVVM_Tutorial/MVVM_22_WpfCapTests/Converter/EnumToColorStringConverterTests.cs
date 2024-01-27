using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
//using System.Windows.Data;
using System.Windows.Media;

namespace MVVM_22_WpfCap.Converter.Tests
{
    [TestClass()]
    public class EnumToColorStringConverterTests
    {
        private EnumToColorStringConverter _testConverter = null!;

        [TestInitialize()]
        public void Init()
        {
            _testConverter = new EnumToColorStringConverter();
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(_testConverter);
            Assert.IsInstanceOfType(_testConverter, typeof(EnumToColorStringConverter));
//            Assert.IsInstanceOfType(_testConverter, typeof(IValueConverter));
            Assert.IsNotNull(_testConverter.colors);
        }

        [DataTestMethod()]
        [DataRow("", null, "#0:#1")]
        [DataRow("#1",true,"#0:#1")]
        [DataRow("#0", false, "#0:#1")]
        [DataRow("#FF008000", true, null)]
        [DataRow("#FFFF0000", false, null)]
        [DataRow("#FF008000", true, "345")]
        [DataRow("#FFFF0000", false, "345")]
        [DataRow("", new bool[] { true, false }, null)]
        [DataRow("", new bool[] { false,true }, null)]
        [DataRow("#FF008000", new bool[] { true, true }, "0")]
        [DataRow("#FF008000", new bool[] { true, false }, "0")]
        [DataRow("#FFFF0000", new bool[] { false, true }, "0")]
        [DataRow("#FFFF0000", new bool[] { false, false }, "0")]
        [DataRow("#FF008000", new bool[] { true, true }, "1")]
        [DataRow("#FFFF0000", new bool[] { true, false }, "1")]
        [DataRow("#FF008000", new bool[] { false, true }, "1")]
        [DataRow("#FFFF0000", new bool[] { false, false }, "1")]
        [DataRow("", new bool[] { false, false }, null)]
        [DataRow("", new bool[] { false, false }, "XX")]
        [DataRow("#FF000000", new int[] { 0, 1, 2, 3, 4, 5 }, "0")]
        [DataRow("#FFFF0000", new int[] { 0, 1, 2, 3, 4, 5 }, "1")]
        [DataRow("#FF008000", new int[] { 0, 1, 2, 3, 4, 5 }, "2")]
        [DataRow("#FFFFFF00", new int[] { 0, 1, 2, 3, 4, 5 }, "3")]
        [DataRow("#FF0000FF", new int[] { 0, 1, 2, 3, 4, 5 }, "4")]
        [DataRow("#FF000000", new int[] { 0, 1, 2, 3, 4, 5 }, "5")]
        [DataRow("", new int[] { 0, 1, 2, 3, 4, 5 }, "6")]
        [DataRow("#FF000000", new int[] { 0, 1, 2, 3, 4, 5, 6 }, "6")]
        [DataRow("#FF000000", new int[] { 5, 4, 3, 2, 1, 0 }, "0")]
        [DataRow("#FF0000FF", new int[] { 5, 4, 3, 2, 1, 0 }, "1")]
        [DataRow("#FFFFFF00", new int[] { 5, 4, 3, 2, 1, 0 }, "2")]
        [DataRow("#FF008000", new int[] { 5, 4, 3, 2, 1, 0 }, "3")]
        [DataRow("#FFFF0000", new int[] { 5, 4, 3, 2, 1, 0 }, "4")]
        [DataRow("#FF000000", new int[] { 5, 4, 3, 2, 1, 0 }, "5")]
        [DataRow("", new int[] { 5, 4, 3, 2, 1, 0 }, "6")]
        [DataRow("", new int[] { 5, 4, 3, 2, 1, 0 }, null)]
        [DataRow("", new int[] { 5, 4, 3, 2, 1, 0 }, "XX")]
        public void ConvertTest(string sExp, object o,object p)
        {
            Assert.AreEqual(sExp,_testConverter.Convert(o,typeof(string),p,null!));
        }

        [DataTestMethod()]
        [DataRow("#1", true, "#0:#1")]
        [DataRow("#0", false, "#0:#1")]
        [DataRow("#FFFFD700", true, null)]
        [DataRow("#FFA52A2A", false, null)]
        [DataRow("#FFFFD700", true, "345")]
        [DataRow("#FFA52A2A", false, "345")]
        [DataRow("", new bool[] { true, false }, null)]
        [DataRow("", new bool[] { false, true }, null)]
        [DataRow("#FFFFD700", new bool[] { true, true }, "0")]
        [DataRow("#FFFFD700", new bool[] { true, false }, "0")]
        [DataRow("#FFA52A2A", new bool[] { false, true }, "00")]
        [DataRow("#FFA52A2A", new bool[] { false, false }, "00")]
        [DataRow("#FFFFD700", new bool[] { true, true }, "1")]
        [DataRow("#FFA52A2A", new bool[] { true, false }, "1")]
        [DataRow("#FFFFD700", new bool[] { false, true }, "01")]
        [DataRow("#FFA52A2A", new bool[] { false, false }, "01")]
        [DataRow("", new bool[] { false, false }, null)]
        [DataRow("#00FFFFFF", new int[] { 0, 1, 2, 3, 4, 5 }, "0")]
        [DataRow("#FFA52A2A", new int[] { 0, 1, 2, 3, 4, 5 }, "1")]
        [DataRow("#FFFFD700", new int[] { 0, 1, 2, 3, 4, 5 }, "2")]
        [DataRow("#FF00008B", new int[] { 0, 1, 2, 3, 4, 5 }, "3")]
        [DataRow("#FFEE82EE", new int[] { 0, 1, 2, 3, 4, 5 }, "4")]
        [DataRow("#FF000000", new int[] { 0, 1, 2, 3, 4, 5 }, "5")]
        [DataRow("#FF000000", new int[] { 0, 1, 2, 3, 4, 5 , 6}, "6")]
        [DataRow("", new int[] { 0, 1, 2, 3, 4, 5 }, "6")]
        [DataRow("#FF000000", new int[] { 5, 4, 3, 2, 1, 0 }, "00")]
        [DataRow("#FFEE82EE", new int[] { 5, 4, 3, 2, 1, 0 }, "01")]
        [DataRow("#FF00008B", new int[] { 5, 4, 3, 2, 1, 0 }, "02")]
        [DataRow("#FFFFD700", new int[] { 5, 4, 3, 2, 1, 0 }, "03")]
        [DataRow("#FFA52A2A", new int[] { 5, 4, 3, 2, 1, 0 }, "04")]
        [DataRow("#00FFFFFF", new int[] { 5, 4, 3, 2, 1, 0 }, "05")]
        [DataRow("#FF000000", new int[] { 6, 5, 4, 3, 2, 1, 0 }, "0")]
        [DataRow("", new int[] { 5, 4, 3, 2, 1, 0 }, "6")]
        [DataRow("", new int[] { 5, 4, 3, 2, 1, 0 }, null)]
        public void ConvertTest2(string sExp, object o, object p)
        {
            _testConverter.colors.Add(Colors.Transparent);
            _testConverter.colors.Add(Colors.Brown);
            _testConverter.colors.Add(Colors.Gold);
            _testConverter.colors.Add(Colors.DarkBlue);
            _testConverter.colors.Add(Colors.Violet);            
            Assert.AreEqual(sExp, _testConverter.Convert(o, typeof(string), p, null!));
        }

        [TestMethod()]
        public void ConvertBackTest()
        {
            Assert.ThrowsException<NotImplementedException>(() => _testConverter.ConvertBack(null!, typeof(string), null!, CultureInfo.InvariantCulture));
        }
    }
}