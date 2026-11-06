using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Property.Editor;

/// <summary>
/// Provides editable properties and draft lifecycle operations independently of a host UI.
/// </summary>
public interface IPropertyProvider
{
    /// <summary>Gets the provider's target, scope, and storage description.</summary>
    PropertyProviderContext Context { get; }

    /// <summary>Gets properties in the current editable draft.</summary>
    IReadOnlyList<IPropertyItem> Properties { get; }

    /// <summary>Gets whether this provider disallows mutation.</summary>
    bool IsReadOnly { get; }

    /// <summary>Loads the current values into the provider draft.</summary>
    Task LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>Validates and persists the current draft.</summary>
    Task ApplyAsync(CancellationToken cancellationToken = default);

    /// <summary>Reloads persisted values and discards the current draft.</summary>
    Task DiscardAsync(CancellationToken cancellationToken = default);

    /// <summary>Resets only the current draft to provider defaults.</summary>
    Task ResetDraftAsync(CancellationToken cancellationToken = default) =>
        Task.FromException(new System.NotSupportedException("This property provider does not support draft-only reset."));
}
