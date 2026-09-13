using System.Collections.Generic;

namespace Config.UI;

/// <summary>
/// Exposes the configuration sections registered by an application host.
/// </summary>
public interface IConfigUiSectionRegistry
{
    /// <summary>Gets all sections in deterministic display order.</summary>
    IReadOnlyList<IConfigUiSection> Sections { get; }
}
