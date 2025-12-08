using DataAnalysis.Core.Import.Interfaces;
using DataAnalysis.Core.Models;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading.Channels;

namespace DataAnalysis.Core.Import;

public sealed class DelimitedTableReader : ITableReader
{
    private const string ErrorPathEmpty = "Pfad darf nicht leer sein";
    private const string ErrorInputNotFound = "Eingabedatei nicht gefunden";
    private const string TimestampFormatRoundTrip = "o";

    private readonly IDelimitedTableParsingProfile _profile;

    public DelimitedTableReader(IDelimitedTableParsingProfile profile)
    {
        _profile = profile ?? throw new ArgumentNullException(nameof(profile));
    }

    public async Task<DataTable> ReadTableAsync(string inputPath, CancellationToken cancellationToken = default, Action<double>? progressCallback = null)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
            throw new ArgumentException(ErrorPathEmpty, nameof(inputPath));
        if (!File.Exists(inputPath))
            throw new FileNotFoundException(ErrorInputNotFound, inputPath);

        var dt = new DataTable(_profile.TableName);
        using var fs = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        long totalBytes = fs.Length;
        long processedBytes = 0;
        using var reader = new StreamReader(fs, DetectEncoding(fs) ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

        // Header
        Dictionary<string, int>? headerMap = null;
        var inclFields = new List<int>();

        if (_profile.HasHeaderRow)
        {
            var headerLine = await reader.ReadLineAsync().ConfigureAwait(false);
            if (headerLine is not null)
            {
                processedBytes += Encoding.UTF8.GetByteCount(headerLine) + Environment.NewLine.Length;
                var headers = SplitLine(headerLine, _profile.Delimiter, _profile.Quote, _profile.TrimWhitespace);
                headerMap = headers.Select((h, i) => new { Name = (h ?? string.Empty).Trim(), i })
                    .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First().i, StringComparer.OrdinalIgnoreCase);
                inclFields.AddRange(headerMap.Select((h, i) => i));
                foreach (var h in headerMap)
                {
                    if (_profile.FixedColumns.Any((f) => f.Source == h.Key))
                        inclFields.Remove(h.Value);
                    if (_profile.ExtractionRules.Any((r) => r.SourceColumn == h.Key))
                        inclFields.Remove(h.Value);
                }
            }
        }

        // Ensure core columns exist
        foreach (var mapping in _profile.FixedColumns)
        {
            EnsureColumn(dt, mapping.Target, mapping.IsDateTime);
        }
        if (headerMap is not null)
        {
            foreach (var i in inclFields)
            {
                var hmi = headerMap.Values.FirstOrDefault(v => v == i);
                EnsureColumn(dt, headerMap.Keys.ToArray()[hmi]);
            }
        }

        DateTime lastReport = DateTime.UtcNow;
        void ReportProgress()
        {
            if (progressCallback is null || totalBytes == 0)
                return;
            if ((DateTime.UtcNow - lastReport).TotalSeconds >= 1)
            {
                progressCallback(Math.Clamp((double)processedBytes / totalBytes, 0d, 1d));
                lastReport = DateTime.UtcNow;
            }
        }
        ;

        var headerKeys = headerMap?.Keys.ToArray();


