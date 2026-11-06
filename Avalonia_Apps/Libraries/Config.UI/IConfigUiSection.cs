using Property.Editor;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Config.UI;

/// <summary>
/// Provides UI-neutral configuration section data and lifecycle operations.
/// </summary>
public interface IConfigUiSection : IPropertyProvider
{
    /// <summary>Gets the localized section metadata.</summary>
    ConfigUiSection Section { get; }

    /// <inheritdoc/>
    PropertyProviderContext IPropertyProvider.Context =>
        new(PropertyScope.Unspecified, Section.DisplayName, Section.DisplayName, readOnlyReason: IsReadOnly ? "This configuration section is read-only." : null);

    /// <summary>Gets the current asynchronous operation state.</summary>
    ConfigUiSectionState State { get; }

    /// <summary>Gets a localized failure message, if an operation failed.</summary>
    string? ErrorMessage { get; }

    /// <summary>Saves the currently valid property values.</summary>
    Task SaveAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    Task IPropertyProvider.ApplyAsync(CancellationToken cancellationToken) => SaveAsync(cancellationToken);

    /// <inheritdoc/>
    Task IPropertyProvider.DiscardAsync(CancellationToken cancellationToken) => LoadAsync(cancellationToken);

    /// <summary>Resets persisted values and loads the host defaults.</summary>
    Task ResetAsync(CancellationToken cancellationToken = default);

    /// <summary>Resets only the editable draft to host defaults.</summary>
    Task IPropertyProvider.ResetDraftAsync(CancellationToken cancellationToken) =>
        Task.FromException(new NotSupportedException("This configuration section does not support draft-only reset."));
}
