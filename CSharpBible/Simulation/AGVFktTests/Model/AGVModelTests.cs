using AGVFkt.Model;
using AGVFkt.Model.Interface;
using MathLibrary.TwoDim;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AGVFkt.Model.Tests;

[TestClass()]
public class AGVModelTests
{
    AGVModel testModel;

    [TestInitialize]
    public void Init()
    {
        testModel = new();
    }

    [DataTestMethod()]
    [DataRow(new[] { 0d, 0d }, 1d, 0d, DisplayName = "05: 0°, 0°")]
    [DataRow(new[] { Math.PI / 4, Math.PI / 4 }, 1d, Math.PI / 4, DisplayName = "06: 45°, 45°")]
    [DataRow(new[] { Math.PI / 2, Math.PI / 2 }, 1d, Math.PI / 2, DisplayName = "07: 90°, 90°")]
    [DataRow(new[] { Math.PI * 3 / 4, Math.PI * 3 / 4 }, 1d, Math.PI * 3 / 4, DisplayName = "08: 135°, 135°")]
    [DataRow(new[] { Math.PI, Math.PI }, 1d, Math.PI, DisplayName = "09: 180°, 180°")]
    [DataRow(new[] { -Math.PI / 4, -Math.PI / 4 }, 1d, -Math.PI / 4, DisplayName = "04: -45°, -45°")]
    [DataRow(new[] { -Math.PI / 2, -Math.PI / 2 }, 1d, -Math.PI / 2, DisplayName = "03: -90°, -90°")]
    [DataRow(new[] { -Math.PI * 3 / 4, -Math.PI * 3 / 4 }, 1d, -Math.PI * 3 / 4, DisplayName = "02: -135°, -135°")]
    [DataRow(new[] { -Math.PI, -Math.PI }, 1d, -Math.PI, DisplayName = "01: -180°, -180°")]
    [DataRow(new[] { 0d, Math.PI }, 1d, Math.PI, DisplayName = "11: 0°, 180°")]
    [DataRow(new[] { Math.PI / 2, -Math.PI / 2 }, -1d, Math.PI / 2, DisplayName = "12: 90°,-90°")]
    [DataRow(new[] { -Math.PI / 2, Math.PI / 2 }, -1d, -Math.PI / 2, DisplayName = "13:-90°, 90°")]
    [DataRow(new[] { Math.PI / 2, -Math.PI / 2 }, -1d, Math.PI / 2, DisplayName = "12: 90°,-90°")]
    [DataRow(new[] { -Math.PI / 2, Math.PI / 2 }, -1d, -Math.PI / 2, DisplayName = "13:-90°, 90°")]
    public void LenkUmschalgFctTest(double[] adVal, double dExSf, double dExp)
    {
        (double, double) r = (0d, 0d);
        foreach (var d in adVal)
            r = testModel.LenkHystereseEugen(d);
        Assert.AreEqual(dExSf, r.Item2);
        Assert.AreEqual(dExp, r.Item1);
    }

