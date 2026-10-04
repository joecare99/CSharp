namespace OFBCreator.Projects.Models;

/// <summary>
/// Creates provider-qualified rule targets from stable external record identifiers.
/// </summary>
public static class OFBExportRuleTarget
{
    /// <summary>Builds a person target from a provider ID and external person identifier.</summary>
    public static string Person(string providerId, string externalId) =>
        Create(providerId, "person", externalId);

    /// <summary>Builds a family target from a provider ID and external family identifier.</summary>
    public static string Family(string providerId, string externalId) =>
        Create(providerId, "family", externalId);

    /// <summary>Builds the person or family owner target for a fact rule.</summary>
    public static string FactOwner(string providerId, string ownerKind, string externalId)
    {
        if (ownerKind is not ("person" or "family"))
            throw new ArgumentException("A fact owner must be a person or family.", nameof(ownerKind));
        return Create(providerId, ownerKind, externalId);
    }

    private static string Create(string providerId, string kind, string externalId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(externalId);
        return $"{providerId.Trim().ToLowerInvariant()}:{kind}:{externalId.Trim()}";
    }
}
