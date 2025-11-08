namespace DataAnalysis.Core.Import.Interfaces;

public interface IDelimitedTableParsingProfile
{
    IList<string> Cultures { get; init; }
    IList<string> TimestampFormats { get; init; }

    // Fixed mappings: Source (name or index as string) -> Target field name
    IList<FixedColumnMapping> FixedColumns { get; init; }

    // Header/format settings
    bool HasHeaderRow { get; init; }
    string TableName { get; init; }
    char Delimiter { get; init; }
    char? Quote { get; init; }
    bool TrimWhitespace { get; init; }

    // Extraction and mapping
    IList<IFieldExtractionRule> ExtractionRules { get; init; }
}