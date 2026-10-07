using System.Collections.Generic;

namespace AhnWin52Backup.Core.Workflows;

/// <summary>Versioned declarative description of the AhnWin from-scratch structure.</summary>
public sealed record StructureTemplateManifest(
    int SchemaVersion,
    string TemplateId,
    string SourceDescription,
    string Encoding,
    uint EncryptionKey,
    int IndexMaximumTableSize,
    IReadOnlyList<StructureTemplateAsset> Assets,
    IReadOnlyList<StructureTemplateTable> Tables);
