using Genealogy.Models;

namespace Genealogy.Drivers;

/// <summary>
/// Result of a genealogy import, including whether recovery affected roundtrip acceptance.
/// </summary>
public sealed class GenealogyImportResult
{
    public GenealogyImportResult(GenealogyDocument document)
    {
        Document = document;
    }

    public GenealogyDocument Document { get; }

    public bool IsRoundTripAccepted => !Document.HasRecoveryIssues;
}
