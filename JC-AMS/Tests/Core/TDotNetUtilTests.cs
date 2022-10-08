using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCAMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCAMS.Core.Tests
{
    [TestClass()]
    public class TDotNetUtilTests
    {
        [TestMethod()]
        public void CheckDotNETVersionTest()
        {
            Assert.IsTrue(TDotNetUtil.CheckDotNETVersion(3.5, 1));
            Assert.IsTrue(TDotNetUtil.CheckDotNETVersion(4.0, 0));
            Assert.IsFalse(TDotNetUtil.CheckDotNETVersion(4.0, 1));
        }

        [TestMethod()]
        public void GetDotNETVersionTest()
        {
            Assert.IsTrue(TDotNetUtil.GetDotNETVersion(out double fw,out int sp));
            Assert.IsTrue(fw >= 4.0);
            Assert.IsTrue(sp >= 0);
        }
    }
}