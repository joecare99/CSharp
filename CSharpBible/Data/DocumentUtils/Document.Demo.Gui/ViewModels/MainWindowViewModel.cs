using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Document.Demo.Gui.Models;
using Document.Demo.Gui.Services;

namespace Document.Demo.Gui.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly DocumentDemoExportService _exportService;

    public MainWindowViewModel(DocumentDemoExportService exportService)
    {
        _exportService = exportService;

        OutputFormats =
        [
            new OutputFormatOption("html", "HTML (*.html)", ".html"),
            new OutputFormatOption("pdf", "PDF (*.pdf)", ".pdf"),
            new OutputFormatOption("docx", "Word (*.docx)", ".docx"),
            new OutputFormatOption("odf", "ODT (*.odf)", ".odf"),
            new OutputFormatOption("xaml", "XAML (*.xaml)", ".xaml")
        ];

        Features =
        [
            new FeatureOption(DemoFeature.Headings, "Überschriften", true),
            new FeatureOption(DemoFeature.TableOfContents, "Inhaltsverzeichnis", true),
            new FeatureOption(DemoFeature.FontStyles, "Schriftformate", true),
            new FeatureOption(DemoFeature.FontFamiliesAndColors, "Schriftarten und Farben", true),
            new FeatureOption(DemoFeature.Bookmarks, "Sprungmarken", true),
            new FeatureOption(DemoFeature.Columns, "Spalten (Tabulator-basiert)", true)
        ];

        SelectedOutputFormat = OutputFormats.First();
        OutputFileName = "DocumentDemo";
        OutputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        StatusMessage = "Bereit";
    }

    public ObservableCollection<OutputFormatOption> OutputFormats { get; }

    public ObservableCollection<FeatureOption> Features { get; }

    [ObservableProperty]
    private OutputFormatOption? _selectedOutputFormat;

    [ObservableProperty]
    private string _outputFileName;

    [ObservableProperty]
    private string _outputDirectory;

    [ObservableProperty]
    private string _statusMessage;

    [RelayCommand]
    private void Export()
    {
        var selectedFormat = SelectedOutputFormat;
        if (selectedFormat is null)
        {
            StatusMessage = "Bitte ein Ausgabeformat wählen.";
            return;
        }

        var selectedFeatures = Features
            .Where(feature => feature.IsSelected)
            .Select(feature => feature.Feature)
            .ToArray();

        var fileName = NormalizeFileName(OutputFileName);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            StatusMessage = "Bitte einen gültigen Dateinamen angeben.";
            return;
        }

        var outputPath = Path.Combine(OutputDirectory, EnsureExtension(fileName, selectedFormat.Extension));
        var request = new DemoExportRequest(outputPath, selectedFormat.Key, selectedFeatures);
        var result = _exportService.Export(request);

        StatusMessage = result.Status switch
        {
            DocumentExportStatus.Success => $"Export erfolgreich: {outputPath}",
            DocumentExportStatus.InvalidPath => "Ungültiger Ausgabepfad.",
            DocumentExportStatus.UnknownFormat => "Unbekanntes Ausgabeformat.",
            DocumentExportStatus.SaveFailed => "Dokument konnte nicht gespeichert werden.",
            DocumentExportStatus.Error => $"Fehler: {result.Exception?.Message}",
            _ => "Unbekannter Status."
        };
    }

    public void ApplyOutputPath(string outputPath)
    {
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            return;
        }

        OutputDirectory = Path.GetDirectoryName(outputPath) ?? OutputDirectory;
        OutputFileName = Path.GetFileNameWithoutExtension(outputPath);

        var extension = Path.GetExtension(outputPath);
        if (string.IsNullOrWhiteSpace(extension))
        {
            return;
        }

        var match = OutputFormats.FirstOrDefault(option =>
            string.Equals(option.Extension, extension, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            SelectedOutputFormat = match;
        }
    }

    partial void OnSelectedOutputFormatChanged(OutputFormatOption? value)
    {
        if (value is null || string.IsNullOrWhiteSpace(OutputFileName))
        {
            return;
        }

        OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName);
    }

    private static string EnsureExtension(string fileName, string extension)
    {
        if (fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
        {
            return fileName;
        }

        return fileName + extension;
    }

    private static string NormalizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(fileName.Where(character => !invalidChars.Contains(character)).ToArray()).Trim();
        return cleaned;
    }
}
