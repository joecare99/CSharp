using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TraceAnalysis.Workbench.Core.Models;
using TraceAnalysis.Workbench.Core.Services;

namespace TraceAnalysis.Widgets.Wpf.ViewModels;

/// <summary>
/// Represents the interactive trace chart surface and its cursor-driven summary state.
/// </summary>
public sealed class TraceChartViewModel : ViewModelBase
{
    private readonly TraceSeriesProjector _traceSeriesProjector;
    private string _groupFilterText;
    private double _viewStart;
    private double _viewEnd;
    private double _cursorA;
    private double _cursorB;
    private string _chartStatusText;
    private string _horizontalAxisText;
    private string _verticalAxisText;
    private string _cursorModeText;

    public TraceChartViewModel(TraceSeriesProjector traceSeriesProjector)
    {
        _traceSeriesProjector = traceSeriesProjector ?? throw new ArgumentNullException(nameof(traceSeriesProjector));
        Series = new ObservableCollection<ChartSeriesViewModel>();
        CursorValues = new ObservableCollection<TraceCursorValueViewModel>();
        CursorStatistics = new ObservableCollection<TraceCursorStatisticsViewModel>();
        Series.CollectionChanged += OnSeriesCollectionChanged;
        ChartStatusText = "Load a trace to visualize its value series.";
        HorizontalAxisText = "Time";
        VerticalAxisText = "Value";
        GroupFilterText = "All groups";
        CursorModeText = "Dual cursor";
        ShowAllSeriesCommand = new DelegateCommand(ShowAllSeries);
        HideAllSeriesCommand = new DelegateCommand(HideAllSeries);
        ZoomToVisibleDataCommand = new DelegateCommand(ZoomToVisibleData);
        SetCursorsToWindowCommand = new DelegateCommand(SetCursorsToWindow);
    }

    /// <summary>
    /// Gets the projected series shown in the chart and series list.
    /// </summary>
    public ObservableCollection<ChartSeriesViewModel> Series { get; }

    /// <summary>
    /// Gets the nearest cursor values for visible series.
    /// </summary>
    public ObservableCollection<TraceCursorValueViewModel> CursorValues { get; }

    /// <summary>
    /// Gets the calculated statistics for visible series.
    /// </summary>
    public ObservableCollection<TraceCursorStatisticsViewModel> CursorStatistics { get; }

    /// <summary>
    /// Gets the currently visible series.
    /// </summary>
    public IReadOnlyList<ChartSeriesViewModel> VisibleSeries => Series.Where(static series => series.IsVisible).ToArray();

    /// <summary>
    /// Gets the minimum visible horizontal value.
    /// </summary>
    public double VisibleMinimumX => VisibleSeries.Count == 0 ? 0d : VisibleSeries.Min(static series => series.MinimumX);

    /// <summary>
    /// Gets the maximum visible horizontal value.
    /// </summary>
    public double VisibleMaximumX => VisibleSeries.Count == 0 ? 1d : VisibleSeries.Max(static series => series.MaximumX);

    /// <summary>
    /// Gets the minimum visible vertical value.
    /// </summary>
    public double VisibleMinimumY => VisibleSeries.Count == 0 ? 0d : VisibleSeries.Min(static series => series.MinimumY);

    /// <summary>
    /// Gets the maximum visible vertical value.
    /// </summary>
    public double VisibleMaximumY => VisibleSeries.Count == 0 ? 1d : VisibleSeries.Max(static series => series.MaximumY);

    /// <summary>
    /// Gets the horizontal range inside the active cursors.
    /// </summary>
    public double CursorDeltaX => Math.Abs(CursorB - CursorA);

    /// <summary>
    /// Gets the formatted active group filter.
    /// </summary>
    public string GroupFilterText
    {
        get => _groupFilterText;
        set
        {
            if (!SetProperty(ref _groupFilterText, value))
                return;

            RaisePropertyChanged(nameof(GroupSummaryText));
        }
    }

    /// <summary>
    /// Gets a compact group filter summary.
    /// </summary>
    public string GroupSummaryText => string.IsNullOrWhiteSpace(GroupFilterText) ? "All groups" : GroupFilterText;

    /// <summary>
    /// Gets or sets the start of the visible time window.
    /// </summary>
    public double ViewStart
    {
        get => _viewStart;
        set
        {
            if (!SetProperty(ref _viewStart, value))
                return;

            RaisePropertyChanged(nameof(ViewWindowText));
            RebuildDerivedState();
        }
    }

