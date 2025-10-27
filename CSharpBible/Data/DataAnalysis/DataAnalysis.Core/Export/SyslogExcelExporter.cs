using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DataAnalysis.Core.Models;

namespace DataAnalysis.Core.Export;

/// <summary>
/// Excel-Exporter für Syslog-Analyseergebnisse. Implementiert die Export-Schnittstelle.
/// </summary>
public sealed class SyslogExcelExporter : ISyslogAnalysisExporter
{
 public Task<string> ExportAsync(SyslogAnalysisResult result, string inputPath, string? outputPath, CancellationToken cancellationToken)
 {
 outputPath ??= BuildDefaultOutputPath(inputPath);
 Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

 using var wb = new XLWorkbook();

 var wsInfo = wb.AddWorksheet("Übersicht");
 wsInfo.Cell(1,1).Value = "Datei"; wsInfo.Cell(1,2).Value = Path.GetFileName(inputPath);
 wsInfo.Cell(2,1).Value = "Zeilen gesamt"; wsInfo.Cell(2,2).Value = result.TotalLines;
 wsInfo.Cell(3,1).Value = "Zeilen geparst"; wsInfo.Cell(3,2).Value = result.ParsedLines;
 wsInfo.Cell(4,1).Value = "Zeilen übersprungen"; wsInfo.Cell(4,2).Value = result.SkippedLines;
 wsInfo.Cell(5,1).Value = "Erster Zeitstempel"; wsInfo.Cell(5,2).Value = result.FirstTimestamp?.LocalDateTime;
 wsInfo.Cell(6,1).Value = "Letzter Zeitstempel"; wsInfo.Cell(6,2).Value = result.LastTimestamp?.LocalDateTime;
 wsInfo.Columns().AdjustToContents();

 var wsSev = wb.AddWorksheet("Schweregrad");
 wsSev.Cell(1,1).Value = "Schwere"; wsSev.Cell(1,2).Value = "Anzahl";
 int r =2;
 foreach (var kv in Enum.GetValues<SyslogSeverity>().Select(s => new { s, c = result.BySeverity.TryGetValue(s, out var v) ? v :0 }))
 {
 wsSev.Cell(r,1).Value = kv.s.ToString();
 wsSev.Cell(r,2).Value = kv.c;
 r++;
 }
 wsSev.Columns().AdjustToContents();

 var wsSrc = wb.AddWorksheet("Quelle");
 wsSrc.Cell(1,1).Value = "Quelle"; wsSrc.Cell(1,2).Value = "Anzahl";
 r =2;
 foreach (var kv in result.BySource.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key))
 {
 wsSrc.Cell(r,1).Value = kv.Key;
 wsSrc.Cell(r,2).Value = kv.Value;
 r++;
 }
 wsSrc.Columns().AdjustToContents();

 var wsHour = wb.AddWorksheet("Stunde");
 wsHour.Cell(1,1).Value = "Stunde"; wsHour.Cell(1,2).Value = "Anzahl";
 r =2;
 foreach (var kv in result.ByHour.OrderBy(kv => kv.Key))
 {
 wsHour.Cell(r,1).Value = kv.Key.LocalDateTime;
 wsHour.Cell(r,2).Value = kv.Value;
 r++;
 }
 wsHour.Columns().AdjustToContents();

 var wsTop = wb.AddWorksheet("Top-Meldungen");
 wsTop.Cell(1,1).Value = "Meldung"; wsTop.Cell(1,2).Value = "Anzahl";
 r =2;
 foreach (var (msg, count) in result.TopMessages)
 {
 wsTop.Cell(r,1).Value = msg;
 wsTop.Cell(r,2).Value = count;
 r++;
 }
 wsTop.Columns().AdjustToContents();

 wb.SaveAs(outputPath);
 return Task.FromResult(outputPath);
 }

 private static string BuildDefaultOutputPath(string inputPath)
 {
 var dir = Path.GetDirectoryName(inputPath)!;
 var name = Path.GetFileNameWithoutExtension(inputPath);
 if (!name.EndsWith("_Auswertung", StringComparison.OrdinalIgnoreCase))
 {
 name += "_Auswertung";
 }
 return Path.Combine(dir, name + ".xlsx");
 }
}
