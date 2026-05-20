using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using TraceAnalysis.Workbench.Core.Models;
using TraceAnalysis.Workbench.Core.Services;
using TraceAnalysis.Workbench.Wpf.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents the processing editor state hosted by the shell.
/// </summary>
public partial class ProcessingEditorViewModel : ObservableObject
{
    private readonly IProcessingConfigurationStorage _storage;
    private readonly IFileDialogService _fileDialogService;
    private readonly ObservableCollection<ChannelOptionViewModel> _availableChannels;
    private readonly ObservableCollection<EditableProcessingStepViewModel> _editableSteps;
    private readonly ObservableCollection<TraceSeriesModel> _sourceSeries;
    private string? _currentFilePath;

    public ProcessingEditorViewModel(
        ObservableCollection<ProcessingStepState> steps,
        IProcessingConfigurationStorage storage,
        IFileDialogService fileDialogService)
    {
        _storage = storage;
        _fileDialogService = fileDialogService;
        _availableChannels = new ObservableCollection<ChannelOptionViewModel>();
        _editableSteps = new ObservableCollection<EditableProcessingStepViewModel>();
        _sourceSeries = new ObservableCollection<TraceSeriesModel>();

        foreach (var step in steps)
            _editableSteps.Add(new EditableProcessingStepViewModel(step, _availableChannels));

        RefreshAvailableChannels(); // StepList ist hier noch null

        StepList = new ProcessingStepListViewModel(_editableSteps);
        SelectedStepDetail = new ProcessingStepDetailViewModel(StepList.SelectedStep);
        PreviewSummary = new PreviewSummaryViewModel();
        PreviewSummary.UpdateFromStep(StepList.SelectedStep);
        StatusText = "Configuration has not been saved yet.";

        StepList.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ProcessingStepListViewModel.SelectedStep))
            {
                SynchronizeSelection();
                (RemoveStepCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        };
    }

    /// <summary>
    /// Gets the step list state.
    /// </summary>
    public ProcessingStepListViewModel StepList { get; }

    /// <summary>
    /// Gets the contextual preview summary for the selected step.
    /// </summary>
    public PreviewSummaryViewModel PreviewSummary { get; }

    /// <summary>
    /// Gets the currently available channel options for input selection.
    /// </summary>
    public ObservableCollection<ChannelOptionViewModel> AvailableChannels => _availableChannels;

    [ObservableProperty]
    public partial ProcessingStepDetailViewModel? SelectedStepDetail{ get; set; }

    /// <summary>
    /// Gets the current save/load status text.
    /// </summary>
    [ObservableProperty]
    public partial string StatusText
    {
        get;
        private set ;
    }

    /// <summary>
    /// Synchronizes the detail view with the current step-list selection.
    /// </summary>
    public void SynchronizeSelection()
    {
        SelectedStepDetail?.SetStep(StepList.SelectedStep);
        PreviewSummary.UpdateFromStep(StepList.SelectedStep);
    }

    /// <summary>
    /// Updates the available input channels from the currently loaded trace series.
    /// </summary>
    public void UpdateAvailableChannels(IReadOnlyList<TraceSeriesModel> sourceSeries)
    {
        _sourceSeries.Clear();
        if (sourceSeries != null)
        {
            foreach (var series in sourceSeries)
                _sourceSeries.Add(series);
        }

        RefreshAvailableChannels();
    }

    [RelayCommand]
    private void New()
    {
        _editableSteps.Clear();
        var step = CreateDefaultStep(_availableChannels);
        StepList.AddStep(step);
        RefreshAvailableChannels();
        SynchronizeSelection();
        _currentFilePath = null;
        StatusText = "Created a new configuration.";
    }

    [RelayCommand]
    private void Open()
    {
        var filePath = _fileDialogService.ShowOpenFileDialog();
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        var configuration = _storage.Load(filePath);

        _editableSteps.Clear();
        foreach (var step in configuration.Steps)
            _editableSteps.Add(new EditableProcessingStepViewModel(step, _availableChannels));

        if (_editableSteps.Count == 0)
            _editableSteps.Add(CreateDefaultStep(_availableChannels));

        RefreshAvailableChannels();

        StepList.SelectedStep = _editableSteps[0];
        SynchronizeSelection();
        _currentFilePath = filePath;
        StatusText = $"Loaded configuration from '{filePath}'.";
    }
    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(_currentFilePath))
        {
            SaveAs();
            return;
        }

        SaveToPath(_currentFilePath);
    }
    [RelayCommand]
    private void SaveAs()
    {
        var filePath = _fileDialogService.ShowSaveFileDialog(_currentFilePath);
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        SaveToPath(filePath);
        _currentFilePath = filePath;
    }

    private void SaveToPath(string filePath)
    {
        RefreshAvailableChannels();

        var model = new ProcessingConfigurationModel(
            configurationName: "Workbench Configuration",
            steps: _editableSteps.Select(step => step.ToModel()).ToList());

        _storage.Save(filePath, model);
        StatusText = $"Saved configuration to '{filePath}'.";
    }
    [RelayCommand]
    private void AddStep()
    {
        var step = CreateDefaultStep(_availableChannels);
        StepList.AddStep(step);
        RefreshAvailableChannels();
        SynchronizeSelection();
        StatusText = "Added a new processing step.";
    }
    bool CanRemoveStep() => StepList?.SelectedStep != null;
    [RelayCommand(CanExecute =nameof(CanRemoveStep))]
    private void RemoveStep()
    {
        StepList.RemoveSelectedStep();
        if (_editableSteps.Count == 0)
            StepList.AddStep(CreateDefaultStep(_availableChannels));

        RefreshAvailableChannels();
        SynchronizeSelection();
        StatusText = "Removed the selected processing step.";
    }

    private void RefreshAvailableChannels()
    {
        var selectedChannelName = StepList?.SelectedStep?.Inputs.FirstOrDefault()?.SelectedChannel?.ChannelName;

        _availableChannels.Clear();
        foreach (var channel in _sourceSeries)
        {
            if (!_availableChannels.Any(option => option.ChannelName == channel.Name))
                _availableChannels.Add(new ChannelOptionViewModel(channel.Name, isDerived: false));
        }

        if (_availableChannels.Count == 0)
        {
            foreach (var channel in new[] { "Speed", "Angle", "StatusWord" })
                _availableChannels.Add(new ChannelOptionViewModel(channel, isDerived: false));
        }

        foreach (var step in _editableSteps)
        {
            foreach (var output in step.Outputs)
            {
                if (!string.IsNullOrWhiteSpace(output.ChannelName) && !_availableChannels.Any(channel => channel.ChannelName == output.ChannelName))
                    _availableChannels.Add(new ChannelOptionViewModel(output.ChannelName, isDerived: true));
            }
        }

        if (!string.IsNullOrWhiteSpace(selectedChannelName) && !_availableChannels.Any(channel => channel.ChannelName == selectedChannelName))
            _availableChannels.Add(new ChannelOptionViewModel(selectedChannelName, isDerived: false));
    }

    private static EditableProcessingStepViewModel CreateDefaultStep(ObservableCollection<ChannelOptionViewModel> availableChannels)
    {
        var step = new ProcessingStepState(
            stepId: "step-new",
            operationName: "scale",
            isEnabled: true,
            inputs: [new ProcessingInputState("source", "Speed", sourceStepId: null)],
            parameters: [new ProcessingParameterState("value", "1.0")],
            outputs: [new ProcessingOutputState(null, "SpeedScaled", string.Empty)]);

        return new EditableProcessingStepViewModel(step, availableChannels);
    }
}
