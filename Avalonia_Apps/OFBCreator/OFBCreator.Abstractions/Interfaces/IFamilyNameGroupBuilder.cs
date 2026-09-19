using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Abstractions.Interfaces;

/// <summary>
/// Contract for building family name groups from a collection of families.
/// </summary>
public interface IFamilyNameGroupBuilder
{
    /// <summary>
    /// Builds family name groups from the provided families.
    /// Groups are keyed by sorted parent surname combinations.
    /// </summary>
    void BuildGroups( IEnumerable<IGenFamily> families );

    /// <summary>
    /// Returns all unique group keys (sorted surnames).
    /// </summary>
    IReadOnlyCollection<string> GroupKeys { get; }

    /// <summary>
    /// Gets families in a specific name group.
    /// </summary>
    IReadOnlyList<IGenFamily>? GetGroup( string groupName );
}
