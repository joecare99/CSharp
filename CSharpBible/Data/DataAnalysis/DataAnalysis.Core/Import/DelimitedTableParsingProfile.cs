using System;
using System.Collections.Generic;
using DataAnalysis.Core.Models;

namespace DataAnalysis.Core.Import;

/// <summary>
/// Beschreibt eine tabellarische (CSV/TSV) Log-Datei und die Spaltenzuordnung.
/// </summary>
public sealed class DelimitedTableParsingProfile : IDelimitedTableParsingProfile
{
    private const string cDefaultTableName = "SysLogTable";

    public string TableName { get; init; } = cDefaultTableName;
    public bool HasHeaderRow { get; init; } = true;
    public char Delimiter { get; init; } = ';';
    public char? Quote { get; init; } = '"';
    public bool TrimWhitespace { get; init; } = true;

    // Feste Zuordnungen: Quelle (Name oder Index als Text) -> Ziel-Feldname
    public IList<FixedColumnMapping> FixedColumns { get; init; } = new List<FixedColumnMapping>();

    public IList<string> Cultures { get; init; } = new List<string> { "", "de-DE", "en-US" };
    public IList<string> TimestampFormats { get; init; } = new List<string>
    {
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd HH:mm:ss,fff",
        "yyyy-MM-dd HH:mm:ss.fff",
        "dd.MM.yyyy HH:mm:ss",
        "dd.MM.yyyy HH:mm:ss,fff",
        "dd.MM.yyyy HH:mm:ss.fff",
        "yy-MM-dd HH:mm:ss",
        "yy-MM-dd HH:mm:ss,fff",
        "yy-MM-dd HH:mm:ss.fff",
        "yyyy-MM-ddTHH:mm:ssK",
        "yyyy-MM-ddTHH:mm:ss.fffK",
        "yyyy-MM-dd HH:mm:ssK",
        "yyyy-MM-dd HH:mm:ss.fffK",
    };

    public IList<IFieldExtractionRule> ExtractionRules { get; init; } = new List<IFieldExtractionRule>();
}
