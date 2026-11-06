using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Config.Secrets;

/// <summary>
/// Provides persistent credentials through an explicitly selected secure backend.
/// Implementations must not write secret values to ordinary configuration files or logs.
/// </summary>
public interface ICredentialProvider
{
    /// <summary>Gets the stable provider identifier.</summary>
    string ProviderId { get; }

    /// <summary>Checks availability without prompting for a secret.</summary>
    ValueTask<CredentialProviderStatus> GetStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a credential, or returns null when no value exists.
    /// The caller owns and must dispose a returned secret.
    /// </summary>
    ValueTask<CredentialSecret?> GetAsync(CredentialId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores the credential in this provider.
    /// The secret is borrowed only for the duration of this call; the provider must not retain the instance.
    /// </summary>
    ValueTask SetAsync(
        CredentialId id,
        CredentialSecret secret,
        CancellationToken cancellationToken = default);

    /// <summary>Deletes the credential from this provider; absent entries are a no-op.</summary>
    ValueTask DeleteAsync(CredentialId id, CancellationToken cancellationToken = default);
}