    /// <summary>
    /// Gets or sets the end of the visible time window.
    /// </summary>
    public double ViewEnd
    {
        get => _viewEnd;
        set
        {
            if (!SetProperty(ref _viewEnd, value))
                return;

            RaisePropertyChanged(nameof(ViewWindowText));
            RebuildDerivedState();
        }
    }

    /// <summary>
    /// Gets or sets the first cursor position.
    /// </summary>
    public double CursorA
    {
        get => _cursorA;
        set
        {
            if (!SetProperty(ref _cursorA, value))
                return;

            RaisePropertyChanged(nameof(CursorWindowText));
            RaisePropertyChanged(nameof(CursorDeltaX));
            RebuildDerivedState();
        }
    }

    /// <summary>
    /// Gets or sets the second cursor position.
    /// </summary>
    public double CursorB
    {
        get => _cursorB;
        set
        {
            if (!SetProperty(ref _cursorB, value))
                return;

            RaisePropertyChanged(nameof(CursorWindowText));
            RaisePropertyChanged(nameof(CursorDeltaX));
            RebuildDerivedState();
        }
    }

    /// <summary>
    /// Gets a human-readable window summary.
    /// </summary>
    public string ViewWindowText => string.Format(CultureInfo.InvariantCulture, "Window {0:0.###} .. {1:0.###}", Math.Min(ViewStart, ViewEnd), Math.Max(ViewStart, ViewEnd));

    /// <summary>
    /// Gets a human-readable cursor summary.
    /// </summary>
    public string CursorWindowText => string.Format(CultureInfo.InvariantCulture, "A {0:0.###} | B {1:0.###}", CursorA, CursorB);

    /// <summary>
    /// Gets or sets the chart status text.
    /// </summary>
    public string ChartStatusText
    {
        get => _chartStatusText;
        set => SetProperty(ref _chartStatusText, value);
    }

    /// <summary>
    /// Gets or sets the horizontal axis title.
    /// </summary>
    public string HorizontalAxisText
    {
        get => _horizontalAxisText;
        set => SetProperty(ref _horizontalAxisText, value);
    }

    /// <summary>
    /// Gets or sets the vertical axis title.
    /// </summary>
    public string VerticalAxisText
    {
        get => _verticalAxisText;
        set => SetProperty(ref _verticalAxisText, value);
    }

    /// <summary>
    /// Gets or sets a short cursor mode description.
    /// </summary>
    public string CursorModeText
    {
        get => _cursorModeText;
        set => SetProperty(ref _cursorModeText, value);
    }

    /// <summary>
    /// Gets the command that makes all series visible.
    /// </summary>
    public DelegateCommand ShowAllSeriesCommand { get; }

    /// <summary>
    /// Gets the command that hides all series.
    /// </summary>
    public DelegateCommand HideAllSeriesCommand { get; }

    /// <summary>
    /// Gets the command that zooms the view window to the visible data extent.
    /// </summary>
    public DelegateCommand ZoomToVisibleDataCommand { get; }

    /// <summary>
    /// Gets the command that aligns both cursors to the current view window.
    /// </summary>
    public DelegateCommand SetCursorsToWindowCommand { get; }

    /// <summary>
    /// Loads projected chart series from the current trace source state.
    /// </summary>
    /// <param name="sourceState">The loaded trace source state.</param>
    public void Update(TraceSourceState sourceState)
    {
        foreach (var series in Series)
            series.PropertyChanged -= OnSeriesPropertyChanged;

        Series.Clear();
        CursorValues.Clear();
        CursorStatistics.Clear();

        if (sourceState.Series.Count == 0)
        {
            ChartStatusText = "The loaded trace does not expose numeric series for the chart yet.";
            ViewStart = 0d;
            ViewEnd = 1d;
            CursorA = 0d;
            CursorB = 1d;
            RaiseVisibleBoundsChanged();
            return;
        }

        foreach (var series in sourceState.Series)
        {
            var seriesViewModel = new ChartSeriesViewModel(series);
            seriesViewModel.PropertyChanged += OnSeriesPropertyChanged;
            Series.Add(seriesViewModel);
        }

        ViewStart = VisibleMinimumX;
        ViewEnd = VisibleMaximumX;
        CursorA = ViewStart;
        CursorB = ViewStart + ((ViewEnd - ViewStart) / 2d);
        GroupFilterText = "All groups";
        ChartStatusText = string.Format(CultureInfo.InvariantCulture, "Loaded {0} numeric series.", Series.Count);
        RaiseVisibleBoundsChanged();
        RebuildDerivedState();
    }

