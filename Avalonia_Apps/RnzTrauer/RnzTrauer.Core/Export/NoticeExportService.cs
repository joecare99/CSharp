using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RnzTrauer.Core.Domain;
using RnzTrauer.Core.Services;

namespace RnzTrauer.Core.Export;

/// <summary>UTF-8-with-BOM TSV and GEDCOM 5.5.1 writer preserving the original export intent.</summary>
public sealed class NoticeExportService : IExportService
{
    public async Task ExportCsvAsync(string fileName, IReadOnlyCollection<DeathNotice> notices, CancellationToken cancellationToken = default)
    {
        await using var writer = new StreamWriter(fileName, false, new UTF8Encoding(true));
        await writer.WriteLineAsync("idAnzeige\tAuftrag\tNachname\tVorname\tGeburtsname\tGeb\tGest\tBegr\tOrt\tRubrik\tPDF\tPfad");
        foreach (var n in Exportable(notices))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await writer.WriteLineAsync(string.Join('\t', [n.Id, Clean(n.OrderNumber), Clean(n.FamilyName), Clean(n.GivenName), Clean(n.MaidenName), Date(n.BirthDate), Date(n.DeathDate), Date(n.BurialDate), Clean(n.Place), ((int)n.Category).ToString(), Clean(n.PdfFile), Clean(n.Path)]));
        }
    }

    public async Task ExportGedcomAsync(string fileName, IReadOnlyCollection<DeathNotice> notices, CancellationToken cancellationToken = default)
    {
        await using var writer = new StreamWriter(fileName, false, new UTF8Encoding(true));
        await writer.WriteLineAsync("0 HEAD\n1 SOUR RNZ\n2 VERS 1.0.1480\n1 CHAR UTF-8\n1 GEDC\n2 VERS 5.5.1\n2 FORM LINEAGE-LINKED");
        foreach (var n in Exportable(notices))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await writer.WriteLineAsync($"0 @I{n.Id}@ INDI\n1 NAME {Clean(n.GivenName)} /{Clean(n.MaidenName ?? n.FamilyName)}/\n1 REFN {Clean(n.OrderNumber)}");
            if (!string.IsNullOrWhiteSpace(n.Sex)) await writer.WriteLineAsync($"1 SEX {n.Sex}");
            await WriteEventAsync(writer, "BIRT", n.BirthDate, null, cancellationToken);
            await WriteEventAsync(writer, "DEAT", n.DeathDate, n.Place, cancellationToken);
            await WriteEventAsync(writer, "BURI", n.BurialDate, n.Place, cancellationToken);
            if (!string.IsNullOrWhiteSpace(n.Text)) await writer.WriteLineAsync($"1 NOTE {Clean(n.Text)}");
        }
        await writer.WriteAsync("0 TRLR");
    }

    private static async Task WriteEventAsync(StreamWriter writer, string tag, DateTime? date, string? place, CancellationToken cancellationToken)
    {
        if (date is null) return;
        cancellationToken.ThrowIfCancellationRequested();
        await writer.WriteLineAsync($"1 {tag}\n2 DATE {date.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}" + (string.IsNullOrWhiteSpace(place) ? string.Empty : $"\n2 PLAC {Clean(place)}"));
    }

    private static IEnumerable<DeathNotice> Exportable(IEnumerable<DeathNotice> notices) => notices.Where(n => (int)n.Category is > 8040 and < 8090);
    private static string Date(DateTime? value) => value?.ToString("yyyy-MM-dd") ?? string.Empty;
    private static string Clean(string? value) => (value ?? string.Empty).Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
}
