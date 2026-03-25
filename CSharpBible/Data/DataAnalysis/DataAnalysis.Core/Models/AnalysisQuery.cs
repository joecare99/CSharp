using System.Collections.Generic;

namespace DataAnalysis.Core.Models;

public sealed class AnalysisQuery
{
    // Anzeigename für den Export
    public required string Title { get; init; }

    // Liste der Dimensionen:1D => Serie,2D => Kreuztabelle
    public required IReadOnlyList<DimensionKind> Dimensions { get; init; }

    // Optional: feste Spalten für2D (z. B. Reihenfolge der Severity-Spalten)
    public IReadOnlyList<string>? Columns { get; init; }

    // Optional: TopN-Begrenzung für1D-Serien
    public int? TopN { get; init; }

    // Optional: TopN pro Zeile für2D-Kreuztabellen
    public int? RowTopN { get; init; }

    // Optional: DBSCAN-Clustering der Punkte (bei2D X/Y)
    public bool IsDBScan { get; init; } = false;
    // Optional: Radius (Epsilon) für DBSCAN (gleiche Einheit wie X/Y)
    public double? DbEps { get; init; }
    // Optional: Mindestanzahl Punkte pro Cluster
    public int? DbMinPts { get; init; }

    // Optional: Auswertungsspezifischer Filter (DTO, serialisierbar)
    public FilterDefinition? Filter { get; init; }

    // Optional: Gruppierte Ausgabe (für2D-Tabellen)
    public bool Grouped { get; init; }
}
