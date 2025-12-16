using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataAnalysis.Core.Models;

/// <summary>
/// Serialisierbares DTO für Value- und Gruppen-Filter.
/// "type" diskriminiert den konkreten Filter.
/// </summary>
public abstract class FilterDefinition
{
    [JsonPropertyName("type")] public string Type { get; init; }

    public bool Negate { get; init; } = false;

}

public sealed class ValueFilterDefinition : FilterDefinition
{
    public ValueFilterDefinition() => Type = "value";

    public required string Field { get; init; }
    public required FilterOperator Operator { get; init; }
    public string? Value { get; init; }
    public string? ValueTo { get; init; } // für Between
    public FilterDataType DataType { get; init; } = FilterDataType.String;
    public bool CaseSensitive { get; init; } = false;
    public string? EnumType { get; init; } // optional: vollqualifizierter Typname, wenn DataType==Enum
    public string? Culture { get; init; }
    public string[]? Formats { get; init; }
}

public sealed class GroupFilterDefinition : FilterDefinition
{
    public GroupFilterDefinition() => Type = "group";

    // "and" oder "or"
    public required string Mode { get; init; }
    public required IReadOnlyList<FilterDefinition> Filters { get; init; }
}
