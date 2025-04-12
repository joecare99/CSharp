using BaseLib.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenFree.Helper.Tests
{
    [TestClass()]
    public class ObjectHelper2Tests
    {
        [DataTestMethod()]
        [DataRow(1, "0")]
        [DataRow(2, "Uno")]
        [DataRow(3, "Zwei")]
        [DataRow(4, "Three")]
        public void SetIndexTest(int iIdx, string sExp)
        {
            var test = new ControlArray<string>();
            test.SetIndex("0", 0);
            test.SetIndex("Uno", 1);
            test.SetIndex("Zwei", 2);
            test.SetIndex("Three", 3);
            Assert.AreEqual(sExp, test[iIdx]);
        }

        [DataTestMethod()]
        [DataRow(0, "0")]
        [DataRow(1, "Uno")]
        [DataRow(2, "Zwei")]
        [DataRow(3, "Three")]
        [DataRow(-1, "Dummy")]
        public void GetIndexTest(int iExp, string sAct)
        {
            var test = new ControlArray<string?>() { { 1, "0" }, { 2, "Uno" }, { 3, "Zwei" }, { 4, "Three" }, { 5, null } };
            Assert.AreEqual(iExp, test.GetIndex(sAct));
        }
    }
}