    [DataTestMethod()]
    [DataRow(new[] { 0d, 0d }, 1d, 0d, DisplayName = "05: 0°, 0°")]
    [DataRow(new[] { Math.PI / 4, Math.PI / 4 }, 1d, Math.PI / 4, DisplayName = "06: 45°, 45°")]
    [DataRow(new[] { Math.PI / 2, Math.PI / 2 }, 1d, Math.PI / 2, DisplayName = "07: 90°, 90°")]
    [DataRow(new[] { Math.PI * 3 / 4, Math.PI * 3 / 4 }, 1d, Math.PI * 3 / 4, DisplayName = "08: 135°, 135°")]
    [DataRow(new[] { Math.PI, Math.PI }, 1d, Math.PI, DisplayName = "09: 180°, 180°")]
    [DataRow(new[] { -Math.PI / 4, -Math.PI / 4 }, 1d, -Math.PI / 4, DisplayName = "04: -45°, -45°")]
    [DataRow(new[] { -Math.PI / 2, -Math.PI / 2 }, 1d, -Math.PI / 2, DisplayName = "03: -90°, -90°")]
    [DataRow(new[] { -Math.PI * 3 / 4, -Math.PI * 3 / 4 }, 1d, -Math.PI * 3 / 4, DisplayName = "02: -135°, -135°")]
    [DataRow(new[] { -Math.PI, -Math.PI }, 1d, -Math.PI, DisplayName = "01: -180°, -180°")]
    [DataRow(new[] { 0d, Math.PI }, -1d, 0, DisplayName = "11: (!) 0°, 180°")]
    [DataRow(new[] { Math.PI / 2, -Math.PI / 2 }, -1d, Math.PI / 2, DisplayName = "12: 90°,-90°")]
    [DataRow(new[] { -Math.PI / 2, Math.PI / 2 }, -1d, -Math.PI / 2, DisplayName = "13:-90°, 90°")]
    [DataRow(new[] { Math.PI / 2, -Math.PI / 2 }, -1d, Math.PI / 2, DisplayName = "12: 90°,-90°")]
    [DataRow(new[] { -Math.PI / 2, Math.PI / 2 }, -1d, -Math.PI / 2, DisplayName = "13:-90°, 90°")]
    public void LenkHysterese(double[] adVal, double dExSf, double dExp)
    {

        var r = testModel.LenkHysterese(adVal[1], adVal[0]);
        Assert.AreEqual(dExSf, r.Item2);
        Assert.AreEqual(dExp, r.Item1);
    }

    [TestMethod()]
    [DataRow(E_NumberLCP.E_LCP_1_TRACK_FOUND, -10, 0, 20, 1.0, E_SelectTrack.Center, 10, 0.0, 1, E_ChosenTrack.e_center_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_LEFT_DIV, -10, 0, 20, 1.0, E_SelectTrack.Center, 10, 0.0, 1, E_ChosenTrack.e_center_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_LEFT_DIV, -10, 0, 20, -1.0, E_SelectTrack.Center, 10, 0.0, 1, E_ChosenTrack.e_center_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_LEFT_DIV, -10, 0, 20, -1.0, E_SelectTrack.Left, 10, 0.0, 1, E_ChosenTrack.e_center_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_LEFT_DIV, -10, 0, 20, 1.0, E_SelectTrack.Left, 10, 0.0, 1, E_ChosenTrack.e_center_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_LEFT_DIV, -10, 0, 20, 1.0, E_SelectTrack.Right, 10, 0.0, 1, E_ChosenTrack.e_center_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_LEFT_DIV, -10, 0, 20, 1.0, E_SelectTrack.Right, 5, -10.0, 2, E_ChosenTrack.e_no_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_LEFT_DIV, -10, 0, 20, -1.0, E_SelectTrack.Right, 5, 0.0, 2, E_ChosenTrack.e_no_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_LEFT_DIV, -10, 0, 20, -1.0, E_SelectTrack.Right, 10, 0.0, 2, E_ChosenTrack.e_no_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_LEFT_DIV, -19, 0, 20, -1.0, E_SelectTrack.Left, 10, 19.0, 2, E_ChosenTrack.e_no_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_LEFT_DIV, -20, 0, 20, 1.0, E_SelectTrack.Left, 10, 0.0, 2, E_ChosenTrack.e_no_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_RIGHT_DIV, -10, 0, 20, 1.0, E_SelectTrack.Left, 10, 20.0, 2, E_ChosenTrack.e_maximum_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_RIGHT_DIV, -11, 0, 20, -1.0, E_SelectTrack.Center, 10, -20.0, 2, E_ChosenTrack.e_minimum_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_RIGHT_DIV, -12, 0, 20, -1.0, E_SelectTrack.Left, 10, 0.0, 2, E_ChosenTrack.e_maximum_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_RIGHT_DIV, -13, 0, 20, -1.0, E_SelectTrack.Right, 5, -20.0, 2, E_ChosenTrack.e_minimum_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_RIGHT_DIV, -14, 0, 20, 1.0, E_SelectTrack.Center, 10, 0.0, 2, E_ChosenTrack.e_minimum_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_RIGHT_DIV, -15, 0, 20, 1.0, E_SelectTrack.Right, 5, 0.0, 2, E_ChosenTrack.e_minimum_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_RIGHT_DIV, -16, 0, 10, -1.0, E_SelectTrack.Left, 10, 0.0, 1, E_ChosenTrack.e_center_value)]
    [DataRow(E_NumberLCP.E_LCP_2_TRACK_FOUND_RIGHT_DIV, -17, 0, 10, 1.0, E_SelectTrack.Left, 10, 0.0, 1, E_ChosenTrack.e_center_value)]
    [DataRow(E_NumberLCP.E_LCP_3_TRACK_FOUND_OR_INTERSECTION, -21, 1, 21, 1.0, E_SelectTrack.Left, 10, 21.0, 3, E_ChosenTrack.e_maximum_value)]
    [DataRow(E_NumberLCP.E_LCP_NO_TRACK_FOUND, -10, 0, 20, 1.0, E_SelectTrack.Left, 10, 0.0, 0, E_ChosenTrack.e_no_value)]

