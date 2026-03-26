using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCAMS.Core.Components.Coloring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace JCAMS.Core.Components.Coloring.Tests
{
    [TestClass()]
    public class CColorListTests
    {
        [TestMethod()]
        public void CColorListTest()
        {
           var colorList = new CColorList();
            Assert.IsNotNull(colorList);            
            var colorList2 = new CColorList("ALabel",new Color[] {Color.Empty,Color.Transparent,Color.Red,Color.Lime,Color.Blue });
            Assert.IsNotNull(colorList2);
        }

        [TestMethod()]
        public void CColorListTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DisposeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void WriteXmlTest()
        {
            Assert.Fail();
        }
    }
}