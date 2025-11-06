using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DataAnalysis.Core.Export.Interfaces;
using DataAnalysis.Core.Models;

namespace DataAnalysis.Core.Export;

/// <summary>
/// Excel-Exporter für Analyseergebnisse.
/// </summary>
public sealed class SyslogExcelExporter : ISyslogAnalysisExporter
{
 public Task<string> ExportAsync(AnalysisResult result, string inputPath, string? outputPath, CancellationToken cancellationToken)
 {
 outputPath ??= BuildDefaultOutputPath(inputPath);
 Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

 using var wb = new XLWorkbook();

 var wsInfo = wb.AddWorksheet("Übersicht");
 wsInfo.Cell(1,1).Value = "Datei";
 wsInfo.Cell(1,2).Value = Path.GetFileName(inputPath);
 wsInfo.Cell(2,1).Value = "Zeilen gesamt";
 wsInfo.Cell(2,2).Value = result.TotalLines;
 wsInfo.Cell(3,1).Value = "Zeilen geparst";
 wsInfo.Cell(3,2).Value = result.ParsedLines;
 wsInfo.Cell(4,1).Value = "Zeilen übersprungen";
 wsInfo.Cell(4,2).Value = result.SkippedLines;
 wsInfo.Cell(5,1).Value = "Erster Zeitstempel";
 wsInfo.Cell(5,2).Value = result.FirstTimestamp?.LocalDateTime;
 wsInfo.Cell(6,1).Value = "Letzter Zeitstempel";
 wsInfo.Cell(6,2).Value = result.LastTimestamp?.LocalDateTime;
 if (!string.IsNullOrWhiteSpace(result.GlobalFilterText))
 {
 wsInfo.Cell(7,1).Value = "Globaler Filter";
 wsInfo.Cell(7,2).Value = result.GlobalFilterText;
 wsInfo.Cell(7,2).Style.Font.Italic = true;
 }
 wsInfo.Columns().AdjustToContents();

 // Aggregationen exportieren
 foreach (var agg in result.Aggregations)
 {
 var ws = wb.AddWorksheet(TrimSheetName(agg.Title));
 var hasLocalFilter = !string.IsNullOrWhiteSpace(agg.FilterText);
 int headerRow = hasLocalFilter ?2 :1;

 // Falls lokaler Filter vorhanden, Kommentarzeile oben einfügen
 if (hasLocalFilter)
 {
 ws.Cell(1,1).Value = $"# Filter: {agg.FilterText}";
 ws.Cell(1,1).Style.Font.Italic = true;
 ws.Cell(1,1).Style.Font.FontColor = XLColor.Gray;
 }

 if (agg.Matrix is not null)
 {
 //2D Matrix: write headers from agg.Columns exactly as provided to preserve mapping
 ws.Cell(headerRow,1).Value = "Schlüssel";
 var cols = agg.Columns ?? Array.Empty<string>();
 for (int i =0; i < cols.Count; i++)
 ws.Cell(headerRow,2 + i).Value = cols[i];

 // Rotate column header text90° and center
 if (cols.Count >0)
 {
 var headerRange = ws.Range(headerRow,2, headerRow,1 + cols.Count);
 headerRange.Style.Alignment.TextRotation =90;
 headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
 headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
 headerRange.Style.Font.Bold = true;
 }

 int r = headerRow +1;
 var keys = agg.Matrix.Keys.ToList();
 // Keep Summe as last row if present
 keys.Sort((a, b) => string.Equals(a, "Summe", StringComparison.OrdinalIgnoreCase) ?1 : string.Equals(b, "Summe", StringComparison.OrdinalIgnoreCase) ? -1 : string.Compare(a, b, StringComparison.OrdinalIgnoreCase));
 foreach (var rowKey in keys)
 {
 var row = agg.Matrix[rowKey];
 ws.Cell(r,1).Value = rowKey;
 for (int i =0; i < cols.Count; i++)
 {
 var colKey = cols[i];
 ws.Cell(r,2 + i).Value = row.TryGetValue(colKey, out var v) ? v :0;
 }
 r++;
 }
 }
 else if (agg.Series is not null && agg.Dimensions.Count ==2)
 {
 //2D Punkte (Cluster)
 ws.Cell(headerRow,1).Value = "X";
 ws.Cell(headerRow,2).Value = "Y";
 ws.Cell(headerRow,3).Value = "Anzahl";
 // Bold headers
 ws.Range(headerRow,1, headerRow,3).Style.Font.Bold = true;
 int r = headerRow +1;
 foreach (var kv in agg.Series.OrderByDescending(kv => kv.Value))
 if (kv.Key is Vector2 p)
 {
 ws.Cell(r,1).Value = p.X;
 ws.Cell(r,2).Value = p.Y;
 ws.Cell(r,3).Value = kv.Value;
 r++;
 }
 }
 else if (agg.Series is not null)
 {
 //1D
 ws.Cell(headerRow,1).Value = "Schlüssel";
 ws.Cell(headerRow,2).Value = "Anzahl";
 // Bold headers
 ws.Range(headerRow,1, headerRow,2).Style.Font.Bold = true;
 int r = headerRow +1;
 foreach (var kv in agg.Series.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key))
 { ws.Cell(r,1).Value = kv.Key.ToString(); ws.Cell(r,2).Value = kv.Value; r++; }
 }
 ws.Columns().AdjustToContents();
 }

 wb.SaveAs(outputPath);
 return Task.FromResult(outputPath);
 }

 private static string BuildDefaultOutputPath(string inputPath)
 {
 var dir = Path.GetDirectoryName(inputPath)!;
 var name = Path.GetFileNameWithoutExtension(inputPath);
 if (!name.EndsWith("_Auswertung", StringComparison.OrdinalIgnoreCase))
 name += "_Auswertung";
 return Path.Combine(dir, name + ".xlsx");
 }

 private static string TrimSheetName(string name)
 {
 var invalid = Path.GetInvalidFileNameChars();
 var n = new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
 return n.Length >31 ? n[..31] : n;
 }
}