    public void iActTrackOffsetTest(E_NumberLCP eNumberLCP, int iDeviationLCP1, int iDeviationLCP2, int iDeviationLCP3, double lrFactor, E_SelectTrack eSelectedTrack, double lrCfgOffsetDifferenceThreshold, double lrExp, int iExp, E_ChosenTrack eExp)
    {
        Assert.AreEqual(lrExp, testModel.iActTrackOffset(eNumberLCP, iDeviationLCP1, iDeviationLCP2, iDeviationLCP3, lrFactor, eSelectedTrack, lrCfgOffsetDifferenceThreshold), 1e-5);
        Assert.AreEqual(iExp, testModel._usiNoOfTracks, "NoOfTracks");
        Assert.AreEqual(eExp, testModel._eChosenTrack, "Chosen Track");
    }

    [TestMethod()]
    // Alles Null/False, sollte true sein
    [DataRow(false, false,new[] { 0d, 0d },0d, new[] { 0d, 0d }, 0d, 1d,new[] { 0d, 0d },0d, true )]
    // Kleine Bewegung, mit Em-Stop, sollte true sein
    [DataRow(true, false, new[] { 1d, 0d }, 0d, new[] { 1000d, 0d }, 0d, 1d, new[] { 0d, 0d }, 0d, true)]
    // Kleine neg. Bewegung, mit Em-Stop, sollte true sein
    [DataRow(true, false, new[] { -1d, 0d }, 0d, new[] { 1000d, 0d }, 0d, 1d, new[] { 0d, 0d }, 0d, true)]
    // Mittlere Bewegung, mit Em-Stop, maximale Verz. sollte false sein
    [DataRow(true, false, new[] { 500d, 0d }, 0d, new[] { 1000d, 0d }, 0d, 1d, new[] { 485d, 0d }, 0d, false)]
    // Mittlere neg. Bewegung, mit Em-Stop, maximale Verz. sollte false sein
    [DataRow(true, false, new[] { -500d, 0d }, 0d, new[] { 1000d, 0d }, 0d, 1d, new[] { -485d, 0d }, 0d, false)]
    // Mittlere Bewegung, mit Em-Stop, Ziel hat Y-Anteil maximale Verz. sollte false sein
    [DataRow(true, false, new[] { 500d, 0d }, 0d, new[] { 1000d, 250d }, 0d, 1d, new[] { 497.0781230056279d, 8.827601516760556d }, 0d, false)]
    [DataRow(true, false, new[] { 500d, 0d }, 0d, new[] {-1000d, 250d }, 0d, 1d, new[] { 497.0781230056279d, -8.827601516760556d }, 0d, false)]
    [DataRow(true, false, new[] { 500d, 0d }, 0d, new[] { 900d, 0d }, 1d, 1d, new[] { 496.0393585357513d, 0d }, 0.057870658429525296d, false)]
    [DataRow(true, true, new[] { 0d, 0d }, 0d, new[] { 1d, 0d }, 0d, 1d, new[] { 1d, 0d }, 0d, false)]
    [DataRow(true, true, new[] { 0.01d, 0d }, 0d, new[] { 1d, 0d }, 0d, 1d, new[] { 1d, 0d }, 0d, false)]
    [DataRow(true, true, new[] { 0.01d, 0d }, 0d, new[] { 0d, 0d }, 0d, 1d, new[] { 0d, 0d }, 0d, true)]
    [DataRow(true, true, new[] { 500d, 0d }, 0d, new[] { 0d, 0d }, 0d, 1d, new[] { 488d, 0d }, 0d, false)]
    [DataRow(true, true, new[] { 500d, 0d }, 0d, new[] { 0d, 0.1d }, 0d, 1d, new[] { 488d, 0d }, 0d, false)]
    [DataRow(true, true, new[] { 500d, 0d }, 0d, new[] { 0d, 1d }, 0d, 1d, new[] { 492.5739035d, 7.0696438d }, 0d, false)]
    [DataRow(true, true, new[] { 500d, 0d }, 0d, new[] { 1000d, 0d }, 0d, 1d, new[] { 509d, 0d }, 0d, false)]
    [DataRow(true, true, new[] { 500d, 0d }, 0d, new[] { 1000d, 0d }, 0d, 0.1d, new[] { 488d, 0d }, 0d, false)]
    [DataRow(true, true, new[] { 999d, 0d }, 0d, new[] { 1000d, 0d }, 0d, 1d, new[] { 1000d, 0d }, 0d, false)]
    [DataRow(true, true, new[] { 500d, 0d }, 0d, new[] {-1000d, 0d }, 0d, 1d, new[] { 488d, 0d }, 0d, false)]
    [DataRow(true, true, new[] { 500d, 0d }, 0d, new[] { 1000d, 250d }, 0d, 1d, new[] { 500.59037585167795d, 8.980615588797d }, 0d, false)]
    [DataRow(true, true, new[] { 500d, 0d }, 0d, new[] {-1000d, 250d }, 0d, 1d, new[] { 497.10881399598287d, -8.734877615812579d }, 0d, false)]
    [DataRow(true, true, new[] { 500d, 0d }, 0d, new[] { 900d, 0d }, 1d, 1d, new[] { 501.2053898310831d, 0d }, 0.05945942981193943d, false)]
    public void M30_LimitMasterFltTest(bool xEnable, bool xEmergencyStopOK, double[] lastV, double lastOmega, double[] SetpLin, double setpOmega, double lrLimFak, double[] vExpOut,double lrExpOut, bool xExp )
    {
        testModel.SetLastVal(lastV, lastOmega,xEnable);
        testModel.M30_LimitMasterFlt(xEnable,xEmergencyStopOK, Math2d.Vec(SetpLin),setpOmega,lrLimFak,out var vOut, out var lrOut, out var xOut) ;
        Assert.AreEqual(xExp, xOut,"xOut");
        Assert.AreEqual(lrExpOut, lrOut,1e-5, "lrOut");
        Assert.AreEqual(vExpOut[0], vOut.x, 1e-5, "vOut.x");
        Assert.AreEqual(vExpOut[1], vOut.y, 1e-5, "vOut.y");
    }

    private Math2d.Vector ToVec(double[] setpLin)
    {
        return Math2d.Vec(setpLin);
    }
}