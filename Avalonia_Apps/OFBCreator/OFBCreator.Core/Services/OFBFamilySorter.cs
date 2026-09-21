using System;
using System.Collections.Generic;
using System.Linq;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Abstractions.Models;

namespace OFBCreator.Core.Services;

/// <summary>
/// Sorter for OFB families: sorts by FamilyName group + formation date, assigns global numbers.
/// Implements IOFBFamilySorter from Abstractions layer.
/// </summary>
public class OFBFamilySorter : IOFBFamilySorter
{
    private const int MinNumberWidth = 4;
    private const int MaxNumberWidth = 5;

    /// <summary>
    /// Sorts families by name group and formation date, assigns global numbers.
    /// </summary>
    /// <param name="families">Raw family collection from GEDCOM import.</param>
    /// <param name="placeId">Optional place filter — only include families matching this place ID.</param>
    /// <param name="includeDescendants">Whether to include descendants of matched families.</param>
    /// <returns>Sorted and numbered family array ready for export.</returns>
    public IReadOnlyList<OFBFamilyModel> SortAndNumber(
        IEnumerable<IGenFamily> families, string? placeId = null, bool includeDescendants = false )
    {
        ArgumentNullException.ThrowIfNull( families );

        // Filter by place if specified
        var filteredFamilies = new List<IGenFamily>( families );

        if ( !string.IsNullOrWhiteSpace( placeId ) )
        {
            filteredFamilies = filteredFamilies.Where( f => PlaceMatches( f, placeId ) ).ToList();

            if ( includeDescendants )
            {
                var descendants = FindDescendantFamilies( filteredFamilies.AsReadOnly(), placeId );
                foreach ( var desc in descendants )
                    if ( !filteredFamilies.Contains( desc ) )
                        filteredFamilies.Add( desc );
            }
        }

        // Convert to OFB domain model (from filtered set)
        var ofbFamilies = new List<OFBFamilyModel>();

        foreach ( var family in filteredFamilies )
        {
            var husbandSurname = family.Husband?.Surname ?? "Unbekannt";
            var wifeSurname = family.Wife?.Surname;
            
            // Compute group name: combine surnames alphabetically for married couples
            string familyName;
            if ( !string.IsNullOrWhiteSpace( wifeSurname ) && 
                 string.Compare( husbandSurname, wifeSurname, StringComparison.Ordinal ) <= 0 )
            {
                familyName = $"{husbandSurname}/{wifeSurname}";
            }
            else if ( !string.IsNullOrWhiteSpace( wifeSurname ) )
            {
                familyName = $"{wifeSurname}/{husbandSurname}";
            }
            else
            {
                familyName = husbandSurname;
            }

            var ofb = new OFBFamilyModel
            {
                Husband = family.Husband,
                Wife = family.Wife,
                Children = ToReadOnly( (IEnumerable<IGenPerson>)family.Children ),
                MarriagePlace = family.MarriagePlace,
                SourceRefId = family.FamilyRefID ?? string.Empty,
                FamilyName = familyName,
                FormationDate = ComputeFormationDate( family ),
            };

            ofbFamilies.Add( ofb );
        }

        // Sort families: first by name group, then by formation date within each group
        ofbFamilies.Sort( CompareForOFBSort );

        // Assign global sequential numbers
        for ( int i = 0; i < ofbFamilies.Count; i++ )
            ofbFamilies[ i ].GlobalNumber = PadNumber( i + 1, MinNumberWidth );

        return ofbFamilies;
    }

    /// <summary>
    /// Compares two OFB families for proper sorting order.
    /// </summary>
    private static int CompareForOFBSort( OFBFamilyModel a, OFBFamilyModel b )
    {
        // Primary: name group sort key (surname alphabetically)
        var nameCompare = string.Compare( a.FamilyName, b.FamilyName, StringComparison.Ordinal );
        if ( nameCompare != 0 ) return nameCompare;

        // Secondary: formation date (earlier dates first, unknown dates at end)
        var dateA = a.FormationDate;
        var dateB = b.FormationDate;

        if ( !dateA.HasValue && !dateB.HasValue ) return 0;
        if ( !dateA.HasValue ) return 1; // nulls last
        if ( !dateB.HasValue ) return -1;

        return dateA.Value.CompareTo( dateB.Value );
    }

