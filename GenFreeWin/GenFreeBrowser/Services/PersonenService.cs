using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GenFreeBrowser.Model;

namespace GenFreeBrowser;

public sealed class PersonenService : IPersonenService
{
    private static readonly Regex IndiStart = new(@"^0 @I(\d+)@ INDI", RegexOptions.Compiled);
    private const string NamePrefix = "1 NAME ";
    private const string BirthDatePrefix = "2 DATE ";
    private const string BirthTag = "1 BIRT";

    private IReadOnlyList<DispPersones>? _cache;

    public Task<IEnumerable<DispPersones>> LadeAlleAsync(CancellationToken ct = default)
        => Task.FromResult<IEnumerable<DispPersones>>(EnsureCache(ct));

    public Task<(IReadOnlyList<DispPersones> Items, int TotalCount)> QueryAsync(PersonenQuery query, CancellationToken ct = default)
    {
        var data = EnsureCache(ct).AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.NameContains))
        {
            var term = query.NameContains.Trim();
            data = data.Where(p => p.Vollname.Contains(term, StringComparison.OrdinalIgnoreCase));
        }
        if (query.BirthYearFrom.HasValue)
            data = data.Where(p => p.GeburtsDatum.HasValue && p.GeburtsDatum.Value.Year >= query.BirthYearFrom.Value);
        if (query.BirthYearTo.HasValue)
            data = data.Where(p => p.GeburtsDatum.HasValue && p.GeburtsDatum.Value.Year <= query.BirthYearTo.Value);
        var total = data.Count();
        var skip = query.PageIndex * query.PageSize;
        var page = data.OrderBy(p => p.Id).Skip(skip).Take(query.PageSize).ToList();
        return Task.FromResult(((IReadOnlyList<DispPersones>)page, total));
    }

    private IReadOnlyList<DispPersones> EnsureCache(CancellationToken ct)
    {
        if (_cache != null) return _cache;
        _cache = LoadFromGedcom(ct).ToList();
        return _cache;
    }

    private IEnumerable<DispPersones> LoadFromGedcom(CancellationToken ct)
    {
        var solutionRoot = FindSolutionRoot();
        var gedPath = Path.Combine(solutionRoot, "WinAhnenNew", "WinAhnenClsTests", "Resources", "Muster_GEDCOM_UTF-8.ged");
        if (!File.Exists(gedPath)) yield break;

        var lines = File.ReadAllLines(gedPath);
        int? currentId = null; string? currentName = null; DateTime? birth = null; bool inBirth = false;
        foreach (var raw in lines)
        {
            ct.ThrowIfCancellationRequested();
            var line = raw.TrimEnd();
            var m = IndiStart.Match(line);
            if (m.Success)
            {
                if (currentId.HasValue)
                    yield return CreatePerson(currentId, currentName, birth);
                currentId = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                currentName = null; birth = null; inBirth = false; continue;
            }
            if (currentId.HasValue)
            {
                if (line.StartsWith(NamePrefix, StringComparison.Ordinal))
                    currentName = line.Substring(NamePrefix.Length).Replace("/", "").Trim();
                else if (line == BirthTag)
                    inBirth = true;
                else if (inBirth && line.StartsWith(BirthDatePrefix, StringComparison.Ordinal))
                {
                    var dateRaw = line.Substring(BirthDatePrefix.Length).Trim();
                    birth = ParseGedcomDate(dateRaw);
                    inBirth = false;
                }
            }
        }
        if (currentId.HasValue)
            yield return CreatePerson(currentId, currentName, birth);
    }

    private static DispPersones CreatePerson(int? id, string? name, DateTime? birth)
        => new(id!.Value, name ?? $"Unbekannt {id}", birth);

    private static DateTime? ParseGedcomDate(string dateRaw)
    {
        var parts = dateRaw.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 3 && int.TryParse(parts[0], out var day) && int.TryParse(parts[2], out var year))
        {
            if (TryMonth(parts[1], out var m))
            {
                try { return new DateTime(year, m, day); } catch { }
            }
        }
        if (parts.Length == 1 && int.TryParse(parts[0], out var y))
        {
            try { return new DateTime(y, 1, 1); } catch { }
        }
        return null;
    }

    private static bool TryMonth(string token, out int month)
    {
        month = token switch
        {
            "JAN" => 1,
            "FEB" => 2,
            "MAR" => 3,
            "APR" => 4,
            "MAY" => 5,
            "JUN" => 6,
            "JUL" => 7,
            "AUG" => 8,
            "SEP" => 9,
            "OCT" => 10,
            "NOV" => 11,
            "DEC" => 12,
            _ => 0
        };
        return month != 0;
    }

    private static string FindSolutionRoot()
    {
        var dir = AppContext.BaseDirectory;
        for (int i = 0; i < 6; i++)
        {
            if (Directory.GetFiles(dir, "*.sln").Length > 0) return dir;
            var parent = Directory.GetParent(dir); if (parent == null) break; dir = parent.FullName;
        }
        return AppContext.BaseDirectory;
    }
}
