using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseGenClasses.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenInterfaces.Interfaces.Genealogic;
using BaseGenClasses.Model;

namespace BaseGenClasses.Helper.Tests
{
    [TestClass()]
    public class GenConverterTests
    {
        private GenConverter<GenDate, IGenDate> genConverter;

        [TestInitialize]
        public void Initialize()
        {
            genConverter = new();
        }

        [TestMethod()]
        [DataRow(0,false)]
        [DataRow(1,true)]
        public void CanConvertTest(int i,bool xExp)
        {
            Assert.AreEqual(xExp, genConverter.CanConvert(i switch {
                0 => typeof(object),
                1 => typeof(IGenDate)
            }));
        }

        [TestMethod()]
        public void ReadTest()
        {

        }

        [TestMethod()]
        public void WriteTest()
        {
            Assert.Fail();
        }
    }
}