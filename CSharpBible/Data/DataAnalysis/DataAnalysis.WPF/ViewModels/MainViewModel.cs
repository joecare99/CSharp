using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAnalysis.Core.Models;
using DataAnalysis.Core.Import;
using Microsoft.Win32;

namespace DataAnalysis.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
{
 private readonly SyslogAnalysisModel _model;
 private readonly ITableReader _tableReader;

 [ObservableProperty]
 private string? inputPath;

 [ObservableProperty]
 private string status = "Bereit";

 public ObservableCollection<KeyValuePair<string, string>> SummaryItems { get; } = new();

 private DataView? previewTable;
 public DataView? PreviewTable
 {
 get => previewTable;
 private set => SetProperty(ref previewTable, value);
 }

 public IAsyncRelayCommand AnalyzeCommand { get; }
 public IRelayCommand BrowseCommand { get; }
 public IAsyncRelayCommand PreviewCommand { get; }

 public MainViewModel(SyslogAnalysisModel model, ITableReader tableReader)
 {
 _model = model;
 _tableReader = tableReader;
 AnalyzeCommand = new AsyncRelayCommand(AnalyzeAsync, CanAnalyze);
 PreviewCommand = new AsyncRelayCommand(LoadPreviewAsync, CanAnalyze);
 BrowseCommand = new RelayCommand(Browse);
 PropertyChanged += (_, e) =>
 {
 if (e.PropertyName == nameof(InputPath))
 {
 AnalyzeCommand.NotifyCanExecuteChanged();
 PreviewCommand.NotifyCanExecuteChanged();
 }
 };
 }

 private bool CanAnalyze() => !string.IsNullOrWhiteSpace(InputPath);

 private void Browse()
 {
 var dlg = new OpenFileDialog
 {
 Filter = "Syslog (*.txt)|*.txt|Alle Dateien (*.*)|*.*",
 CheckFileExists = true
 };
 if (dlg.ShowDialog() == true)
 {
 InputPath = dlg.FileName;
 }
 }

 private async Task LoadPreviewAsync()
 {
 if (string.IsNullOrWhiteSpace(InputPath)) return;
 Status = "Lade Vorschau...";
 try
 {
 var table = await _tableReader.ReadTableAsync(InputPath!, CancellationToken.None).ConfigureAwait(false);
 var limited = table.AsEnumerable().Take(200).CopyToDataTableOrEmpty();
 await App.Current.Dispatcher.InvokeAsync(() => { PreviewTable = limited.DefaultView; });
 Status = $"Vorschau: {limited.Rows.Count} Zeilen";
 }
 catch (System.Exception ex)
 {
 Status = $"Fehler (Vorschau): {ex.Message}";
 }
 }

 private async Task AnalyzeAsync()
 {
 if (string.IsNullOrWhiteSpace(InputPath)) return;
 Status = "Analysiere...";
 try
 {
 var output = await _model.AnalyzeAsync(InputPath, null, CancellationToken.None);
 Status = $"Fertig: {output}";
 SummaryItems.Clear();
 SummaryItems.Add(new KeyValuePair<string, string>("Eingabe", InputPath));
 SummaryItems.Add(new KeyValuePair<string, string>("Ausgabe", output));
 }
 catch (System.Exception ex)
 {
 Status = $"Fehler: {ex.Message}";
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
