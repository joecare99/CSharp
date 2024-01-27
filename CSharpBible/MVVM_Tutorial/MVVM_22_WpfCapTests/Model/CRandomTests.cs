using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MVVM_22_WpfCap.Model.Tests
{
    [TestClass()]
    public class CRandomTests
    {
        private CRandom testRand =null!;

        [TestInitialize]
        public void Init()
        {
            testRand = new CRandom();
            testRand.Seed(0);
        }
        [TestMethod()]
        public void CRandomTest()
        {
            var _rnd=new CRandom();
            Assert.IsNotNull(_rnd);
            Assert.IsInstanceOfType(_rnd, typeof(CRandom));
            Assert.IsInstanceOfType(_rnd, typeof(IRandom));
            Assert.AreEqual(0, _rnd.Next(1));
            Assert.AreEqual(0, _rnd.Next(2) / 2);
        }

        [TestMethod()]
        public void NextTest1()
        {
            Assert.AreEqual(726,testRand.Next(1000),"Next1");
            Assert.AreEqual(817, testRand.Next(1000), "Next2");
            Assert.AreEqual(768, testRand.Next(1000), "Next3");
            Assert.AreEqual(558, testRand.Next(1000), "Next4");
        }

        [DataTestMethod()]
        [DataRow(0,1000,new int[] {726,817, 768, 558, 206 })]
        [DataRow(1, 1000, new int[] { 248, 110, 467, 771, 657 })]
        public void SeedTest(int iSeed,int iMax , int[] aiExp)
        {
            testRand.Seed(iSeed);
            foreach(int i in aiExp)
                Assert.AreEqual(i, testRand.Next(iMax),$"Imax({iSeed},{iMax}).Next =={i}" );
        }
    }
}