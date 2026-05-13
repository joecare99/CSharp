using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Ollama.Tools.ContentAnalysis;
using Ollama.Wpf.TextAnalysis.Services;

namespace Ollama.Wpf.TextAnalysis.ViewModels;

/// <summary>
/// Represents the main content analysis view model.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private readonly IContentAnalysisService _contentAnalysisService;
    private string? _loadedFileName;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AnalyzeTextCommand))]
    private string _inputText = "This is a local WPF text analysis sample. It has enough words and multiple sentences so the UI can display a structured result.";

    [ObservableProperty]
    private ContentAnalysisMode _selectedMode = ContentAnalysisMode.Auto;

    [ObservableProperty]
    private string _selectedModeText = "Auto";

    [ObservableProperty]
    private string _summary = string.Empty;

    [ObservableProperty]
    private string _scoreText = "-";

    [ObservableProperty]
    private string _confidenceText = "-";

    [ObservableProperty]
    private string _rationaleText = string.Empty;

    [ObservableProperty]
    private string _findingsText = string.Empty;

    [ObservableProperty]
    private string _suggestionsText = string.Empty;

    [ObservableProperty]
    private string _statusText = "Ready.";

    [ObservableProperty]
    private string _routingReasonText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AnalyzeTextCommand))]
    private bool _isBusy;

    public MainWindowViewModel(IContentAnalysisService contentAnalysisService)
    {
        _contentAnalysisService = contentAnalysisService ?? throw new ArgumentNullException(nameof(contentAnalysisService));
    }

    private bool CanAnalyzeText() => !IsBusy && !string.IsNullOrWhiteSpace(InputText);

    [RelayCommand(CanExecute = nameof(CanAnalyzeText))]
    private async Task AnalyzeTextAsync()
    {
        try
        {
            IsBusy = true;
            StatusText = SelectedMode switch
            {
                ContentAnalysisMode.CSharp => "Analyzing C# source...",
                ContentAnalysisMode.Text => "Analyzing text...",
                _ => "Detecting content type and analyzing...",
            };

            ContentAnalysisExecutionResult executionResult = await _contentAnalysisService.AnalyzeAsync(InputText, _loadedFileName, SelectedMode);
            ContentAnalysisResult result = executionResult.Result;
            Summary = result.Summary;
            ScoreText = result.Score?.ToString("0.00") ?? "-";
            ConfidenceText = result.Confidence?.ToString("0.00") ?? "-";
            RationaleText = result.Rationale;
            RoutingReasonText = executionResult.Decision.Reason;
            SelectedModeText = executionResult.Decision.AnalysisLabel;
            FindingsText = result.Findings.Count == 0
                ? "No findings."
                : string.Join(Environment.NewLine + Environment.NewLine, result.Findings.Select(static finding => $"[{finding.Severity}] {finding.Title}{Environment.NewLine}{finding.Message}{Environment.NewLine}{finding.Evidence}"));
            SuggestionsText = result.Suggestions.Count == 0
                ? "No suggestions."
                : string.Join(Environment.NewLine + Environment.NewLine, result.Suggestions.Select(static suggestion => $"({suggestion.Priority}) {suggestion.Title}{Environment.NewLine}{suggestion.Description}"));
            StatusText = "Analysis completed.";
        }
        catch (Exception ex)
        {
            StatusText = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadFileAsync()
    {
        OpenFileDialog dialog = new()
        {
            Filter = "Text files|*.txt;*.md;*.log|All files|*.*",
            CheckFileExists = true,
            Multiselect = false,
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        InputText = await File.ReadAllTextAsync(dialog.FileName);
        _loadedFileName = dialog.FileName;
        StatusText = $"Loaded {Path.GetFileName(dialog.FileName)}.";
    }

    [RelayCommand]
    private void SelectAutoMode()
    {
        SelectedMode = ContentAnalysisMode.Auto;
        SelectedModeText = "Auto";
    }

    [RelayCommand]
    private void SelectTextMode()
    {
        SelectedMode = ContentAnalysisMode.Text;
        SelectedModeText = "Text analysis";
    }

    [RelayCommand]
    private void SelectCSharpMode()
    {
        SelectedMode = ContentAnalysisMode.CSharp;
        SelectedModeText = "C# source analysis";
    }
}