    /// <summary>
    /// Checks if a family matches the given place ID filter.
    /// Uses GOV_ID as the primary reference for place matching.
    /// </summary>
    private static bool PlaceMatches( IGenFamily family, string placeId )
    {
        // Marriage place
        if ( family.MarriagePlace != null &&
             !string.IsNullOrEmpty( family.MarriagePlace.GOV_ID ) &&
             string.Equals( family.MarriagePlace.GOV_ID, placeId, StringComparison.Ordinal ) )
            return true;

        // Husband's birth place
        var husbandBirthPlace = family.Husband?.BirthPlace;
        if ( husbandBirthPlace != null &&
             !string.IsNullOrEmpty( husbandBirthPlace.GOV_ID ) &&
             string.Equals( husbandBirthPlace.GOV_ID, placeId, StringComparison.Ordinal ) )
            return true;

        // Wife's birth place
        var wifeBirthPlace = family.Wife?.BirthPlace;
        if ( wifeBirthPlace != null &&
             !string.IsNullOrEmpty( wifeBirthPlace.GOV_ID ) &&
             string.Equals( wifeBirthPlace.GOV_ID, placeId, StringComparison.Ordinal ) )
            return true;

        // Children's birth places
        var children = family.Children;
        if ( children?.Count > 0 )
        {
            foreach ( var child in children )
            {
                var childBirthPlace = child?.BirthPlace;
                if ( childBirthPlace != null &&
                     !string.IsNullOrEmpty( childBirthPlace.GOV_ID ) &&
                     string.Equals( childBirthPlace.GOV_ID, placeId, StringComparison.Ordinal ) )
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Finds descendant families for a given place ID (for includeDescendants mode).
    /// Traverses children → their own families recursively.
    /// </summary>
    private static List<IGenFamily> FindDescendantFamilies(
        IReadOnlyList<IGenFamily> allFamilies, string placeId )
    {
        var result = new List<IGenFamily>();
        var visited = new HashSet<string>( StringComparer.Ordinal );

        // Build a quick lookup of children → parent families to find reverse links
        foreach ( var family in allFamilies )
        {
            var children = family.Children;
            if ( children?.Count > 0 )
            {
                foreach ( var child in children )
                {
                    if ( child == null ) continue;

                    // Find families where this child is husband or wife
                    foreach ( var otherFamily in allFamilies )
                    {
                        if ( result.Contains( otherFamily ) || visited.Contains( otherFamily.ToString() ?? string.Empty ) )
                            continue;

                        if ( ( otherFamily.Husband != null && string.Equals( otherFamily.Husband.ToString(), child.ToString(), StringComparison.Ordinal ) ) ||
                             ( otherFamily.Wife != null && string.Equals( otherFamily.Wife.ToString(), child.ToString(), StringComparison.Ordinal ) ) )
                        {
                            if ( PlaceMatches( otherFamily, placeId ) )
                            {
                                result.Add( otherFamily );
                                visited.Add( otherFamily.ToString() ?? string.Empty );

                                // Recursively find grandchildren's families
                                var grandChildren = FindDescendantFamiliesFromFamily( allFamilies, otherFamily, placeId, visited );
                                foreach ( var gc in grandChildren )
                                    if ( !result.Contains( gc ) )
                                        result.Add( gc );
                            }
                        }
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Recursively finds families where children of the given family form their own families.
    /// </summary>
    private static List<IGenFamily> FindDescendantFamiliesFromFamily(
        IReadOnlyList<IGenFamily> allFamilies, IGenFamily parent, string placeId, HashSet<string> visited )
    {
        var result = new List<IGenFamily>();
        var children = parent.Children;

        if ( children?.Count > 0 )
        {
            foreach ( var child in children )
            {
                if ( child == null ) continue;

                foreach ( var otherFamily in allFamilies )
                {
                    if ( result.Contains( otherFamily ) || visited.Contains( otherFamily.ToString() ?? string.Empty ) )
                        continue;

                    if ( ( otherFamily.Husband != null && string.Equals( otherFamily.Husband.ToString(), child.ToString(), StringComparison.Ordinal ) ) ||
                         ( otherFamily.Wife != null && string.Equals( otherFamily.Wife.ToString(), child.ToString(), StringComparison.Ordinal ) ) )
                    {
                        if ( PlaceMatches( otherFamily, placeId ) )
                        {
                            result.Add( otherFamily );
                            visited.Add( otherFamily.ToString() ?? string.Empty );

                            var grandchildren = FindDescendantFamiliesFromFamily( allFamilies, otherFamily, placeId, visited );
                            foreach ( var gc in grandchildren )
                                if ( !result.Contains( gc ) )
                                    result.Add( gc );
                        }
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Computes the family formation date following the priority rules from the Delphi reference:
    /// Priority 1: MarriageDate (MARR) → primary
    /// Priority 2: FirstChildBirthDate → fallback for unmarried partnerships
    /// Priority 3: FirstChildBaptismDate → additional fallback (from child's baptism)
    /// Priority 4: Wife.BirthDate/BaptDate - 5 years → estimated if no MARR and children exist
    /// Priority 5: Husband.BirthDate/BaptDate + 30 years → ultimate fallback
    /// Priority 6: Childless — Husband/Wife BirthDate/BaptDate directly
    /// </summary>
    private static DateTime? ComputeFormationDate( IGenFamily family )
    {
        // Priority 1: Marriage date (MARR)
        if ( family.MarriageDate != null && family.MarriageDate.Date1 != default )
            return family.MarriageDate.Date1;

        // Has children? → use child dates for estimation
        var children = family.Children;
        if ( children?.Count > 0 )
        {
            // Priority 2: First child's birth date
            foreach ( var child in children )
            {
                if ( child == null || child.BirthDate == null || child.BirthDate.Date1 == default )
                    continue;

                return child.BirthDate.Date1;
            }

            // Priority 3: First child's baptism date (additional fallback from Delphi)
            foreach ( var child in children )
            {
                if ( child == null || child.BaptDate == null || child.BaptDate.Date1 == default )
                    continue;

                return child.BaptDate.Date1;
            }

            // Priority 4: Wife's baptism/birth date - 5 years (estimated)
            var wife = family.Wife;
            if ( wife != null )
            {
                if ( wife.BaptDate != null && wife.BaptDate.Date1 != default )
                    return wife.BaptDate.Date1.AddYears( -5 );

                if ( wife.BirthDate != null && wife.BirthDate.Date1 != default )
                    return wife.BirthDate.Date1.AddYears( -5 );
            }

            // Priority 5a: Husband's baptism/birth date + 30 years (fallback)
            var husband = family.Husband;
            if ( husband != null )
            {
                if ( husband.BaptDate != null && husband.BaptDate.Date1 != default )
                    return husband.BaptDate.Date1.AddYears( 30 );

                if ( husband.BirthDate != null && husband.BirthDate.Date1 != default )
                    return husband.BirthDate.Date1.AddYears( 30 );
            }
        }
        else
        {
            // Childless couple — use parent dates directly (not estimated)
            var husband = family.Husband;
            if ( husband != null )
            {
                if ( husband.BirthDate != null && husband.BirthDate.Date1 != default )
                    return husband.BirthDate.Date1;

                if ( husband.BaptDate != null && husband.BaptDate.Date1 != default )
                    return husband.BaptDate.Date1;
            }

            var wife = family.Wife;
            if ( wife != null )
            {
                if ( wife.BirthDate != null && wife.BirthDate.Date1 != default )
                    return wife.BirthDate.Date1;

                if ( wife.BaptDate != null && wife.BaptDate.Date1 != default )
                    return wife.BaptDate.Date1;
            }
        }

        return null; // Cannot determine formation date
    }

    /// <summary>
    /// Pads a number with leading zeros to at least MinNumberWidth digits.
    /// Numbers exceeding MaxNumberWidth are returned without padding.
    /// </summary>
    private static string PadNumber( int number, int minDigits )
    {
        if ( number >= 1 && number <= 99999 )
            return number.ToString( $"D{Math.Max( minDigits, MaxNumberWidth )}" );

        return number.ToString();
    }

    /// <summary>
    /// Converts a potentially null collection to a safe read-only list.
    /// </summary>
    private static IReadOnlyList<IGenPerson> ToReadOnly( IEnumerable<IGenPerson>? source )
    {
        if ( source == null )
            return Array.Empty<IGenPerson>();

        var list = new List<IGenPerson>();
        foreach ( var item in source )
        {
            if ( item != null )
                list.Add( item );
        }

        return list.Count == 0 ? Array.Empty<IGenPerson>() : list.AsReadOnly();
    }
}
