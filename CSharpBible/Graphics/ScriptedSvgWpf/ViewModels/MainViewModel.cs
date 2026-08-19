using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScriptedSvgWpf.Dsl;
using ScriptedSvgWpf.Rendering;
using ScriptedSvgWpf.Samples;
using ScriptedSvgWpf.Services;

namespace ScriptedSvgWpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ScriptInterpreter _interpreter;
    private readonly SvgExporter _svgExporter;
    private readonly IDocumentFileService _fileService;
    private bool _suppressDirtyTracking;
    private string? _currentPath;

    [ObservableProperty]
    private string _scriptText;

    [ObservableProperty]
    private RenderDocument? _previewDocument;

    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private string _errorText = string.Empty;

    [ObservableProperty]
    private bool _isDirty;

    public MainViewModel(
        ScriptInterpreter interpreter,
        SvgExporter svgExporter,
        IDocumentFileService fileService)
    {
        _interpreter = interpreter;
        _svgExporter = svgExporter;
        _fileService = fileService;
        _scriptText = CheckerboardScript.Source;
        Run();
        _isDirty = false;
    }

    public string DocumentName => _currentPath is null ? "Untitled.ssvg" : System.IO.Path.GetFileName(_currentPath);

    partial void OnScriptTextChanged(string value)
    {
        if (!_suppressDirtyTracking)
        {
            IsDirty = true;
        }
    }

    [RelayCommand]
    private void NewDocument()
    {
        SetDocument("canvas(640, 480, \"white\");\n", null);
        Run();
        StatusText = "New document";
    }

    [RelayCommand]
    private void OpenDocument()
    {
        var path = _fileService.ChooseOpenPath();
        if (path is null)
        {
            return;
        }

        try
        {
            SetDocument(_fileService.ReadText(path), path);
            Run();
            StatusText = $"Opened {DocumentName}";
        }
        catch (Exception exception)
        {
            ErrorText = exception.Message;
            StatusText = "Open failed";
        }
    }

    [RelayCommand]
    private void SaveDocument()
    {
        var path = _currentPath ?? _fileService.ChooseSavePath(
            DocumentName,
            "Scripted SVG (*.ssvg)|*.ssvg|Text files (*.txt)|*.txt|All files (*.*)|*.*");
        if (path is null)
        {
            return;
        }

        try
        {
            _fileService.WriteText(path, ScriptText);
            _currentPath = path;
            IsDirty = false;
            OnPropertyChanged(nameof(DocumentName));
            StatusText = $"Saved {DocumentName}";
        }
        catch (Exception exception)
        {
            ErrorText = exception.Message;
            StatusText = "Save failed";
        }
    }

    [RelayCommand]
    private void Run()
    {
        try
        {
            ErrorText = string.Empty;
            PreviewDocument = _interpreter.Execute(ScriptText);
            StatusText = $"Rendered {PreviewDocument.Commands.Count} commands";
        }
        catch (Exception exception)
        {
            PreviewDocument = null;
            ErrorText = ScriptErrorFormatter.Format(ScriptText, exception);
            StatusText = "Render failed";
        }
    }

    [RelayCommand]
    private void ExportSvg()
    {
        if (PreviewDocument is null)
        {
            Run();
        }

        if (PreviewDocument is null)
        {
            return;
        }

        var path = _fileService.ChooseSavePath(
            System.IO.Path.GetFileNameWithoutExtension(DocumentName) + ".svg",
            "SVG files (*.svg)|*.svg|All files (*.*)|*.*");
        if (path is null)
        {
            return;
        }

        try
        {
            _fileService.WriteText(path, _svgExporter.Export(PreviewDocument));
            StatusText = $"Exported {System.IO.Path.GetFileName(path)}";
        }
        catch (Exception exception)
        {
            ErrorText = exception.Message;
            StatusText = "Export failed";
        }
    }

    private void SetDocument(string text, string? path)
    {
        _suppressDirtyTracking = true;
        try
        {
            ScriptText = text;
            _currentPath = path;
            IsDirty = false;
            ErrorText = string.Empty;
            OnPropertyChanged(nameof(DocumentName));
        }
        finally
        {
            _suppressDirtyTracking = false;
        }
    }
}
