namespace DataAnalysis.Core.Import;

/// <summary>
/// Beschreibt eine feste Spaltenzuordnung: Quelle (Name oder Index als Text) -> Ziel-Feldname.
/// </summary>
public sealed class FixedColumnMapping
{
 /// <summary>
 /// Quellspalte. Entweder Spaltenname (bei Header) oder Index als Text (z. B. "0").
 /// </summary>
 public required string Source { get; init; }

 /// <summary>
 /// Ziel-Feldname in der Ausgabetabelle (z. B. "Timestamp", "Severity", "Source", "Message" oder ein beliebiges Attribut).
 /// </summary>
 public required string Target { get; init; }

    /// <summary>
    /// Gets a value indicating whether the current instance represents a DateTime value.
    /// </summary>
 public bool IsDateTime { get; init; } = false;
}
