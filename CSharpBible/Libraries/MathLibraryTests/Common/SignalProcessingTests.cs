using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathLibrary.Common;
using System;

namespace MathLibrary.Tests.Common;

[TestClass]
public class SignalProcessingTests
{
    [TestMethod]
    [DataRow(1d, 0d, 0d, 1d, 0d)]
    [DataRow(1d, 0d, 0.5d, 1d, 0.5d)]
    [DataRow(1d, 0.5d, 0.5d, 1d, 0.75d)]
    [DataRow(2d, 1d, 1d, 3d, 6d)]
    public void ApplyFirstOrderLowPass_UpdatesState(double input, double initialState, double alpha, double gain, double expected)
    {
        double state = initialState;

        double actual = SignalProcessing.ApplyFirstOrderLowPass(input, ref state, alpha, gain);

        Assert.AreEqual(expected, actual, 1e-12);
        Assert.AreEqual(expected, state, 1e-12);
    }

    [TestMethod]
    [DataRow(-0.1d)]
    [DataRow(1.1d)]
    [DataRow(double.NaN)]
    [DataRow(double.PositiveInfinity)]
    public void ApplyFirstOrderLowPass_RejectsInvalidAlpha(double alpha)
    {
        double state = 2d;

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => SignalProcessing.ApplyFirstOrderLowPass(1d, ref state, alpha));
        Assert.AreEqual(2d, state);
    }

    [TestMethod]
    [DataRow(new double[] { 0d }, new double[] { 1d, 2d }, new double[] { 1d, 2d })]
    [DataRow(new double[] { 0d, 1d, 2d }, new double[] { 2d, 3d, 3d }, new double[] { 5d / 3d, 7d / 3d, 8d / 3d })]
    public void UpdateMovingAverage_UpdatesCircularWindow(double[] initial, double[] samples, double[] expected)
    {
        double[] buffer = (double[])initial.Clone();
        int nextIndex = 0;

        for (int i = 0; i < samples.Length; i++)
        {
            double actual = SignalProcessing.UpdateMovingAverage(samples[i], buffer, ref nextIndex);
            Assert.AreEqual(expected[i], actual, 1e-12, $"Sample {i}");
            Assert.AreEqual((i + 1) % buffer.Length, nextIndex, $"Index {i}");
        }
    }

    [TestMethod]
    [DataRow(new double[] { 0d, 1d }, new double[] { 0d, 0d })]
    [DataRow(new double[] { 0d, 1d, 1d, 1d, 1d, 1d }, new double[] { 0d, 0d, 0d, 1d, 1d, 1d })]
    public void UpdateCircularQuantile_PreservesHistoricalSelectableIndex(double[] samples, double[] expected)
    {
        double[] buffer = new double[5];
        int nextIndex = 0;

        for (int i = 0; i < samples.Length; i++)
        {
            double actual = SignalProcessing.UpdateCircularQuantile(samples[i], buffer, ref nextIndex, 0.5d);
            Assert.AreEqual(expected[i], actual, 1e-12, $"Sample {i}");
        }
    }

    [TestMethod]
    public void UpdateCircularQuantile_ClampsUpperEndpoint()
    {
        double[] buffer = { 1d, 3d, 2d };
        int nextIndex = 0;

        double actual = SignalProcessing.UpdateCircularQuantile(4d, buffer, ref nextIndex, 1d);

        Assert.AreEqual(4d, actual);
        Assert.AreEqual(1, nextIndex);
    }

    [TestMethod]
    [DataRow(-0.1d)]
    [DataRow(1.1d)]
    [DataRow(double.NaN)]
    public void UpdateCircularQuantile_RejectsInvalidQuantile(double quantile)
    {
        double[] buffer = { 1d };
        int nextIndex = 0;

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => SignalProcessing.UpdateCircularQuantile(2d, buffer, ref nextIndex, quantile));
        Assert.AreEqual(1d, buffer[0]);
        Assert.AreEqual(0, nextIndex);
    }

    [TestMethod]
    [DataRow(0d, 0d, 0d, 0d)]
    [DataRow(1d, 0d, 1d, 0d)]
    [DataRow(1d, System.Math.PI / 2d, 0d, 1d)]
    public void MagnitudePhaseToComplex_ConvertsRadians(double magnitude, double phaseRadians, double expectedReal, double expectedImaginary)
    {
        var actual = SignalProcessing.MagnitudePhaseToComplex(magnitude, phaseRadians);

        Assert.AreEqual(expectedReal, actual.Real, 1e-12);
        Assert.AreEqual(expectedImaginary, actual.Imaginary, 1e-12);
    }

    [TestMethod]
    [DataRow(new double[] { 0d }, new double[] { 1d, 2d }, new double[] { 1d, 2d })]
    [DataRow(new double[] { 0d, 1d, 2d }, new double[] { 3d, 4d, 2d }, new double[] { 2d, 3d, 3d })]
    [DataRow(new double[] { 1d, 3d }, new double[] { 4d }, new double[] { 3.5d })]
    public void UpdateMovingMedian_UpdatesCircularWindow(double[] initial, double[] samples, double[] expected)
    {
        double[] buffer = (double[])initial.Clone();
        int nextIndex = 0;

        for (int i = 0; i < samples.Length; i++)
        {
            double actual = SignalProcessing.UpdateMovingMedian(samples[i], buffer, ref nextIndex);
            Assert.AreEqual(expected[i], actual, 1e-12, $"Sample {i}");
            Assert.AreEqual((i + 1) % buffer.Length, nextIndex, $"Index {i}");
        }
    }

    [TestMethod]
    public void UpdateMovingAverage_RejectsInvalidBufferAndIndex()
    {
        double[] empty = Array.Empty<double>();
        int index = 0;
        Assert.ThrowsExactly<ArgumentNullException>(() => SignalProcessing.UpdateMovingAverage(1d, null!, ref index));
        Assert.ThrowsExactly<ArgumentException>(() => SignalProcessing.UpdateMovingAverage(1d, empty, ref index));
        index = 1;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => SignalProcessing.UpdateMovingAverage(1d, new double[1], ref index));
    }

}
