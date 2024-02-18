using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.ADODB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.ADODB.Tests
{
    [TestClass()]
    public class AdoDbProcsTests
    {
        [TestMethod()]
        public void ADODBStreamTest()
        {
            AdoDbProcs.ADODBStream("Test 123 ÖÄÜ", "somefile.tst");
        }
    }
}