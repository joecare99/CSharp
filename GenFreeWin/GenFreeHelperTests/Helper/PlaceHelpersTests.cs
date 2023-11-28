using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenFree.Helper.Tests
{
    [TestClass()]
    public class PlaceHelpersTests
    {
        [DataTestMethod()]
        [DataRow("A", 0, 0, 0, 0, 0)]
        [DataRow("B", 0, 0, 0, 1f, 111.3239f)]
        [DataRow("C", 0, 0, 1f, 0f, 111.3239f)]
        [DataRow("C2", 0, 89.9f, 0.1f, 89.9f, 0.01942952f)]
        [DataRow("C3", 0, 89.999f, 0.001f, 89.999f, 0f)]
        [DataRow("D", 0, 0, 1f, 1f, 157.4317f)]
        [DataRow("E", 0, 0, 2f, 2f, 314.8395f)]
        [DataRow("F", 0, 0, 3f, 3f, 472.1992f)]
        [DataRow("L", 0, 0, 9f, 9f, 1413.996f)]
        [DataRow("M", 0, 0, 10f, 10f, 1570.339f)]
        [DataRow("L2", 9f, 9f, 0, 0, 1413.996f)]
        [DataRow("M2", 10f, 10f, 0, 0, 1570.339f)]
        [DataRow("X2", 0f, 0f, 180f, 0, 20038.2963f)]
        [DataRow("Aachen-Frankfurt O.", 6.0839f, 50.7753f, 14.5506f, 52.3472f, 611.1159f)]
        [DataRow("Konstanz-Flensburg", 9.1732f, 47.6779f, 9.4470f, 54.7937f, 792.3866f)]
        public void CalcDistanceTest(string name, float dLonA, float dLatA, float dLonB, float dLatB, float fExp)
        {
            Assert.AreEqual(fExp, PlaceHelpers.CalcDistance(dLonA, dLatA, dLonB, dLatB), 1e-3f);
        }
    }
}