using System.Collections.Generic;

namespace Osb.Core.Media;

/// <summary>
/// Abstraction for enumerating picture records relevant to the legacy Bildaus routine.
/// Implementations live in the UI/OSB assembly and translate the GenFree recordset model
/// into <see cref="BildausEntry"/> instances.
/// </summary>
public interface IBildausRepository
{
    /// <summary>
    /// Enumerates picture entries for the given owner number and kind (person/family).
    /// </summary>
    IEnumerable<BildausEntry> EnumerateEntries(BildausKind kind, int ownerNumber);
}
