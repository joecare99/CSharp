// ***********************************************************************
// Assembly         : AA25_RichTextEdit
// Author           : Mir
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AA25_RichTextEdit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using AA25_RichTextEdit.ViewModels.Interfaces;
using Avalonia.ViewModels;
using Avln_CommonDialogs.Base.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AA25_RichTextEdit.ViewModels;

public partial class RichTextEditViewModel : BaseViewModelCT, IRichTextEditViewModel
{
    private static readonly IReadOnlyList<FileTypeFilter> DocumentFileFilters =
    [
        new("Text documents", ["*.txt"]),
        new("FlowDocument XAML", ["*.xaml"]),
        new("All files", ["*.*"])
    ];

    private readonly IRichTextEditModel _model;
    private readonly IServiceProvider _serviceProvider;

    public DateTime Now => _model.Now;

    [ObservableProperty]
    private string _document = string.Empty; // Avalonia uses plain string for now

    public Action? CloseApp { get; set; }

    // Dialog delegates – set by the View, called by ViewModel commands
    public Func<string, IOpenFileDialog, Action<string, IOpenFileDialog>?, Task<bool?>>? FileOpenDialog { get; set; }
    public Func<string, ISaveFileDialog, Action<string, ISaveFileDialog>?, Task<bool?>>? FileSaveAsDialog { get; set; }
    public Func<IPrintDialog, Action<IPrintDialog, object?>?, Task<bool?>>? dPrintDialog { get; set; }

    public string XamlFileName { get; private set; } = string.Empty;

    // Image asset path adapted for Avalonia (use avares URI)
    public string AllImgSource => $"avares://{Assembly.GetExecutingAssembly().GetName().Name}/Resources/all64_2.png";

    public RichTextEditViewModel(IRichTextEditModel model, IServiceProvider serviceProvider)
    {
        _model = model;
        _serviceProvider = serviceProvider;
        _model.PropertyChanged += OnMPropertyChanged;
        _document = _model.EmptyText; // string = string
    }

    private void OnMPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IRichTextEditModel.Now)) OnPropertyChanged(nameof(Now));
    }

    [RelayCommand]
    private void NewText()
    {
        XamlFileName = string.Empty;
        Document = _model.EmptyText;
    }

    partial void OnDocumentChanged(string value)
    {
        OnPropertyChanged(nameof(Document));
    }

    [RelayCommand]
    private async Task OpenText()
    {
        if (FileOpenDialog is null)
        {
            return;
        }

        var dialog = CreateOpenFileDialog();
        await FileOpenDialog.Invoke(XamlFileName, dialog, (fileName, _) => LoadDocumentFromFile(fileName));
    }

    [RelayCommand]
    private async Task SaveText()
    {
        if (!string.IsNullOrWhiteSpace(XamlFileName))
        {
            SaveDocumentToFile(XamlFileName);
            return;
        }

        if (FileSaveAsDialog is null)
        {
            return;
        }

        var dialog = CreateSaveFileDialog();
        await FileSaveAsDialog.Invoke(GetSuggestedFileName(), dialog, (fileName, _) => SaveDocumentToFile(fileName));
    }

    [RelayCommand]
    private void PrintText()
    {
        // Placeholder: Avalonia printing not implemented
    }

    [RelayCommand]
    private void Exit() => CloseApp?.Invoke();

    private void LoadDocumentFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return;
        }

        using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        Document = _model.DocumentFromStream(fs);
        XamlFileName = fileName;
    }

    private void SaveDocumentToFile(string fileName)
    {
        var directory = Path.GetDirectoryName(fileName);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
        _model.DocumentToStream(fs, Document);
        XamlFileName = fileName;
    }

    private IOpenFileDialog CreateOpenFileDialog()
    {
        var dialog = _serviceProvider.GetRequiredService<IOpenFileDialog>();
        dialog.Title = "Open document";
        dialog.AllowMultiple = false;
        dialog.CheckFileExists = true;
        dialog.DefaultExtension = "txt";

        if (!string.IsNullOrWhiteSpace(XamlFileName))
        {
            dialog.InitialDirectory = Path.GetDirectoryName(XamlFileName);
        }

        AddFilters(dialog.MutableFilters);
        return dialog;
    }

    private ISaveFileDialog CreateSaveFileDialog()
    {
        var dialog = _serviceProvider.GetRequiredService<ISaveFileDialog>();
        dialog.Title = "Save document";
        dialog.DefaultExtension = "txt";
        dialog.AddExtension = true;
        dialog.OverwritePrompt = true;

        if (!string.IsNullOrWhiteSpace(XamlFileName))
        {
            dialog.InitialDirectory = Path.GetDirectoryName(XamlFileName);
        }

        AddFilters(dialog.MutableFilters);
        return dialog;
    }

    private string GetSuggestedFileName()
        => string.IsNullOrWhiteSpace(XamlFileName)
            ? "Document.txt"
            : Path.GetFileName(XamlFileName);

    private static void AddFilters(IList<FileTypeFilter> filters)
    {
        filters.Clear();
        foreach (var filter in DocumentFileFilters)
        {
            filters.Add(filter);
        }
    }
}
