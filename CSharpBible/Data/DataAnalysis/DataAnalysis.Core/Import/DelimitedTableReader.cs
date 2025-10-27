using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DataAnalysis.Core.Models;

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

    public async Task<DataTable> ReadTableAsync(string inputPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(inputPath)) throw new ArgumentException(ErrorPathEmpty, nameof(inputPath));
        if (!File.Exists(inputPath)) throw new FileNotFoundException(ErrorInputNotFound, inputPath);

        var dt = new DataTable(_profile.TableName);
        using var fs = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fs, DetectEncoding(fs) ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

        // Header
        Dictionary<string,int>? headerMap = null;
        if (_profile.HasHeaderRow)
        {
            var headerLine = await reader.ReadLineAsync().ConfigureAwait(false);
            if (headerLine is not null)
            {
                var headers = SplitLine(headerLine, _profile.Delimiter, _profile.Quote, _profile.TrimWhitespace);
                headerMap = headers.Select((h,i)=> new { Name = (h ?? string.Empty).Trim(), i})
                .GroupBy(x=>x.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g=>g.Key, g=>g.First().i, StringComparer.OrdinalIgnoreCase);
            }
        }

        // Ensure core columns exist
        foreach (var mapping in _profile.FixedColumns)
        {
            EnsureColumn(dt, mapping.Target);
        }

        string? line;
        while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line)) continue;
            var fields = SplitLine(line, _profile.Delimiter, _profile.Quote, _profile.TrimWhitespace);

            // extraction rules
            var attributes = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            ApplyExtractionRules(fields, headerMap, attributes);

            // grow columns for attributes if needed
            foreach (var key in attributes.Keys)
            {
                if (!_profile.FixedColumns.Any((f) => f.Source == key))
                    EnsureColumn(dt, key);
            }

            var row = dt.NewRow();

            foreach (var mapping in _profile.FixedColumns)
                row[mapping.Target] = Extract(mapping, fields, headerMap, attributes);
            
            foreach (var kv in attributes)
                if (!_profile.FixedColumns.Any((f)=> f.Source == kv.Key))
                row[kv.Key] = kv.Value ?? string.Empty;
            dt.Rows.Add(row);
        }

        return dt;
    }

    private void ApplyExtractionRules(IReadOnlyList<string?> fields, Dictionary<string,int>? headerMap, Dictionary<string,string?> attributes)
    {
        if (_profile.ExtractionRules.Count ==0) return;
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

    private static void EnsureColumn(DataTable dt, string name)
    {
        if (!dt.Columns.Contains(name)) dt.Columns.Add(name, typeof(string));
    }

    // helpers reused
    private string Extract(
    FixedColumnMapping mapping, 
    IReadOnlyList<string?> fields,
    Dictionary<string, int>? headerMap,
    IReadOnlyDictionary<string, string?>? attributes = null)
{
    // Hilfsfunktionen
    int? ResolveFieldIndex(string? selector)
    {
        if (string.IsNullOrWhiteSpace(selector)) return null;
        if (int.TryParse(selector, out var idx)) return idx;
        if (headerMap is null) return null;
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

    // Werte �ber fields (Index/Header) ODER attributes extrahieren
    var tsRaw = GetBySelector(mapping.Source, out var _);

    // Normalisierung/Parsing
    if (mapping.IsDateTime)
       tsRaw = ParseTimestamp(tsRaw)?.ToString(TimestampFormatRoundTrip) ?? "";
   

    return tsRaw;
}

    private DateTimeOffset? ParseTimestamp(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        raw = raw.Trim().Trim('"').Trim('[', ']');
        foreach (var cultureName in _profile.Cultures)
        {
            var culture = string.IsNullOrEmpty(cultureName) ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(cultureName);
            if (_profile.TimestampFormats.Count >0 && DateTimeOffset.TryParseExact(raw, _profile.TimestampFormats.ToArray(), culture, DateTimeStyles.AssumeLocal, out var dto))
                return dto;
            if (DateTimeOffset.TryParse(raw, culture, DateTimeStyles.AssumeLocal, out dto)) return dto;
        }
        return null;
    }

    private static List<string?> SplitLine(string line, char delimiter, char? quote, bool trim)
    {
        var result = new List<string?>();
        var sb = new StringBuilder();
        bool inQuotes = false;
        char q = quote ?? '\0';
        for (int i=0; i<line.Length; i++)
        {
            char c = line[i];
            if (quote.HasValue && c == q)
            {
                if (inQuotes && i +1 < line.Length && line[i +1] == q)
                { sb.Append(q); i++; }
                else { inQuotes = !inQuotes; }
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
        fs.Position =0;
        int read = fs.Read(bom);
        fs.Position = original;
        if (read >=3 && bom[0] ==0xEF && bom[1] ==0xBB && bom[2] ==0xBF) return Encoding.UTF8;
        if (read >=2 && bom[0] ==0xFF && bom[1] ==0xFE) return Encoding.Unicode;
        if (read >=2 && bom[0] ==0xFE && bom[1] ==0xFF) return Encoding.BigEndianUnicode;
        if (read >=4 && bom[0] ==0xFF && bom[1] ==0xFE && bom[2] ==0x00 && bom[3] ==0x00) return Encoding.UTF32;
        if (read >=4 && bom[0] ==0x00 && bom[1] ==0x00 && bom[2] ==0xFE && bom[3] ==0xFF) return new UTF32Encoding(bigEndian: true, byteOrderMark: true);
        return null;
    }
}
