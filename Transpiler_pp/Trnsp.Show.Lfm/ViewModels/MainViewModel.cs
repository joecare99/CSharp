using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Trnsp.Show.Lfm.Models.Components;
using Trnsp.Show.Lfm.Services;
using Trnsp.Show.Lfm.Services.Interfaces;

namespace Trnsp.Show.Lfm.ViewModels;

/// <summary>
/// Main ViewModel for the LFM Viewer application.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly ILfmParserService _parserService;
    private readonly IComponentFactory _componentFactory;
    private readonly IXamlExporter _xamlExporter;

    [ObservableProperty]
    private string _title = "LFM Viewer";

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "Bereit";

    [ObservableProperty]
    private LfmObjectViewModel? _rootObject;

    [ObservableProperty]
    private LfmObjectViewModel? _selectedObject;

    [ObservableProperty]
    private LfmPropertyViewModel? _selectedProperty;

    [ObservableProperty]
    private string _sourceText = string.Empty;

    [ObservableProperty]
    private LfmComponentBase? _designerRoot;

    [ObservableProperty]
    private LfmComponentBase? _selectedComponent;

    [ObservableProperty]
    private double _zoom = 1.0;

    [ObservableProperty]
    private int _selectedViewIndex;

    public ObservableCollection<LfmObjectViewModel> RootObjects { get; } = [];

    public MainViewModel(ILfmParserService parserService, IComponentFactory componentFactory, IXamlExporter xamlExporter)
    {
        _parserService = parserService;
        _componentFactory = componentFactory;
        _xamlExporter = xamlExporter;
    }

    [RelayCommand]
    private void OpenFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "LFM Files (*.lfm)|*.lfm|All Files (*.*)|*.*",
            Title = "LFM Datei öffnen"
        };

        if (dialog.ShowDialog() == true)
        {
            LoadFile(dialog.FileName);
        }
    }

    [RelayCommand]
    private void LoadFile(string path)
    {
        try
        {
            FilePath = path;
            SourceText = File.ReadAllText(path);
            
            var lfmObject = _parserService.LoadFromFile(path);
            
            RootObjects.Clear();
            
            if (lfmObject != null)
            {
                // TreeView model
                RootObject = LfmObjectViewModel.FromModel(lfmObject);
                if (RootObject != null)
                {
                    RootObjects.Add(RootObject);
                }

                // Visual designer model
                DesignerRoot = _componentFactory.CreateComponentTree(lfmObject);

                StatusMessage = $"Geladen: {Path.GetFileName(path)} - {CountObjects(lfmObject)} Objekte, {CountProperties(lfmObject)} Eigenschaften";
                Title = $"LFM Viewer - {Path.GetFileName(path)}";
            }
            else
            {
                StatusMessage = "Fehler beim Parsen der Datei";
                Title = "LFM Viewer";
                DesignerRoot = null;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Fehler: {ex.Message}";
            RootObject = null;
            RootObjects.Clear();
            DesignerRoot = null;
        }
    }

    [RelayCommand]
    private void ParseText()
    {
        try
        {
            var lfmObject = _parserService.Parse(SourceText);
            
            RootObjects.Clear();
            
            if (lfmObject != null)
            {
                // TreeView model
                RootObject = LfmObjectViewModel.FromModel(lfmObject);
                if (RootObject != null)
                {
                    RootObjects.Add(RootObject);
                }

                // Visual designer model
                DesignerRoot = _componentFactory.CreateComponentTree(lfmObject);

                StatusMessage = $"Geparst: {CountObjects(lfmObject)} Objekte, {CountProperties(lfmObject)} Eigenschaften";
            }
            else
            {
                StatusMessage = "Fehler beim Parsen des Textes";
                DesignerRoot = null;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Fehler: {ex.Message}";
            RootObject = null;
            RootObjects.Clear();
            DesignerRoot = null;
        }
    }

    [RelayCommand]
    private void ExpandAll() => SetExpanded(RootObjects, true);

    [RelayCommand]
    private void CollapseAll() => SetExpanded(RootObjects, false);

    [RelayCommand]
    private void ZoomIn() => Zoom = Math.Min(4.0, Zoom + 0.25);

    [RelayCommand]
    private void ZoomOut() => Zoom = Math.Max(0.25, Zoom - 0.25);

    [RelayCommand]
    private void ZoomReset() => Zoom = 1.0;

    private static void SetExpanded(IEnumerable<LfmObjectViewModel> objects, bool expanded)
    {
        foreach (var obj in objects)
        {
            obj.IsExpanded = expanded;
            SetExpanded(obj.Children, expanded);
        }
    }

    [RelayCommand]
    private void ExportXaml()
    {
        try
        {
            if (DesignerRoot == null)
            {
                StatusMessage = "Kein XAML exportierbar – kein Designerinhalt vorhanden.";
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = "XAML Dateien (*.xaml)|*.xaml|Alle Dateien (*.*)|*.*",
                Title = "XAML exportieren",
                FileName = string.IsNullOrWhiteSpace(FilePath)
                    ? "Export.xaml"
                    : Path.ChangeExtension(Path.GetFileName(FilePath), ".xaml")
            };

            if (saveDialog.ShowDialog() != true)
            {
                return;
            }

            _xamlExporter.ExportToFile(DesignerRoot, saveDialog.FileName);            

            StatusMessage = $"XAML exportiert: {Path.GetFileName(saveDialog.FileName)}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Fehler beim XAML-Export: {ex.Message}";
        }
    }

    private static int CountObjects(TranspilerLib.Pascal.Models.LfmObject obj) => 1 + obj.Children.Sum(CountObjects);

    private static int CountProperties(TranspilerLib.Pascal.Models.LfmObject obj) => obj.Properties.Count + obj.Children.Sum(CountProperties);
}
