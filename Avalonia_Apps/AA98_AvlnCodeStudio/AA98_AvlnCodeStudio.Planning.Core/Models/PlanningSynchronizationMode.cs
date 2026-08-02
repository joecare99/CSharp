namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Defines the direction used to synchronize planning items.
/// </summary>
public enum PlanningSynchronizationMode
{
    /// <summary>
    /// Indicates that no synchronization direction has been selected.
    /// </summary>
    None,

    /// <summary>
    /// Imports external planning items into the local planning model.
    /// </summary>
    Import,

    /// <summary>
    /// Exports local planning items to an external provider.
    /// </summary>
    Export,
}
