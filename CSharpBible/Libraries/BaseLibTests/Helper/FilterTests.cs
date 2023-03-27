using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLib.Helper.Tests
{
    [TestClass()]
    public class FilterTests
    {
        [DataTestMethod()]
        [DataRow(0.0f, 0.0f, 0.0f, 0.0f)]
        [DataRow(0.5f, 0.0f, 0.0f, 0.0f)]
        [DataRow(0.5f, 0.0f, 1.0f, 0.5d)]
        [DataRow(1.0f, 0.0f, 0.0f, 0.0f)]
        [DataRow(1.0f, 0.0f, 1.0f, 1.0d)]
        [DataRow(1.0f, 0.0f, 0.5d, 0.5d)]
        [DataRow(1.0f, 0.5d, 0.5d, 0.75d)]
        public void PT1Test(double fNew, double fAct, double fDt, double fExp)
        {
            Assert.AreEqual(fExp, fNew.PT1(ref fAct, fDt), 1e-12);
            Assert.AreEqual(fExp, fAct, 1e-12);
        }

        [DataTestMethod()]
        [DataRow(0.0f, 0.0f, 0.0f, 1.0f, 0.0f)]
        [DataRow(0.5d, 0.0f, 0.1d, 0.5d, 0.025d)]
        [DataRow(1.0f, 0.0f, 0.2d, 1.0f, 0.2d)]
        public void PT1Test1(double fNew, double fAct, double fDt, double fKP, double fExp)
        {
            Assert.AreEqual(fExp, fNew.PT1(ref fAct, fDt, fKP), 1e-12);
            Assert.AreEqual(fExp, fAct, 1e-12);
            /*
            Assert.AreEqual(0.875f, fFilter.use(1.0f));
            Assert.AreEqual(0.9375f, fFilter.use(1.0f));

            Assert.AreEqual(0.46875f, fFilter.use(0.0f));
            Assert.AreEqual(0.734375f, fFilter.use(1.0f));
            Assert.AreEqual(0.3671875f, fFilter.use(0.0f));
            Assert.AreEqual(0.6835938f, fFilter.use(1.0f),1e-6);
            */
        }

        [TestMethod()]
        public void PT1Test2()
        {
            var fAct = 1.0d;
            for (int i = 0; i < 39; i++)
            {
                Assert.AreNotEqual(0.0f, 0.0d.PT1(ref fAct, 0.5f, 1.0f), 1e-12, $"Test({i})={fAct}");
            }
            Assert.AreEqual(0.0f, 0.0d.PT1(ref fAct, 1.0f, 1.0f));
        }

        [DataTestMethod()]
        [DataRow(1, new double[] { 0.0f,1.0f }, new double[] { 0.0f,1.0f })]
        [DataRow(5, new double[] { 0.0f, 1.0f,1.0f, 1.0f, 1.0f, 1.0f }, new double[] { 0.0f, 0.2d, 0.4d, 0.6d, 0.8d, 1.0d })]
        public void MeanTest(int iCnt, double[] fNew, double[] fExp)
        {
            var aBuf = new double[iCnt];
            aBuf.Initialize();
            int iX = 0;
            for (int i = 0; i < fNew.Length; i++)
            {
                Assert.AreEqual(fExp[i], fNew[i].Mean(aBuf, ref iX), 1e-12,$"{i}");
                Assert.AreEqual((i+1) % iCnt, iX,$"{i}.cnt");
            }
        }

        [DataTestMethod()]
        [DataRow(1, new double[] { 0.0f, 1.0f }, new double[] { 0.0f, 1.0f })]
        [DataRow(5, new double[] { 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new double[] { 0.0f, 0.0d, 0.0d, 1.0d, 1.0d, 1.0d })]
        public void MedianTest(int iCnt, double[] fNew, double[] fExp)
        {
            var aBuf = new double[iCnt];
            aBuf.Initialize();
            int iX = 0;
            for (int i = 0; i < fNew.Length; i++)
            {
                Assert.AreEqual(fExp[i], fNew[i].Median(aBuf, ref iX), 1e-12, $"{i}");
                Assert.AreEqual((i + 1) % iCnt, iX, $"{i}.cnt");
            }
        }

        [TestMethod()]
        public void MeanTest2()
        {
            var aBuf = new double[20];
            aBuf.Initialize();
                int ix = 0;

            var fAct = 0.0d;
            for (int i = 0; i < 19; i++)
            {
                Assert.AreNotEqual(1.0d, 1.0d.Mean(aBuf,ref ix), 1e-12, $"Test({i})={fAct}");
            }
            Assert.AreEqual(1.0d, 1.0d.Mean(aBuf, ref ix));
        }

        [TestMethod()]
        public void MedianTest2()
        {
            var aBuf = new double[20];
            aBuf.Initialize();
            int ix = 0;

            var fAct = 0.0d;
            for (int i = 0; i < 9; i++)
            {
                Assert.AreNotEqual(1.0d, 1.0d.Median(aBuf, ref ix), 1e-12, $"Test({i})={fAct}");
            }
            Assert.AreEqual(1.0d, 1.0d.Median(aBuf, ref ix));
        }

    }
}