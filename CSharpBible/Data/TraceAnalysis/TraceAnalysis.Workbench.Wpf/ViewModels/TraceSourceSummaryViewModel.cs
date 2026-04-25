using System.Collections.ObjectModel;
using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents the loaded trace source summary and derived data basis shown by the shell.
/// </summary>
public sealed class TraceSourceSummaryViewModel : ViewModelBase
{
    private string _sourceText;
    private string _parseErrorsText;
    private string _timeBaseText;

    public TraceSourceSummaryViewModel(TraceSourceState sourceState)
    {
        _sourceText = string.IsNullOrWhiteSpace(sourceState.SourceId)
            ? "No trace source loaded"
            : sourceState.SourceId;
        _parseErrorsText = sourceState.ParseErrorCount == 0
            ? "No parse errors"
            : $"Parse errors: {sourceState.ParseErrorCount}";
        _timeBaseText = sourceState.DataBasis?.TimeBaseText ?? "No data basis loaded";

        DataBasisItems = new ObservableCollection<TraceDataBasisItemViewModel>();
        if (sourceState.DataBasis != null)
        {
            foreach (var item in sourceState.DataBasis.Items)
                DataBasisItems.Add(new TraceDataBasisItemViewModel(item));
        }
    }

    /// <summary>
    /// Gets or sets the source summary text.
    /// </summary>
    public string SourceText
    {
        get => _sourceText;
        set => SetProperty(ref _sourceText, value);
    }

    /// <summary>
    /// Gets or sets the parse error summary text.
    /// </summary>
    public string ParseErrorsText
    {
        get => _parseErrorsText;
        set => SetProperty(ref _parseErrorsText, value);
    }

    /// <summary>
    /// Gets or sets the time-base text.
    /// </summary>
    public string TimeBaseText
    {
        get => _timeBaseText;
        set => SetProperty(ref _timeBaseText, value);
    }

    /// <summary>
    /// Gets the derived structural items.
    /// </summary>
    public ObservableCollection<TraceDataBasisItemViewModel> DataBasisItems { get; }

    /// <summary>
    /// Replaces the current source summary state.
    /// </summary>
    public void Update(TraceSourceState sourceState)
    {
        SourceText = string.IsNullOrWhiteSpace(sourceState.SourceId)
            ? "No trace source loaded"
            : sourceState.SourceId;
        ParseErrorsText = sourceState.ParseErrorCount == 0
            ? "No parse errors"
            : $"Parse errors: {sourceState.ParseErrorCount}";
        TimeBaseText = sourceState.DataBasis?.TimeBaseText ?? "No data basis loaded";

        DataBasisItems.Clear();
        if (sourceState.DataBasis != null)
        {
            foreach (var item in sourceState.DataBasis.Items)
                DataBasisItems.Add(new TraceDataBasisItemViewModel(item));
        }
    }
}
