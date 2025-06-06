﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JCAMS.Core.Converter.Tests
{
    [TestClass()]
    public class CBoolClassConverterTests
    {
        [DataTestMethod()]
        [DataRow(false,"Nein")]
        [DataRow(true, "Ja")]
        public void ConvertToTest(bool xVal,string sExp)
        {
            var cv = new CBoolClassConverter();
            Assert.AreEqual(sExp,cv.ConvertTo(xVal,typeof(string)));
        }

        [TestMethod()]
        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow( "Nein",false)]
        [DataRow( "Ja",true)]
        [DataRow("Leberwust", false)]
        public void ConvertFromTest(string xVal, bool xExp)
        {
            var cv = new CBoolClassConverter();
            Assert.AreEqual(xExp, cv.ConvertFrom(xVal));
        }
    }
}