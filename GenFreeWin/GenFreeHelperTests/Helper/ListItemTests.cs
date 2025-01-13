using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenFree.Helper.Tests
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