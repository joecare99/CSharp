using Property.Editor;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Config.UI;

/// <summary>
/// Provides UI-neutral configuration section data and lifecycle operations.
/// </summary>
public interface IConfigUiSection
{
    /// <summary>Gets the localized section metadata.</summary>
    ConfigUiSection Section { get; }

    /// <summary>Gets the editable properties after a successful load.</summary>
    IReadOnlyList<IPropertyItem> Properties { get; }

    /// <summary>Gets whether the host permits mutation of this section.</summary>
    bool IsReadOnly { get; }

    /// <summary>Gets the current asynchronous operation state.</summary>
    ConfigUiSectionState State { get; }

    /// <summary>Gets a localized failure message, if an operation failed.</summary>
    string? ErrorMessage { get; }

    /// <summary>Loads the section's persisted values.</summary>
    Task LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>Saves the currently valid property values.</summary>
    Task SaveAsync(CancellationToken cancellationToken = default);

    /// <summary>Resets persisted values and loads the host defaults.</summary>
    Task ResetAsync(CancellationToken cancellationToken = default);
}
