// ***********************************************************************
// Assembly         : AA25_RichTextEdit
// Author           : Mir
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BaseLib.Helper;
using AA25_RichTextEdit.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using AA25_RichTextEdit.ViewModels.Interfaces;
using Avalonia.ViewModels;
using AvRichTextBox;

namespace AA25_RichTextEdit.ViewModels;

public partial class RichTextEditViewModel : BaseViewModelCT, IRichTextEditViewModel
{
    private readonly IRichTextEditModel _model;

    public DateTime Now => _model.Now;

    [ObservableProperty]
    private FlowDocument _document; // Avalonia uses plain string for now

    public Action? CloseApp { get; set; }

    public string XamlFileName { get; private set; } = string.Empty;

    // Image asset path adapted for Avalonia (use avares URI)
    public string AllImgSource => $"avares://{Assembly.GetExecutingAssembly().GetName().Name}/Resources/all64_2.png";

    public RichTextEditViewModel() : this(IoC.GetRequiredService<IRichTextEditModel>()) { }

    public RichTextEditViewModel(IRichTextEditModel model)
    {
        _model = model;
        _model.PropertyChanged += OnMPropertyChanged;
        _document = _model.EmptyText;
    }

    private void OnMPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IRichTextEditModel.Now)) OnPropertyChanged(nameof(Now));
    }

    [RelayCommand]
    private void NewText() => Document = _model.EmptyText;

    [RelayCommand]
    private void OpenText()
    {
        // Simplified: open via File stream using stored filename
        if (string.IsNullOrWhiteSpace(XamlFileName)) return;
        if (File.Exists(XamlFileName))
        {
            using var fs = new FileStream(XamlFileName, FileMode.Open);
            Document = _model.DocumentFromStream(fs);
        }
    }

    [RelayCommand]
    private void SaveText()
    {
        if (string.IsNullOrWhiteSpace(XamlFileName)) return;
        using var fs = new FileStream(XamlFileName, FileMode.Create);
        _model.DocumentToStream(fs, Document);
    }

    [RelayCommand]
    private void PrintText()
    {
        // Placeholder: Avalonia printing not implemented
    }

    [RelayCommand]
    private void Exit() => CloseApp?.Invoke();
}
