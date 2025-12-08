using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DataAnalysis.Core.Export.Interfaces;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace DataAnalysis.Core.Export;

public sealed class TableExcelExporter : ITableExporter
{
    public Task<string> ExportAsync(DataTable table, string inputPath, string? outputPath, CancellationToken cancellationToken = default, Action<double>? progressCallback = null)
    {
        outputPath ??= BuildDefaultOutputPath(inputPath);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet(string.IsNullOrWhiteSpace(table.TableName) ? "Tabelle" : TrimSheetName(table.TableName));

        int totalRows = table.Rows.Count;
        int processedRows = 0;
        DateTime lastReport = DateTime.UtcNow;
        void Report()
        {
            if (progressCallback is null)
                return;
            if ((DateTime.UtcNow - lastReport).TotalSeconds >= 1)
                lock (progressCallback)
                {
                    progressCallback(Math.Clamp(totalRows == 0 ? 1.0 : (double)processedRows / totalRows*0.7d, 0d, 0.7d));
                    lastReport = DateTime.UtcNow;
                }
        }

        // Header
        for (int c = 0; c < table.Columns.Count; c++)
            ws.Cell(1, c + 1).Value = table.Columns[c].ColumnName;
        ws.Range(1, 1, 1, Math.Max(1, table.Columns.Count)).Style.Font.Bold = true;

        // Rows

        Parallel.For(2, table.Rows.Count + 2, (r) =>
        {
            var row = table.Rows[r - 2];
            cancellationToken.ThrowIfCancellationRequested();
            for (int c = 0; c < table.Columns.Count; c++)
                if (table.Columns[c].DataType == typeof(DateTime) && row[c] is DateTime dt)
                {
                    lock (ws)
                    {
                        ws.Cell(r, c + 1).Value = dt;
                        ws.Cell(r, c + 1).Style.DateFormat.Format = "yyyy-mm-dd hh:mm:ss.ms";
                    }
                }
                else
                {
                    var value = ToXLCellValue(row[c]);
                 //   if (r==2)
                   //     value = 
                    lock (ws)
                        ws.Cell(r, c + 1).Value = value;
                }
            processedRows++;
            if (r % 100 == 0)
                Report();
        });

        //   ws.Columns().AdjustToContents();
        progressCallback?.Invoke(0.8); // Abschluss
        wb.SaveAs(outputPath);
        progressCallback?.Invoke(1.0); // Abschluss
        return Task.FromResult(outputPath);
    }

    private void UnParallel_For(int v1, int v2, Action<int> value)
    {
        for (var i = v1; i < v2; i++)
            value(i);
    }

    private XLCellValue ToXLCellValue(object v)
    {
        static bool IsWholeNumber(double d) => Math.Abs(d % 1) < 1e-10;

        if (v is null || v == DBNull.Value)
            return string.Empty;

        if (v is bool b)
            return b ? true : false;

        switch (v)
        {
            case int i:
                return i;
            case long l:
                if (l >= int.MinValue && l <= int.MaxValue)
                    return (int)l;
                return (double)l;
            case short s:
                return (int)s;
            case byte by:
                return (int)by;
            case sbyte sb:
                return (int)sb;
            case uint ui:
                if (ui <= int.MaxValue)
                    return (int)ui;
                return (double)ui;
            case ulong ul:
                if (ul <= (ulong)int.MaxValue)
                    return (int)ul;
                return (double)ul;
            case float f:
                if (float.IsNaN(f) || float.IsInfinity(f))
                    return f.ToString(System.Globalization.CultureInfo.InvariantCulture);
                if (IsWholeNumber(f) && f >= int.MinValue && f <= int.MaxValue)
                    return (int)f;
                return (double)f;
            case double d:
                if (double.IsNaN(d) || double.IsInfinity(d))
                    return d.ToString(System.Globalization.CultureInfo.InvariantCulture);
                if (IsWholeNumber(d) && d >= int.MinValue && d <= int.MaxValue)
                    return (int)d;
                return d;
            case string str:
                {
                    if (str.ToLower() == "true")
                        return true;
                    if (str.ToLower() == "false")
                        return false;

                    str = str.Trim();
                    if (str.Length == 0)
                        return string.Empty;

                    if (int.TryParse(str, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var si))
                    {
                        if (!str.StartsWith('0') || str == "0")
                            return si;
                        else
                            return str;
                    }

                    if (double.TryParse(str, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands,
                                         System.Globalization.CultureInfo.InvariantCulture, out var sd))
                    {
                        if (str.StartsWith('0'))
                            return str;
                        if (IsWholeNumber(sd) && sd >= int.MinValue && sd <= int.MaxValue)
                            return (int)sd;
                        return sd;
                    }

                    return str;
                }
        }

        return v.ToString() ?? string.Empty;
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
