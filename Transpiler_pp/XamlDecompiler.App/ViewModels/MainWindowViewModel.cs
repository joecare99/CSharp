using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XamlDecompiler.Core.Models;
using XamlDecompiler.Core.Services;

namespace XamlDecompiler.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly GeneratedMauiDecompiler _decompiler;
    private DecompilationResult? _lastResult;

    public MainWindowViewModel(GeneratedMauiDecompiler decompiler)
    {
        _decompiler = decompiler;
        StatusText = "Load a generated .cs file to start the analysis.";
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AnalyzeCommand))]
    private string _loadedFilePath = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AnalyzeCommand))]
    private string _sourceText = string.Empty;

    [ObservableProperty]
    private string _xamlPreview = string.Empty;

    [ObservableProperty]
    private string _codeBehindPreview = string.Empty;

    [ObservableProperty]
    private string _diagnosticsText = string.Empty;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private string _suggestedXamlFileName = "Decompiled.xaml";

    public bool CanSave => _lastResult is not null;

    public async Task LoadSourceFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        LoadedFilePath = filePath;
        SourceText = await File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(true);
        StatusText = $"Loaded '{Path.GetFileName(filePath)}'.";
        Analyze();
    }

    public async Task SaveOutputAsync(string xamlFilePath, CancellationToken cancellationToken = default)
    {
        if (_lastResult is null)
        {
            return;
        }

        string codeBehindPath = xamlFilePath + ".cs";
        await File.WriteAllTextAsync(xamlFilePath, XamlPreview, cancellationToken).ConfigureAwait(true);
        await File.WriteAllTextAsync(codeBehindPath, CodeBehindPreview, cancellationToken).ConfigureAwait(true);
        StatusText = $"Saved '{Path.GetFileName(xamlFilePath)}' and '{Path.GetFileName(codeBehindPath)}'.";
    }

    [RelayCommand(CanExecute = nameof(CanAnalyze))]
    private void Analyze()
    {
        try
        {
            _lastResult = _decompiler.Decompile(SourceText);
            XamlPreview = _lastResult.XamlText;
            CodeBehindPreview = _lastResult.CodeBehindText;
            DiagnosticsText = _lastResult.Diagnostics.Count == 0
                ? "No parser diagnostics."
                : string.Join(Environment.NewLine, _lastResult.Diagnostics);
            SuggestedXamlFileName = Path.GetFileName(_lastResult.XamlFilePath);
            StatusText = $"Analysis completed for '{_lastResult.ClassName}'.";
            OnPropertyChanged(nameof(CanSave));
        }
        catch (Exception ex)
        {
            _lastResult = null;
            XamlPreview = string.Empty;
            CodeBehindPreview = string.Empty;
            DiagnosticsText = ex.ToString();
            StatusText = "Analysis failed.";
            OnPropertyChanged(nameof(CanSave));
        }
    }

    [RelayCommand]
    private void Clear()
    {
        _lastResult = null;
        LoadedFilePath = string.Empty;
        SourceText = string.Empty;
        XamlPreview = string.Empty;
        CodeBehindPreview = string.Empty;
        DiagnosticsText = string.Empty;
        SuggestedXamlFileName = "Decompiled.xaml";
        StatusText = "Workspace cleared.";
        OnPropertyChanged(nameof(CanSave));
    }

    private bool CanAnalyze()
    {
        return !string.IsNullOrWhiteSpace(SourceText);
    }
}
