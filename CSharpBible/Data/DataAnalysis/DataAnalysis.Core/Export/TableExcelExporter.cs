using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DataAnalysis.Core.Export.Interfaces;

namespace DataAnalysis.Core.Export;

public sealed class TableExcelExporter : ITableExporter
{
    public Task<string> ExportAsync(DataTable table, string inputPath, string? outputPath, CancellationToken cancellationToken = default)
    {
        outputPath ??= BuildDefaultOutputPath(inputPath);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet(string.IsNullOrWhiteSpace(table.TableName) ? "Tabelle" : TrimSheetName(table.TableName));

        // Header
        for (int c = 0; c < table.Columns.Count; c++)
            ws.Cell(1, c + 1).Value = table.Columns[c].ColumnName;
        ws.Range(1, 1, 1, Math.Max(1, table.Columns.Count)).Style.Font.Bold = true;

        // Rows
        int r = 2;
        foreach (DataRow row in table.Rows)
        {
            cancellationToken.ThrowIfCancellationRequested();
            for (int c = 0; c < table.Columns.Count; c++)
                if (table.Columns[c].DataType == typeof(DateTime) && row[c] is DateTime dt)
                {
                    ws.Cell(r, c + 1).Value = dt;
                    ws.Cell(r, c + 1).Style.DateFormat.Format = "yyyy-mm-dd hh:mm:ss.ms";
                }
                else
                    ws.Cell(r, c + 1).Value = row[c]?.ToString() ?? string.Empty;
            r++;
        }

        ws.Columns().AdjustToContents();
        wb.SaveAs(outputPath);
        return Task.FromResult(outputPath);
    }

    private static string BuildDefaultOutputPath(string inputPath)
    {
        var dir = Path.GetDirectoryName(inputPath)!;
        var name = Path.GetFileNameWithoutExtension(inputPath);
        if (!name.EndsWith("_Export", StringComparison.OrdinalIgnoreCase))
            name += "_Export";
        return Path.Combine(dir, name + ".xlsx");
    }

    private static string TrimSheetName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var n = new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        return n.Length > 31 ? n[..31] : n;
    }
}
