using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Globalization;

namespace MathLibrary.TwoDim.Tests;

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
        CProcAntennaValues testClass = new(avData, [0]);
        Assert.IsTrue(testClass.HandleMovement(fR, fT, fDt));
        System.Diagnostics.Debug.WriteLine($"{{{string.Join(", ", ToDArr(testClass.Debug.aPoints).Select(d => d.ToString(CultureInfo.InvariantCulture) + "d"))}}}");
        CollectionAssert.AreEqual(adExp, ToDArr(testClass.Debug.aPoints), new DObjComaprer());
    }

    class DObjComaprer : System.Collections.IComparer
    {
        public int Compare(object? x, object? y) => x is double dx && y is double dy ? (Math.Abs(dx - dy) < 1e-8 ? 0 : -1) : x!.Equals(y) ? 0 : -1;
    }

    [TestMethod]
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
        CProcAntennaValues testClass = new(avData, [0]);
        Assert.IsTrue(testClass.HandleStdAntennaValue(fValue, xDetect));
        System.Diagnostics.Debug.WriteLine($"{{{string.Join(", ", ToDArr(testClass.Debug.aPoints).Select(d => d.ToString(CultureInfo.InvariantCulture) + "d"))}}}");
        CollectionAssert.AreEqual(adExp, ToDArr(testClass.Debug.aPoints), new DObjComaprer());
    }

    [TestMethod]
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
        CProcAntennaValues testClass = new(avData, [0]);
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

    [TestMethod]
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

    [TestMethod]
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
        AssertAreEqual(CreateTrackSegment(adExp), _Seg);
    }

    private static void AssertAreEqual(StTrackSeg tsExp, StTrackSeg _Seg)
    {
        Assert.AreEqual(tsExp.vFootPoint.x, _Seg.vFootPoint.x, 1e-8, "Seg.vFootPoint.x");
        Assert.AreEqual(tsExp.vFootPoint.y, _Seg.vFootPoint.y, 1e-8, "Seg.vFootPoint.y");
        Assert.AreEqual(tsExp.vNormal.x, _Seg.vNormal.x, 1e-8, "Seg.vNormal.x");
        Assert.AreEqual(tsExp.vNormal.y, _Seg.vNormal.y, 1e-8, "Seg.vNormal.y");
        Assert.AreEqual(tsExp.lrRadius, _Seg.lrRadius, 1e-8, "Seg.lrRadius");
    }

    [TestMethod()]
    [DataRow(new[] { 0.0, 0.0, 0.0, 1.0, 0.0}, 400.0d, 0 , 0, DisplayName = "1 - Default")]
    [DataRow(new[] { 0.0, 123.0, 0.0, 1.0, 0.0}, 400.0d, 0 , 123.0, DisplayName = "2 - Straight (y+123)")]
    [DataRow(new[] { 0.0, 123.0, -0.5, 0.86602540378, 0 }, 400.0d, 0.523598775600518, 353.940107677034, DisplayName = "2a - Straight (y+123 30°)")]
    [DataRow(new[] { 0.0, 123.0,  0.5, 0.86602540378, 0 }, 400.0d, -0.523598775600518, -107.940107677034, DisplayName = "2b - Straight (y+123 -30°)")]
    [DataRow(new[] {-1000.0, -1000.0, 0.0, 1.0, 5000.0}, 1000.0d, 0.41151684606748806, -582.57569495584, DisplayName = "3 - Curve ((-1,4), 5m) s=1m")]
    [DataRow(new[] {-1000.0, -1000.0, 0.0, 1.0, 5000.0}, 2000.0d, 0.64350110879328437, 0.0, DisplayName = "4 - Curve ((-1,4), 5m) s=2m")]
    [DataRow(new[] {    0.0,     0.0, 0.0, 1.0, 2000.0}, 400.0d, 0.201357920790331, 40.4082057734577, DisplayName = "5 - Curve ((0,2), 2m) s=40cm")]
    [DataRow(new[] {    0.0,     0.0, 0.0, 1.0, -2000.0}, 400.0d, -0.201357920790331, -40.4082057734577, DisplayName = "6 - Curve ((0,-2), 2m) s=40cm")]
    [DataRow(new[] {    0.0,     0.0, 0.0, 1.0, 2000.0}, -400.0d, -0.201357920790331, 40.4082057734577, DisplayName = "7 - Curve ((0,2), 2m) s=-40cm")]
    [DataRow(new[] {    0.0,     0.0, 0.0, 1.0, -2000.0}, -400.0d, 0.201357920790331, -40.4082057734577, DisplayName = "8 - Curve ((0,-2), 2m) s=-40cm")]

    public void ComputeAngleOffsetTest(double[] adSeg,double lrDist,double lrExpAngle,double lrOffset)
    {
        StTrackSeg _Seg = CreateTrackSegment(adSeg);
        CProcAntennaValues.ComputeAngleOffset(_Seg, lrDist, out var _fAngle, out var _fOffset);
        Assert.AreEqual(lrExpAngle, _fAngle, 1e-8, "fAngle");
        Assert.AreEqual(lrOffset, _fOffset, 1e-8, "fOffset");
    }

    /// <summary>
    /// Verifies that <see cref="CProcAntennaValues.HandleAntennaValue(double, double, bool)"/> keeps the stored points unchanged when no detection is reported.
    /// </summary>
    [TestMethod]
    public void HandleAntennaValueLeavesPointsUnchangedWhenNoDetectionOccurs()
    {
        var points = CreateDefaultStoredPoints();
        var expected = ToDArr(points);
        var testClass = new CProcAntennaValues(points, [0]);

        Assert.IsTrue(testClass.HandleAntennaValue(150.0d, 150.0d, false));

        CollectionAssert.AreEqual(expected, ToDArr(testClass.Debug.aPoints), new DObjComaprer());
    }

    /// <summary>
    /// Verifies that <see cref="CProcAntennaValues.HandleAntennaValue(double, double, bool)"/> inserts a new point at the front when the measured offset is ahead of the current range.
    /// </summary>
    [TestMethod]
    public void HandleAntennaValueInsertsPointAtFrontWhenOffsetIsAheadOfStoredRange()
    {
        var points = CreateDefaultStoredPoints();
        points[0] = new Math2d.Vector(279.9d, 0.0d);
        points[1] = new Math2d.Vector(260.0d, 0.0d);
        var testClass = new CProcAntennaValues(points, [0]);

        Assert.IsTrue(testClass.HandleAntennaValue(300.0d, 150.0d, true));

        Assert.AreEqual(300.0d, testClass.Debug.aPoints[0].x, 1e-8, "Points[0].x");
        Assert.AreEqual(150.0d, testClass.Debug.aPoints[0].y, 1e-8, "Points[0].y");
        Assert.AreEqual(279.9d, testClass.Debug.aPoints[1].x, 1e-8, "Points[1].x");
        Assert.AreEqual(37.5d, testClass.HMI[0].y, 1e-8, "HMI[0].y");
    }

    /// <summary>
    /// Verifies that <see cref="CProcAntennaValues.HandleAntennaValue(double, double, bool)"/> appends a new point at the end when the measured offset is behind the current range.
    /// </summary>
    [TestMethod]
    public void HandleAntennaValueAppendsPointAtEndWhenOffsetIsBehindStoredRange()
    {
        var points = CreateDefaultStoredPoints();
        points[29] = new Math2d.Vector(321.0d, 0.0d);
        points[30] = new Math2d.Vector(321.0d, 0.0d);
        var testClass = new CProcAntennaValues(points, [0]);

        Assert.IsTrue(testClass.HandleAntennaValue(300.0d, 150.0d, true));

        Assert.AreEqual(280.0d, testClass.Debug.aPoints[0].x, 1e-8, "Points[0].x");
        Assert.AreEqual(300.0d, testClass.Debug.aPoints[30].x, 1e-8, "Points[30].x");
        Assert.AreEqual(150.0d, testClass.Debug.aPoints[30].y, 1e-8, "Points[30].y");
        Assert.IsFalse(testClass.Debug.bFlag, "Appending at the end should clear the internal flag.");
        Assert.AreEqual(37.5d, testClass.HMI[30].y, 1e-8, "HMI[30].y");
    }

    /// <summary>
    /// Verifies that <see cref="CProcAntennaValues.HandleAntennaValue(double, double, bool)"/> blends the nearest point when the measured offset falls within the stored range.
    /// </summary>
    [TestMethod]
    public void HandleAntennaValueBlendsNearestPointWhenOffsetFallsInsideStoredRange()
    {
        var points = CreateDefaultStoredPoints();
        var testClass = new CProcAntennaValues(points, [0]);

        Assert.IsTrue(testClass.HandleAntennaValue(150.0d, 150.0d, true));

        Assert.AreEqual(159.5d, testClass.Debug.aPoints[7].x, 1e-8, "Points[7].x");
        Assert.AreEqual(7.5d, testClass.Debug.aPoints[7].y, 1e-8, "Points[7].y");
        Assert.AreEqual(140.0d, testClass.Debug.aPoints[8].x, 1e-8, "Points[8].x");
        Assert.AreEqual(0.0d, testClass.Debug.aPoints[8].y, 1e-8, "Points[8].y");
    }

    /// <summary>
    /// Verifies that <see cref="CProcAntennaValues.ComputeTrackAndLookAhead(double, double, double, out StTrackSeg, out StTrackSeg, out double)"/> returns the expected straight track for the default stored points.
    /// </summary>
    [TestMethod]
    public void ComputeTrackAndLookAheadReturnsStraightTrackForDefaultPoints()
    {
        var testClass = new CProcAntennaValues(CreateDefaultStoredPoints(), [0]);

        Assert.IsTrue(testClass.ComputeTrackAndLookAhead(0.0d, 0.0d, 0.5d, out var track, out var lookAheadTrack, out var yDeviation));

        AssertAreEqual(CreateTrackSegment([0.0d, 0.0d, 0.0d, 1.0d, 0.0d]), track);
        AssertAreEqual(CreateTrackSegment([0.0d, 0.0d, 0.0d, 1.0d, 0.0d]), lookAheadTrack);
        Assert.AreEqual(0.0d, yDeviation, 1e-8, nameof(yDeviation));
        Assert.AreEqual(0.0d, testClass.HMI[29].x, 1e-8, "HMI[29].x");
        Assert.AreEqual(0.0d, testClass.HMI[29].y, 1e-8, "HMI[29].y");
    }

    /// <summary>
    /// Verifies that <see cref="CProcAntennaValues.ComputeTrackAndLookAhead(double, double, double, out StTrackSeg, out StTrackSeg, out double)"/> composes the helper calculations consistently for a curved point set.
    /// </summary>
    [TestMethod]
    public void ComputeTrackAndLookAheadMatchesHelperCompositionForCurvedInput()
    {
        var points = ToVArr([
            300.0d, 150.0d, 279.9d, 0d, 260d, 0d, 240d, 0d, 220d, 0d, 200d, 0d, 180d, 0d, 160d, 0d,
            140d, 0d, 120d, 0d, 100d, 0d, 80d, 0d, 60d, 0d, 40d, 0d, 20d, 0d, 0d, 0d,
            0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d,
            0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d]);
        var testClass = new CProcAntennaValues(points, [0]);
        testClass.Calculate3DistinctPoints(out var front, out var middle, out var back);
        var lookAheadPoints = CProcAntennaValues.CalculateLookAhead(1.0d, 300.0d, 1.0d, [front, middle, back]);
        var expectedTrack = testClass.CalcTrack([front, middle, back], out var expectedCenter, out var expectedDeviation);
        var expectedLookAheadTrack = testClass.CalcTrack(lookAheadPoints, out _, out _);

        Assert.IsTrue(testClass.ComputeTrackAndLookAhead(1.0d, 300.0d, 1.0d, out var track, out var lookAheadTrack, out var yDeviation));

        AssertAreEqual(expectedTrack, track);
        AssertAreEqual(expectedLookAheadTrack, lookAheadTrack);
        Assert.AreEqual(expectedDeviation, yDeviation, 1e-8, nameof(yDeviation));
        Assert.AreEqual(expectedCenter.x * 0.25d, testClass.HMI[29].x, 1e-8, "HMI[29].x");
        Assert.AreEqual(expectedCenter.y * 0.25d, testClass.HMI[29].y, 1e-8, "HMI[29].y");
    }

    /// <summary>
    /// Verifies that <see cref="CProcAntennaValues.Config(double, double, double)"/> updates internal settings used by <see cref="CProcAntennaValues.ComputeVAntennaVal(StTrackSeg, bool)"/>.
    /// </summary>
    [TestMethod]
    public void ConfigUpdatesParametersUsedByComputeVAntennaVal()
    {
        var testClass = new CProcAntennaValues();
        var track = CreateTrackSegment([-1000.0d, 0.0d, 0.0d, 1.0d, 0.0d]);

        var defaultAngle = testClass.ComputeVAntennaVal(track, false);

        Assert.AreEqual(500.0d, defaultAngle, 1e-8, "DefaultAngle");
        Assert.AreEqual(-250.0d, testClass.HMI[28].x, 1e-8, "Default.HMI[28].x");
        Assert.AreEqual(0.0d, testClass.HMI[27].x, 1e-8, "Default.HMI[27].x");

        Assert.IsTrue(testClass.Config(2000.0d, 123.0d, 5.0d));

        var configuredAngle = testClass.ComputeVAntennaVal(track, false);

        Assert.AreEqual(0.0d, configuredAngle, 1e-8, "ConfiguredAngle");
        Assert.AreEqual(250.0d, testClass.HMI[27].x, 1e-8, "Configured.HMI[27].x");
    }

    /// <summary>
    /// Verifies that <see cref="CProcAntennaValues.ComputeVAntennaVal(StTrackSeg, bool)"/> returns zero for straight tracks without a valid normal.
    /// </summary>
    [TestMethod]
    public void ComputeVAntennaValReturnsZeroForStraightTrackWithoutNormal()
    {
        var testClass = new CProcAntennaValues();
        var track = CreateTrackSegment([40.0d, -20.0d, 0.0d, 0.0d, 0.0d]);

        var angle = testClass.ComputeVAntennaVal(track, false);

        Assert.AreEqual(0.0d, angle, 1e-8);
        Assert.AreEqual(10.0d, testClass.HMI[28].x, 1e-8, "HMI[28].x");
        Assert.AreEqual(-5.0d, testClass.HMI[28].y, 1e-8, "HMI[28].y");
    }

    /// <summary>
    /// Verifies straight-track steering computation for forward and reverse driving modes.
    /// </summary>
    [TestMethod]
    public void ComputeVAntennaValStraightTrackReturnsExpectedForwardAndReverseAngles()
    {
        var testClass = new CProcAntennaValues();
        var track = CreateTrackSegment([0.0d, 0.0d, 0.0d, 1.0d, 0.0d]);

        var forwardAngle = testClass.ComputeVAntennaVal(track, false);
        var reverseAngle = testClass.ComputeVAntennaVal(track, true);

        Assert.AreEqual(500.0d, forwardAngle, 1e-8, "ForwardAngle");
        Assert.AreEqual(-500.0d, reverseAngle, 1e-8, "ReverseAngle");
        Assert.AreEqual(-250.0d, testClass.HMI[27].x, 1e-8, "Reverse.HMI[27].x");
        Assert.AreEqual(0.0d, testClass.HMI[27].y, 1e-8, "Reverse.HMI[27].y");
    }

    /// <summary>
    /// Verifies curved-track steering computation and HMI target point projection.
    /// </summary>
    [TestMethod]
    public void ComputeVAntennaValCurvedTrackReturnsExpectedAngleAndHmiTarget()
    {
        var testClass = new CProcAntennaValues();
        var track = CreateTrackSegment([0.0d, 0.0d, 0.0d, 1.0d, 2000.0d]);

        var angle = testClass.ComputeVAntennaVal(track, false);

        Assert.AreEqual(1497.3969638106419d, angle, 1e-8, "Angle");
        Assert.AreEqual(-248.049416807332d, testClass.HMI[27].x, 1e-8, "HMI[27].x");
        Assert.AreEqual(-31.1686833463069d, testClass.HMI[27].y, 1e-8, "HMI[27].y");
        Assert.AreEqual(0.0d, testClass.HMI[28].x, 1e-8, "HMI[28].x");
        Assert.AreEqual(0.0d, testClass.HMI[28].y, 1e-8, "HMI[28].y");
    }

    private Math2d.Vector[] CreateDefaultStoredPoints()
    {
        var result = new Math2d.Vector[31];
        for (var i = 0; i < 16; i++)
            result[i] = new Math2d.Vector(300.0d - (20.0d * i), 0.0d);
        for (var i = 16; i < result.Length; i++)
            result[i] = new Math2d.Vector(0.0d, 0.0d);
        return result;
    }

    private StTrackSeg CreateTrackSegment(double[] adSeg)
    {
        return new StTrackSeg(ToVArr(adSeg)[0], ToVArr(adSeg)[1], adSeg[4]);
    }
}
