using System.Threading;
using System.Threading.Tasks;

namespace Config.UI;

/// <summary>
/// Optional section contract for discarding drafts shared across multiple providers.
/// </summary>
public interface IConfigUiDraftDiscard
{
    /// <summary>
    /// Gets the stable object identity shared by providers participating in the same editable
    /// draft. Consumers compare identities by reference, not by value.
    /// </summary>
    object DraftIdentity { get; }

    /// <summary>
    /// Reloads persisted values and discards the associated shared draft. A failed operation
    /// does not guarantee that the shared draft is unchanged; consumers must not treat
    /// other sections as refreshed until a subsequent discard succeeds.
    /// </summary>
    Task DiscardDraftAsync(CancellationToken cancellationToken = default);
}
