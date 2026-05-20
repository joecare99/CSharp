#if NET8_0_OR_GREATER
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TraceAnalysis.Base.Models;
using TraceAnalysis.Base.Models.Interfaces;
using TraceAnalysis.Workbench.Core.Services;

namespace TraceCsv2realCsv.Tests;

[TestClass]
public class TraceSeriesProjectorTests
{
    [TestMethod]
    public void Project_WithNumericFields_CreatesOnlyNumericSeries()
    {
        var projector = new TraceSeriesProjector();
        var dataSet = new TraceDataSet(
            new TraceMetadata(
                "sample",
                new ITraceFieldMetadata[]
                {
                    new TraceFieldMetadata("PLC_PRG.lrAGVPos", typeof(double)),
                    new TraceFieldMetadata("PLC_PRG.lrAGVSpeed", typeof(double)),
                    new TraceFieldMetadata("ModeText", typeof(string))
                }),
            new ITraceRecord[]
            {
                new TraceRecord(0d, new Dictionary<string, object?>
                {
                    ["PLC_PRG.lrAGVPos"] = 714.325d,
                    ["PLC_PRG.lrAGVSpeed"] = 200d,
                    ["ModeText"] = "Auto"
                }),
                new TraceRecord(50d, new Dictionary<string, object?>
                {
                    ["PLC_PRG.lrAGVPos"] = 716.325d,
                    ["PLC_PRG.lrAGVSpeed"] = 201d,
                    ["ModeText"] = "Manual"
                })
            });

        var result = projector.Project(dataSet);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("PLC_PRG.lrAGVPos", result[0].Name);
        Assert.AreEqual(2, result[0].Points.Count);
        Assert.AreEqual(50d, result[1].Points[1].Time);
        Assert.AreEqual(201d, result[1].Points[1].Value);
    }

    [TestMethod]
    public void CalculateStatistics_WithCursorWindow_ReturnsExpectedMetrics()
    {
        var projector = new TraceSeriesProjector();
        var series = projector.Project(
            new TraceDataSet(
                new TraceMetadata("sample", new ITraceFieldMetadata[] { new TraceFieldMetadata("Signal", typeof(double)) }),
                new ITraceRecord[]
                {
                    new TraceRecord(0d, new Dictionary<string, object?> { ["Signal"] = 0d }),
                    new TraceRecord(1d, new Dictionary<string, object?> { ["Signal"] = 2d }),
                    new TraceRecord(2d, new Dictionary<string, object?> { ["Signal"] = 4d })
                }))[0];

        var statistics = projector.CalculateStatistics(series, 0d, 2d);

        Assert.IsNotNull(statistics);
        Assert.AreEqual(3, statistics.SampleCount);
        Assert.AreEqual(0d, statistics.Minimum, 1e-9);
        Assert.AreEqual(4d, statistics.Maximum, 1e-9);
        Assert.AreEqual(2d, statistics.Mean, 1e-9);
        Assert.AreEqual(2d, statistics.Median, 1e-9);
        Assert.AreEqual(8d / 3d, statistics.Variance, 1e-9);
        Assert.AreEqual(4d, statistics.Integral, 1e-9);
        Assert.AreEqual(2d, statistics.Slope, 1e-9);
    }

    [TestMethod]
    public void Project_WithDateTimeTimestamps_UsesOaDateTimeAxis()
    {
        var projector = new TraceSeriesProjector();
        var start = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var end = start.AddSeconds(5);
        var dataSet = new TraceDataSet(
            new TraceMetadata("sample", new ITraceFieldMetadata[] { new TraceFieldMetadata("Signal", typeof(double)) }),
            new ITraceRecord[]
            {
                new TraceRecord(start, new Dictionary<string, object?> { ["Signal"] = 10d }),
                new TraceRecord(end, new Dictionary<string, object?> { ["Signal"] = 20d })
            });

        var result = projector.Project(dataSet);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(start.ToOADate(), result[0].Points[0].Time, 1e-9);
        Assert.AreEqual(end.ToOADate(), result[0].Points[1].Time, 1e-9);
    }
}
#endif
