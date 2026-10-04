using System;

namespace Genealogy.Models;

/// <summary>
/// Opaque source-provider data retained for fidelity beyond the common model projection.
/// </summary>
public sealed class GenealogyProviderData
{
    public string Provider { get; set; } = string.Empty;

    public string MediaType { get; set; } = "application/octet-stream";

    public byte[] Data { get; set; } = Array.Empty<byte>();

    public string? CanonicalHash { get; set; }
}
