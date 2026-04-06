using System;
using System.Collections.Generic;
using GenInterfaces.Data;
using global::GenSecure.Contracts;

namespace GenSecure.Core;

internal sealed class GenealogyManifestRecord
{
    public required string Version { get; init; }

    public required string GenealogyId { get; init; }

    public Guid UId { get; init; }

    public DateTimeOffset UpdatedUtc { get; init; }

    public required List<GenealogyManifestEntry> Persons { get; init; }

    public required List<GenealogyManifestEntry> Families { get; init; }

    public required List<GenealogyManifestEntry> Places { get; init; }
}

internal sealed class GenealogyManifestEntry
{
    public required string RecordId { get; init; }

    public Guid UId { get; init; }

    public StoreMode? StoreMode { get; init; }
}

internal sealed class GenealogyEntityRecord
{
    public required string RecordId { get; init; }

    public required string EntityKind { get; init; }

    public Guid UId { get; init; }

    public int ID { get; init; }

    public DateTime? LastChange { get; init; }

    public required List<GenealogyFactRecord> Facts { get; init; }

    public required List<GenealogyConnectionRecord> Connections { get; init; }
}

internal sealed class GenealogyFactRecord
{
    public required string RecordId { get; init; }

    public Guid UId { get; init; }

    public int ID { get; init; }

    public DateTime? LastChange { get; init; }

    public EFactType FactType { get; init; }

    public string? Data { get; init; }

    public GenealogyDateRecord? Date { get; init; }

    public string? PlaceRecordId { get; init; }

    public required List<GenealogyConnectionRecord> Connections { get; init; }
}

internal sealed class GenealogyConnectionRecord
{
    public Guid UId { get; init; }

    public required string TargetRecordId { get; init; }

    public EGenConnectionType ConnectionType { get; init; }
}

internal sealed class GenealogyDateRecord
{
    public Guid UId { get; init; }

    public int ID { get; init; }

    public DateTime? LastChange { get; init; }

    public EDateModifier DateModifier { get; init; }

    public EDateType DateType1 { get; init; }

    public DateTime Date1 { get; init; }

    public EDateType? DateType2 { get; init; }

    public DateTime? Date2 { get; init; }

    public string? DateText { get; init; }
}

internal sealed class GenealogyPlaceRecord
{
    public required string RecordId { get; init; }

    public Guid UId { get; init; }

    public int ID { get; init; }

    public DateTime? LastChange { get; init; }

    public string? Name { get; init; }

    public string? Type { get; init; }

    public string? GovId { get; init; }

    public double Latitude { get; init; }

    public double Longitude { get; init; }

    public string? Notes { get; init; }

    public string? ParentRecordId { get; init; }
}
