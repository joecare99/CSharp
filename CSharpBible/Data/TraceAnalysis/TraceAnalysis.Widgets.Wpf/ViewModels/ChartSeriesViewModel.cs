using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Widgets.Wpf.ViewModels;

/// <summary>
/// Represents one plotted chart series and exposes precomputed bounds for rendering.
/// </summary>
public sealed class ChartSeriesViewModel : ViewModelBase
{
    private readonly TraceSeriesModel _series;
    private bool _isVisible = true;

    public ChartSeriesViewModel(TraceSeriesModel series)
    {
        _series = series ?? throw new ArgumentNullException(nameof(series));
        Name = series.Name;
        GroupName = series.GroupName ?? string.Empty;
        FormatText = string.IsNullOrWhiteSpace(series.FormatText) ? "G6" : series.FormatText;
        Points = series.Points;
        MinimumX = Points.Count == 0 ? 0d : Points.Min(static point => point.Time);
        MaximumX = Points.Count == 0 ? 0d : Points.Max(static point => point.Time);
        MinimumY = Points.Count == 0 ? 0d : Points.Min(static point => point.Value);
        MaximumY = Points.Count == 0 ? 0d : Points.Max(static point => point.Value);
    }

    /// <summary>
    /// Gets the stable series name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the optional group name.
    /// </summary>
    public string GroupName { get; }

    /// <summary>
    /// Gets the preferred value format.
    /// </summary>
    public string FormatText { get; }

    /// <summary>
    /// Gets the source sample points.
    /// </summary>
    public IReadOnlyList<TracePointModel> Points { get; }

    /// <summary>
    /// Gets the minimum horizontal value.
    /// </summary>
    public double MinimumX { get; }

    /// <summary>
    /// Gets the maximum horizontal value.
    /// </summary>
    public double MaximumX { get; }

    /// <summary>
    /// Gets the minimum vertical value.
    /// </summary>
    public double MinimumY { get; }

    /// <summary>
    /// Gets the maximum vertical value.
    /// </summary>
    public double MaximumY { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the series is visible in the chart.
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    /// <summary>
    /// Gets a compact point count summary for the UI.
    /// </summary>
    public string PointCountText => string.Format(CultureInfo.InvariantCulture, "{0} samples", Points.Count);

    /// <summary>
    /// Gets the underlying core model.
    /// </summary>
    public TraceSeriesModel ToModel() => _series;
}
