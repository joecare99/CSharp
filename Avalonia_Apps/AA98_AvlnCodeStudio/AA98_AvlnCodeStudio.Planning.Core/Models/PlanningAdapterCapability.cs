using System;

namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Defines the operations supported by an external planning adapter.
/// </summary>
[Flags]
public enum PlanningAdapterCapability
{
    /// <summary>
    /// Indicates that no external operations are available.
    /// </summary>
    None = 0,

    /// <summary>
    /// Indicates that external planning items can be imported.
    /// </summary>
    Import = 1,

    /// <summary>
    /// Indicates that local planning items can be exported.
    /// </summary>
    Export = 2,
}
