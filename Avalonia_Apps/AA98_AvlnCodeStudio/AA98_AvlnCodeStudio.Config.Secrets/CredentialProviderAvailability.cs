namespace AA98_AvlnCodeStudio.Config.Secrets;

/// <summary>Describes whether a credential provider can currently be used.</summary>
public enum CredentialProviderAvailability
{
    /// <summary>The provider is available for credential operations.</summary>
    Available,

    /// <summary>The provider exists but must be unlocked before use.</summary>
    Locked,

    /// <summary>The provider is not installed or is unavailable on this system.</summary>
    Unavailable,
}
