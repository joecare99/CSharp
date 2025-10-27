using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DataAnalysis.Core.Import;

/// <summary>
/// Regel, die einen Quell-Text (z. B. Spalte oder Feld) per Regex in benannte Gruppen zerlegt
/// und diese als zusätzliche Attribute bereitstellt. Gruppen können optional umbenannt werden.
/// </summary>
public sealed class FieldExtractionRule : IFieldExtractionRule
{
 /// <summary>
 /// Quelle (Spaltenname, RegEx-Name oder Index als Text, z. B. "0").
 /// </summary>
 public string? SourceColumn { get; init; }

 /// <summary>
 /// Regulärer Ausdruck mit benannten Gruppen.
 /// </summary>
 public required string RegexPattern { get; init; }

 /// <summary>
 /// Optional: Zuordnung von Regex-Gruppennamen zu Attributnamen.
 /// </summary>
 public IDictionary<string, string>? GroupMap { get; init; }

 /// <summary>
 /// RegexOptions (Standard: CultureInvariant | IgnoreCase).
 /// </summary>
 public RegexOptions Options { get; init; } = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
    public bool Multible { get; set; }
}
