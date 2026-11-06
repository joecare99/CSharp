using System.Threading;
using System.Threading.Tasks;

namespace Config.UI;

/// <summary>
/// Optional section contract for discarding drafts shared across multiple providers.
/// </summary>
public interface IConfigUiDraftDiscard
{
    /// <summary>Gets the identity shared by providers participating in the same editable draft.</summary>
    object DraftIdentity { get; }

    /// <summary>Reloads persisted values and discards the associated shared draft.</summary>
    Task DiscardDraftAsync(CancellationToken cancellationToken = default);
}
