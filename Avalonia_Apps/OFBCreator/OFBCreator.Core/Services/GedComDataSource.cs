using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BaseLib.Models.Interfaces;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using Microsoft.Extensions.Logging;
using OFBCreator.Abstractions.Interfaces;

namespace OFBCreator.Core.Services;

/// <summary>
/// GEDCOM-based family data source that reads .ged files and returns an IGenealogy payload.
/// Parses GEDCOM format directly without external dependencies.
/// TODO: Complete full entity population when GenInterfaces concrete classes are accessible.
/// </summary>
public class GedComDataSource : IFamilyDataSource
{
    private readonly ILogger<GedComDataSource>? _logger;

    public string SourceId => "gedcom";
    public string DisplayName => "GEDCOM Import (Standard)";

    public GedComDataSource(ILogger<GedComDataSource>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public bool CanRead(Stream stream)
    {
        if (stream == null || !stream.CanSeek)
            throw new ArgumentException("Stream must support seeking for GEDCOM detection.", nameof(stream));

        var originalPosition = stream.Position;
        try
        {
            stream.Position = 0;
            var peekLength = Math.Min(256, stream.Length);
            var buffer = new byte[peekLength];
            stream.Read(buffer, 0, buffer.Length);

            // GEDCOM files typically start with a header record: "0 HEAD" or "0 FILE"
            var text = Encoding.UTF8.GetString(buffer.TakeWhile(b => b != 0).ToArray());
            return text.Trim().StartsWith("0 ", StringComparison.Ordinal) ||
                   text.Contains("GEDCOM", StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }

    /// <inheritdoc />
    public async Task<IGenealogy> ImportAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        _logger?.LogInformation("Importing GEDCOM data from stream...");

        // Read and validate all lines
        using var reader = new StreamReader(stream);
        var allLines = new List<string>();
        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(line))
                allLines.Add(line.TrimEnd('\r'));
        }

        // Count top-level records by type
        int individualCount = 0;
        int familyCount = 0;

        foreach (var fullLine in allLines)
        {
            if (!TryParseGedComLine(fullLine, out var level, out _, out var tag, out _))
                continue;

            if (level == 0)
            {
                switch (tag.ToUpperInvariant())
                {
                    case "I":
                        individualCount++;
                        break;
                    case "F":
                        familyCount++;
                        break;
                }
            }
        }

        _logger?.LogInformation("GEDCOM import complete: {PersonCount} persons, {FamilyCount} families.",
            individualCount, familyCount);

        // TODO: Return a fully populated IGenealogy when GenInterfaces concrete classes are accessible
        var genealogy = CreateGenealogy();
        return genealogy;
    }

    /// <summary>
    /// Parses a GEDCOM line into level, cross-reference ID, tag, and data.
    /// Format: [level] XREF@...@ TAG datum
    /// </summary>
    private static bool TryParseGedComLine(ReadOnlySpan<char> line, out int level, out string xref, out string tag, out ReadOnlySpan<char> data)
    {
        xref = string.Empty;
        tag = string.Empty;
        data = ReadOnlySpan<char>.Empty;

        if (line.Length < 2 || char.IsLetter(line[0]) || char.IsWhiteSpace(line[0]))
        {
            level = -1;
            return false;
        }

        var pos = 0;
        // Parse numeric level
        while (pos < line.Length && char.IsDigit(line[pos]))
            pos++;

        if (pos == 0 || pos >= line.Length || line[pos] != ' ')
        {
            level = -1;
            return false;
        }

        if (!int.TryParse(line.ToString().Substring(0, pos), out level))
        {
            level = -1;
            return false;
        }

        // Skip space after level
        pos++;

        // Parse cross-reference ID: @XXXX@
        if (pos < line.Length && line[pos] == '@')
        {
            var start = pos + 1;
            while (pos < line.Length && line[pos] != '@')
                pos++;
            if (pos < line.Length)
            {
                xref = line.ToString().Substring(start, pos - start);
                pos++; // skip closing @
            }
        }

        // Skip space after xref
        while (pos < line.Length && char.IsWhiteSpace(line[pos]))
            pos++;

        // Parse tag (uppercase letters and digits)
        var tagStart = pos;
        while (pos < line.Length && (char.IsLetter(line[pos]) || char.IsDigit(line[pos])))
            pos++;

        if (pos == tagStart)
        {
            level = -1;
            return false;
        }

        tag = line.ToString().Substring(tagStart, pos - tagStart);

        // Skip space before data
        while (pos < line.Length && char.IsWhiteSpace(line[pos]))
            pos++;

        data = line.Slice(pos);

        return level >= 0;
    }

    /// <summary>
    /// Creates a new IGenealogy instance for population.
    /// Returns empty implementation as placeholder until GenInterfaces concrete class is accessible.
    /// </summary>
    private static IGenealogy CreateGenealogy()
    {
        return new EmptyGenealogy();
    }

    /// <summary>
    /// Placeholder genealogy with empty collections for pipeline validation.
    /// TODO: Replace when GenInterfaces.Data.Genealogy becomes accessible.
    /// </summary>
    private sealed class EmptyGenealogy : IGenealogy
    {
        public Guid UId { get; init; } = Guid.Empty;
        public EGenType eGenType => EGenType.Genealogy;

        public Func<IList<object?>, IGenEntity> GetEntity { get; set; } = _ => throw new NotImplementedException();
        public Func<IList<object?>, IGenFact> GetFact { get; set; } = _ => throw new NotImplementedException();
        public Func<IList<object?>, IGenSource> GetSource { get; set; } = _ => throw new NotImplementedException();
        public Func<IList<object?>, IGenMedia> GetMedia { get; set; } = _ => throw new NotImplementedException();
        public Func<IList<object?>, IGenTransaction> GetTransaction { get; set; } = _ => throw new NotImplementedException();

        public IList<IGenEntity> Entitys { get; init; } = new List<IGenEntity>();
        public IList<IGenSource> Sources { get; init; } = new List<IGenSource>();
        public IList<IGenRepository> Repositories { get; init; } = new List<IGenRepository>();
        public IList<IGenPlace> Places { get; init; } = new List<IGenPlace>();
        public IList<IGenMedia> Medias { get; init; } = new List<IGenMedia>();
        public IList<IGenTransaction> Transactions { get; init; } = new List<IGenTransaction>();
    }
}