    private void ShowAllSeries()
    {
        foreach (var series in Series)
            series.IsVisible = true;

        GroupFilterText = "All groups";
        RaiseVisibleBoundsChanged();
        RebuildDerivedState();
    }

    private void HideAllSeries()
    {
        foreach (var series in Series)
            series.IsVisible = false;

        GroupFilterText = "Hidden";
        RaiseVisibleBoundsChanged();
        RebuildDerivedState();
    }

    private void ZoomToVisibleData()
    {
        if (VisibleSeries.Count == 0)
            return;

        ViewStart = VisibleMinimumX;
        ViewEnd = VisibleMaximumX;
        CursorA = ViewStart;
        CursorB = ViewStart + ((ViewEnd - ViewStart) / 2d);
        RaiseVisibleBoundsChanged();
        RebuildDerivedState();
    }

    private void SetCursorsToWindow()
    {
        CursorA = Math.Min(ViewStart, ViewEnd);
        CursorB = Math.Max(ViewStart, ViewEnd);
        RebuildDerivedState();
    }

    private void OnSeriesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (ChartSeriesViewModel series in e.OldItems)
                series.PropertyChanged -= OnSeriesPropertyChanged;
        }

        if (e.NewItems != null)
        {
            foreach (ChartSeriesViewModel series in e.NewItems)
                series.PropertyChanged += OnSeriesPropertyChanged;
        }
    }

    private void OnSeriesPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ChartSeriesViewModel.IsVisible))
            return;

        GroupFilterText = VisibleSeries.Count == Series.Count ? "All groups" : "Visible selection";
        RaiseVisibleBoundsChanged();
        RebuildDerivedState();
    }

    private void RebuildDerivedState()
    {
        CursorValues.Clear();
        CursorStatistics.Clear();

        var visibleSeries = VisibleSeries;
        if (visibleSeries.Count == 0)
        {
            ChartStatusText = Series.Count == 0
                ? "Load a trace to visualize its value series."
                : "No visible series selected.";
            return;
        }

        var selectionStart = Math.Min(ViewStart, ViewEnd);
        var selectionEnd = Math.Max(ViewStart, ViewEnd);

        foreach (var series in visibleSeries)
        {
            var windowPoints = series.Points
                .Where(point => point.Time >= selectionStart && point.Time <= selectionEnd)
                .ToArray();
            if (windowPoints.Length == 0)
                continue;

            AddCursorValue(series, CursorA, isCursorA: true);
            AddCursorValue(series, CursorB, isCursorA: false);

            var statistics = _traceSeriesProjector.CalculateStatistics(series.ToModel(), CursorA, CursorB);
            if (statistics != null)
                CursorStatistics.Add(new TraceCursorStatisticsViewModel(series.Name, series.GroupName, statistics));
        }

        ChartStatusText = string.Format(
            CultureInfo.InvariantCulture,
            "Visible series: {0} | Cursors span: {1:0.###}",
            visibleSeries.Count,
            CursorDeltaX);
    }

    private void AddCursorValue(ChartSeriesViewModel series, double cursorPosition, bool isCursorA)
    {
        var point = series.Points
            .OrderBy(point => Math.Abs(point.Time - cursorPosition))
            .FirstOrDefault();

        if (point == null)
            return;

        CursorValues.Add(new TraceCursorValueViewModel(series.Name, series.GroupName, point.Time, point.Value, isCursorA));
    }

    private void RaiseVisibleBoundsChanged()
    {
        RaisePropertyChanged(nameof(VisibleSeries));
        RaisePropertyChanged(nameof(VisibleMinimumX));
        RaisePropertyChanged(nameof(VisibleMaximumX));
        RaisePropertyChanged(nameof(VisibleMinimumY));
        RaisePropertyChanged(nameof(VisibleMaximumY));
        RaisePropertyChanged(nameof(ViewWindowText));
        RaisePropertyChanged(nameof(CursorWindowText));
        RaisePropertyChanged(nameof(CursorDeltaX));
    }
}
