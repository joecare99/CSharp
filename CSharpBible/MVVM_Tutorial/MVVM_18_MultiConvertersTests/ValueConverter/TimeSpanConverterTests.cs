using BaseLib.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_18_MultiConverters.Model;
using System.Globalization;
using System.Windows.Data;

namespace MVVM_18_MultiConverters.ValueConverter.Tests
{
    [TestClass()]
    public class TimeSpanConverterTests
    {
        TimeSpanConverter testConverter;

        [TestInitialize]
        public void Init() {
            testConverter = new TimeSpanConverter();
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testConverter);
            Assert.IsInstanceOfType(testConverter,typeof(IMultiValueConverter));
            Assert.IsInstanceOfType(testConverter, typeof(TimeSpanConverter));
        }

        [DataTestMethod()]
        [DataRow("Null", TypeCode.String, null,null,"0")]
        [DataRow("Null2", TypeCode.String, new object[] { },  null, "0")]
        [DataRow("1", TypeCode.String, new object[] {"1" },  null, "0")]
        [DataRow("3.6 Days", TypeCode.String, new object[] {(double)3.6d },  null, "3.6")]
        [DataRow("4.1 Days", TypeCode.String, new object[] { 4.1d, DateDifFormat.Hours }, null, "98.39999999997222")]
        [DataRow("5.9 Days", TypeCode.String, new object[] { 5.9d, DateDifFormat.Minutes }, null, "8496")]
        [DataRow("6.001 Days", TypeCode.String, new object[] { 6.001d, DateDifFormat.Seconds }, null, "518486.4")]
        [DataRow("6.501 Days", TypeCode.String, new object[] { 6.501d, (DateDifFormat)4 }, null, "0")]
        [DataRow("7.123 Days", TypeCode.String, new object[] { 7.123d, "Weeks" }, null, "7.123")]
        public void ConvertTest(string name, TypeCode tc, object[] oVal, object oPar, object oExp)
        {
            if (oVal?.Length > 0 && oVal[0] is decimal d) oVal[0] = TimeSpan.FromDays((double)d);
            if (oVal?.Length > 0 && oVal[0] is double d2) oVal[0] = TimeSpan.FromDays(d2);
            var tVal = tc.ToType();
            Assert.AreEqual(oExp,testConverter.Convert(oVal,tVal,oPar,CultureInfo.InvariantCulture), $"Convert({name}).result");
        }

        [TestMethod()]
        public void ConvertBackTest()
        {
            Assert.ThrowsException<NotImplementedException>(()=>testConverter.ConvertBack(null!,null!,null!,null!));
        }
    }
}