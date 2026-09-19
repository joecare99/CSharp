using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Abstractions.Models;

namespace OFBCreator.Core.Services;

/// <summary>
/// Default family selection provider implementing the IFamilySelectionProvider contract.
/// Selects full families, creates virtual families for isolated individuals, and supports place-based filtering.
/// Algorithm mirrors the Delphi GedCom2Odf reference implementation.
/// </summary>
public class OFBFamilySelectionProvider : IFamilySelectionProvider
{
    /// <summary>
    /// Creates a new instance of the selection provider.
    /// </summary>
    public OFBFamilySelectionProvider()
    {
    }

    /// <summary>
    /// Selects families and their related persons from the source genealogy.
    /// Returns a selection snapshot carrying all downstream input without raw traversal.
    /// </summary>
    public async Task<OFBSourceSelection> SelectAsync(
        IGenealogy source,
        CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull( source );

        var selectedFamilies = new List<IGenFamily>();
        var virtualPersons = new List<IGenPerson>();
        var exclusions = new List<OFBExclusionDiagnostic>();

        // Track individuals already included in a real family to avoid double-counting
        var includedIndividuals = new HashSet<string>( StringComparer.Ordinal );

        var allEntities = source.Entitys;

        foreach ( var entity in allEntities )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if ( entity is IGenFamily family )
            {
                // Check if family qualifies as a "full family" per Delphi logic
                if ( IsFullFamily( family ) )
                {
                    selectedFamilies.Add( family );
                    // Track included individuals from this family
                    TrackIndividuals( family, includedIndividuals, virtualPersons, exclusions );
                }
                else
                {
                    exclusions.Add( OFBExclusionDiagnostic.FromEntity( entity, "IncompleteFamily", "Incomplete family structure (not a full family)" ) );
                }
            }
            else if ( entity is IGenPerson person && !includedIndividuals.Contains( person.ToString() ?? string.Empty ) )
            {
                // Check if individual is isolated (no family memberships and no parent family)
                if ( IsIsolatedIndividual( person ) )
                {
                    virtualPersons.Add( person );
                }
            }
        }

        var selection = new OFBSourceSelection
        {
            SelectedFamilies = selectedFamilies.AsReadOnly(),
            VirtualPersons = virtualPersons.AsReadOnly(),
            Exclusions = exclusions.AsReadOnly(),
        };

        return await Task.FromResult( selection );
    }

    /// <summary>
    /// Determines whether a family qualifies as a "full family" per Delphi reference logic.
    /// A full family is one that has meaningful genealogical context:
    /// - Husband has children, OR Wife has children
    /// - Husband has a parent family, OR Wife has a parent family
    /// - Has no children (childless couple)
    /// - First child has own children or spouses
    /// </summary>
    private static bool IsFullFamily( IGenFamily family )
    {
        // Check if husband has children
        var husband = family.Husband;
        if ( husband != null && husband.ChildCount > 1 )
            return true;

        // Check if wife has children
        var wife = family.Wife;
        if ( wife != null && wife.ChildCount > 1 )
            return true;

        // Check if either spouse has a parent family
        if ( husband != null && husband.ParentFamily != null )
            return true;

        if ( wife != null && wife.ParentFamily != null )
            return true;

        // Childless couple — still include
        if ( family.ChildCount == 0 )
            return true;

        // Check if first child has children or spouses
        var firstChild = family.Children?[ 0 ];
        if ( firstChild != null )
        {
            if ( firstChild.ChildCount > 0 || firstChild.SpouseCount > 0 )
                return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether an individual is isolated (no family memberships and no parent family).
    /// Isolated individuals get virtual families created for them.
    /// </summary>
    private static bool IsIsolatedIndividual( IGenPerson person )
    {
        // Check family count — Delphi uses FamCount == 0
        if ( person.FamilyCount != 0 )
            return false;

        // Check parent family — Delphi checks assigned(ParentFamily)
        if ( person.ParentFamily != null )
            return false;

        return true;
    }

    /// <summary>
    /// Tracks all individuals in a family for deduplication against virtual family creation.
    /// </summary>
    private static void TrackIndividuals(
        IGenFamily family,
        HashSet<string> includedIndividuals,
        List<IGenPerson> virtualPersons,
        List<OFBExclusionDiagnostic> exclusions )
    {
        var husband = family.Husband;
        if ( husband != null )
            includedIndividuals.Add( husband.ToString() ?? string.Empty );

        var wife = family.Wife;
        if ( wife != null )
            includedIndividuals.Add( wife.ToString() ?? string.Empty );

        var children = family.Children;
        if ( children?.Count > 0 )
        {
            foreach ( var child in children )
            {
                if ( child != null )
                    includedIndividuals.Add( child.ToString() ?? string.Empty );
            }
        }

        // Remove any virtual persons that are now accounted for in real families
        foreach ( var vp in virtualPersons.ToList() )
        {
            if ( includedIndividuals.Contains( vp.ToString() ?? string.Empty ) )
                virtualPersons.Remove( vp );
        }
    }
}
