using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using OFBCreator.Abstractions.Models;

namespace OFBCreator.Abstractions.Interfaces;

/// <summary>
/// Contract for generating an alphabetical place index (entries sorted by place name).
/// </summary>
public interface IAlphabeticalPlaceIndexGenerator : IIndexGenerator<OFBIndexEntry> { }

/// <summary>
/// Contract for generating a hierarchical place index (tree structure with parent-child relationships).
/// </summary>
public interface IHierarchicalPlaceIndexGenerator
{
    /// <summary>
    /// Generates hierarchical place nodes from the supplied selected families.
    /// </summary>
    Task<IReadOnlyList<OFBPlaceHierarchyNode>> GenerateAsync(
        IReadOnlyList<OFBFamilyModel> selectedFamilies,
        CancellationToken cancellationToken = default );
}
