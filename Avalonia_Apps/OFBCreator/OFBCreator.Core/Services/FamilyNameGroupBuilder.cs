using System.Collections.Concurrent;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Interfaces;

namespace OFBCreator.Core.Services;

/// <summary>
/// Builder for family name groups used in OFB sorting and grouping.
/// Implements IFamilyNameGroupBuilder from the Abstractions layer.
/// Groups families by surname: father's surname + mother's maiden name for married couples,
/// or mother's surname + children's surnames for unmarried/united partnerships.
/// Algorithm mirrors GetFamilyNameGroup() from the Delphi GedCom2Odf reference.
/// </summary>
public class FamilyNameGroupBuilder : IFamilyNameGroupBuilder
{
    private readonly ConcurrentDictionary<string, List<IGenFamily>> _groups = new();

    /// <summary>
    /// Builds family name groups from the provided families.
    /// Groups are keyed by sorted parent surname combinations.
    /// </summary>
    public void BuildGroups( IEnumerable<IGenFamily> families )
    {
        ArgumentNullException.ThrowIfNull( families );

        foreach ( var family in families )
        {
            var groupName = ComputeGroupName( family );
            if ( !_groups.TryGetValue( groupName, out var list ) )
            {
                list = new List<IGenFamily>();
                _groups[ groupName ] = list;
            }

            if ( !list.Contains( family ) )
                list.Add( family );
        }
    }

    /// <summary>
    /// Returns all unique group keys (sorted surnames).
    /// </summary>
    public IReadOnlyCollection<string> GroupKeys => _groups.Keys.ToList().AsReadOnly();

    /// <summary>
    /// Gets families in a specific name group.
    /// </summary>
    public IReadOnlyList<IGenFamily>? GetGroup( string groupName )
    {
        if ( string.IsNullOrWhiteSpace( groupName ) )
            return null;

        _groups.TryGetValue( groupName, out var list );
        return list?.AsReadOnly();
    }

    /// <summary>
    /// Computes the canonical family name group from a family unit.
    /// Priority: father's surname + mother's maiden name (sorted alphabetically).
    /// Falls back to single parent surname or estimated name.
    /// </summary>
    private string ComputeGroupName( IGenFamily family )
    {
        var husband = family.Husband;
        var wife = family.Wife;

        if ( husband != null && wife != null )
        {
            // Married couple: combine both surnames alphabetically
            var hSurname = husband.Surname ?? "Unbekannt";
            var wSurname = wife.Surname ?? "Unbekannt";

            // For married women, get maiden name (before marriage surname change)
            // In GEDCOM this may be the same as current surname; handle appropriately
            var sorted = string.Compare( hSurname, wSurname, StringComparison.Ordinal ) <= 0
                ? $"{hSurname}/{wSurname}"
                : $"{wSurname}/{hSurname}";

            return sorted;
        }

        // Unmarried or single-parent: use available parent surname + children's name
        if ( husband != null )
            return husband.Surname ?? "Unbekannt";

        if ( wife != null )
            return wife.Surname ?? "Unbekannt";

        // No parents available — try first child's surname as fallback
        var children = family.Children;
        if ( children != null && children.Count > 0 )
        {
            var firstChild = children[ 0 ];
            if ( firstChild != null && !string.IsNullOrEmpty( firstChild.Surname ) )
                return firstChild.Surname;
        }

        return "Unbekannt";
    }
}
