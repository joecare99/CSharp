using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Globalization;

namespace MathLibrary.TwoDim.Tests
{
    [TestClass()]
    public class CProcAntennaValuesTests
    {
        private Math2d.Vector[] ToVArr(double[] aD)
        {
            var Result = new Math2d.Vector[aD.Length / 2];
            for (int i = 0; i < aD.Length / 2; i++)
            {
                Result[i] = new Math2d.Vector(aD[i * 2], aD[i * 2 + 1]);
            }
            return Result;
        }
        private double[] ToDArr(Math2d.Vector[] aV)
        {
            var Result = new double[aV.Length * 2];
            for (int i = 0; i < aV.Length; i++)
            {
                Result[i * 2] = aV[i].x;
                Result[i * 2 + 1] = aV[i].y;
            }
            return Result;
        }

        [TestMethod()]
        [DataRow(new[] { 300.0, 0, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0 },
            0, 0, 0.01,
            new[] { 300.0, 0, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0 ,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0})]
        [DataRow(new[] {300.0, 0, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            10d, 0, 0.01,
            new[] { 299.9, 0, 279.9, 0, 259.9, 0, 239.9, 0, 219.9, 0, 199.9, 0, 179.9, 0, 159.9, 0, 139.9, 0, 119.9, 0, 99.9, 0, 79.9, 0, 59.9, 0, 39.9, 0, 19.9, 0,
                -0.1, 0, -0.1, 0, -0.1, 0, -0.1, 0, -0.1, 0, -0.1, 0, -0.1, 0, -0.1, 0, -0.1, 0, -0.1, 0, -0.1, 0, -0.1, 0, -0.1, 0, -0.1, 0, -0.1, 0, -0.1, 0 })]
        [DataRow(new[] {300.0, 0, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            0d, -Math.PI, 0.5,
            new[] { 1.83690953073357E-14d, 300d, 1.71444889535133E-14d, 280d, 1.59198825996909E-14d, 260d, 1.46952762458685E-14d, 240d, 1.34706698920461E-14d, 220d,
                1.22460635382238E-14d, 200d, 1.10214571844014E-14d, 180d, 9.79685083057902E-15d, 160d, 8.57224447675664E-15d, 140d, 7.34763812293426E-15d, 120d,
                6.12303176911189E-15d, 100d, 4.89842541528951E-15d, 80d, 3.67381906146713E-15d, 60d, 2.44921270764475E-15d, 40d, 1.22460635382238E-15d, 20d,
                0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d })]
        [DataRow(new[] {300.0, 0, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            10d, 1, 0.1,
            new[] { 297.50624541813d, -29.8501915774016d, 277.606162112569d, -27.8535232444651d, 257.706078807009d, -25.8568549115285d, 237.805995501448d, -23.8601865785919d,
                217.905912195888d, -21.8635182456554d, 198.005828890327d, -19.8668499127188d, 178.105745584767d, -17.8701815797822d, 158.205662279206d, -15.8735132468457d,
                138.305578973646d, -13.8768449139091d, 118.405495668085d, -11.8801765809726d, 98.5054123625246d, -9.88350824803599d, 78.605329056964d, -7.88683991509942d,
                58.7052457514035d, -5.89017158216286d, 38.805162445843d, -3.8935032492263d, 18.9050791402825d, -1.89683491628973d, -0.995004165278026d, 0.0998334166468282d,
                -0.995004165278026d, 0.0998334166468282d, -0.995004165278026d, 0.0998334166468282d, -0.995004165278026d, 0.0998334166468282d, -0.995004165278026d, 0.0998334166468282d,
                -0.995004165278026d, 0.0998334166468282d, -0.995004165278026d, 0.0998334166468282d, -0.995004165278026d, 0.0998334166468282d, -0.995004165278026d, 0.0998334166468282d,
                -0.995004165278026d, 0.0998334166468282d, -0.995004165278026d, 0.0998334166468282d, -0.995004165278026d, 0.0998334166468282d, -0.995004165278026d, 0.0998334166468282d,
                -0.995004165278026d, 0.0998334166468282d, -0.995004165278026d, 0.0998334166468282d, -0.995004165278026d, 0.0998334166468282d })]
        public void HandleMovementTest(double[] aDVal, double fT, double fR, double fDt, double[] adExp)
        {
            var avData = ToVArr(aDVal);
            CProcAntennaValues testClass = new(avData);
            Assert.IsTrue(testClass.HandleMovement(fR, fT, fDt));
            System.Diagnostics.Debug.WriteLine($"{{{string.Join(", ", ToDArr(testClass.Debug.aPoints).Select(d => d.ToString(CultureInfo.InvariantCulture) + "d"))}}}");
            CollectionAssert.AreEqual(adExp, ToDArr(testClass.Debug.aPoints), new DObjComaprer());
        }

        class DObjComaprer : System.Collections.IComparer
        {
            public int Compare(object? x, object? y) => x is double dx && y is double dy ? (Math.Abs(dx - dy) < 1e-8 ? 0 : -1) : x!.Equals(y) ? 0 : -1;
        }

        [DataTestMethod]
        [DataRow(new[] {300.0, 0, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, false, double.NaN,
            new[] {300.0, 0, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 })]
        [DataRow(new[] {300.0, 0, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, true, 0d,
            new[] {300.0, 0, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 })]
        [DataRow(new[] {300.0, 0, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, true, 150d,
            new[] {300.0, 7.5d, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 })]
        [DataRow(new[] {279.9, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, true, 150d,
            new[] {300.0, 150.0d, 279.9d, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 })]
        [DataRow(new[] {579.9, 0, 560, 0, 540, 0, 520, 0, 500, 0, 480, 0, 460, 0, 440, 0, 420, 0, 400, 0, 380, 0, 360, 0, 340, 0, 321, 0, 321, 0,
            321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0 }, true, 150d,
            new[] { 560d, 0, 540, 0, 520, 0, 500, 0, 480, 0, 460, 0, 440, 0, 420, 0, 400, 0, 380, 0, 360, 0, 340, 0, 321, 0, 321, 0,
            321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,300,150 })]
        public void HandleStdAntennaValueTest(double[] aDVal, bool xDetect, double fValue, double[] adExp)
        {
            var avData = ToVArr(aDVal);
            CProcAntennaValues testClass = new(avData);
            Assert.IsTrue(testClass.HandleStdAntennaValue(fValue, xDetect));
            System.Diagnostics.Debug.WriteLine($"{{{string.Join(", ", ToDArr(testClass.Debug.aPoints).Select(d => d.ToString(CultureInfo.InvariantCulture) + "d"))}}}");
            CollectionAssert.AreEqual(adExp, ToDArr(testClass.Debug.aPoints), new DObjComaprer());
        }

        [DataTestMethod]
        [DataRow(new[] {300.0, 0, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, new[] { 150d, 0d }, new[] { 45d, 0d }, new[] { 1.25d, 0d }, DisplayName = "1 - Default")]
        [DataRow(new[] {300.0, 1d, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, new[] { 150d, 0.0625d }, new[] { 45d, 0d }, new[] { 1.25d, 0d }, DisplayName = "2 - Change Some Point")]
        [DataRow(new[] {300.0, 7.5d, 280, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, new[] { 150d, 0.46875d }, new[] { 45d, 0d }, new[] { 1.25d, 0d }, DisplayName = "3 - Append Start")]
        [DataRow(new[] {300.0, 150.0d, 279.9d, 0, 260, 0, 240, 0, 220, 0, 200, 0, 180, 0, 160, 0, 140, 0, 120, 0, 100, 0, 80, 0, 60, 0, 40, 0, 20, 0, 0, 0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, new[] { 149.99375d, 9.375d }, new[] { 45d, 0d }, new[] { 1.25d, 0d }, DisplayName = "4")]
        [DataRow(new[] { 560d, 0, 540, 0, 520, 0, 500, 0, 480, 0, 460, 0, 440, 0, 420, 0, 400, 0, 380, 0, 360, 0, 340, 0, 321, 0, 321, 0,
            321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,321,0,300,150 },
             new[] { 417.75d, 0d }, new[] { 339.4375d, 0d }, new[] { 321d, 0d }, DisplayName = "5 - Append End")]
        public void Calculate3DistinctPointsTest(double[] aDVal, double[] adExp1, double[] adExp2, double[] adExp3)
        {
            var avData = ToVArr(aDVal);
            CProcAntennaValues testClass = new(avData);
            testClass.Calculate3DistinctPoints(out var _v1, out var _v2, out var _v3);
            System.Diagnostics.Debug.Write($"new[]{{{_v1.x}d, {_v1.y}d}}, ");
            System.Diagnostics.Debug.Write($"new[]{{{_v2.x}d, {_v2.y}d}}, ");
            System.Diagnostics.Debug.WriteLine($"new[]{{{_v3.x}d, {_v3.y}d}}");
            Assert.AreEqual(adExp1[0], _v1.x, 1e-8, "v1.x");
            Assert.AreEqual(adExp1[1], _v1.y, 1e-8, "v1.y");
            Assert.AreEqual(adExp2[0], _v2.x, 1e-8, "v2.x");
            Assert.AreEqual(adExp2[1], _v2.y, 1e-8, "v2.y");
            Assert.AreEqual(adExp3[0], _v3.x, 1e-8, "v3.x");
            Assert.AreEqual(adExp3[0], _v3.x, 1e-8, "v3.x");
            Assert.AreEqual(adExp3[1], _v3.y, 1e-8, "v3.y");
        }

        [DataTestMethod]
        [DataRow(new[] { 150d, 0d, 45d, 0d, 1.25d, 0d }, 0d, 0d, 0.5d, new[] { 150d, 0d, 45d, 0d, 1.25d, 0d }, DisplayName = "0 - No Movement")]
        [DataRow(new[] { 150d, 0d, 45d, 0d, 1.25d, 0d }, Math.PI, 1000d, 0d, new[] { 150d, 0d, 45d, 0d, 1.25d, 0d }, DisplayName = "1 - No Time")]
        [DataRow(new[] { 150d, 0.0625d, 45d, 0d, 1.25d, 0d }, 0d, 100d, 0.5d, new[] { 100d, 0.0625d, -5d, 0d, -48.75d, 0d }, DisplayName = "2 - only linear")]
        [DataRow(new[] { 150d, 0.46875d, 45d, 0d, 1.25d, 0d }, Math.PI, 0d, 0.5d, new[] { 0.46875d, -150d, 0d, -45d, 0d, -1.25d }, DisplayName = "3 - only rotation")]
        [DataRow(new[] { 150d, 0d, 100d, 100d, 0d, -10d }, Math.PI, 100d * Math.PI, 0.5d, new[] { -100d, -50d, 0d, 0d, -110d, 100d }, DisplayName = "4")]
        [DataRow(new[] { 417.75d, 0d, 339.4375d, 0d, 321d, 0d }, 1d, 300d, 1d, new[] { -26.7300071659536d, -213.615195663941d, -69.0424314942523d, -147.717499166172d, -79.0042552586961d, -132.202877883777d }, DisplayName = "5")]
        public void CalculateLookAheadTest(double[] aDVal, double fRot, double fTransl, double fLHTime, double[] adExp)
        {
            var aPoints = CProcAntennaValues.CalculateLookAhead(fRot, fTransl, fLHTime, ToVArr(aDVal));
            System.Diagnostics.Debug.WriteLine($"{{{string.Join(", ", ToDArr(aPoints).Select(d => d.ToString(CultureInfo.InvariantCulture) + "d"))}}}");
            CollectionAssert.AreEqual(adExp, ToDArr(aPoints), new DObjComaprer());
        }

        [DataTestMethod]
        [DataRow(new[] { 150d, 0d, 100d, 0d, 150d, 0d }, new[] { 150d, 0d, 0d, 0d, 0d }, 0d, 0d, double.NaN, DisplayName = "0 - Nothing")]
        [DataRow(new[] { 150d, 0d, 100d, 0d, 50d, 0d }, new[] { 0d, 0d, 0d, 1d, 0d }, 0d, 0d, 0d, DisplayName = "1 - Gerade bei y=0")]
        [DataRow(new[] { 150d, 10d, 100d, 10d, 50d, 10d }, new[] { 0d, 10d, 0d, 1d, 0d }, 0d, 0d, -10d, DisplayName = "1a - Gerade bei y=10")]
        [DataRow(new[] { 300d, 0d, 200d, -10d, 100d, -10d }, new[] { 0d, 0d, -0.149069358148899d, -0.988826742387702d, -1006.2430123981003d }, 150d, 995d, 0d, DisplayName = "2 - Kurve um (150,995) r=1006.24")]
        [DataRow(new[] { 300d, 10d, 200d, 20d, 100d, 20d }, new[] { -1.48849713313414d, 9.77446450758083d, -0.150548620230518d, 0.988602606180404d, 1006.2430123981003d }, 150d, -985d, -9.8871522758227d, DisplayName = "2a - Kurve um (150,-985) r=1006.24")]
        [DataRow(new[] { 300d, 0d, 200d, -0.83d, 100d, 0d }, new[] { 0d, 0d, 0d, 1d, 0d }, 200d, 6023.68138554212d, 0d, DisplayName = "3 - keine Kurve da r>6000")]
        [DataRow(new[] { 300d, 0d, 200d, -12.5d, 100d, 0d }, new[] { 0d, 0d, 0d, 1d, 0d }, 200d, 393.75d, 0d, DisplayName = "4 - keine Kurve da r<500")]
        public void CalcTrack(double[] adPnts, double[] adExp, double fExpX, double fExpY, double fExpDist)
        {
            var testClass = new CProcAntennaValues();
            var _Seg = testClass.CalcTrack(ToVArr(adPnts), out var _VCenter, out var _fDist);
            Assert.IsNotNull(_Seg);
            Assert.IsInstanceOfType(_Seg, typeof(StTrackSeg));
            Assert.AreEqual(fExpX, _VCenter.x, 1e-8, "VCenter.x");
            Assert.AreEqual(fExpY, _VCenter.y, 1e-8, "VCenter.y");
            if (double.IsNaN(fExpDist))
                Assert.AreEqual(fExpDist, _fDist, "fDist = NaN");
            else
                Assert.AreEqual(fExpDist, _fDist, 1e-8, "fDist");
            Assert.AreEqual(adExp[0], _Seg.vFootPoint.x, 1e-8, "Seg.vFootPoint.x");
            Assert.AreEqual(adExp[1], _Seg.vFootPoint.y, 1e-8, "Seg.vFootPoint.y");
            Assert.AreEqual(adExp[2], _Seg.vNormal.x, 1e-8, "Seg.vNormal.x");
            Assert.AreEqual(adExp[3], _Seg.vNormal.y, 1e-8, "Seg.vNormal.y");
            Assert.AreEqual(adExp[4], _Seg.lrRadius, 1e-8, "Seg.lrRadius");
        }



    }
}