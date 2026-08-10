using System;

namespace RnzTrauer.Places;

public enum CoordinateSchemaStatus
{
    Available,
    Missing,
    Unverified,
}

public sealed record CoordinateSchemaReport(
    CoordinateSchemaStatus Status,
    string[] RequiredColumns,
    string Diagnostic,
    string DiagnosticCode)
{
    public bool CanPersist => Status == CoordinateSchemaStatus.Available;

    public static CoordinateSchemaReport Create(CoordinateSchemaStatus status) =>
        new(
            status,
            ["Orte.Latitude", "Orte.Longitude"],
            status switch
            {
                CoordinateSchemaStatus.Available =>
                    "Coordinate columns are configured for persistence.",
                CoordinateSchemaStatus.Missing =>
                    "Coordinate columns are missing; persistence must remain disabled.",
                CoordinateSchemaStatus.Unverified =>
                    "Coordinate columns have not been verified against the connected schema.",
                _ => throw new ArgumentOutOfRangeException(nameof(status)),
            },
            status switch
            {
                CoordinateSchemaStatus.Available => "schema.available",
                CoordinateSchemaStatus.Missing => "schema.missing_columns",
                CoordinateSchemaStatus.Unverified => "schema.unverified",
                _ => throw new ArgumentOutOfRangeException(nameof(status)),
            });

    public static CoordinateSchemaReport CreateUnverified(
        string diagnostic,
        string diagnosticCode = "schema.probe_failed") =>
        new(
            CoordinateSchemaStatus.Unverified,
            ["Orte.Latitude", "Orte.Longitude"],
            diagnostic,
            diagnosticCode);
}
