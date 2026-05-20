using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TraceAnalysis.Base.Models.Interfaces;
using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Core.Services;

/// <summary>
/// Projects canonical trace records into numeric series that can be charted by the workbench.
/// </summary>
public sealed class TraceSeriesProjector
{
    /// <summary>
    /// Builds chartable numeric series from the specified canonical trace data set.
    /// </summary>
    /// <param name="dataSet">The canonical trace data set.</param>
    /// <returns>The projected numeric series.</returns>
    public IReadOnlyList<TraceSeriesModel> Project(ITraceDataSet dataSet)
    {
        if (dataSet == null)
            throw new ArgumentNullException(nameof(dataSet));

        var projectedTime = ProjectTimeAxis(dataSet.Records);
        var series = new List<TraceSeriesModel>();

        foreach (var field in dataSet.Metadata.Fields)
        {
            var points = new List<TracePointModel>();
            for (var i = 0; i < dataSet.Records.Count; i++)
            {
                if (!dataSet.Records[i].Values.TryGetValue(field.sName, out var rawValue))
                    continue;

                if (!TryConvertToDouble(rawValue, out var numericValue))
                    continue;

                points.Add(new TracePointModel(projectedTime[i], numericValue));
            }

            if (points.Count == 0)
                continue;

            series.Add(new TraceSeriesModel(field.sName, field.sGroup, field.sFormat, points));
        }

        return series;
    }

    /// <summary>
    /// Calculates statistics for the segment between two cursor positions.
    /// </summary>
    /// <param name="series">The source series.</param>
    /// <param name="cursorStart">The first cursor position.</param>
    /// <param name="cursorEnd">The second cursor position.</param>
    /// <returns>The calculated statistics, or <c>null</c> when no points are inside the segment.</returns>
    public TraceCursorStatisticsModel? CalculateStatistics(TraceSeriesModel series, double cursorStart, double cursorEnd)
    {
        if (series == null)
            throw new ArgumentNullException(nameof(series));

        var start = Math.Min(cursorStart, cursorEnd);
        var end = Math.Max(cursorStart, cursorEnd);
        var selectedPoints = series.Points
            .Where(static point => !double.IsNaN(point.Time) && !double.IsNaN(point.Value))
            .Where(point => point.Time >= start && point.Time <= end)
            .ToArray();

        if (selectedPoints.Length == 0)
            return null;

        var values = selectedPoints.Select(static point => point.Value).OrderBy(static value => value).ToArray();
        var sampleCount = values.Length;
        var mean = values.Average();
        var variance = values.Select(value => Math.Pow(value - mean, 2d)).Average();
        var median = sampleCount % 2 == 0
            ? (values[(sampleCount / 2) - 1] + values[sampleCount / 2]) / 2d
            : values[sampleCount / 2];

        double integral = 0d;
        for (var i = 1; i < selectedPoints.Length; i++)
        {
            var previous = selectedPoints[i - 1];
            var current = selectedPoints[i];
            var deltaTime = current.Time - previous.Time;
            integral += deltaTime * ((previous.Value + current.Value) / 2d);
        }

        var firstPoint = selectedPoints[0];
        var lastPoint = selectedPoints[^1];
        var totalTime = lastPoint.Time - firstPoint.Time;
        var slope = Math.Abs(totalTime) < double.Epsilon
            ? 0d
            : (lastPoint.Value - firstPoint.Value) / totalTime;

        return new TraceCursorStatisticsModel(
            sampleCount,
            values[0],
            values[^1],
            mean,
            median,
            variance,
            integral,
            slope);
    }

    private static double[] ProjectTimeAxis(IReadOnlyList<ITraceRecord> records)
    {
        var projected = new double[records.Count];
        for (var i = 0; i < records.Count; i++)
        {
            projected[i] = TryConvertTimestampToDouble(records[i].Timestamp, out var numericTimestamp)
                ? numericTimestamp
                : i;
        }

        return projected;
    }

    private static bool TryConvertTimestampToDouble(object timestamp, out double value)
    {
        if (timestamp is DateTime dateTime)
        {
            value = dateTime.ToOADate();
            return true;
        }

        if (timestamp is DateTimeOffset dateTimeOffset)
        {
            value = dateTimeOffset.UtcDateTime.ToOADate();
            return true;
        }

        return TryConvertToDouble(timestamp, out value);
    }

    private static bool TryConvertToDouble(object? rawValue, out double value)
    {
        switch (rawValue)
        {
            case null:
                value = default;
                return false;
            case byte byteValue:
                value = byteValue;
                return true;
            case sbyte sbyteValue:
                value = sbyteValue;
                return true;
            case short shortValue:
                value = shortValue;
                return true;
            case ushort ushortValue:
                value = ushortValue;
                return true;
            case int intValue:
                value = intValue;
                return true;
            case uint uintValue:
                value = uintValue;
                return true;
            case long longValue:
                value = longValue;
                return true;
            case ulong ulongValue:
                value = ulongValue;
                return true;
            case float floatValue:
                value = floatValue;
                return true;
            case double doubleValue:
                value = doubleValue;
                return true;
            case decimal decimalValue:
                value = (double)decimalValue;
                return true;
            case string text when double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var parsedValue):
                value = parsedValue;
                return true;
            default:
                if (rawValue is IConvertible convertible)
                {
                    try
                    {
                        value = convertible.ToDouble(CultureInfo.InvariantCulture);
                        return true;
                    }
                    catch (FormatException)
                    {
                    }
                    catch (InvalidCastException)
                    {
                    }
                    catch (OverflowException)
                    {
                    }
                }

                value = default;
                return false;
        }
    }
}