        /*
        // PARALLELE VERARBEITUNG (Producer / Consumer)
        var channel = Channel.CreateBounded<string?>(new BoundedChannelOptions(2048)
        {
            SingleWriter = true,
            SingleReader = false,
            FullMode = BoundedChannelFullMode.Wait
        });

        int workerCount = Math.Max(1, Environment.ProcessorCount);

        var consumers = Enumerable.Range(0, workerCount).Select(_ => Task.Run(async () =>
        {
            await foreach (var line in channel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
            {
                var flowControl = ProcessLine(dt, headerMap, inclFields, headerKeys, line);
                if (!flowControl)
                {
                    continue;
                }

            }
        }, cancellationToken)).ToArray();

        // Producer liest Datei
        string? readLine;
        while ((readLine = await reader.ReadLineAsync().ConfigureAwait(false)) is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            processedBytes += Encoding.UTF8.GetByteCount(readLine) + Environment.NewLine.Length;
            ReportProgress();
            await channel.Writer.WriteAsync(readLine, cancellationToken).ConfigureAwait(false);
        }
        channel.Writer.Complete();

        await Task.WhenAll(consumers).ConfigureAwait(false);
        */
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            processedBytes += Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length;
            ReportProgress();
            var flowControl = ProcessLine(dt, headerMap, inclFields, headerKeys, line);
            if (!flowControl)
            {
                continue;
            }
        }

        progressCallback?.Invoke(1.0);
        return dt;
    }

    private bool ProcessLine(DataTable dt, Dictionary<string, int>? headerMap, List<int> inclFields, string[]? headerKeys, string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return false;

        var fields = SplitLine(line, _profile.Delimiter, _profile.Quote, _profile.TrimWhitespace);

        // extraction rules
        var attributes = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        ApplyExtractionRules(fields, headerMap, attributes);

        // grow columns for attributes if needed
        if (attributes.Count > 0)
        {
            foreach (var key in attributes.Keys)
            {
                if (!_profile.FixedColumns.Any((f) => f.Source == key))
                    EnsureColumn(dt, key);
            }
        }

        // DataRow füllen
        var rowValues = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (var mapping in _profile.FixedColumns)
        {
            var value = Extract(mapping, fields, headerMap, attributes);
            rowValues[mapping.Target] = value ?? string.Empty;
        }

        if (headerMap is not null && headerKeys is not null)
        {
            foreach (var iField in inclFields)
            {
                if (fields.Count > iField)
                {
                    var hmi = headerMap.Values.FirstOrDefault(v => v == iField);
                    var colName = headerKeys[hmi];
                    rowValues[colName] = fields[iField] ?? string.Empty;
                }
            }
        }

        foreach (var kv in attributes)
        {
            if (!_profile.FixedColumns.Any((f) => f.Source == kv.Key))
                rowValues[kv.Key] = kv.Value ?? string.Empty;
        }


        lock (dt)
        {
            var row = dt.NewRow();
            foreach (var kv in rowValues)
                row[kv.Key] = kv.Value ?? string.Empty;

            dt.Rows.Add(row);
        }
        return true;
    }

    private void ApplyExtractionRules(IReadOnlyList<string?> fields, Dictionary<string, int>? headerMap, Dictionary<string, string?> attributes)
    {
        if (_profile.ExtractionRules.Count == 0)
            return;
        foreach (var rule in _profile.ExtractionRules)
        {
            var xFlag = true;
            do
            {
                string? sourceText = null;
                if (!string.IsNullOrWhiteSpace(rule.SourceColumn))
                {
                    if (int.TryParse(rule.SourceColumn, out var idx) && idx >= 0 && idx < fields.Count)
                        sourceText = fields[idx];
                    else if (headerMap is not null && headerMap.TryGetValue(rule.SourceColumn, out var col) && col >= 0 && col < fields.Count)
                        sourceText = fields[col];
                    else if (attributes.ContainsKey(rule.SourceColumn))
                        sourceText = attributes[rule.SourceColumn];
                }
                if (string.IsNullOrEmpty(sourceText))
                    break;
                var rx = new Regex(rule.RegexPattern, rule.Options);
                var m = rx.Match(sourceText);
                if (!m.Success)
                    break;
                foreach (var gName in rx.GetGroupNames())
                {
                    if (int.TryParse(gName, out _))
                        continue;

                    var attrName = rule.GroupMap is not null && rule.GroupMap.TryGetValue(gName, out var mapped) ? mapped : gName;
                    if (attrName.ToLower() == "value" && m.Groups.ContainsKey("key"))
                        attrName = m.Groups["key"].Value;
                    if (attrName.ToLower() != "key")
                        attributes[attrName] = m.Groups[gName].Success ? m.Groups[gName].Value : null;
                }
                xFlag = rule.Multible;
            }
            while (xFlag);
        }
    }

    private static void EnsureColumn(DataTable dt, string name, bool IsDateTime = false)
    {
        lock (dt)
            if (!dt.Columns.Contains(name))
                dt.Columns.Add(name, IsDateTime ? typeof(DateTime) : typeof(string));
    }

    private string Extract(
        FixedColumnMapping mapping,
        IReadOnlyList<string?> fields,
        Dictionary<string, int>? headerMap,
        IReadOnlyDictionary<string, string?>? attributes = null)
    {
        int? ResolveFieldIndex(string? selector)
        {
            if (string.IsNullOrWhiteSpace(selector))
                return null;
            if (int.TryParse(selector, out var idx))
                return idx;
            if (headerMap is null)
                return null;
            return headerMap.TryGetValue(selector, out var col) ? col : null;
        }

        string? GetByIndex(int? idx) => idx is >= 0 && idx < fields.Count ? fields[idx.Value] : null;

        string? GetBySelector(string? selector, out int? usedIndex)
        {
            usedIndex = ResolveFieldIndex(selector);
            if (usedIndex is not null)
                return GetByIndex(usedIndex);

            if (!string.IsNullOrWhiteSpace(selector) && attributes is not null && attributes.TryGetValue(selector, out var value))
                return value;

            return null;
        }

        var tsRaw = GetBySelector(mapping.Source, out _);
        return tsRaw;
    }

    private DateTimeOffset? ParseTimestamp(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;
        raw = raw.Trim().Trim('"').Trim('[', ']');
        foreach (var cultureName in _profile.Cultures)
        {
            var culture = string.IsNullOrEmpty(cultureName) ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(cultureName);
            if (_profile.TimestampFormats.Count > 0 && DateTimeOffset.TryParseExact(raw, _profile.TimestampFormats.ToArray(), culture, DateTimeStyles.AssumeLocal, out var dto))
                return dto;
            if (DateTimeOffset.TryParse(raw, culture, DateTimeStyles.AssumeLocal, out dto))
                return dto;
        }
        return null;
    }

    private static List<string?> SplitLine(string line, char delimiter, char? quote, bool trim)
    {
        var result = new List<string?>();
        var sb = new StringBuilder();
        bool inQuotes = false;
        char q = quote ?? '\0';
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (quote.HasValue && c == q)
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == q)
                {
                    sb.Append(q);
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
                continue;
            }
            if (!inQuotes && c == delimiter)
            {
                var field = sb.ToString();
                result.Add(trim ? field.Trim() : field);
                sb.Clear();
                continue;
            }
            sb.Append(c);
        }
        var last = sb.ToString();
        result.Add(trim ? last.Trim() : last);
        return result;
    }

    private static Encoding? DetectEncoding(FileStream fs)
    {
        var original = fs.Position;
        Span<byte> bom = stackalloc byte[4];
        fs.Position = 0;
        int read = fs.Read(bom);
        fs.Position = original;
        if (read >= 3 && bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
            return Encoding.UTF8;
        if (read >= 2 && bom[0] == 0xFF && bom[1] == 0xFE)
            return Encoding.Unicode;
        if (read >= 2 && bom[0] == 0xFE && bom[1] == 0xFF)
            return Encoding.BigEndianUnicode;
        if (read >= 4 && bom[0] == 0xFF && bom[1] == 0xFE && bom[2] == 0x00 && bom[3] == 0x00)
            return Encoding.UTF32;
        if (read >= 4 && bom[0] == 0x00 && bom[1] == 0x00 && bom[2] == 0xFE && bom[3] == 0xFF)
            return new UTF32Encoding(bigEndian: true, byteOrderMark: true);
        return null;
    }
}
