using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAnalysis.Core.Models;
using DataAnalysis.Core.Import;
using Microsoft.Win32;
using DataAnalysis.WPF.Properties;
using DataAnalysis.Core.Export;

namespace DataAnalysis.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly AnalysisModel _model;
    private readonly ITableReader _tableReader;
    private readonly ISyslogAnalysisExporter _exporter;
    private AnalysisResult? _lastResult;
    private string? _lastInputPath;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AnalyzeCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviewCommand))]
    private string? inputPath;

    [ObservableProperty]
    private string status = Resources.StatusReady;

    [ObservableProperty]
    private AnalysisResultViewModel? analysisVM;

    public ObservableCollection<KeyValuePair<string, string>> SummaryItems { get; } = new();

    private DataView? previewTable;
    public DataView? PreviewTable
    {
        get => previewTable;
        private set => SetProperty(ref previewTable, value);
    }

    public MainViewModel(AnalysisModel model, ITableReader tableReader, ISyslogAnalysisExporter exporter)
    {
        _model = model;
        _tableReader = tableReader;
        _exporter = exporter;
    }

    private bool CanAnalyze() => !string.IsNullOrWhiteSpace(InputPath);
    private bool CanExport() => _lastResult is not null && !string.IsNullOrWhiteSpace(_lastInputPath);

    [RelayCommand]
    private void Browse()
    {
        var dlg = new OpenFileDialog
        {
            Filter = Resources.DialogFilter,
            CheckFileExists = true
        };
        if (dlg.ShowDialog() == true)
        {
            InputPath = dlg.FileName;
        }
    }

    [RelayCommand(CanExecute = nameof(CanAnalyze))]
    private async Task Preview()
    {
        if (string.IsNullOrWhiteSpace(InputPath)) return;
        Status = Resources.StatusLoadingPreview;
        try
        {
            var table = await _tableReader.ReadTableAsync(InputPath!, CancellationToken.None).ConfigureAwait(false);
            var limited = table.AsEnumerable().Take(200).CopyToDataTableOrEmpty();
            await App.Current.Dispatcher.InvokeAsync(() => { PreviewTable = limited.DefaultView; });
            Status = string.Format(Resources.StatusPreviewFmt, limited.Rows.Count);
        }
        catch (System.Exception ex)
        {
            Status = string.Format(Resources.StatusPreviewErrorFmt, ex.Message);
        }
    }

    [RelayCommand(CanExecute = nameof(CanAnalyze))]
    private async Task Analyze()
    {
        if (string.IsNullOrWhiteSpace(InputPath)) return;
        Status = Resources.StatusAnalyzing;
        try
        {
            var result = await _model.AnalyzeAsync(InputPath!, CancellationToken.None);
            _lastResult = result; _lastInputPath = InputPath;
            await App.Current.Dispatcher.InvokeAsync(() => { AnalysisVM = new AnalysisResultViewModel(result); });
            Status = Resources.StatusReady;
            SummaryItems.Clear();
            SummaryItems.Add(new KeyValuePair<string, string>(Resources.AppTitle, InputPath!));
            SummaryItems.Add(new KeyValuePair<string, string>("Total", result.TotalLines.ToString()));
            SummaryItems.Add(new KeyValuePair<string, string>("Parsed", result.ParsedLines.ToString()));
            ExportCommand.NotifyCanExecuteChanged();
        }
        catch (System.Exception ex)
        {
            Status = string.Format(Resources.StatusErrorFmt, ex.Message);
        }
    }

    [RelayCommand(CanExecute = nameof(CanExport))]
    private async Task Export()
    {
        if (_lastResult is null || string.IsNullOrWhiteSpace(_lastInputPath)) return;
        Status = "Exporting...";
        try
        {
            var output = await _exporter.ExportAsync(_lastResult, _lastInputPath!, null, CancellationToken.None);
            Status = string.Format(Resources.StatusDoneFmt, output);
        }
        catch (System.Exception ex)
        {
            Status = string.Format(Resources.StatusErrorFmt, ex.Message);
        }
    }
}

internal static class DataTableExtensions
{
    public static DataTable CopyToDataTableOrEmpty(this System.Collections.Generic.IEnumerable<DataRow> rows)
    {
        using var e = rows.GetEnumerator();
        if (!e.MoveNext()) return new DataTable();
        var dt = e.Current!.Table.Clone();
        dt.ImportRow(e.Current);
        while (e.MoveNext()) dt.ImportRow(e.Current!);
        return dt;
    }
}
