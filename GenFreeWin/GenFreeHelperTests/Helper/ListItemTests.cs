using Microsoft.VisualStudio.TestTools.UnitTesting;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Tests
{
    [TestClass()]
    public class ListItemTests
    {
        [DataTestMethod()]
        [DataRow("A")]
        [DataRow("B")]
        [DataRow("C")]
        public void ToStringTest(string sExp)
        {
            Assert.AreEqual(sExp,new ListItem(sExp,null).ToString());
        }
    }
}