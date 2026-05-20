using System.Collections.ObjectModel;
using System.Windows.Input;
using TraceAnalysis.Workbench.Core.Models;
using TraceAnalysis.Workbench.Core.Services;
using TraceAnalysis.Widgets.Wpf.ViewModels;
using TraceAnalysis.Workbench.Wpf.Services;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents the shell-level view model for the first WPF workbench baseline.
/// </summary>
public sealed class MainWorkbenchViewModel
{
    private readonly ITraceSourceLoader _traceSourceLoader;
    private readonly IFileDialogService _fileDialogService;
    private readonly IWorkbenchMenuService _menuService;
    private readonly TraceSeriesProjector _traceSeriesProjector;
    private WorkbenchContextKind _activeContext;

    public MainWorkbenchViewModel(
        WorkbenchSessionModel session,
        ITraceSourceLoader traceSourceLoader,
        TraceSeriesProjector traceSeriesProjector,
        IProcessingConfigurationStorage storage,
        IWorkbenchMenuService menuService,
        IFileDialogService fileDialogService)
    {
        _traceSourceLoader = traceSourceLoader;
        _traceSeriesProjector = traceSeriesProjector;
        _fileDialogService = fileDialogService;
        _menuService = menuService;
        _activeContext = WorkbenchContextKind.None;

        ConfigurationName = session.ConfigurationName;

        var channels = new ObservableCollection<TraceChannelItem>(session.Channels);
        var sourceIssues = new ObservableCollection<ValidationIssue>(session.Diagnostics.SourceIssues);
        var validationIssues = new ObservableCollection<ValidationIssue>(session.Diagnostics.ValidationIssues);

        Steps = new ObservableCollection<ProcessingStepState>(session.Steps);
        SourceSummary = new TraceSourceSummaryViewModel(session.TraceSource);
        ChannelBrowser = new ChannelBrowserViewModel(channels);
        Diagnostics = new DiagnosticsPanelViewModel(sourceIssues, validationIssues);
        ProcessingEditor = new ProcessingEditorViewModel(Steps, storage, fileDialogService);
        TraceChart = new TraceChartViewModel(_traceSeriesProjector);
        TraceChart.Update(session.TraceSource);
        LoadTraceCommand = new DelegateCommand(LoadTrace);
        MainMenu = _menuService.CreateMenu(this);
    }

    /// <summary>
    /// Gets the current configuration name.
    /// </summary>
    public string ConfigurationName { get; }

    /// <summary>
    /// Gets the configured processing steps.
    /// </summary>
    public ObservableCollection<ProcessingStepState> Steps { get; }

    /// <summary>
    /// Gets the trace source summary state.
    /// </summary>
    public TraceSourceSummaryViewModel SourceSummary { get; }

    /// <summary>
    /// Gets the channel browser state.
    /// </summary>
    public ChannelBrowserViewModel ChannelBrowser { get; }

    /// <summary>
    /// Gets the main menu state.
    /// </summary>
    public MainMenuViewModel MainMenu { get; }

    /// <summary>
    /// Gets the diagnostics surface state.
    /// </summary>
    public DiagnosticsPanelViewModel Diagnostics { get; }

    /// <summary>
    /// Gets the processing editor state.
    /// </summary>
    public ProcessingEditorViewModel ProcessingEditor { get; }

    /// <summary>
    /// Gets the chart visualization state.
    /// </summary>
    public TraceChartViewModel TraceChart { get; }

    /// <summary>
    /// Gets the command that loads a trace file into the shell.
    /// </summary>
    public ICommand LoadTraceCommand { get; }

    /// <summary>
    /// Gets or sets the active shell context for menu adaptation.
    /// </summary>
    public WorkbenchContextKind ActiveContext
    {
        get => _activeContext;
        set
        {
            _activeContext = value;
            _menuService.ApplyContext(MainMenu, value);
        }
    }

    private void LoadTrace()
    {
        var filePath = _fileDialogService.ShowOpenTraceFileDialog();
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        var sourceState = _traceSourceLoader.Load(filePath);
        SourceSummary.Update(sourceState);
        ChannelBrowser.UpdateFromSeries(sourceState.Series);
        ProcessingEditor.UpdateAvailableChannels(sourceState.Series);
        TraceChart.Update(sourceState);
    }
}
