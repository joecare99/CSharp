using System;
using System.IO;
using TraceAnalysis.Workbench.Core.Services;
using TraceAnalysis.Widgets.Wpf.ViewModels;
using TraceAnalysis.Show.Wpf.Services;

namespace TraceAnalysis.Show.Wpf.ViewModels;

/// <summary>
/// Coordinates trace loading and chart display for the lightweight quick viewer.
/// </summary>
public sealed class TraceViewerViewModel : ViewModelBase
{
    private readonly ITraceSourceLoader _traceSourceLoader;
    private readonly ITraceViewerFileDialogService _fileDialogService;
    private string _windowTitle;
    private string _statusText;
    private bool _isOpenTraceAreaVisible;

    public TraceViewerViewModel(ITraceSourceLoader traceSourceLoader, TraceChartViewModel traceChart, ITraceViewerFileDialogService fileDialogService)
    {
        _traceSourceLoader = traceSourceLoader;
        _fileDialogService = fileDialogService;
        TraceChart = traceChart;
        OpenTraceCommand = new DelegateCommand(OpenTrace);
        _windowTitle = "Trace Analysis Quick Viewer";
        _statusText = "Open a trace file to start the quick analysis view.";
        _isOpenTraceAreaVisible = true;
    }

    /// <summary>
    /// Gets the chart state displayed by the viewer window.
    /// </summary>
    public TraceChartViewModel TraceChart { get; }

    /// <summary>
    /// Gets the quick open command.
    /// </summary>
    public DelegateCommand OpenTraceCommand { get; }

    /// <summary>
    /// Gets the current window title.
    /// </summary>
    public string WindowTitle
    {
        get => _windowTitle;
        private set => SetProperty(ref _windowTitle, value);
    }

    /// <summary>
    /// Gets the current status text.
    /// </summary>
    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    /// <summary>
    /// Gets a value indicating whether the open-trace area is visible.
    /// </summary>
    public bool IsOpenTraceAreaVisible
    {
        get => _isOpenTraceAreaVisible;
        private set => SetProperty(ref _isOpenTraceAreaVisible, value);
    }

    /// <summary>
    /// Loads an initial trace path if one was supplied on startup.
    /// </summary>
    public void LoadInitialTrace(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        if (!File.Exists(filePath))
        {
            StatusText = $"Trace file '{filePath}' was not found.";
            IsOpenTraceAreaVisible = true;
            return;
        }

        LoadTrace(filePath);
    }

    private void OpenTrace()
    {
        var filePath = _fileDialogService.ShowOpenTraceFileDialog();
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        LoadTrace(filePath);
    }

    private void LoadTrace(string filePath)
    {
        var sourceState = _traceSourceLoader.Load(filePath);
        TraceChart.Update(sourceState);
        WindowTitle = $"Trace Analysis Quick Viewer - {Path.GetFileName(filePath)}";
        StatusText = sourceState.Series.Count == 0
            ? $"Loaded '{filePath}', but no numeric series are available for plotting."
            : $"Loaded '{filePath}' with {sourceState.Series.Count} plotted series.";
        IsOpenTraceAreaVisible = false;
    }
}
