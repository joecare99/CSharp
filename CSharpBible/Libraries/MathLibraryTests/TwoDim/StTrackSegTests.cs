using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLibrary.TwoDim.Tests
{
    [TestClass()]
    public class StTrackSegTests
    {
        [TestMethod]
        public void StTrackSegTests1()
        {
            var st = new StTrackSeg();
            Assert.IsNotNull(st);
            Assert.IsInstanceOfType(st, typeof(StTrackSeg));
            Assert.AreEqual(Math2d.Null, st.vNormal);
            Assert.AreEqual(Math2d.Null, st.vFootPoint);
            Assert.AreEqual(double.NaN, st.lrRadius);
        }

        [DataTestMethod]
        [DataRow(new[] { 1d, 2d }, new[] { 3d, 4d },5d)]
        public void StTrackSegTests1(double[] adAct1, double[] adAct2,double fAct3)
        {
            var st = new StTrackSeg(new(adAct1[0], adAct1[1]),new(adAct2[0], adAct2[1]),fAct3);
            Assert.IsNotNull(st);
            Assert.IsInstanceOfType(st, typeof(StTrackSeg));
            Assert.AreEqual(adAct1[0], st.vNormal.x);
            Assert.AreEqual(adAct1[1], st.vNormal.y);
            Assert.AreEqual(adAct2[0], st.vFootPoint.x);
            Assert.AreEqual(adAct2[1], st.vFootPoint.y);
            Assert.AreEqual(fAct3, st.lrRadius);
        }
    }
}