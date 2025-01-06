using Microsoft.VisualStudio.TestTools.UnitTesting;

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