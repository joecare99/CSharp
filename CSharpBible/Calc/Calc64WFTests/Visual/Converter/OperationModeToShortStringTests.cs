using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc64Base;

namespace Calc64WF.Visual.Converter.Tests
{
    [TestClass()]
    public class OperationModeToShortStringTests
    {
       
        [TestMethod()]
        public void ConvertTest()
        {
            var vcTest = new OperationModeToShortString();
            foreach(var e in typeof(Calc64Model.eOpMode).GetEnumValues())
            {
                Assert.IsNotNull(vcTest.Convert(e, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture));
            }
        }

    }
}