using System;

namespace AA98_AvlnCodeStudio.Config.Secrets;

/// <summary>Indicates that a credential backend is present but its keyring is locked.</summary>
public sealed class CredentialProviderLockedException : InvalidOperationException
{
    /// <summary>Creates a non-secret diagnostic for a locked provider.</summary>
    public CredentialProviderLockedException(string providerId)
        : base($"Credential provider '{providerId}' is locked. Unlock the operating-system keyring and retry.")
    {
        ProviderId = providerId;
    }

    /// <summary>Gets the stable provider ID.</summary>
    public string ProviderId { get; }
}
