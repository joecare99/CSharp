using System;

namespace AA98_AvlnCodeStudio.Config.Secrets;

/// <summary>Reports non-secret availability information for one credential provider.</summary>
public sealed record CredentialProviderStatus
{
    /// <summary>Creates provider status without including credential contents.</summary>
    public CredentialProviderStatus(string providerId, CredentialProviderAvailability availability)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerId);
        if (!string.Equals(providerId, providerId.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("Credential provider IDs must be trimmed.", nameof(providerId));
        }

        if (!Enum.IsDefined(availability))
        {
            throw new ArgumentOutOfRangeException(nameof(availability));
        }

        ProviderId = providerId;
        Availability = availability;
    }

    /// <summary>Gets the stable provider identifier.</summary>
    public string ProviderId { get; }

    /// <summary>Gets the current availability.</summary>
    public CredentialProviderAvailability Availability { get; }
}
