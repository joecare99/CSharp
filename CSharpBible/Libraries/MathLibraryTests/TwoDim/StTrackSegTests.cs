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
        [DataRow(new[] { 6d, 5d }, new[] { 4d, 3d },2d)]
        public void StTrackSegTests2(double[] adAct1, double[] adAct2,double fAct3)
        {
            var st = new StTrackSeg(new(adAct1[0], adAct1[1]),new(adAct2[0], adAct2[1]),fAct3);
            Assert.IsNotNull(st);
            Assert.IsInstanceOfType(st, typeof(StTrackSeg));
            Assert.AreEqual(adAct1[0], st.vFootPoint.x, nameof(st.vFootPoint));
            Assert.AreEqual(adAct1[1], st.vFootPoint.y, nameof(st.vFootPoint));
            Assert.AreEqual(adAct2[0], st.vNormal.x,nameof(st.vNormal));
            Assert.AreEqual(adAct2[1], st.vNormal.y, nameof(st.vNormal));
            Assert.AreEqual(fAct3, st.lrRadius, nameof(st.lrRadius));
        }
    }
}