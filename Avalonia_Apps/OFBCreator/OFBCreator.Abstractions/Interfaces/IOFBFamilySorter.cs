using System.Collections.Generic;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Models;

namespace OFBCreator.Abstractions.Interfaces;

/// <summary>
/// Contract for sorting families by name group and formation date, assigning global OFB numbers.
/// </summary>
public interface IOFBFamilySorter
{
    /// <summary>
    /// Sorts families by name group and formation date, assigns global numbers.
    /// </summary>
    /// <param name="families">Raw family collection from GEDCOM import.</param>
    /// <param name="placeId">Optional place filter.</param>
    /// <param name="includeDescendants">Whether to include descendants of matched families.</param>
    IReadOnlyList<OFBFamilyModel> SortAndNumber( IEnumerable<IGenFamily> families, string? placeId = null, bool includeDescendants = false );
}
