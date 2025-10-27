
using System.Text.RegularExpressions;

namespace DataAnalysis.Core.Import;

public interface IFieldExtractionRule
{
    string? SourceColumn { get; init; }
    string RegexPattern { get; init; }
    IDictionary<string, string>? GroupMap { get; init; }
    RegexOptions Options { get; init; }
    bool Multible { get; }
}